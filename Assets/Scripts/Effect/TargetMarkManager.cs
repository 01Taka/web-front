using UnityEngine;

public class TargetMarkManager : MonoBehaviour
{
    // �V���O���g���C���X�^���X���Ǘ�����v���C�x�[�g�ȐÓI�ϐ�
    private static TargetMarkManager _instance;

    // �O������A�N�Z�X���邽�߂̌��J�v���p�e�B
    public static TargetMarkManager Instance
    {
        get
        {
            // �C���X�^���X���܂����݂��Ȃ��ꍇ�ɃG���[�𔭐�������
            if (_instance == null)
            {
                Debug.LogError("TargetMarkManager is not initialized. Please ensure the instance exists in the scene.");
                return null;
            }
            return _instance;
        }
    }

    // �V���O���g��������Ώۂ̃R���|�[�l���g
    [SerializeField] private TargetMarkController _targetMarkController;

    private void Awake()
    {
        // ���ɃC���X�^���X�����݂��A�����ꂪ�������g�ł͂Ȃ��ꍇ�́A�d�����Ă��邽�ߔj������
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            // �C���X�^���X���܂����݂��Ȃ��ꍇ�́A�������g���C���X�^���X�ɐݒ肷��
            _instance = this;
        }
    }

    private void OnDestroy()
    {
        // �C���X�^���X���j�������ۂɁA�ÓI�ϐ����N���A����
        if (_instance == this)
        {
            _instance = null;
        }
    }

    // --- �V�K�ǉ��̃��b�p�[���\�b�h ---

    /// <summary>
    /// �^�[�Q�b�g���Z�b�g���āA���b�N�I�����J�n���܂��B
    /// ���݃��b�N�I�����ł͂Ȃ��ꍇ�ɂ̂ݎ��s�\�ł��B
    /// </summary>
    /// <param name="target">���b�N�I������^�[�Q�b�g��Transform</param>
    public void StartLockOn(Transform target)
    {
        // TargetMarkController�����݂��Ȃ��A�܂��̓��b�N�I�����ł���΃G���[
        if (_targetMarkController == null)
        {
            Debug.LogError("TargetMarkController is not assigned to the TargetMarkManager.");
            return;
        }

        if (_targetMarkController.CurrentState != TargetMarkController.MarkState.Idle)
        {
            Debug.LogError("Cannot start new lock-on. A lock-on is already in progress. Please call ReleaseLockOn() first.");
            return;
        }

        _targetMarkController.SetTarget(target);
    }

    /// <summary>
    /// ���݂̃��b�N�I�����������܂��B
    /// ���b�N�I�����ł͂Ȃ��ꍇ�͉������܂���B
    /// </summary>
    public void ReleaseLockOn()
    {
        if (_targetMarkController == null)
        {
            Debug.LogError("TargetMarkController is not assigned to the TargetMarkManager.");
            return;
        }

        if (_targetMarkController.CurrentState != TargetMarkController.MarkState.Idle)
        {
            _targetMarkController.ReleaseLockOn();
        }
        else
        {
            Debug.LogWarning("ReleaseLockOn was called, but no lock-on was active.");
        }
    }
}