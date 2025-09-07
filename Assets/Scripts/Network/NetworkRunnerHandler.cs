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

        _runnerPrefab = runnerPrefab;
        _sessionName = sessionName;
        _gameSceneName = gameSceneName;

        try
        {
            // await������ƈȉ��̊֐��͏I���Ȃ������ł��邱�Ƃɒ���
            InitializeAndConnect();

            DeviceStateManager deviceStateManager = GetComponentInParent<DeviceStateManager>();
            if (!deviceStateManager)
            {
                Debug.LogError("Not Found DeviceStateManager In Parent");
                return;
            }
            deviceStateManager.SetDeviceState(DeviceState.Online);
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
        _runner = Instantiate(_runnerPrefab);
        _runner.ProvideInput = true;

        var startArgs = new StartGameArgs
        {
            GameMode = GameMode.Shared,
            SessionName = _sessionName,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        };

        var result = await _runner.StartGame(startArgs);

        if (!result.Ok)
        {
            Debug.LogError($"Failed to start game. Reason: {result.ShutdownReason}");
            return;
        }

        if (_runner.IsSharedModeMasterClient)
        {
            Debug.Log($"Master Client has started loading a scene: {_gameSceneName}");

            try
            {
                // WebGL Build��͈ȉ��̏����̓Q�[���I���܂ŏI���Ȃ�
                // ����ȍ~�ɏ����������Ă����s����Ȃ�
                await _runner.LoadScene(_gameSceneName);
                Debug.Log($"Master Client has loaded a scene: {_gameSceneName}");
            }
            catch (Exception ex)
            {
                Debug.LogError($"Failed to load scene: {ex.Message}");
                throw;
            }
        }
    }
}