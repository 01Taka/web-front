using UnityEngine;
using UnityEngine.Events;

public enum DeathReason
{
    PlayerDefeated,
    Exploded
}

[System.Serializable]
public class DeathEvent : UnityEvent<DeathReason> { }

/// <summary>
/// ボスのお供（Horde）の敵キャラクターの挙動を制御するクラス。
/// HealthManagerとIDamageableインターフェースを実装する。
/// </summary>
public class HordeEnemy : MonoBehaviour, IDamageable
{
    [Tooltip("敵の設定を格納したScriptableObject")]
    [SerializeField]
    private HordeEnemySettings _settings;

    // 親に通知するための死亡イベント。引数に死亡理由を含む。
   [SerializeField] public DeathEvent _onHordeDeath;

    // ヘルス管理クラスのインスタンス
    private HealthManager _healthManager;

    // プレイヤーのTransform（追跡用）
    private Transform _playerTransform;

    /// <summary>
    /// コンポーネントが有効になったときに呼び出される。
    /// HealthManagerの初期化とイベント登録を行う。
    /// </summary>
    private void OnEnable()
    {
        if (_settings == null)
        {
            Debug.LogError("HordeEnemySettingsが設定されていません。", this);
            return;
        }

        InitializeHealthManager();
        SetSize(_settings.enemyScale);
    }

    public void AddDeathAction(UnityAction<DeathReason> action)
    {
        _onHordeDeath.AddListener(action);
    }

    /// <summary>
    /// ターゲットを設定するメソッド。HordeSpawnerから呼び出される。
    /// </summary>
    public void SetTarget(Transform target)
    {
        _playerTransform = target;
    }

    /// <summary>
    /// 大きさを設定するメソッド。外部から大きさを動的に変更できる。
    /// </summary>
    public void SetSize(float scale)
    {
        transform.localScale = new Vector3(scale, scale, 1.0f);
    }

    /// <summary>
    /// Update is called once per frame
    /// </summary>
    private void Update()
    {
        // プレイヤーを追跡する基本的なロジック
        if (_playerTransform != null && _healthManager.IsAlive)
        {
            // ターゲットに体を向ける
            RotateTowardsTarget();

            // ターゲットに向かって移動
            Vector2 direction = (_playerTransform.position - transform.position).normalized;
            transform.Translate(direction * _settings.moveSpeed * Time.deltaTime, Space.World);

            // ターゲットとの距離をチェックし、一定距離まで近づいたら自爆
            if (Vector2.Distance(transform.position, _playerTransform.position) <= _settings.explosionDistance)
            {
                Explode();
            }
        }
    }

    /// <summary>
    /// ターゲットの方向に向かって体を回転させる。
    /// </summary>
    private void RotateTowardsTarget()
    {
        Vector3 direction = _playerTransform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle + _settings.adjustAngle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _settings.moveSpeed * Time.deltaTime);
    }


    /// <summary>
    /// HealthManagerの初期化とイベントの紐づけを行う。
    /// </summary>
    private void InitializeHealthManager()
    {
        _healthManager = new HealthManager(_settings.maxHealth);
    }

    /// <summary>
    /// IDamageableインターフェースの実装。
    /// 外部からダメージを受けるために呼び出される。
    /// </summary>
    public void TakeDamage(float amount)
    {
        if (!_healthManager.IsAlive) return;

        // ダメージ音を再生
        if (SoundManager.Instance != null && _settings.damageSound != null)
        {
            SoundManager.Instance.PlayEffect(_settings.damageSound);
        }

        float previousHealth = _healthManager.CurrentHealth;
        _healthManager.TakeDamage(amount);

        if (!_healthManager.IsAlive && previousHealth > 0)
        {
            OnPlayerDefeated();
        }
    }

    /// <summary>
    /// 自爆処理。
    /// </summary>
    private void Explode()
    {
        if (!_healthManager.IsAlive) return;

        if (SoundManager.Instance != null && _settings.explodeSound != null)
        {
            SoundManager.Instance.PlayEffect(_settings.explodeSound);
        }

        _healthManager.Kill();
        // 自爆イベントを発火
        _onHordeDeath?.Invoke(DeathReason.Exploded);
    }

    /// <summary>
    /// プレイヤーによって倒されたときに呼ばれるメソッド。
    /// </summary>
    private void OnPlayerDefeated()
    {
        if (SoundManager.Instance != null && _settings.destroySound != null)
        {
            SoundManager.Instance.PlayEffect(_settings.destroySound);
        }
        // プレイヤーに倒されたイベントを発火
        _onHordeDeath?.Invoke(DeathReason.PlayerDefeated);
    }

    /// <summary>
    /// スクリプトが破棄されるときに呼び出される。
    /// 不要なリスナーを削除してメモリリークを防ぐ。
    /// </summary>
    private void OnDestroy()
    {
        if (_healthManager != null)
        {
            _onHordeDeath.RemoveAllListeners();
        }
    }
}