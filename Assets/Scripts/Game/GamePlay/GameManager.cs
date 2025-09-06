using UnityEngine;


public class GameManager : MonoBehaviour
{
    // �eUI�p�l���ւ̎Q�Ƃ��C���X�y�N�^�[�Őݒ�ł���悤�ɂ��܂�
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject playPanel;
    [SerializeField] private GameObject scorePanel;

    // GamePlayingManager�ւ̎Q�Ƃ�ǉ�
    [SerializeField] private GamePlayingManager _gamePlayingManager;
    [SerializeField] private GameScoreManager _gameScoreManager;

    private ScoreBreakdown _scoreBreakdown;

    // �Q�[���̏�Ԃ��Ǘ����邽�߂̗񋓌^�ienum�j
    public enum GameState
    {
        Title,
        Playing,
        Score
    }

    // ���݂̃Q�[���̏�Ԃ�ێ�����ϐ�
    private GameState _currentState;

    void Start()
    {
        // �Q�[���J�n���̓^�C�g����ʂ���n�߂�
        _currentState = GameState.Title;
        UpdateUIState();
    }

    /// <summary>
    /// �Q�[���̏�Ԃ�؂�ւ��郁�\�b�h
    /// </summary>
    /// <param name="newState">�V�����Q�[���̏��</param>
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
    /// ���݂̃Q�[����Ԃɉ�����UI�̕\�����X�V����
    /// </summary>
    private void UpdateUIState()
    {
        // ���ׂẴp�l������U��\���ɂ���
        titlePanel.SetActive(_currentState == GameState.Title);
        playPanel.SetActive(_currentState == GameState.Playing);
        scorePanel.SetActive(_currentState == GameState.Score);

        switch (_currentState)
        {
            case GameState.Title:
                break;

            case GameState.Playing:
                // �Q�[���v���C�̊J�n������GamePlayingManager�ɈϏ�
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