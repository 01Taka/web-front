using UnityEngine;
using UnityEngine.Events;

/// <summary>
/// �w���X�Ǘ��N���X�B�_���[�W�E�񕜁E���S�C�x���g�������B
/// </summary>
public class HealthManager
{
    private float _maxHealth;
    private float _currentHealth;

    private UnityEvent<float> _onHealthChanged;
    private UnityEvent _onDeathEvent;

    /// <summary>
    /// �R���X�g���N�^�B�C���X�^���X�����ɍő�HP��ݒ�B
    /// </summary>
    public HealthManager(float maxHealth)
    {
        _maxHealth = Mathf.Max(1f, maxHealth); // 0�╉�l��h��
        _currentHealth = _maxHealth;

        _onHealthChanged = new UnityEvent<float>();
        _onDeathEvent = new UnityEvent();
    }

    /// <summary>
    /// �ő�HP�i�O������͓ǂݎ���p�A�p���N���X����̂ݕύX�j�B
    /// </summary>
    public float MaxHealth
    {
        get => _maxHealth;
        protected set => _maxHealth = Mathf.Max(1f, value); // 1�ȏ�ɕۏ�
    }

    /// <summary>
    /// ���݂�HP�i�ǂݎ���p�j�B
    /// </summary>
    public float CurrentHealth => _currentHealth;

    /// <summary>
    /// ��������B
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
    /// �_���[�W��^����B
    /// </summary>
    public virtual void TakeDamage(float amount)
    {
        if (amount <= 0f || !IsAlive) return;

        UpdateHealth(_currentHealth - amount);
    }

    /// <summary>
    /// HP���񕜂���B
    /// </summary>
    public virtual void Heal(float amount)
    {
        if (amount <= 0f || !IsAlive) return;

        UpdateHealth(_currentHealth + amount);
    }

    /// <summary>
    /// �����I�Ɏ��S������B
    /// HP��0�ɂ��A���S�C�x���g�𔭉΂�����B
    /// </summary>
    public void Kill()
    {
        if (!IsAlive) return;

        _currentHealth = 0f;
        _onHealthChanged?.Invoke(_currentHealth);
        Die();
    }

    /// <summary>
    /// �ő�HP��ݒ肷��B
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
            // ����HP���ő�HP�𒴂��Ă���Β���
            if (_currentHealth > MaxHealth)
            {
                UpdateHealth(MaxHealth);
            }
        }
    }

    /// <summary>
    /// �����I��HP��ύX���A�C�x���g�⎀�S�������ꌳ�Ǘ��B
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
    /// ���S�����B
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
