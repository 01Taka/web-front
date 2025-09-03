using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public abstract class BaseBossManager : MonoBehaviour
{
    [Header("Boss Settings")]
    [SerializeField] private BossSettings _bossSettings;
    [SerializeField] private BossPart bodyPart;
    [SerializeField] private AudioClip hitClip;
    [SerializeField] [Range(0, 1f)] private float hitVolum = 1f;

    private int _currentPhaseIndex = 0;

    [SerializeField] private bool _resetHealthOnPhaseChange = true;

    private HealthManager _healthManager;
    private Coroutine _attackRoutine;

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

        _healthManager = new HealthManager(CurrentPhaseSettings.maxHealth);
        _healthManager.AddOnDeathAction(StopAttacking);

        // 最初のフェーズを開始。
        _healthManager.SetMaxHealth(CurrentPhaseSettings.maxHealth, true);
        StartCoroutine(StartPhaseRoutine());
    }

    public void AddOnDeathAction(UnityAction action)
    {
        _onDeathEvent.AddListener(action);
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

        if (_healthManager.CurrentHealth <= 0)
        {
            bool isFinalPhase = _currentPhaseIndex >= _bossSettings.GetPhaseCount() - 1;

            if (isFinalPhase)
            {
                _healthManager.Kill();
                _onDeathEvent?.Invoke();
            }
            else
            {
                TransitionToNextPhase();

            }
        }
    }

    private void TransitionToNextPhase()
    {
        _currentPhaseIndex++;
        Debug.Log($"Transitioning to Phase {_currentPhaseIndex + 1}");
        
        _healthManager.SetMaxHealth(CurrentPhaseSettings.maxHealth, _resetHealthOnPhaseChange);
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

    public void PerformAttack(BossAttackType attackType)
    {
        AttackPattern pattern = FindAttackPattern(attackType);

        if (pattern != null)
        {
            ExecuteAttack(pattern);
        }
        else
        {
            Debug.LogError($"AttackType '{attackType}' is not available in current phase.");
        }
    }

    public AttackPattern PerformRandomAttack()
    {
        AttackPattern[] patterns = CurrentPhaseSettings.attackPatterns;

        if (patterns == null || patterns.Length == 0)
        {
            Debug.LogWarning("No attack patterns found for the current phase.");
            return null;
        }

        int randomIndex = Random.Range(0, patterns.Length);
        AttackPattern selectedPattern = patterns[randomIndex];
        ExecuteAttack(selectedPattern);
        return selectedPattern;
    }

    public void StartAttacking()
    {
        if (_attackRoutine != null)
        {
            StopCoroutine(_attackRoutine);
        }
        _attackRoutine = StartCoroutine(AttackRoutine());
    }

    public void StopAttacking()
    {
        if (_attackRoutine != null)
        {
            StopCoroutine(_attackRoutine);
            _attackRoutine = null;
        }
    }

    private IEnumerator AttackRoutine()
    {
        yield return new WaitForSeconds(Random.Range(CurrentPhaseSettings.minAttackInterval, CurrentPhaseSettings.maxAttackInterval));

        while (_healthManager.IsAlive)
        {
            var executedAttack = PerformRandomAttack();

            float waitTime = Random.Range(CurrentPhaseSettings.minAttackInterval, CurrentPhaseSettings.maxAttackInterval);

            if (executedAttack != null)
            {
                waitTime += executedAttack.AttackPreparationTime + executedAttack.AttackDuration;
            }

            yield return new WaitForSeconds(waitTime);
        }
    }

    private AttackPattern FindAttackPattern(BossAttackType attackType)
    {
        foreach (var pattern in CurrentPhaseSettings.attackPatterns)
        {
            if (pattern.AttackType == attackType)
            {
                return pattern;
            }
        }
        return null;
    }

    protected abstract void ExecuteAttack(AttackPattern pattern);

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