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
/// �{�X�̂����iHorde�j�̓G�L�����N�^�[�̋����𐧌䂷��N���X�B
/// HealthManager��IDamageable�C���^�[�t�F�[�X����������B
/// </summary>
public class HordeEnemy : MonoBehaviour, IDamageable
{
    [Tooltip("�G�̐ݒ���i�[����ScriptableObject")]
    [SerializeField]
    private HordeEnemySettings _settings;

    // �e�ɒʒm���邽�߂̎��S�C�x���g�B�����Ɏ��S���R���܂ށB
   [SerializeField] public DeathEvent _onHordeDeath;

    // �w���X�Ǘ��N���X�̃C���X�^���X
    private HealthManager _healthManager;

    // �v���C���[��Transform�i�ǐ՗p�j
    private Transform _playerTransform;

    /// <summary>
    /// �R���|�[�l���g���L���ɂȂ����Ƃ��ɌĂяo�����B
    /// HealthManager�̏������ƃC�x���g�o�^���s���B
    /// </summary>
    private void OnEnable()
    {
        if (_settings == null)
        {
            Debug.LogError("HordeEnemySettings���ݒ肳��Ă��܂���B", this);
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
    /// �^�[�Q�b�g��ݒ肷�郁�\�b�h�BHordeSpawner����Ăяo�����B
    /// </summary>
    public void SetTarget(Transform target)
    {
        _playerTransform = target;
    }

    /// <summary>
    /// �傫����ݒ肷�郁�\�b�h�B�O������傫���𓮓I�ɕύX�ł���B
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
        // �v���C���[��ǐՂ����{�I�ȃ��W�b�N
        if (_playerTransform != null && _healthManager.IsAlive)
        {
            // �^�[�Q�b�g�ɑ̂�������
            RotateTowardsTarget();

            // �^�[�Q�b�g�Ɍ������Ĉړ�
            Vector2 direction = (_playerTransform.position - transform.position).normalized;
            transform.Translate(direction * _settings.moveSpeed * Time.deltaTime, Space.World);

            // �^�[�Q�b�g�Ƃ̋������`�F�b�N���A��苗���܂ŋ߂Â����玩��
            if (Vector2.Distance(transform.position, _playerTransform.position) <= _settings.explosionDistance)
            {
                Explode();
            }
        }
    }

    /// <summary>
    /// �^�[�Q�b�g�̕����Ɍ������đ̂���]������B
    /// </summary>
    private void RotateTowardsTarget()
    {
        Vector3 direction = _playerTransform.position - transform.position;
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion rotation = Quaternion.AngleAxis(angle + _settings.adjustAngle, Vector3.forward);
        transform.rotation = Quaternion.Slerp(transform.rotation, rotation, _settings.moveSpeed * Time.deltaTime);
    }


    /// <summary>
    /// HealthManager�̏������ƃC�x���g�̕R�Â����s���B
    /// </summary>
    private void InitializeHealthManager()
    {
        _healthManager = new HealthManager(_settings.maxHealth);
    }

    /// <summary>
    /// IDamageable�C���^�[�t�F�[�X�̎����B
    /// �O������_���[�W���󂯂邽�߂ɌĂяo�����B
    /// </summary>
    public void TakeDamage(float amount)
    {
        if (!_healthManager.IsAlive) return;

        // �_���[�W�����Đ�
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
    /// ���������B
    /// </summary>
    private void Explode()
    {
        if (!_healthManager.IsAlive) return;

        if (SoundManager.Instance != null && _settings.explodeSound != null)
        {
            SoundManager.Instance.PlayEffect(_settings.explodeSound);
        }

        _healthManager.Kill();
        // �����C�x���g�𔭉�
        _onHordeDeath?.Invoke(DeathReason.Exploded);
    }

    /// <summary>
    /// �v���C���[�ɂ���ē|���ꂽ�Ƃ��ɌĂ΂�郁�\�b�h�B
    /// </summary>
    private void OnPlayerDefeated()
    {
        if (SoundManager.Instance != null && _settings.destroySound != null)
        {
            SoundManager.Instance.PlayEffect(_settings.destroySound);
        }
        // �v���C���[�ɓ|���ꂽ�C�x���g�𔭉�
        _onHordeDeath?.Invoke(DeathReason.PlayerDefeated);
    }

    /// <summary>
    /// �X�N���v�g���j�������Ƃ��ɌĂяo�����B
    /// �s�v�ȃ��X�i�[���폜���ă��������[�N��h���B
    /// </summary>
    private void OnDestroy()
    {
        if (_healthManager != null)
        {
            _onHordeDeath.RemoveAllListeners();
        }
    }
}