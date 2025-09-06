using Fusion;
using UnityEngine;

public class NetworkGameManager : MonoBehaviour
{
    public static NetworkGameManager Instance { get; private set; }

    [SerializeField] private NetworkPrefabRef playerPrefab;

    private Vector3 _spawnPosition = Vector3.zero;

    // ゲーム開始時にインスタンスを初期化
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // シーン内にすでにインスタンスが存在する場合、重複するインスタンスを破棄する
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            // シーンを切り替えてもインスタンスを維持する場合
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void SpawnPlayer(NetworkRunner runner, PlayerRef sharedMasterRef)
    {
        if (runner == null)
        {
            Debug.LogError("NetworkRunner is null. Cannot spawn player.");
            return;
        }

        if (runner.LocalPlayer == null || runner.LocalPlayer.IsNone)
        {
            Debug.LogError("Runner LocalPlayer is null. Cannot spawn player.");
            return;
        }

        if (playerPrefab == null || !playerPrefab.IsValid)
        {
            Debug.LogError("Player prefab is not valid.");
            return;
        }

        if (!SharedModeMasterClientTracker.IsPlayerSharedModeMasterClient(sharedMasterRef))
        {
            Debug.LogError($"Error: The provided PlayerRef ({sharedMasterRef}) is not the MasterClient.");
            return;
        }

        try
        {
            NetworkObject playerObj = runner.Spawn(playerPrefab, _spawnPosition, Quaternion.identity, runner.LocalPlayer);

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
}