using UnityEngine.Events;

using UnityEngine;

/// <summary>
/// �����̃R���|�[�l���g��A�g�����ă|�[�g�̋����𐧌�
/// </summary>
public class BossFiringPortController : MonoBehaviour, IDamageable
{
    public IDamageable parentDamageable;

    public BossFiringPort portType;
    public bool IsOccupied { get; private set; }
    public bool IsPreparing { get; private set; }

    // �w���p�[�N���X�̃C���X�^���X
    private AttackTimer attackTimer;
    private DamageThresholdHandler damageHandler;

    // �C�x���g
    public UnityEvent OnPreparationStarted;
    public UnityEvent OnAttackLaunched;
    public UnityEvent OnPreparationCancelled;
    public UnityEvent<float> OnDamageTaken; // �O���ʒm�p

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

    // �O������_���[�W�������l��ݒ肷�邽�߂̃��\�b�h
    public void SetDamageThreshold(float threshold)
    {
        damageHandler.SetThreshold(threshold);
    }

    private void OnTimerComplete()
    {
        if (IsPreparing)
        {
            OnAttackLaunched?.Invoke();
            // �U�����s���W�b�N
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
        Debug.Log($"{gameObject.name} �̍U�����L�����Z������܂����I");
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