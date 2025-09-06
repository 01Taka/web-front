using UnityEngine;


public class GameManager : MonoBehaviour
{
    // 各UIパネルへの参照をインスペクターで設定できるようにします
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject playPanel;
    [SerializeField] private GameObject scorePanel;

    // GamePlayingManagerへの参照を追加
    [SerializeField] private GamePlayingManager _gamePlayingManager;
    [SerializeField] private GameScoreManager _gameScoreManager;

    private ScoreBreakdown _scoreBreakdown;

    // ゲームの状態を管理するための列挙型（enum）
    public enum GameState
    {
        Title,
        Playing,
        Score
    }

    // 現在のゲームの状態を保持する変数
    private GameState _currentState;

    void Start()
    {
        // ゲーム開始時はタイトル画面から始める
        _currentState = GameState.Title;
        UpdateUIState();
    }

    /// <summary>
    /// ゲームの状態を切り替えるメソッド
    /// </summary>
    /// <param name="newState">新しいゲームの状態</param>
    public void SetGameState(GameState newState)
    {
        if (_currentState == newState || !GlobalRegistry.Instance.CheckIsMasterClient())
        {
            return;
        }

        _currentState = newState;
        UpdateUIState();
    }

    /// <summary>
    /// 現在のゲーム状態に応じてUIの表示を更新する
    /// </summary>
    private void UpdateUIState()
    {
        // すべてのパネルを一旦非表示にする
        titlePanel.SetActive(_currentState == GameState.Title);
        playPanel.SetActive(_currentState == GameState.Playing);
        scorePanel.SetActive(_currentState == GameState.Score);

        switch (_currentState)
        {
            case GameState.Title:
                break;

            case GameState.Playing:
                // ゲームプレイの開始処理をGamePlayingManagerに委譲
                if (_gamePlayingManager != null)
                {
                    _gamePlayingManager.StartGame(this);
                }
                break;

            case GameState.Score:
                ShowScore();
                break;
        }
    }

    public void ShowScore()
    {
        if (_scoreBreakdown != null)
        {
            _gameScoreManager.ShowScore(_scoreBreakdown);
        }
    }

    public void SetScoreBreakDown(ScoreBreakdown scoreBreakdown)
    {
        _scoreBreakdown = scoreBreakdown;
    }

    public void OnGoToNextState()
    {
        if (_currentState == GameState.Title)
        {
            SetGameState(GameState.Playing);
        }
        if (_currentState == GameState.Score)
        {
            SetGameState(GameState.Title);
        }
    }
}