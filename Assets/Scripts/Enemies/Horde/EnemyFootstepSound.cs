using UnityEngine;
using System.Collections;

public class EnemyFootstepSound : MonoBehaviour
{
    // === Public Variables ===
    public AudioClip[] footstepClips; // �ݒ�\�ȕ����̑����N���b�v
    public float minPitch = 0.9f;     // �s�b�`�̍ŏ��l
    public float maxPitch = 1.1f;     // �s�b�`�̍ő�l
    public float minVolume = 0.8f;    // ���ʂ̍ŏ��l
    public float maxVolume = 1.0f;    // ���ʂ̍ő�l
    public float minInterval = 0.4f;  // �����̊Ԋu�̍ŏ��l
    public float maxInterval = 0.6f;  // �����̊Ԋu�̍ő�l

    // === Private Variables ===
    private Coroutine footstepCoroutine;

    /// <summary>
    /// �����̍Đ����J�n����
    /// </summary>
    public void StartFootsteps()
    {
        // ���ɃR���[�`�������s���Ȃ�A���������~���čĊJ����
        if (footstepCoroutine != null)
        {
            StopCoroutine(footstepCoroutine);
        }
        footstepCoroutine = StartCoroutine(PlayFootstepsLoop());
    }

    /// <summary>
    /// �����̍Đ����~����
    /// </summary>
    public void StopFootsteps()
    {
        if (footstepCoroutine != null)
        {
            StopCoroutine(footstepCoroutine);
            footstepCoroutine = null;
        }
    }

    /// <summary>
    /// �����������_���ȊԊu�ŌJ��Ԃ��Đ�����R���[�`��
    /// </summary>
    private IEnumerator PlayFootstepsLoop()
    {
        while (true)
        {
            // �����̍Đ��Ԋu�������_���Ɍ���
            float interval = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(interval);

            // �����N���b�v���ݒ肳��Ă��邩�m�F
            if (footstepClips.Length == 0)
            {
                Debug.LogWarning("Footstep clips are not assigned in the inspector.");
                yield break; // �x����\�����ăR���[�`�����I��
            }

            // �����_���ȑ����N���b�v��I��
            AudioClip randomClip = footstepClips[Random.Range(0, footstepClips.Length)];

            // SoundManager��ʂ��đ������Đ�
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayEffect(randomClip, minVolume, maxVolume, minPitch, maxPitch);
            }
        }
    }
}