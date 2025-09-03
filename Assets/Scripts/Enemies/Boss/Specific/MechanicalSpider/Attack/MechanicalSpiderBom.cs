using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class MechanicalSpiderBom : MonoBehaviour, IDamageable
{
    // --- フィールド ---
    [Header("回転設定")]
    [Tooltip("Z軸周りの回転速度 (度/秒)")]
    [SerializeField]
    private float _rotationSpeed = 100f;

    [Header("点滅設定")]
    [Tooltip("点滅の規定色")]
    [SerializeField]
    private Color _blinkColor = Color.red;
    [Tooltip("点滅の開始頻度 (秒)")]
    [SerializeField]
    private float _startBlinkInterval = 0.5f;
    [Tooltip("点滅の終了頻度 (秒)")]
    [SerializeField]
    private float _endBlinkInterval = 0.1f;
    [Header("スプライト設定")]
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;

    [Header("爆発設定")]
    [SerializeField]
    private AudioClip _bomClip;
    [Tooltip("爆発時に呼ばれるイベント")]
    [SerializeField]
    private UnityEvent _onExplosion;
    private float _explosionTime;
    private float _timeElapsed;

    // 新しいフィールド: 爆発プレハブ
    [Tooltip("爆発時に生成するプレハブ")]
    [SerializeField]
    private GameObject _explosionPrefab;

    // 新しいフィールド: 爆発までの時間ランダム化
    [Tooltip("爆発までの時間に加算するランダム値の範囲 (秒)")]
    [SerializeField]
    private float _explosionTimeRandomRange = 0.5f;

    // 新しいフィールド: 爆発位置ランダム化
    [Tooltip("爆発時にターゲット座標からランダムにずらす半径")]
    [SerializeField]
    private float _explosionRadius = 0.5f;

    // 外部から渡されたリスナーを保持するためのプライベートフィールド
    private UnityAction _explosionAction;

    // 新しいフィールド
    [Header("追跡とHP設定")]
    [Tooltip("追跡速度")]
    [SerializeField]
    private float _seekSpeed = 5f;

    private Vector2 _targetPosition;

    // 新しいUnityEvent
    [Header("目標到達イベント")]
    [Tooltip("目標に到達した時に呼ばれるイベント")]
    [SerializeField]
    private UnityEvent _onTargetReached;

    // 外部から渡されたリスナーを保持するためのプライベートフィールド
    private UnityAction _targetReachedAction;

    // HealthManagerのインスタンス
    private HealthManager _healthManager;

    // --- メソッド ---

    /// <summary>
    /// 爆弾を起動するメソッド。
    /// </summary>
    /// <param name="explosionDuration">爆発までの時間</param>
    /// <param name="maxHealth">最大HP</param>
    /// <param name="target">追跡目標</param>
    /// <param name="onExplosionAction">爆発時に実行されるアクション</param>
    /// <param name="onTargetReachedAction">目標到達時に実行されるアクション</param>
    public void Activate(float explosionDuration, float maxHealth, Vector2 target,
                         UnityAction onExplosionAction, UnityAction onTargetReachedAction)
    {
        // 爆発までの時間にランダムな値を加算
        _explosionTime = explosionDuration + Random.Range(0, _explosionTimeRandomRange);
        _targetPosition = target;
        _timeElapsed = 0f;

        _healthManager.SetMaxHealth(maxHealth);

        // リスナーをフィールドに保持
        _explosionAction = onExplosionAction;
        _targetReachedAction = onTargetReachedAction;

        // 外部から渡されたリスナーをイベントに登録
        _onExplosion.AddListener(_explosionAction);
        _onTargetReached.AddListener(_targetReachedAction);

        // 点滅と爆発のコルーチンを開始
        StartCoroutine(BlinkEffectCoroutine());
        StartCoroutine(ExplosionCoroutine());
    }

    private void Awake()
    {
        // HealthManagerを初期化
        _healthManager = new HealthManager(100);
        // HPが0になったらターゲットを追跡する処理を登録
        _healthManager.AddOnDeathAction(StartSeeking);
    }

    private void Start()
    {
        if (_spriteRenderer != null)
        {
            _originalColor = _spriteRenderer.color;
        }
    }

    private void Update()
    {
        // Z軸周りの回転
        transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime);

        // 追跡状態の場合、ターゲットに向かって移動
        if (!_healthManager.IsAlive)
        {
            // ターゲットが存在する場合
            if (_targetPosition != null)
            {
                // ターゲットに向かって移動
                transform.position = Vector2.MoveTowards(transform.position, _targetPosition, _seekSpeed * Time.deltaTime);

                // ターゲットに十分に近づいたらイベントを起動し、オブジェクトを破壊
                if (Vector2.Distance(transform.position, _targetPosition) < 0.1f)
                {
                    // 爆発位置にランダムなオフセットを加える
                    Vector3 explosionPosition = (Vector3)_targetPosition + (Vector3)Random.insideUnitCircle * _explosionRadius;

                    // 爆発プレハブを生成
                    if (_explosionPrefab != null)
                    {
                        Instantiate(_explosionPrefab, explosionPosition, Quaternion.identity);
                    }

                    _onTargetReached.Invoke();
                    PlayExplosionSound();
                    Destroy(gameObject);
                }
            }
        }
    }

    private void PlayExplosionSound()
    {
        SoundManager.Instance.PlayEffect(_bomClip);
    }

    /// <summary>
    /// ダメージを受けるメソッド
    /// </summary>
    /// <param name="amount">ダメージ量</param>
    public void TakeDamage(float amount)
    {
        
        _healthManager.TakeDamage(amount);
    }

    private void StartSeeking()
    {
        // 爆発のコルーチンを停止
        StopCoroutine(ExplosionCoroutine());
        // 点滅のコルーチンを停止
        StopCoroutine(BlinkEffectCoroutine());
        // 視覚的な変化
        _spriteRenderer.color = _blinkColor;
        _rotationSpeed = 300f; // 回転速度を上げるなど
    }

    /// <summary>
    /// 点滅エフェクトを制御するコルーチン。
    /// </summary>
    private IEnumerator BlinkEffectCoroutine()
    {
        // SpriteRendererが存在しない場合は終了
        if (_spriteRenderer == null) yield break;

        while (_timeElapsed < _explosionTime && _healthManager.IsAlive)
        {
            // 経過時間に基づいて点滅頻度を計算
            float normalizedTime = _timeElapsed / _explosionTime;
            float currentBlinkInterval = Mathf.Lerp(_startBlinkInterval, _endBlinkInterval, normalizedTime);

            // 色を_blinkColorに変更
            _spriteRenderer.color = _blinkColor;
            yield return new WaitForSeconds(currentBlinkInterval);

            // 色を白(#FFFFFF)に変更
            _spriteRenderer.color = _originalColor;
            yield return new WaitForSeconds(currentBlinkInterval);
        }
    }

    /// <summary>
    /// 爆発までの時間をカウントダウンし、イベントを起動するコルーチン。
    /// </summary>
    private IEnumerator ExplosionCoroutine()
    {
        // 経過時間をカウント
        while (_timeElapsed < _explosionTime && _healthManager.IsAlive)
        {
            _timeElapsed += Time.deltaTime;
            yield return null;
        }

        // 爆弾の体力が残っている場合は爆発
        if (_healthManager.IsAlive)
        {
            // 爆発プレハブを生成
            if (_explosionPrefab != null)
            {
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            }

            PlayExplosionSound();
            _onExplosion.Invoke();
            
            Destroy(gameObject);
        }
    }

    // オブジェクトが破棄されるときに呼ばれる
    private void OnDestroy()
    {
        // UnityEventからリスナーを解除
        // Nullチェックは、オブジェクトが先に破棄される場合があるため必須
        if (_onExplosion != null && _explosionAction != null)
        {
            _onExplosion.RemoveListener(_explosionAction);
        }

        if (_onTargetReached != null && _targetReachedAction != null)
        {
            _onTargetReached.RemoveListener(_targetReachedAction);
        }

        // HealthManagerのリスナーも解除
        if (_healthManager != null)
        {
            _healthManager.ClearEvents();
        }
    }
}