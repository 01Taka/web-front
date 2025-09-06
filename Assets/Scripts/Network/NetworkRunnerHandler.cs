using Fusion;
using System;
using System.Threading.Tasks;
using UnityEngine;

public class NetworkRunnerHandler : MonoBehaviour
{
    private string _gameSceneName;
    private string _sessionName;
    private NetworkRunner _runnerPrefab;
    private NetworkRunner _runner;

    // 非同期処理の完了を待つためのTaskCompletionSource
    private TaskCompletionSource<bool> _initializationTcs;

    public async Task StartGame(string gameSceneName, string sessionName, NetworkRunner runnerPrefab)
    {
        Debug.Log("StartGame");

        if (_runner != null)
        {
            Debug.LogWarning("ゲームは既に開始されています。");
            return;
        }

        if (runnerPrefab == null)
        {
            Debug.LogError("Runner prefab or PlayerSpawner is not assigned.");
            return;
        }

        _runnerPrefab = runnerPrefab;
        _sessionName = sessionName;
        _gameSceneName = gameSceneName;

        // 初期化の完了を待つためのTaskを準備
        _initializationTcs = new TaskCompletionSource<bool>();

        try
        {
            await InitializeAndConnect();
        }
        catch (Exception ex)
        {
            Debug.LogError($"Unhandled exception in StartGame(): {ex}");
            if (_runner != null)
            {
                await _runner.Shutdown();
            }
        }
    }

    private async Task InitializeAndConnect()
    {
        _runner = Instantiate(_runnerPrefab);
        _runner.ProvideInput = true;

        var startArgs = new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = _sessionName,
        };

        try
        {
            var result = await _runner.StartGame(startArgs);

            if (!result.Ok)
            {
                Debug.LogError($"Failed to start game: {result.ShutdownReason}");
                return;
            }

            if (_runner.IsSharedModeMasterClient)
            {
                await _runner.LoadScene(_gameSceneName);
                Debug.Log($"Master Client has loaded the scene: {_gameSceneName}");
            }
        }
        catch (Exception ex)
        {
            // 予期せぬ例外が発生した場合
            Debug.LogError($"Unhandled exception during initialization: {ex}");
            // 例外を通知してタスクを失敗させる
            _initializationTcs.TrySetException(ex);
            return;
        }
        finally
        {
            // 初期化が完了したことを通知 (成功または失敗)
            // ここに到達すれば、必ずタスクが完了する
            if (!_initializationTcs.Task.IsCompleted)
            {
                _initializationTcs.TrySetResult(true);
            }
        }
    }
}