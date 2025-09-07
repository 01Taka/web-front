using Fusion;
using System.Collections.Generic;
using UnityEngine;
using Fusion.Sockets;
using System;
using NUnit.Framework;

/// <summary>
/// プレイヤーの参加・退出を管理し、インデックスを割り当てるマネージャ。
/// NetworkRunner に登録して利用する。
/// </summary>
public class NetworkPlayerManager : NetworkBehaviour, INetworkRunnerCallbacks
{
    [Networked]
    [UnityNonSerialized]
    public NetworkBool HasGameStarted { get; set; }

    [Networked, Capacity(8)]
    private NetworkDictionary<PlayerRef, int> PlayerIndexes => default;

    private List<int> _freeIndexes = new List<int>();

    public int PlayerCount => PlayerIndexes.Count;

    public override void Spawned() => Runner.AddCallbacks(this);
    public override void Despawned(NetworkRunner runner, bool hasState) => runner.RemoveCallbacks(this);

    public bool IsOnline => Runner != null && Runner.IsRunning;
    public bool IsMasterClient => Runner != null && Runner.IsRunning && Runner.IsSharedModeMasterClient;

    //--------------------------------------------------------------------------------
    // ゲーム開始
    //--------------------------------------------------------------------------------
    [Rpc(RpcSources.StateAuthority, RpcTargets.All)]
    public void Rpc_StartGame()
    {
        DeviceStateManager deviceStateManager = GetComponentInParent<DeviceStateManager>();
        if (!deviceStateManager)
        {
            Debug.LogError("Not Found DeviceStateManager In Parent");
            return;
        }

        if (Runner.IsSharedModeMasterClient)
        {
            deviceStateManager.SetDeviceState(DeviceState.Host, true);
        }
        else
        {
            deviceStateManager.SetDeviceState(DeviceState.Client, true);
        }

        if (HasStateAuthority)
        {
            HasGameStarted = true;
            Debug.Log("[Rpc_StartGame] Game started.");
        }
    }

    public void RequestStartGame()
    {
        if (!HasStateAuthority)
        {
            Debug.LogWarning("You must have the state permission to call RequestStartGame.");
            return;
        }
        HasGameStarted = true;
        Rpc_StartGame();
    }

    //--------------------------------------------------------------------------------
    // Player Handling
    //--------------------------------------------------------------------------------
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsSharedModeMasterClient) return;

        if (HasGameStarted)
        {
            Debug.LogError($"[PlayerJoined] ID:{player.PlayerId} → Game already started. Rejecting.");
            runner.Disconnect(player);
            return;
        }

        NetworkGameManager.Instance.SpawnPlayer(runner, player);

        int newIndex = GetNextAvailableIndex();
        PlayerIndexes.Add(player, newIndex);
        Debug.Log($"[PlayerJoined] ID:{player.PlayerId} >>> Index:{newIndex}");
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsSharedModeMasterClient) return;

        if (!PlayerIndexes.ContainsKey(player)) return;

        int removedIndex = PlayerIndexes[player];
        PlayerIndexes.Remove(player);
        _freeIndexes.Add(removedIndex);

        Debug.Log($"[PlayerLeft] ID:{player.PlayerId} (Index:{removedIndex}) → Free");
    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason reason)
    {
        SharedModeMasterClientTracker.ResetInstance();

        if (HasStateAuthority)
        {
            PlayerIndexes.Clear();
            _freeIndexes.Clear();
        }
    }

    //--------------------------------------------------------------------------------
    // Utility
    //--------------------------------------------------------------------------------
    private int GetNextAvailableIndex()
    {
        if (_freeIndexes.Count > 0)
        {
            int index = _freeIndexes[_freeIndexes.Count - 1];
            _freeIndexes.RemoveAt(_freeIndexes.Count - 1);
            return index;
        }
        return PlayerIndexes.Count;
    }

    public bool TryGetPlayerIndex(PlayerRef player, out int index)
    {
        return PlayerIndexes.TryGetValueSafe(player, out index);
    }

    /// <summary>
    /// マスタークライアント以外のプレイヤーが、0から連番になるようにインデックスを取得する。
    /// PlayerIndexes に存在しないプレイヤーは false を返す。
    /// </summary>
    public bool TryGetCompactedIndex(PlayerRef player, out int compactedIndex)
    {
#if UNITY_EDITOR
        var playersCount = PlayerIndexes.Count - 1; // マスターを除いた人数
        if (playersCount > 0)
        {
            compactedIndex = UnityEngine.Random.Range(0, playersCount);
            Debug.Log($"[DEBUG MODE] TryGetCompactedIndex: Returning random {compactedIndex}.");
            return true;
        }
#endif
        compactedIndex = -1;

        if (SharedModeMasterClientTracker.IsPlayerSharedModeMasterClient(player))
        {
            Debug.LogError("TryGetCompactedIndex: Master client cannot get compacted index.");
            return false;
        }

        if (!PlayerIndexes.ContainsKey(player))
        {
            Debug.LogError($"TryGetCompactedIndex: Player {player.PlayerId} not in PlayerIndexes.");
            return false;
        }

        var sortedPlayers = new List<PlayerRef>(PlayerIndexes.GetKeys());
        sortedPlayers.Remove(SharedModeMasterClientTracker.MasterClientPlayerRef);
        sortedPlayers.Sort((a, b) => a.PlayerId.CompareTo(b.PlayerId));

        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            if (sortedPlayers[i] == player)
            {
                compactedIndex = i;
                return true;
            }
        }

        Debug.LogError($"TryGetCompactedIndex: Player {player.PlayerId} not found after sorting.");
        return false;
    }

    //--------------------------------------------------------------------------------
    // Misc
    //--------------------------------------------------------------------------------
    public void DisconnectFromSession()
    {
        if (Runner == null)
        {
            Debug.LogWarning("NetworkRunner is null. Cannot shutdown.");
            return;
        }
        if (!Runner.IsRunning)
        {
            Debug.LogWarning("NetworkRunner is not running.");
            return;
        }
        Runner.Shutdown(shutdownReason: ShutdownReason.Ok);
    }

    public void OnSceneLoadStart(NetworkRunner runner) => Debug.Log("[Fusion] Scene load started");
    public void OnSceneLoadDone(NetworkRunner runner) => Debug.Log("[Fusion] Scene load done");

    // Empty callbacks
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}
