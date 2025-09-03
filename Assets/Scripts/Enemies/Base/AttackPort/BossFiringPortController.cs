using UnityEngine.Events;

using UnityEngine;

/// <summary>
/// 複数のコンポーネントを連携させてポートの挙動を制御
/// </summary>
public class BossFiringPortController : MonoBehaviour, IDamageable
{
    public IDamageable parentDamageable;

    public BossFiringPort portType;
    public bool IsOccupied { get; private set; }
    public bool IsPreparing { get; private set; }

    // ヘルパークラスのインスタンス
    private AttackTimer attackTimer;
    private DamageThresholdHandler damageHandler;

    // イベント
    public UnityEvent OnPreparationStarted;
    public UnityEvent OnAttackLaunched;
    public UnityEvent OnPreparationCancelled;
    public UnityEvent<float> OnDamageTaken; // 外部通知用

    private void Awake()
    {
        attackTimer = new AttackTimer();
        damageHandler = new DamageThresholdHandler();
    }

    private void Update()
    {
        attackTimer.Tick(Time.deltaTime);
    }

    public void StartPreparation(AttackPattern pattern, float cancelThreshold)
    {
        IsPreparing = true;
        IsOccupied = true;

        attackTimer.StartTimer(pattern.AttackPreparationTime);
        damageHandler.SetThreshold(cancelThreshold);

        attackTimer.OnTimerComplete += OnTimerComplete;
        damageHandler.OnThresholdReached += CancelPreparation;

        OnPreparationStarted?.Invoke();
    }

    // 外部からダメージしきい値を設定するためのメソッド
    public void SetDamageThreshold(float threshold)
    {
        damageHandler.SetThreshold(threshold);
    }

    private void OnTimerComplete()
    {
        if (IsPreparing)
        {
            OnAttackLaunched?.Invoke();
            // 攻撃実行ロジック
        }
        ResetPortState();
    }

    public void TakeDamage(int damage)
    {
        TakeDamage((float)damage);
    }

    public void TakeDamage(float damage)
    {
        if (!IsPreparing) return;

        if (parentDamageable != null)
        {
            parentDamageable.TakeDamage(damage);
        }

        damageHandler.AddDamage(damage);
        OnDamageTaken?.Invoke(damage);
    }

    public void CancelPreparation()
    {
        if (!IsPreparing) return;

        IsPreparing = false;
        Debug.Log($"{gameObject.name} の攻撃がキャンセルされました！");
        OnPreparationCancelled?.Invoke();
        ResetPortState();
    }

    private void ResetPortState()
    {
        IsPreparing = false;
        IsOccupied = false;

        attackTimer.OnTimerComplete -= OnTimerComplete;
        damageHandler.OnThresholdReached -= CancelPreparation;

        attackTimer.StopTimer();
        damageHandler.ResetDamage();
    }
}