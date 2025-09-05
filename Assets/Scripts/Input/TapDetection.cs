using UnityEngine;
using UnityEngine.Events;
using UnityEngine.InputSystem;


public class TapDetection : MonoBehaviour
{
    // �@ InputActions�̃C���X�^���X���t�B�[���h�Ƃ��ĕێ�����
    private InputActions inputActions;
    private InputAction tapAction;
    private int tapCount = 0;
    private float lastTapTime = 0f;

    [Header("Settings")]
    [SerializeField] private int _targetTapCount = 3;
    [SerializeField] private float _minTimeBetweenTaps = 0.02f;
    [SerializeField] private float _maxTimeBetweenTaps = 0.5f;
    [SerializeField] private UnityEvent _onDetectedTaps;

    void Awake()
    {
        // �A �C���X�^���X�𐶐����ăt�B�[���h�ɑ��
        inputActions = new InputActions();
        // �B ���������C���X�^���X����A�N�V�������擾
        tapAction = inputActions.Touch.TouchPress;
        tapAction.canceled += ctx => OnTapPerformed();
        tapAction.Enable();
    }

    void OnTapPerformed()
    {
        float currentTime = Time.time;

        // �Ō�̃^�b�v����̌o�ߎ��Ԃ��ŏ����Ԗ����̏ꍇ�͏����𒆒f
        if (currentTime - lastTapTime < _minTimeBetweenTaps)
        {
            lastTapTime = currentTime;
            return;
        }

        // �Ō�̃^�b�v����̌o�ߎ��Ԃ��ő厞�Ԃ��傫���ꍇ�̓^�b�v�������Z�b�g
        if (currentTime - lastTapTime > _maxTimeBetweenTaps)
        {
            tapCount = 0;
        }

        tapCount++;
        lastTapTime = currentTime;

        // �ڕW�̃^�b�v���ɒB������C�x���g�𔭉΂��A�^�b�v�������Z�b�g
        if (tapCount >= _targetTapCount)
        {
            _onDetectedTaps?.Invoke();
            tapCount = 0;
        }
    }

    void OnDestroy()
    {
        // �C �C���X�^���X�S�̂�j������
        inputActions.Disable();
        inputActions.Dispose();
    }
}