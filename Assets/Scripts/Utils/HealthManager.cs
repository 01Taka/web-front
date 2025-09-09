using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// ヘルス管理クラス。ダメージ・回復・死亡イベントを扱う。
/// </summary>
public class HealthManager
{
    private float _maxHealth;
    private float _currentHealth;

    private UnityEvent<float> _onHealthChanged;
    private UnityEvent _onDeathEvent;

    /// <summary>
    /// コンストラクタ。インスタンス化時に最大HPを設定。
    /// </summary>
    public HealthManager(float maxHealth)
    {
        _maxHealth = Mathf.Max(1f, maxHealth); // 0や負値を防ぐ
        _currentHealth = _maxHealth;

        _onHealthChanged = new UnityEvent<float>();
        _onDeathEvent = new UnityEvent();
    }

    /// <summary>
    /// 最大HP（外部からは読み取り専用、継承クラスからのみ変更可）。
    /// </summary>
    public float MaxHealth
    {
        get => _maxHealth;
        protected set => _maxHealth = Mathf.Max(1f, value); // 1以上に保証
    }

    /// <summary>
    /// 現在のHP（読み取り専用）。
    /// </summary>
    public float CurrentHealth => _currentHealth;

    /// <summary>
    /// 生存判定。
    /// </summary>
    public bool IsAlive => _currentHealth > 0f;

    public void AddOnDeathAction(UnityAction action)
    {
        _onDeathEvent.AddListener(action);
    }

    public void AddOnHealthChangeAction(UnityAction<float> action)
    {
        _onHealthChanged.AddListener(action);
    }

    public void RemoveOnDeathAction(UnityAction action)
    {
        _onDeathEvent.RemoveListener(action);
    }

    public void RemoveOnHealthChangeAction(UnityAction<float> action)
    {
        _onHealthChanged.RemoveListener(action);
    }

    /// <summary>
    /// ダメージを与える。
    /// </summary>
    public virtual void TakeDamage(float amount)
    {
        if (amount <= 0f || !IsAlive) return;

        UpdateHealth(_currentHealth - amount);
    }

    /// <summary>
    /// HPを回復する。
    /// </summary>
    public virtual void Heal(float amount)
    {
        if (amount <= 0f || !IsAlive) return;

        UpdateHealth(_currentHealth + amount);
    }

    /// <summary>
    /// 強制的に死亡させる。
    /// HPを0にし、死亡イベントを発火させる。
    /// </summary>
    public void Kill()
    {
        if (!IsAlive) return;

        _currentHealth = 0f;
        _onHealthChanged?.Invoke(_currentHealth);
        Die();
    }

    /// <summary>
    /// 最大HPを設定する。
    /// </summary>
    public void SetMaxHealth(float newMaxHealth, bool healToFull = true)
    {
        MaxHealth = newMaxHealth;

        if (healToFull)
        {
            UpdateHealth(MaxHealth);
        }
        else
        {
            // 現在HPが最大HPを超えていれば調整
            if (_currentHealth > MaxHealth)
            {
                UpdateHealth(MaxHealth);
            }
        }
    }

    /// <summary>
    /// 内部的にHPを変更し、イベントや死亡処理を一元管理。
    /// </summary>
    private void UpdateHealth(float newHealth)
    {
        _currentHealth = Mathf.Clamp(newHealth, 0f, MaxHealth);
        _onHealthChanged?.Invoke(_currentHealth);

        if (_currentHealth <= 0f)
        {
            Die();
        }
    }

    /// <summary>
    /// 死亡処理。
    /// </summary>
    protected virtual void Die()
    {
        _onDeathEvent?.Invoke();
    }

    public void ClearEvents()
    {
        _onDeathEvent.RemoveAllListeners();
        _onHealthChanged.RemoveAllListeners();
    }
}
