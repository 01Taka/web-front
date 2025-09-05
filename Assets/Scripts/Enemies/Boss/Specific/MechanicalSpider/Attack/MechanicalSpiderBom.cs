using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class MechanicalSpiderBom : MonoBehaviour, IDamageable
{
    // --- フィールド ---
    [Tooltip("このスパイダーの設定を定義するScriptableObject")]
    [SerializeField]
    private MechanicalSpiderBomSettings _settings;

    [Header("イベントと参照")]
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    [Tooltip("爆発時に呼ばれるイベント")]
    [SerializeField]
    private UnityEvent _onExplosion;
    [Tooltip("目標に到達した時に呼ばれるイベント")]
    [SerializeField]
    private UnityEvent _onTargetReached;

    private Color _originalColor = Color.white;
    private HealthManager _healthManager;
    private UnityAction _explosionAction;
    private UnityAction _targetReachedAction;

    private float _baseExplosionTime;
    private float _explosionTime;
    private float _timeElapsed;
    private Vector2 _targetPosition;
    private Vector2 _targetPositionOnDestroyed;
    private bool _isDestroyed = false;

    // --- メソッド ---

    /// <summary>
    /// 爆弾を起動するメソッド。
    /// </summary>
    /// <param name="explosionDuration">爆発までの時間</param>
    /// <param name="maxHealth">最大HP</param>
    /// <param name="target">破壊前の追跡目標</param>
    /// <param name="targetOnDestroyed">破壊後の追跡目標</param>
    /// <param name="onExplosionAction">爆発時に実行されるアクション</param>
    /// <param name="onTargetReachedAction">目標到達時に実行されるアクション</param>
    public void Activate(float explosionDuration, float maxHealth, Vector2 target, Vector2 targetOnDestroyed,
                         UnityAction onExplosionAction, UnityAction onTargetReachedAction)
    {
        _baseExplosionTime = explosionDuration;
        _explosionTime = explosionDuration + Random.Range(0, _settings.ExplosionTimeRandomRange);
        _targetPosition = target;
        _targetPositionOnDestroyed = targetOnDestroyed;
        _timeElapsed = 0f;

        _explosionAction = onExplosionAction;
        _targetReachedAction = onTargetReachedAction;

        _onExplosion.AddListener(_explosionAction);
        _onTargetReached.AddListener(_targetReachedAction);

        _healthManager.SetMaxHealth(maxHealth);

        StartCoroutine(StartTimerAndEffects());
    }

    private void Awake()
    {
        if (_spriteRenderer != null)
        {
            _originalColor = _spriteRenderer.color;

            _healthManager = new HealthManager(100);
            _healthManager.AddOnDeathAction(StartSeekingOnDestroyed);
        }
    }

    private void Update()
    {
        if (!_isDestroyed)
        {
            transform.Rotate(0, 0, _settings.RotationSpeed * Time.deltaTime);
            transform.position = Vector2.MoveTowards(transform.position, _targetPosition, _settings.SeekSpeed * Time.deltaTime);
        }
        else
        {
            transform.Rotate(0, 0, _settings.RotationSpeedOnDestroyed * Time.deltaTime);
            transform.position = Vector2.MoveTowards(transform.position, _targetPositionOnDestroyed, _settings.SeekSpeedOnDestroyed * Time.deltaTime);

            if (Vector2.Distance(transform.position, _targetPositionOnDestroyed) < 0.1f)
            {
                InstantiateExplosion(_settings.ExplosionPrefabOnDestroy, _settings.ExplosionClipOnDestroy, _targetPositionOnDestroyed);
                _onTargetReached.Invoke();
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// 爆発エフェクトとサウンドを生成する汎用メソッド
    /// </summary>
    private void InstantiateExplosion(GameObject explosionPrefab, AudioClip explosionClip, Vector2 position)
    {
        if (explosionPrefab != null)
        {
            Vector3 explosionPosition = (Vector3)position + (Vector3)Random.insideUnitCircle * _settings.ExplosionRadius;
            Instantiate(explosionPrefab, explosionPosition, Quaternion.identity);
        }

        if (explosionClip != null)
        {
            SoundManager.Instance.PlayEffect(explosionClip);
        }
    }

    public void TakeDamage(float amount)
    {
        _healthManager.TakeDamage(amount);
        if (_healthManager.CurrentHealth == 0)
        {
            StartSeekingOnDestroyed();
        }
    }

    private void StartSeekingOnDestroyed()
    {
        _isDestroyed = true;
        StopAllCoroutines();
        _spriteRenderer.color = _settings.BlinkColor;
    }

    private IEnumerator StartTimerAndEffects()
    {
        if (_spriteRenderer == null) yield break;

        _timeElapsed = 0f;
        int nextBlinkIndex = 0;
        List<float> blinkTimings = CalculateBlinkTimings();

        while (_timeElapsed < _explosionTime && _healthManager.IsAlive)
        {
            _timeElapsed += Time.deltaTime;

            if (nextBlinkIndex < blinkTimings.Count && _timeElapsed >= blinkTimings[nextBlinkIndex])
            {
                _spriteRenderer.color = (_spriteRenderer.color == _originalColor) ? _settings.BlinkColor : _originalColor;
                nextBlinkIndex++;
            }

            yield return null;
        }

        if (_healthManager.IsAlive)
        {
            InstantiateExplosion(_settings.ExplosionPrefab, _settings.BomClip, transform.position);
            _onExplosion.Invoke();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 点滅タイミングを事前に計算するヘルパーメソッド
    /// </summary>
    private List<float> CalculateBlinkTimings()
    {
        List<float> timings = new List<float>();
        float currentTime = 0f;

        while (currentTime < _baseExplosionTime)
        {
            float normalizedTime = currentTime / _baseExplosionTime;
            float blinkInterval = Mathf.Lerp(_settings.StartBlinkInterval, _settings.EndBlinkInterval, normalizedTime);

            timings.Add(currentTime);
            currentTime += blinkInterval;
            if (currentTime < _baseExplosionTime)
            {
                timings.Add(currentTime);
                currentTime += blinkInterval;
            }
        }
        return timings;
    }

    private void OnDestroy()
    {
        if (_onExplosion != null && _explosionAction != null)
        {
            _onExplosion.RemoveListener(_explosionAction);
        }

        if (_onTargetReached != null && _targetReachedAction != null)
        {
            _onTargetReached.RemoveListener(_targetReachedAction);
        }

        if (_healthManager != null)
        {
            _healthManager.ClearEvents();
        }
    }
}