using UnityEngine.Events;
using UnityEngine;

public enum DeathReason
{
    PlayerDefeated,
    Exploded
}

[System.Serializable]
public class DeathEvent : UnityEvent<DeathReason> { }

public class HordeEnemy : MonoBehaviour, IDamageable
{
    [SerializeField] private HordeEnemySettings _settings;
    [SerializeField] public DeathEvent _onHordeDeath;

    private HealthManager _healthManager;
    private Transform _playerTransform;

    private void OnEnable()
    {
        if (_settings == null)
        {
            Debug.LogError("HordeEnemySettingsÇ™ê›íËÇ≥ÇÍÇƒÇ¢Ç‹ÇπÇÒÅB", this);
            return;
        }

        InitializeHealthManager();
        SetSize(_settings.enemyScale);
        _onHordeDeath.RemoveAllListeners(); // ÉäÉXÉiÅ[ÇÃëΩèdìoò^ÇñhÇÆ
        AddDeathAction(PlayDestoryEffect);
    }

    public void AddDeathAction(UnityAction<DeathReason> action)
    {
        _onHordeDeath.AddListener(action);
    }

    public void SetTarget(Transform target)
    {
        _playerTransform = target;
    }

    public void SetSize(float scale)
    {
        transform.localScale = new Vector3(scale, scale, 1.0f);
    }

    private void Update()
    {
        if (_playerTransform != null && _healthManager.IsAlive)
        {
            RotateTowardsTarget();
            Vector2 direction = (_playerTransform.position - transform.position).normalized;
            transform.Translate(direction * _settings.moveSpeed * Time.deltaTime, Space.World);

            if (Vector2.Distance(transform.position, _playerTransform.position) <= _settings.explosionDistance)
            {
                Explode();
            }
        }
    }

    private void PlayDestoryEffect(DeathReason deathReason)
    {
        if (deathReason == DeathReason.PlayerDefeated)
        {
            ExplosionEffectPoolManager.Instance.PlayExplosion(transform.position, _settings.DestroyEffectSize, _settings.DestroyExplosionType);
        }
    }

    private void RotateTowardsTarget()
    {
        Vector3 direction = _playerTransform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle + _settings.adjustAngle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _settings.moveSpeed * Time.deltaTime);
    }

    private void InitializeHealthManager()
    {
        _healthManager = new HealthManager(_settings.maxHealth);
    }

    public void TakeDamage(float amount)
    {
        if (!_healthManager.IsAlive) return;

        if (SoundManager.Instance != null && _settings.damageSound != null)
        {
            SoundManager.Instance.PlayEffect(_settings.damageSound, _settings.SoundVolume);
        }

        float previousHealth = _healthManager.CurrentHealth;
        _healthManager.TakeDamage(amount);

        if (!_healthManager.IsAlive && previousHealth > 0)
        {
            OnPlayerDefeated();
        }
    }

    private void Explode()
    {
        if (!_healthManager.IsAlive) return;

        if (SoundManager.Instance != null && _settings.explodeSound != null)
        {
            SoundManager.Instance.PlayEffect(_settings.explodeSound, _settings.SoundVolume);
        }

        _healthManager.Kill();
        _onHordeDeath?.Invoke(DeathReason.Exploded);
    }

    private void OnPlayerDefeated()
    {
        if (SoundManager.Instance != null && _settings.destroySound != null)
        {
            SoundManager.Instance.PlayEffect(_settings.destroySound, _settings.SoundVolume);
        }

        _onHordeDeath?.Invoke(DeathReason.PlayerDefeated);
    }
}
