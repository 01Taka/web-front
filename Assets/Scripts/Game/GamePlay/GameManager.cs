using UnityEngine;

public class GameManager : MonoBehaviour
{
    // �eUI�p�l���ւ̎Q�Ƃ��C���X�y�N�^�[�Őݒ�ł���悤�ɂ��܂�
    [SerializeField] private GameObject titlePanel;
    [SerializeField] private GameObject playPanel;
    [SerializeField] private GameObject scorePanel;

    // GamePlayingManager�ւ̎Q�Ƃ�ǉ�
    [SerializeField] private GamePlayingManager gamePlayingManager;

    // �Q�[���̏�Ԃ��Ǘ����邽�߂̗񋓌^�ienum�j
    public enum GameState
    {
        Title,
        Playing,
        Score
    }

    // ���݂̃Q�[���̏�Ԃ�ێ�����ϐ�
    private GameState currentState;

    void Start()
    {
        // �Q�[���J�n���̓^�C�g����ʂ���n�߂�
        SetGameState(GameState.Title);
    }

    /// <summary>
    /// �Q�[���̏�Ԃ�؂�ւ��郁�\�b�h
    /// </summary>
    /// <param name="newState">�V�����Q�[���̏��</param>
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
    /// ���݂̃Q�[����Ԃɉ�����UI�̕\�����X�V����
    /// </summary>
    private void UpdateUIState()
    {
        // ���ׂẴp�l������U��\���ɂ���
        titlePanel.SetActive(currentState == GameState.Title);
        playPanel.SetActive(currentState == GameState.Playing);
        scorePanel.SetActive(currentState == GameState.Score);

        switch (currentState)
        {
            case GameState.Title:
                Debug.Log("�^�C�g����ʂ�\��");
                break;

            case GameState.Playing:
                // �Q�[���v���C�̊J�n������GamePlayingManager�ɈϏ�
                if (gamePlayingManager != null)
                {
                    gamePlayingManager.InitializeGame();
                }
                Debug.Log("�v���C��ʂ�\��");
                break;

            case GameState.Score:
                Debug.Log("�X�R�A��ʂ�\��");
                break;
        }
    }

    //--------------------------------------------------------------------------------
    // �{�^���̃N���b�N�C�x���g�Ɋ��蓖�Ă邽�߂̃p�u���b�N���\�b�h
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