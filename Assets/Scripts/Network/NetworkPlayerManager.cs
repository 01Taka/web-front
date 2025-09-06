using Fusion;
using System.Collections.Generic;
using UnityEngine;
using Fusion.Sockets;
using System;

/// <summary>
/// プレイヤーの参加・退出を管理し、インデックスを割り当てるマネージャ。
/// NetworkRunner に登録して利用する。
/// </summary>
public class NetworkPlayerManager : NetworkBehaviour, INetworkRunnerCallbacks
{
    [Networked] public PlayerRef HostPlayerRef { get; set; }

    [Networked, Capacity(8)]
    private NetworkDictionary<PlayerRef, int> PlayerIndexes => default;
    private List<int> _freeIndexes = new List<int>();

    public int PlayerCount => PlayerIndexes.Count;

    public override void Spawned() => Runner.AddCallbacks(this);
    public override void Despawned(NetworkRunner runner, bool hasState) => runner.RemoveCallbacks(this);

    public bool IsOnline => Runner != null && Runner.IsRunning;
    public bool IsMasterClient => Runner != null && Runner.IsRunning && Runner.IsSharedModeMasterClient;

    //--------------------------------------------------------------------------------
    // Player Handling
    //--------------------------------------------------------------------------------
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (runner.IsSharedModeMasterClient)
        {
            NetworkGameManager.Instance.SpawnPlayer(runner, player);

            int newIndex = GetNextAvailableIndex();
            PlayerIndexes.Add(player, newIndex);
            Debug.Log($"[PlayerJoined] ID:{player.PlayerId} >>> Index:{newIndex}");
        }
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
        // セッション終了時にSharedModeMasterClientTrackerを初期化する
        SharedModeMasterClientTracker.ResetInstance();
    }

    //--------------------------------------------------------------------------------
    // Utility
    //--------------------------------------------------------------------------------

    private int GetNextAvailableIndex()
    {
        if (_freeIndexes.Count > 0)
        {
            // リストの末尾から取得する方が効率的
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
    /// マスタークライアント以外のプレイヤーが、0から連番になるようにインデックスを取得する
    /// </summary>
    /// <param name="player">取得したいプレイヤーのPlayerRef</param>
    /// <param name="compactedIndex">0から連番に詰めたインデックス</param>
    /// <returns>インデックスが取得できたか（マスタークライアントはfalse）</returns>
    public bool TryGetCompactedIndex(PlayerRef player, out int compactedIndex)
    {
        compactedIndex = -1;

        // マスタークライアントは常にfalseを返す
        if (SharedModeMasterClientTracker.IsPlayerSharedModeMasterClient(player)) return false;

        // PlayerIndexesにプレイヤーが存在するか確認
        if (!PlayerIndexes.ContainsKey(player)) return false;

        // PlayerRefをキーとしてソートされたリストを作成
        var sortedPlayers = new List<PlayerRef>(PlayerIndexes.GetKeys());
        sortedPlayers.RemoveAll(player => player.PlayerId == SharedModeMasterClientTracker.MasterClientPlayerRef.PlayerId);
        sortedPlayers.Sort();

        // プレイヤーのインデックスをリスト内で見つける
        for (int i = 0; i < sortedPlayers.Count; i++)
        {
            if (sortedPlayers[i] == player)
            {
                compactedIndex = i;
                return true;
            }
        }

        return false;
    }

    // プレイヤーとしてセッションから切断
    public void DisconnectFromSession()
    {
        // Runnerインスタンスがnullでないことを確認
        if (Runner == null)
        {
            Debug.LogWarning("NetworkRunner is null. Cannot perform shutdown.");
            return;
        }

        // Runnerがすでにシャットダウン中か確認
        if (!Runner.IsRunning)
        {
            Debug.LogWarning("NetworkRunner is not running. Shutdown already in progress or not connected.");
            return;
        }

        // 問題がなければシャットダウンを実行
        Runner.Shutdown(shutdownReason: ShutdownReason.Ok);
    }

    //--------------------------------------------------------------------------------
    // Empty Callbacks
    //--------------------------------------------------------------------------------
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnInput(NetworkRunner runner, NetworkInput input) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
}