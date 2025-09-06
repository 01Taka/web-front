using UnityEngine;

public class GameInitializer : MonoBehaviour
{
    [SerializeField] private GameObject _singletonObjectsPrefab;
    private bool _initialized = false;

    // �B��̃C���X�^���X���i�[����v���C�x�[�g�ȐÓI�ϐ�
    private static GameInitializer _instance;

    // �ǂ�����ł��A�N�Z�X�ł���B��̃C���X�^���X��Ԃ��ÓI�v���p�e�B
    public static GameInitializer Instance
    {
        get
        {
            // �C���X�^���X���܂����݂��Ȃ��ꍇ�A�V�[��������T���Ď擾����
            if (_instance == null)
            {
                _instance = FindAnyObjectByType<GameInitializer>();

                // ����ł�������Ȃ��ꍇ�A�x�����O���o��
                if (_instance == null)
                {
                    Debug.LogWarning("GameInitializer�̃C���X�^���X���V�[���Ɍ�����܂���B");
                }
            }
            return _instance;
        }
    }

    private void Awake()
    {
        // _instance���܂��ݒ肳��Ă��Ȃ��ꍇ�A���̃I�u�W�F�N�g���C���X�^���X�Ƃ��Đݒ�
        if (_instance == null)
        {
            _instance = this;
            // �V�[�����ׂ��ł��I�u�W�F�N�g���j������Ȃ��悤�ɐݒ�
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // ���ɃC���X�^���X�����݂���ꍇ�A�V�������̃I�u�W�F�N�g��j��
            // ���̃`�F�b�N���Ȃ��ƁA�V�[���ړ��Ȃǂŕ����̃C���X�^���X�����������\��������
            if (this != _instance)
            {
                Destroy(gameObject);
            }
        }
    }

    private void Start()
    {
        if (!_initialized)
        {
            Instantiate(_singletonObjectsPrefab);
            _initialized = true;
        }
    }
}