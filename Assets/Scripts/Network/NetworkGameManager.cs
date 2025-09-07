using Fusion;
using UnityEngine;

public class NetworkGameManager : MonoBehaviour
{
    public static NetworkGameManager Instance { get; private set; }

    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private GamePlayingManager _gamePlayingManager;

    private Vector3 _spawnPosition = Vector3.zero;

    // �Q�[���J�n���ɃC���X�^���X��������
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            // �V�[�����ɂ��łɃC���X�^���X�����݂���ꍇ�A�d������C���X�^���X��j������
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            // �V�[����؂�ւ��Ă��C���X�^���X���ێ�����ꍇ
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void OnStartGameButtonClicked()
    {
        StartGame();
    }

    public void StartGame()
    {
        if (!GlobalRegistry.Instance.TryGetNetworkPlayerManager(out var manager))
        {
            Debug.LogError("Not found NetworkPlayerManager");
            return;
        }
        manager.RequestStartGame();
        _gamePlayingManager.Initialize();
    }

    public void SpawnPlayer(NetworkRunner runner)
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

        if (runner.LocalPlayer == null || runner.LocalPlayer.IsNone)
        {
            Debug.LogError("Input authority is None. Spawning a player without specific input authority.");
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
                Debug.Log($"Player spawned: {runner.LocalPlayer}.", this);
            }
        }
        catch (System.Exception ex)
        {
            Debug.LogError($"Exception occurred while spawning player: {ex}");
        }
    }
}