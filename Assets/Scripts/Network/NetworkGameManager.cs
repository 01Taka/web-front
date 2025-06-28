using Fusion;
using UnityEngine;

public class NetworkGameManager : MonoBehaviour
{
    [SerializeField] private NetworkPrefabRef playerPrefab;

    public void SpawnPlayer(NetworkRunner runner, PlayerRef sharedMasterRef)
    {
        if (runner == null)
        {
            Debug.LogError("NetworkRunner is null. Cannot spawn player.");
            return;
        }

        if (!playerPrefab.IsValid)
        {
            Debug.LogError("Player prefab is not valid.");
            return;
        }

        if (!SharedModeMasterClientTracker.IsPlayerSharedModeMasterClient(sharedMasterRef))
        {
            Debug.LogError($"Error: The provided PlayerRef ({sharedMasterRef}) is not the MasterClient.");
            return;
        }

        Vector3 spawnPos = GetSpawnPosition();

        try
        {
            NetworkObject playerObj = runner.Spawn(playerPrefab, spawnPos, Quaternion.identity, runner.LocalPlayer);

            if (playerObj == null)
            {
                Debug.LogError("Player spawn failed: runner.Spawn returned null.");
            }
            else
            {
                Debug.Log($"Player spawned with authority {sharedMasterRef}.");
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Exception occurred while spawning player: {ex}");
        }
    }

    private Vector3 GetSpawnPosition()
    {
        return Vector3.zero;
    }
}
