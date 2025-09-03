using UnityEngine;

public class GameManager : MonoBehaviour
{
    // 各UIパネルへの参照をインスペクターで設定できるようにします
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject playPanel;
    [SerializeField] private GameObject scorePanel;

    // GamePlayingManagerへの参照を追加
    [SerializeField] private GamePlayingManager gamePlayingManager;

    // ゲームの状態を管理するための列挙型（enum）
    public enum GameState
    {
        Title,
        Playing,
        Score
    }

    // 現在のゲームの状態を保持する変数
    private GameState currentState;

    void Start()
    {
        // ゲーム開始時はタイトル画面から始める
        SetGameState(GameState.Title);
    }

    /// <summary>
    /// ゲームの状態を切り替えるメソッド
    /// </summary>
    /// <param name="newState">新しいゲームの状態</param>
    public void SetGameState(GameState newState)
    {
        if (currentState == newState)
        {
            return;
        }

        currentState = newState;
        UpdateUIState();
    }

    /// <summary>
    /// 現在のゲーム状態に応じてUIの表示を更新する
    /// </summary>
    private void UpdateUIState()
    {
        // すべてのパネルを一旦非表示にする
        titlePanel.SetActive(currentState == GameState.Title);
        playPanel.SetActive(currentState == GameState.Playing);
        scorePanel.SetActive(currentState == GameState.Score);

        switch (currentState)
        {
            case GameState.Title:
                Debug.Log("タイトル画面を表示");
                break;

            case GameState.Playing:
                // ゲームプレイの開始処理をGamePlayingManagerに委譲
                if (gamePlayingManager != null)
                {
                    gamePlayingManager.InitializeGame();
                }
                Debug.Log("プレイ画面を表示");
                break;

            case GameState.Score:
                Debug.Log("スコア画面を表示");
                break;
        }
    }

    //--------------------------------------------------------------------------------
    // ボタンのクリックイベントに割り当てるためのパブリックメソッド
    //--------------------------------------------------------------------------------
    public void OnStartButtonClicked()
    {
        SetGameState(GameState.Playing);
    }

    public void OnGoToScoreButtonClicked()
    {
        SetGameState(GameState.Score);
    }

    public void OnGoToTitleButtonClicked()
    {
        SetGameState(GameState.Title);
    }
}