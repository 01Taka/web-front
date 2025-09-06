using UnityEngine;
using System.Collections.Generic;
using System.Threading.Tasks;
using Fusion;

public class NetworkRunnerHandlerManager : MonoBehaviour
{
    [Header("Prefabs and Managers")]
    [SerializeField] private NetworkRunner _runnerPrefab;

    [Header("Game Settings")]
    [SerializeField] private string _gameSceneName = "GameScene";

    // NetworkRunnerHandlerのプレハブをインスペクターから設定
    [SerializeField] private NetworkRunnerHandler _runnerHandlerPrefab;

    [SerializeField] private int _peerCount = 1;

    private List<NetworkRunnerHandler> _activeRunners = new List<NetworkRunnerHandler>();

    public async Task StartGame(string sessionName)
    {
        Debug.Log($"StartSession: {sessionName}");
        await StartMultiplePeers(sessionName, _peerCount);
    }

    // ゲームセッションを開始するための公開メソッド
    // ここでピアの数を指定して、それぞれを起動します
    public async Task StartMultiplePeers(string sessionName, int peerCount)
    {
        if (_runnerHandlerPrefab == null)
        {
            Debug.LogError("NetworkRunnerHandler prefab is not assigned.");
            return;
        }

        for (int i = 0; i < peerCount; i++)
        {
            await CreateAndStartPeer(sessionName);
        }
    }

    private async Task CreateAndStartPeer(string sessionName)
    {

        // 新しい NetworkRunnerHandler インスタンスを生成
        NetworkRunnerHandler newRunnerHandler = Instantiate(_runnerHandlerPrefab, transform);
        _activeRunners.Add(newRunnerHandler);

        // 各ピアのゲームセッションを開始
        // PlayerSpawnerを引数として渡します
        await newRunnerHandler.StartGame(_gameSceneName, sessionName, _runnerPrefab);
    }

    private void OnApplicationQuit()
    {
        // アプリケーション終了時にすべてのランナーをシャットダウン
        foreach (var handler in _activeRunners)
        {
            // ここでは非同期のShutdownを待たずに実行
            handler.GetComponent<NetworkRunner>()?.Shutdown();
        }
    }
}