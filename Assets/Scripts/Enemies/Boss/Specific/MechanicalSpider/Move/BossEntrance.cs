using UnityEngine;
using System.Collections;

public class BossEntrance : MonoBehaviour
{
    private BossEntranceSettings _settings;
    private Coroutine _entranceCoroutine;
    private float _landingYPosition;

    private ScreenShake _screenShake;

    /// <summary>
    /// �{�X�̏o�����o�����������ĊJ�n����
    /// </summary>
    /// <param name="settings">�o�����o�̐ݒ�</param>
    /// <param name="landingYPosition">�{�X�̒��n�ʒu��Y���W</param>
    public void Initialize(BossEntranceSettings settings, float landingYPosition, ScreenShake screenShake)
    {
        _settings = settings;
        _landingYPosition = landingYPosition;
        _screenShake = screenShake;

        if (_settings == null)
        {
            Debug.LogError("BossEntranceSettings���ݒ肳��Ă��܂���B");
            return;
        }
        if (SoundManager.Instance == null)
        {
            Debug.LogError("SoundManager�̃C���X�^���X��������܂���B");
            return;
        }

        if (_entranceCoroutine != null)
        {
            StopCoroutine(_entranceCoroutine);
        }

        // �W�����v�J�n�ʒu��ݒ�
        Vector3 jumpStartPosition = transform.position;
        jumpStartPosition.y = _landingYPosition + _settings.JumpHeight;
        transform.position = jumpStartPosition;

        _entranceCoroutine = StartCoroutine(EntranceSequenceCoroutine());
    }

    private IEnumerator EntranceSequenceCoroutine()
    {
        // �����𕡐���Đ�
        for (int i = 0; i < _settings.NumberOfFootsteps; i++)
        {
            if (_settings.FootstepClips != null && _settings.FootstepClips.Length > 0)
            {
                AudioClip randomFootstepClip = _settings.FootstepClips[Random.Range(0, _settings.FootstepClips.Length)];
                SoundManager.Instance.PlayEffect(randomFootstepClip);
            }
            yield return new WaitForSeconds(_settings.FootstepInterval);
        }

        float remainingTime = _settings.JumpDuration - _settings.JumpMoveDuration;
        if (remainingTime > 0)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        // �W�����v�̈ړ����J�n
        float moveElapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(transform.position.x, _landingYPosition, transform.position.z);

        while (moveElapsedTime < _settings.JumpMoveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, moveElapsedTime / _settings.JumpMoveDuration);
            moveElapsedTime += Time.deltaTime;
            yield return null;
        }

        // �ŏI�ʒu��ۏ�
        transform.position = endPosition;

        // ���n�����Đ�
        if (_settings.LandingClip != null)
        {
            SoundManager.Instance.PlayEffect(_settings.LandingClip);
        }

        // ���n��̐U�����o���J�n
        if (_screenShake != null)
        {
            _screenShake.StartShake(_settings.ScreenShakeSettings);
        }
        StartCoroutine(BossShakeCoroutine(0.2f, 0.05f));
    }

    /// <summary>
    /// �{�X�̃g�����X�t�H�[����h�炷�R���[�`��
    /// </summary>
    private IEnumerator BossShakeCoroutine(float shakeDuration, float shakeMagnitude)
    {
        Vector3 originalPosition = transform.localPosition;
        float timer = 0f;

        while (timer < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            timer += Time.deltaTime;
            yield return null;
        }

        // �ŏI�ʒu�����̈ʒu�ɖ߂�
        transform.localPosition = originalPosition;
    }
}