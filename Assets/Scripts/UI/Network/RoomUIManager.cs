using UnityEngine;
using TMPro;
using System;
using System.Threading.Tasks;

public class RoomUIManager : MonoBehaviour
{
    // �V���A���C�Y�t�B�[���h�Ƃ���UI�R���|�[�l���g�ƎQ�������N���X�̎Q�Ƃ��A�T�C��
    [SerializeField] private TMP_InputField _sessionInputField;
    [SerializeField] private TMP_Text _roomIdText;
    [SerializeField] private NetworkRunnerHandler _networkRunnerHandler;

    [Header("Buttons")]
    [SerializeField] private UnityEngine.UI.Button _joinButton;
    [SerializeField] private UnityEngine.UI.Button _createButton;

    /// <summary>
    /// Unity��Start���\�b�h�ŏ����ݒ���s���܂��B
    /// </summary>
    private void Start()
    {
        // �A�v���P�[�V�����J�n���Ɏ����Ɋ�Â����Z�b�V����ID�𐶐����ĕ\��
        GenerateSessionIdByTimeOfDay();

        // InputField �� onValueChanged �C�x���g�Ɋ֐���o�^
        _sessionInputField.onValueChanged.AddListener(OnInputFieldValueChanged);

        // ����N�����Ƀ{�^���̏�Ԃ��X�V
        UpdateButtonStates();
    }

    /// <summary>
    /// InputField �̓��͒l���ύX���ꂽ�Ƃ��ɌĂ΂�郁�\�b�h�B
    /// </summary>
    /// <param name="value">���݂̓��͒l</param>
    private void OnInputFieldValueChanged(string value)
    {
        UpdateButtonStates();
    }

    /// <summary>
    /// �{�^���̗L���E���������A���^�C���ōX�V���郁�\�b�h�B
    /// </summary>
    private void UpdateButtonStates()
    {
        // �Q���{�^���̗L�������`�F�b�N
        string inputId = _sessionInputField.text;
        bool isJoinValid = IsValidJoinId(inputId);
        _joinButton.interactable = isJoinValid;
    }

    /// <summary>
    /// UI�{�^���̗L���E������؂�ւ��郁�\�b�h�B
    /// </summary>
    /// <param name="interactable">�{�^����L���ɂ��邩�����ɂ��邩</param>
    private void SetButtonsInteractable(bool interactable)
    {
        _joinButton.interactable = interactable;
        _createButton.interactable = interactable;
    }

    /// <summary>
    /// ���͂��ꂽID���Q�������𖞂����Ă��邩�m�F����w���p�[���\�b�h�B
    /// </summary>
    /// <param name="id">���[�U�[�����͂���ID</param>
    /// <returns>�L����ID�ł����true�A�����łȂ����false</returns>
    private bool IsValidJoinId(string id)
    {
        // ���͒l��5���̐��l�ł��邩���m�F
        if (id.Length != 5 || !int.TryParse(id, out int userValue))
        {
            return false;
        }

        // ���͒l���ߋ��̕b���ł��邩���m�F
        int currentSecondsOfDay = (int)(DateTime.UtcNow - DateTime.UtcNow.Date).TotalSeconds;
        if (userValue >= currentSecondsOfDay)
        {
            return false;
        }

        return true;
    }

    /// <summary>
    /// ���ݎ����́u���̓��̌ߑO0������̕b���v����ɃZ�b�V����ID�𐶐����A�\�����郁�\�b�h�B
    /// </summary>
    public void GenerateSessionIdByTimeOfDay()
    {
        // ���ݎ����́A���̓����̕b�����v�Z (0�`86399)
        int uniqueId = (int)(DateTime.UtcNow - DateTime.UtcNow.Date).TotalSeconds;

        string newId = uniqueId.ToString("D5"); // 5���̃[������
        _roomIdText.text = newId;
    }

    /// <summary>
    /// UI�̃{�^������Ă΂�AInputField��ID�ŎQ�������݂郁�\�b�h�B
    /// </summary>
    public void OnJoinButton()
    {
        string inputId = _sessionInputField.text;

        // �{�^�����L���ȏ�Ԃŉ����ꂽ�ꍇ�̂ݏ��������s
        if (IsValidJoinId(inputId))
        {
            // ������0������̕b�����v�Z
            int secondsSinceEpochToday = (int)(DateTime.UtcNow.Date - new DateTime(1970, 1, 1)).TotalSeconds;

            // ���[�U�[�̓��͒l�ɍ����̕b���𑫂��āA��ӂȃZ�b�V����ID�𐶐�
            int userValue = int.Parse(inputId);
            string sessionId = (secondsSinceEpochToday + userValue).ToString();

            // �Q�������N���X�̃��\�b�h���Ăяo���A�{�^���𖳌���
            SetButtonsInteractable(false);

            Debug.Log($"Entered ID: {userValue}");
            _ = HandleJoinAttempt(sessionId);
        }
    }

    /// <summary>
    /// UI�̃{�^������Ă΂�A�����_���������ꂽID�ŎQ�������݂郁�\�b�h�B
    /// </summary>
    public void OnCreateButton()
    {
        string inputId = _roomIdText.text;

        if (string.IsNullOrEmpty(inputId) || !int.TryParse(inputId, out int generatedValue))
        {
            Debug.LogError("The session ID is not generated.");
            return;
        }

        // ������0������̕b�����v�Z
        int secondsSinceEpochToday = (int)(DateTime.UtcNow.Date - new DateTime(1970, 1, 1)).TotalSeconds;

        // �������ꂽ�l�ɍ����̕b���𑫂��āA��ӂȃZ�b�V����ID�𐶐�
        string sessionId = (secondsSinceEpochToday + generatedValue).ToString();

        // �Q�������N���X�̃��\�b�h���Ăяo���A�{�^���𖳌���
        SetButtonsInteractable(false);
        _ = HandleJoinAttempt(sessionId);
    }

    /// <summary>
    /// �Q��������҂��A������Ƀ{�^����L��������񓯊��w���p�[���\�b�h�B
    /// </summary>
    private async Task HandleJoinAttempt(string sessionId)
    {
        Debug.Log($"Joined the Session with {sessionId}");
        try
        {
            await _networkRunnerHandler.StartGame(sessionId);
        }
        finally
        {
            // ����������������{�^����L����
            SetButtonsInteractable(true);
        }
    }
}