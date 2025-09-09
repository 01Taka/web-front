using UnityEngine;
using System.Collections;

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
    /// ���ʉ����Đ�����i�ʏ�Łj
    /// </summary>
    /// <param name="clip">�Đ�����I�[�f�B�I�N���b�v</param>
    /// <param name="volume">�Đ����̉��ʁi0.0f����1.0f�j</param>
    public void PlayEffect(AudioClip clip, float volume = 1.0f)
    {
        if (clip != null && _effectAudioSource != null)
        {
            _effectAudioSource.PlayOneShot(clip, volume);
        }
    }

    /// <summary>
    /// ���ʉ����Đ�����i�s�b�`�Ɖ��ʂ������_���ɒ����\�j
    /// </summary>
    /// <param name="clip">�Đ�����I�[�f�B�I�N���b�v</param>
    /// <param name="minVolume">���ʂ̍ŏ��l</param>
    /// <param name="maxVolume">���ʂ̍ő�l</param>
    /// <param name="minPitch">�s�b�`�̍ŏ��l</param>
    /// <param name="maxPitch">�s�b�`�̍ő�l</param>
    public void PlayEffect(AudioClip clip, float minVolume, float maxVolume, float minPitch, float maxPitch)
    {
        if (clip != null && _effectAudioSource != null)
        {
            // �����_���ȃs�b�`�Ɖ��ʂ�ݒ�
            float randomPitch = Random.Range(minPitch, maxPitch);
            float randomVolume = Random.Range(minVolume, maxVolume);

            // PlayOneShot�̑O��AudioSource�̃s�b�`��ݒ�
            _effectAudioSource.pitch = randomPitch;

            // PlayOneShot�ōĐ�
            _effectAudioSource.PlayOneShot(clip, randomVolume);

            // �Đ���A�s�b�`�����̏�Ԃɖ߂��i���̏����͎���PlayOneShot�܂ŗL���j
            _effectAudioSource.pitch = 1.0f;
        }
    }
}