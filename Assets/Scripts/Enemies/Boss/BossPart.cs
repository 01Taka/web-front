using System.Collections;
using UnityEngine;

public class BossPart : MonoBehaviour, IDamageable
{
    // �X�N���v�^�u���I�u�W�F�N�g�ւ̎Q�Ƃ�ǉ�
    [SerializeField] private BossPartSettings _settings;

    // �X�v���C�g�����_���[�̎Q�Ƃ͂��̂܂ܕێ�
    [SerializeField] private SpriteRenderer _spriteRenderer;

    // �e��BaseBossManager�ւ̎Q��
    private BaseBossManager _bossManager;
    private Coroutine _damageEffectRoutine;
    private Coroutine _vibrationRoutine;

    private void Awake()
    {
        if (_settings == null)
        {
            Debug.LogError("BossPartSettings is not assigned on " + gameObject.name);
            return;
        }

        _bossManager = GetComponentInParent<BaseBossManager>();
        if (_bossManager == null)
        {
            Debug.LogError("BaseBossManager not found in parent objects.");
        }

        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
            if (_spriteRenderer == null)
            {
                Debug.LogWarning("SpriteRenderer not found. Visual effects will not work.");
            }
        }
    }

    public void TakeDamage(float amount)
    {
        // �X�N���v�^�u���I�u�W�F�N�g����ݒ�l��ǂݍ���
        if (_spriteRenderer != null)
        {
            if (_damageEffectRoutine != null)
            {
                StopCoroutine(_damageEffectRoutine);
            }
            _damageEffectRoutine = StartCoroutine(DamageColorRoutine(_settings.damageColorDuration));
        }

        // �X�N���v�^�u���I�u�W�F�N�g����臒l��ǂݍ���
        if (amount >= _settings.vibrationThreshold)
        {
            if (_vibrationRoutine != null)
            {
                StopCoroutine(_vibrationRoutine);
            }
            _vibrationRoutine = StartCoroutine(VibrationRoutine(_settings.vibrationDuration, _settings.vibrationMagnitude));
        }

        if (_bossManager != null)
        {
            // �X�N���v�^�u���I�u�W�F�N�g����_���[�W�{����ǂݍ���
            float totalDamage = amount * _settings.damageMultiplier;
            _bossManager.OnTakeDamage(totalDamage);
        }
    }

    public void PlayDamageAnimation(float amount)
    {
        // �X�v���C�g�̐F�ύX�R���[�`�����J�n
        if (_spriteRenderer != null)
        {
            if (_damageEffectRoutine != null)
            {
                StopCoroutine(_damageEffectRoutine);
            }
            _damageEffectRoutine = StartCoroutine(DamageColorRoutine(_settings.damageColorDuration));
        }

        // �U���R���[�`�����J�n
        if (amount >= _settings.vibrationThreshold)
        {
            if (_vibrationRoutine != null)
            {
                StopCoroutine(_vibrationRoutine);
            }
            _vibrationRoutine = StartCoroutine(VibrationRoutine(_settings.vibrationDuration, _settings.vibrationMagnitude));
        }
    }

    // �_���[�W���̓_�ŏ���
    private IEnumerator DamageColorRoutine(float duration)
    {
        Color originalColor = _spriteRenderer.color;
        _spriteRenderer.color = _settings.damageColor; // �X�N���v�^�u���I�u�W�F�N�g����F���擾
        yield return new WaitForSeconds(duration);
        _spriteRenderer.color = originalColor;
        _damageEffectRoutine = null;
    }

    // �_���[�W���̐U������
    private IEnumerator VibrationRoutine(float duration, float magnitude)
    {
        Vector3 originalPosition = transform.position;
        float elapsed = 0f;

        while (elapsed < duration)
        {
            float x = Random.Range(-1f, 1f) * magnitude;
            float y = Random.Range(-1f, 1f) * magnitude;
            transform.position = originalPosition + new Vector3(x, y, 0);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = originalPosition;
        _vibrationRoutine = null;
    }
}