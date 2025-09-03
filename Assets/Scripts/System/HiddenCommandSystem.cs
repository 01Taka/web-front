using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;

/// <summary>
/// �V�t�g�L�[��������Ă���Ԃ̂݁A�A���t�@�x�b�g�L�[�ɂ��B���R�}���h���͂��󂯕t����X�N���v�g�B
/// </summary>
public class HiddenCommandSystem : MonoBehaviour
{
    // �B���R�}���h�ƃC�x���g���y�A�ŊǗ����邽�߂̃N���X
    [System.Serializable]
    public class CommandEvent
    {
        [Tooltip("��: 'log', 'setting'")]
        public string commandString;
        [Tooltip("���̃R�}���h�����������Ƃ��ɌĂяo�����UnityEvent")]
        public UnityEvent onCommandSuccess;
    }

    // �C���X�y�N�^�[�Őݒ�\�ȉB���R�}���h�ƃC�x���g�̔z��
    [SerializeField]
    private CommandEvent[] _commandEvents;

    // ���͒��̃R�}���h������
    private string _currentInput = "";

    // �L�[�{�[�h�f�o�C�X�ւ̎Q��
    private Keyboard _keyboard;

    void Awake()
    {
        // �L�[�{�[�h�f�o�C�X���擾
        _keyboard = Keyboard.current;
        if (_keyboard == null)
        {
            Debug.LogError("Keyboard device not found!");
            enabled = false; // �X�N���v�g�𖳌���
        }
    }

    void Update()
    {
        // �L�[�{�[�h���Ȃ���Ή������Ȃ�
        if (_keyboard == null) return;

        // �V�t�g�L�[�̏�Ԃ��`�F�b�N
        if (!_keyboard.leftShiftKey.isPressed && !_keyboard.rightShiftKey.isPressed)
        {
            // �V�t�g�L�[�������ꂽ����͂����Z�b�g
            if (_currentInput.Length > 0)
            {
                _currentInput = "";
                Debug.Log("�V�t�g�L�[�������ꂽ���߁A���͂����Z�b�g���܂����B");
            }
            return; // �V�t�g��������Ă��Ȃ��ꍇ�́A����ȍ~�̏������s��Ȃ�
        }

        // �V�t�g�L�[��������Ă���ꍇ�̂݁A�A���t�@�x�b�g�L�[���`�F�b�N
        foreach (var key in _keyboard.allKeys)
        {
            // �L�[���A���t�@�x�b�g�L�[�ł���A�������ꂽ�u�Ԃɏ���
            if (key.keyCode >= Key.A && key.keyCode <= Key.Z && key.wasPressedThisFrame)
            {
                // �L�[�����擾���ď������ɕϊ�
                string keyName = key.displayName.ToLower();
                _currentInput += keyName;

                // �R�}���h����v���邩�`�F�b�N
                CheckForCommandMatch();
            }
        }
    }

    /// <summary>
    /// ���݂̓��͕����񂪉B���R�}���h�Ɉ�v���邩���`�F�b�N���A��v�����ꍇ�̓C�x���g���N�����܂��B
    /// </summary>
    private void CheckForCommandMatch()
    {
        bool foundMatch = false;
        foreach (var commandEvent in _commandEvents)
        {
            // ���݂̓��͂��R�}���h������̐擪�ƈ�v���邩�m�F
            if (commandEvent.commandString.StartsWith(_currentInput))
            {
                foundMatch = true;
                if (_currentInput == commandEvent.commandString)
                {
                    Debug.Log($"Entered Command of {commandEvent.commandString}");
                    commandEvent.onCommandSuccess.Invoke();
                    _currentInput = ""; // ������A���͂����Z�b�g
                    return;
                }
            }
        }

        if (!foundMatch)
        {
            // �ǂ̃R�}���h�̓r���Ƃ���v���Ȃ��ꍇ�A���͂����Z�b�g
            _currentInput = "";
        }
    }
}