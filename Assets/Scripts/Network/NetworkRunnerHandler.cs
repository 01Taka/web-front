using Fusion;
using System;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkRunnerHandler : MonoBehaviour
{
    [Header("Prefabs and Managers")]
    [SerializeField] private NetworkRunner runnerPrefab;
    [SerializeField] private NetworkGameManager gameManager;

    [Header("Game Settings")]
    [SerializeField] private string gameSceneName = "GameScene";

    private NetworkRunner runner;

    /// <summary>
    /// 指定されたセッション名でゲームを開始する非同期メソッド。
    /// このメソッドは、UI管理クラスから呼び出されることを想定しています。
    /// </summary>
    /// <param name="sessionName">参加または作成するセッション名</param>
    public async Task StartGame(string sessionName)
    {
        if (runner != null)
        {
            Debug.LogWarning("ゲームは既に開始されています。");
            return;
        }

        if (runnerPrefab == null)
        {
            Debug.LogError("Runner prefab is not assigned.");
            return;
        }

        if (gameManager == null)
        {
            Debug.LogError("GameManager is not assigned.");
            return;
        }

        try
        {
            runner = Instantiate(runnerPrefab);
            runner.ProvideInput = true;

            void PlayerSpawnCallback(SharedModeMasterClientTracker tracker)
            {
                SharedModeMasterClientTracker.OnTrackerSpawned -= PlayerSpawnCallback;

                if (tracker == null)
                {
                    Debug.LogError("Tracker was null during spawn callback.");
                    return;
                }

                PlayerRef masterRef = tracker.Object.StateAuthority;
                gameManager.SpawnPlayer(runner, masterRef);
            }

            SharedModeMasterClientTracker.OnTrackerSpawned += PlayerSpawnCallback;

            var startArgs = new StartGameArgs
            {
                GameMode = GameMode.Shared,
                SessionName = sessionName,
            };

            var result = await runner.StartGame(startArgs);
            if (!result.Ok)
            {
                Debug.LogError($"Failed to start game: {result.ShutdownReason}");
                return;
            }

            if (runner.IsSharedModeMasterClient)
            {
                try
                {
                    var loadSceneTask = runner.LoadScene(gameSceneName);
                    await loadSceneTask;
                }
                catch (Exception e)
                {
                    Debug.LogError($"Exception while loading scene: {e.Message}");
                    return;
                }
            }

            if (!SetActiveScene(gameSceneName))
            {
                Debug.LogError($"Failed to load or activate scene: {gameSceneName}");
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Unhandled exception in StartGame(): {ex}");
        }
    }

    /// <summary>
    /// 指定されたシーンをアクティブにするヘルパーメソッド。
    /// </summary>
    private bool SetActiveScene(string sceneName)
    {
        Scene scene = SceneManager.GetSceneByName(sceneName);
        if (scene.IsValid() && scene.isLoaded)
        {
            SceneManager.SetActiveScene(scene);
            return true;
        }

        Debug.LogWarning($"Scene '{sceneName}' is not valid or not loaded.");
        return false;
    }
}