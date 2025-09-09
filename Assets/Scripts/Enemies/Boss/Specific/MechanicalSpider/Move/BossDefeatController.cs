using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

public class BossDefeatController : MonoBehaviour
{
    [Header("�ݒ�")]
    [Tooltip("���j���o�̐ݒ���܂Ƃ߂�ScriptableObject")]
    [SerializeField] private BossDefeatSettings _settings;

    private ScreenShake _screenShake;

    [Header("�C�x���g")]
    [Tooltip("���j���o�����������Ƃ��ɌĂяo�����C�x���g")]
    public UnityEvent OnDefeatCompleted = new UnityEvent();

    /// <summary>
    /// ���j���o���J�n����
    /// </summary>
    public void StartDefeatSequence()
    {
        if (SceneComponentManager.Instance.GameCamera.TryGetComponent<ScreenShake>(out var screenShake))
        {
            _screenShake = screenShake;
        }

        if (_settings == null)
        {
            Debug.LogError("BossDefeatSettings���ݒ肳��Ă��܂���B", this);
            return;
        }

        StartCoroutine(DefeatSequenceCoroutine());
    }

    private IEnumerator DefeatSequenceCoroutine()
    {
        // �{�X�̓����蔻��ƕ`��𖳌��ɂ���
        if (TryGetComponent(out Collider2D collider))
        {
            collider.enabled = false;
        }
        if (TryGetComponent(out Renderer renderer))
        {
            renderer.enabled = false;
        }

        // ������������A���ōĐ�
        for (int i = 0; i < _settings.numberOfSmallExplosions; i++)
        {
            // �{�X�̎���̃����_���Ȉʒu���擾
            float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            float radius = UnityEngine.Random.Range(0f, _settings.explosionEffectRadius);

            Vector3 randomPosition = transform.position + new Vector3(
                radius * Mathf.Cos(angle),
                radius * Mathf.Sin(angle),
                _settings.explosionEffectZ
            );

            if (SoundManager.Instance != null && _settings.smallExplosionClip != null)
            {
                SoundManager.Instance.PlayEffect(_settings.smallExplosionClip);
            }
            // �������Đ�
            ExplosionEffectPoolManager.Instance.PlayExplosion(randomPosition, 1.0f, _settings.smallExplosionType);
            yield return new WaitForSeconds(_settings.smallExplosionInterval);
        }

        // �Ō�̑傫�Ȕ���
        if (SoundManager.Instance != null && _settings.finalExplosionClip != null)
        {
            SoundManager.Instance.PlayEffect(_settings.finalExplosionClip);
        }

        // �Ō�̔������Đ�
        ExplosionEffectPoolManager.Instance.PlayExplosion(transform.position, 2.0f, _settings.finalExplosionType);

        // ��ʂ�h�炷
        if (_screenShake != null && _settings.finalShakeSettings != null)
        {
            _screenShake.StartShake(_settings.finalShakeSettings);
        }

        // �����̉��o���ԕ��ҋ@
        yield return new WaitForSeconds(_settings.finalExplosionDuration);

        // �C�x���g���Ăяo��
        OnDefeatCompleted?.Invoke();

        // �I�u�W�F�N�g��j��
        Destroy(gameObject);
    }

    // �I�u�W�F�N�g���j�������O�ɃC�x���g���X�i�[������
    private void OnDestroy()
    {
        OnDefeatCompleted.RemoveAllListeners();
    }
}