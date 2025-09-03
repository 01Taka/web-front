using UnityEngine;

public class SoundManager : MonoBehaviour
{
    // �V���O���g���̃C���X�^���X
    public static SoundManager Instance { get; private set; }

    [SerializeField]
    private AudioSource _effectAudioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// ���ʉ����Đ�����
    /// </summary>
    /// <param name="clip">�Đ�����I�[�f�B�I�N���b�v</param>
    /// <param name="volume">�Đ����̉��ʁi0.0f����1.0f�j</param>
    public void PlayEffect(AudioClip clip, float volume = 1.0f)
    {
        if (clip != null && _effectAudioSource != null)
        {
            // PlayOneShot�̑������ŉ��ʂ��w��
            _effectAudioSource.PlayOneShot(clip, volume);
        }
    }
}