using Fusion;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkRunnerHandler : MonoBehaviour
{
    private string _gameSceneName;
    private string _sessionName;
    private NetworkRunner _runner;

    public async Task StartGame(string gameSceneName, string sessionName, NetworkRunner runnerPrefab)
    {
        Debug.Log("Starting game.");

        if (_runner != null)
        {
            Debug.LogWarning("Game has already started.");
            return;
        }

        if (runnerPrefab == null)
        {
            Debug.LogError("Runner prefab or PlayerSpawner is not assigned.");
            return;
        }

        _sessionName = sessionName;
        _gameSceneName = gameSceneName;

        try
        {
            _runner = Instantiate(runnerPrefab);
            _runner.ProvideInput = true;

            SetSpawnCallback();

            await InitializeAndConnect();

            SetDeviceStateOnline();
        }
        catch (Exception ex)
        {
            Debug.LogError($"An unexpected error occurred while starting the game: {ex.Message}");
            if (_runner != null)
            {
                await _runner.Shutdown();
                _runner = null;
            }
        }
    }

    private async Task InitializeAndConnect()
    {
        var startArgs = new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = _sessionName,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        };

        // StartGameの実行と結果の待機
        var result = await _runner.StartGame(startArgs);

        if (!result.Ok)
        {
            Debug.LogError($"Failed to start game. Reason: {result.ShutdownReason}");
            // 呼び出し元のtry-catchブロックで処理できるように例外をスローする
            throw new InvalidOperationException($"Failed to start game. Reason: {result.ShutdownReason}");
        }

        if (_runner.IsSharedModeMasterClient)
        {
            _ = LoadScene();
        }
    }

    private async Task LoadScene()
    {
        Debug.Log($"Master Client has started loading a scene: {_gameSceneName}");
        await _runner.LoadScene(_gameSceneName);
        Debug.Log($"Master Client has loaded a scene: {_gameSceneName}");
    }

    private void SetSpawnCallback()
    {
        // 念のため、登録する前に一度解除しておくことで、複数回の登録を防ぐ
        SharedModeMasterClientTracker.OnTrackerSpawned -= PlayerSpawnCallback;
        SharedModeMasterClientTracker.OnTrackerSpawned += PlayerSpawnCallback;
    }

    private void PlayerSpawnCallback(SharedModeMasterClientTracker tracker)
    {
        SharedModeMasterClientTracker.OnTrackerSpawned -= PlayerSpawnCallback;

        if (tracker == null)
        {
            Debug.LogError("Tracker was null during spawn callback.");
            return;
        }

        NetworkGameManager.Instance.SpawnPlayer(_runner);
    }

    private void SetDeviceStateOnline()
    {
        DeviceStateManager deviceStateManager = GetComponentInParent<DeviceStateManager>();
        if (!deviceStateManager)
        {
            Debug.LogError("Not Found DeviceStateManager In Parent");
            return;
        }
        deviceStateManager.SetDeviceState(DeviceState.Online);
    }

    private void OnDestroy()
    {
        // オブジェクトが破棄される際に、イベントの購読を確実に解除する
        SharedModeMasterClientTracker.OnTrackerSpawned -= PlayerSpawnCallback;
    }
}