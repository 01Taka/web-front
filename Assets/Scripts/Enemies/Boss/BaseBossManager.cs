using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseBossManager : MonoBehaviour
{
    [Header("Boss Settings")]
    [SerializeField] private BossSettings _bossSettings;
    [SerializeField] private BossPart bodyPart;
    [SerializeField] private AudioClip hitClip;
    [SerializeField][Range(0, 1f)] private float hitVolum = 1f;
    [SerializeField] BossDefeatController _bossDefeatController;

    private int _currentPhaseIndex = 0;
    [SerializeField] private bool _resetHealthOnPhaseChange = false;

    private HealthManager _healthManager;
    private BossAttackController _attackController;

    private UnityEvent _onDeathEvent;
    private UnityEvent<int> _onTransitionPhaseEvent;

    public int CurrentPhaseIndex => _currentPhaseIndex;
    public BossPhaseSettings CurrentPhaseSettings => _bossSettings.phaseSettings[_currentPhaseIndex];
    public HealthManager HealthManager => _healthManager;
    public float RemainingHealthRatio => _healthManager.MaxHealth > 0 ? _healthManager.CurrentHealth / _healthManager.MaxHealth : -1;
    public Vector3 Position => transform.position;
    public bool IsAlive => _healthManager.IsAlive || _currentPhaseIndex != _bossSettings.GetPhaseCount() - 1;

    protected virtual void Awake()
    {
        if (_bossSettings == null || _bossSettings.phaseSettings.Length == 0)
        {
            Debug.LogError("BossSettings is not assigned or is empty.");
            return;
        }

        _onDeathEvent = new UnityEvent();
        _onTransitionPhaseEvent = new UnityEvent<int>();

        _healthManager = new HealthManager(_bossSettings.maxHealth);
        _healthManager.AddOnDeathAction(OnDefeated);

        // BossAttackControllerを初期化
        _attackController = GetComponent<BossAttackController>();
        if (_attackController == null)
        {
            _attackController = gameObject.AddComponent<BossAttackController>();
        }
        _attackController.Initialize(this, CurrentPhaseSettings);

        StartCoroutine(StartPhaseRoutine());
    }

    public void AddOnDeathAction(UnityAction action)
    {
        _bossDefeatController.OnDefeatCompleted.AddListener(action);
    }

    public void AddOnTransitionPhaseAction(UnityAction<int> action)
    {
        _onTransitionPhaseEvent.AddListener(action);
    }

    public void AddOnHealthChangeAction(UnityAction<float> action)
    {
        _healthManager.AddOnHealthChangeAction(action);
    }

    // BossPartからダメージが呼び出される
    public void TakeDamage(float amount)
    {
        OnTakeDamage(amount);
        bodyPart.PlayDamageAnimation(amount);
    }

    public void OnTakeDamage(float amount)
    {
        SoundManager.Instance.PlayEffect(hitClip, hitVolum);

        _healthManager.TakeDamage(amount);

        // ボスの体力が減った後、フェーズ移行の条件をチェック
        if (_currentPhaseIndex < _bossSettings.GetPhaseCount() - 1)
        {
            // 攻撃中でなければフェーズ移行を許可
            if (!_attackController.IsAttacking)
            {
                float nextPhaseTransitionPercent = CurrentPhaseSettings.phaseTransitionHealthPercentage;
                float healthRatio = _healthManager.CurrentHealth / _bossSettings.maxHealth;

                if (healthRatio <= nextPhaseTransitionPercent)
                {
                    TransitionToNextPhase();
                    return;
                }
            }
        }

        // 最終フェーズの死亡判定、または移行条件を満たさなかった場合の死亡判定
        if (!_healthManager.IsAlive)
        {
            _healthManager.Kill();
            _onDeathEvent?.Invoke();
        }
    }

    private void TransitionToNextPhase()
    {
        _currentPhaseIndex++;
        Debug.Log($"Transitioning to Phase {_currentPhaseIndex + 1}");
        _attackController.UpdatePhaseSettings(CurrentPhaseSettings);
        StartCoroutine(StartPhaseRoutine());

        _onTransitionPhaseEvent?.Invoke(_currentPhaseIndex);
    }

    private IEnumerator StartPhaseRoutine()
    {
        StopAttacking();
        Debug.Log($"Phase transition in progress. Waiting for {CurrentPhaseSettings.phaseTransitionTime} seconds.");
        yield return new WaitForSeconds(CurrentPhaseSettings.phaseTransitionTime);
        StartAttacking();
    }

    // AttackControllerが呼び出す
    public void StartAttacking()
    {
        _attackController.StartAttacking();
    }

    public void StopAttacking()
    {
        _attackController.StopAttacking();
    }

    // このメソッドは派生クラスでオーバーライドされる
    public virtual void ExecuteAttack(AttackPattern pattern)
    {
        // 派生クラスで具体的な攻撃処理を実装
    }

    public void OnDefeated()
    {
        StopAttacking();

        if (_bossDefeatController != null)
        {
            // 撃破演出の開始を依頼
            _bossDefeatController.StartDefeatSequence();
        }
        else
        {
            // 撃破コントローラーがない場合は即座に破棄
            DestroySelf();
        }
    }


    public void DestroySelf()
    {
        Destroy(gameObject);
    }

    private void OnDisable()
    {
        _onDeathEvent.RemoveAllListeners();
        _onTransitionPhaseEvent.RemoveAllListeners();
        _healthManager.ClearEvents();
    }
}