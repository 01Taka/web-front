using System.Collections;
using UnityEngine;

public class BossPart : MonoBehaviour, IDamageable
{
    // スクリプタブルオブジェクトへの参照を追加
    [SerializeField] private BossPartSettings _settings;

    // スプライトレンダラーの参照はそのまま保持
    [SerializeField] private SpriteRenderer _spriteRenderer;

    // 親のBaseBossManagerへの参照
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
        // スクリプタブルオブジェクトから設定値を読み込む
        if (_spriteRenderer != null)
        {
            if (_damageEffectRoutine != null)
            {
                StopCoroutine(_damageEffectRoutine);
            }
            _damageEffectRoutine = StartCoroutine(DamageColorRoutine(_settings.damageColorDuration));
        }

        // スクリプタブルオブジェクトから閾値を読み込む
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
            // スクリプタブルオブジェクトからダメージ倍率を読み込む
            float totalDamage = amount * _settings.damageMultiplier;
            _bossManager.OnTakeDamage(totalDamage);
        }
    }

    public void PlayDamageAnimation(float amount)
    {
        // スプライトの色変更コルーチンを開始
        if (_spriteRenderer != null)
        {
            if (_damageEffectRoutine != null)
            {
                StopCoroutine(_damageEffectRoutine);
            }
            _damageEffectRoutine = StartCoroutine(DamageColorRoutine(_settings.damageColorDuration));
        }

        // 振動コルーチンを開始
        if (amount >= _settings.vibrationThreshold)
        {
            if (_vibrationRoutine != null)
            {
                StopCoroutine(_vibrationRoutine);
            }
            _vibrationRoutine = StartCoroutine(VibrationRoutine(_settings.vibrationDuration, _settings.vibrationMagnitude));
        }
    }

    // ダメージ時の点滅処理
    private IEnumerator DamageColorRoutine(float duration)
    {
        Color originalColor = _spriteRenderer.color;
        _spriteRenderer.color = _settings.damageColor; // スクリプタブルオブジェクトから色を取得
        yield return new WaitForSeconds(duration);
        _spriteRenderer.color = originalColor;
        _damageEffectRoutine = null;
    }

    // ダメージ時の振動処理
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