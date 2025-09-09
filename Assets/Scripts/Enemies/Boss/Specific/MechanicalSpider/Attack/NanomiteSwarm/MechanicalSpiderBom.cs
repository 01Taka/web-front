using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// Defines the different states or outcomes for the bomb.
/// </summary>
public enum BombState
{
    /// <summary>
    /// The bomb was destroyed because its health reached zero.
    /// </summary>
    DestroyedByHealth,

    /// <summary>
    /// The bomb reached its secondary target after being destroyed.
    /// </summary>
    TargetReachedAfterDestroyed,

    /// <summary>
    /// The bomb exploded because its internal timer expired.
    /// </summary>
    ExplodedByTimer
}

public class MechanicalSpiderBom : MonoBehaviour, IDamageable, IPoolable
{
    public  bool IsReusable { get; set; }

    private MechanicalSpiderBomSettings _settings;

    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    private Color _originalColor = Color.white;
    private HealthManager _healthManager;

    private UnityAction<BombState> _onBombStateChanged;

    // 外部から注入される移動ロジック
    private IBomMovement _movementStrategy;

    private float _baseExplosionTime;
    private float _explosionTime;
    private float _timeElapsed;
    private Vector2 _targetPosition;
    private Vector2 _targetPositionOnDestroyed;
    private bool _isDestroyed = false;

    private ObjectPool<MechanicalSpiderBom> _pool;

    public void SetPool<T>(ObjectPool<T> pool) where T : Component, IPoolable
    {
        if (pool is ObjectPool<MechanicalSpiderBom> bomPool)
        {
            _pool = bomPool;
        }
    }

    public void ReturnToPool()
    {
        // プールに戻る前に、アクティブな状態をリセットし、実行中のコルーチンを全て停止する
        StopAllCoroutines();

        if (_pool != null)
        {
            _pool.ReturnToPool(this);
        }

        // 念のため、参照をクリア
        _onBombStateChanged = null;
        _movementStrategy = null;
    }

    public void Activate(MechanicalSpiderBomSettings settings, float maxHealth, Vector2 target, Vector2 targetOnDestroyed,
        UnityAction<BombState> onBombStateChanged, IBomMovement movementStrategy)
    {
        _settings = settings;
        _baseExplosionTime = _settings.ExplosionTime;
        _explosionTime = _settings.ExplosionTime + Random.Range(0, _settings.ExplosionTimeRandomRange);
        _targetPosition = target;
        _targetPositionOnDestroyed = targetOnDestroyed;
        _timeElapsed = 0f;
        _isDestroyed = false;

        _onBombStateChanged = onBombStateChanged;
        // 移動ロジックを注入
        _movementStrategy = movementStrategy;

        _healthManager.SetMaxHealth(maxHealth);
        _spriteRenderer.color = _originalColor;

        StartCoroutine(StartTimerAndEffects());
    }

    private void Awake()
    {
        if (_spriteRenderer != null)
        {
            _originalColor = _spriteRenderer.color;
        }
        _healthManager = new HealthManager(100);
    }

    private void OnEnable()
    {
        // イベントを購読
        _healthManager?.AddOnDeathAction(StartSeekingOnDestroyed);
    }

    private void OnDisable()
    {
        // イベント購読を確実に解除
        _healthManager?.RemoveOnDeathAction(StartSeekingOnDestroyed);
        ReturnToPool();
    }

    private void Update()
    {
        if (!_isDestroyed)
        {
            transform.Rotate(0, 0, _settings.RotationSpeed * Time.deltaTime);
            _movementStrategy?.Move(transform, _targetPosition, _settings.SeekSpeed);
        }
        else
        {
            transform.Rotate(0, 0, _settings.RotationSpeedOnDestroyed * Time.deltaTime);
            transform.position = Vector2.MoveTowards(transform.position, _targetPositionOnDestroyed, _settings.SeekSpeedOnDestroyed * Time.deltaTime);

            if (Vector2.Distance(transform.position, _targetPositionOnDestroyed) < 0.1f)
            {
                InstantiateExplosion(ExplosionType.Default, _settings.ExplosionClipOnDestroy, _targetPositionOnDestroyed);
                _onBombStateChanged?.Invoke(BombState.TargetReachedAfterDestroyed);
                ReturnToPool();
            }
        }
    }

    private void InstantiateExplosion(ExplosionType explosionType, AudioClip explosionClip, Vector2 position)
    {
        Vector3 explosionPosition = (Vector3)position + (Vector3)Random.insideUnitCircle * _settings.ExplosionRadius;
        ExplosionEffectPoolManager.Instance.PlayExplosion(explosionPosition, 1, explosionType);

        if (explosionClip != null)
        {
            SoundManager.Instance.PlayEffect(explosionClip);
        }
    }

    public void TakeDamage(float amount)
    {
        _healthManager.TakeDamage(amount);

        if (_settings.HitClip)
        {
            SoundManager.Instance.PlayEffect(_settings.HitClip, _settings.HitClipVolume);
        }
    }

    private void StartSeekingOnDestroyed()
    {
        _isDestroyed = true;
        _spriteRenderer.color = _settings.BlinkColor;

        _onBombStateChanged?.Invoke(BombState.DestroyedByHealth);
    }

    private IEnumerator StartTimerAndEffects()
    {
        if (_spriteRenderer == null) yield break;

        _timeElapsed = 0f;
        int nextBlinkIndex = 0;
        List<float> blinkTimings = CalculateBlinkTimings();

        while (_timeElapsed < _explosionTime && _healthManager.IsAlive && !_isDestroyed)
        {
            _timeElapsed += Time.deltaTime;

            if (nextBlinkIndex < blinkTimings.Count && _timeElapsed >= blinkTimings[nextBlinkIndex])
            {
                _spriteRenderer.color = (_spriteRenderer.color == _originalColor) ? _settings.BlinkColor : _originalColor;
                nextBlinkIndex++;
            }

            yield return null;
        }

        // コルーチン終了時の状態をチェック
        if (_healthManager.IsAlive && !_isDestroyed)
        {
            // タイマー満了で爆発
            InstantiateExplosion(ExplosionType.Blue, _settings.BomClip, transform.position);
            _onBombStateChanged?.Invoke(BombState.ExplodedByTimer);
            ReturnToPool();
        }
        else
        {
            _isDestroyed = true;
        }
    }

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
}
