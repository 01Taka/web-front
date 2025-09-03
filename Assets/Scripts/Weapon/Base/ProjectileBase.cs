using UnityEngine;
using System.Collections.Generic;

public abstract class ProjectileBase : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] protected LayerMask enemyLayer; // Enemy���C���[�ݒ�

    protected ProjectileSpawnParams spawnParams;

    // �G��IDamageable����Ɍ��o��Ԃ�ێ��i�d���Ăяo���h�~�p�j
    private HashSet<IDamageable> alreadyDetectedDamageables = new HashSet<IDamageable>();

    public virtual void Initialize(ProjectileSpawnParams spawnParams)
    {
        this.spawnParams = spawnParams;
    }

    private void Update()
    {
        MoveProjectile(Time.deltaTime);
        DetectEnemies();
        CheckRangeExceeded();
        OnUpdate();
    }

    private void CheckRangeExceeded()
    {
        float distanceTraveled = Vector3.Distance(spawnParams.Position, transform.position);
        if (distanceTraveled >= spawnParams.Range)
        {
            OnRangeExceeded();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// �ʏ�̈ړ������B�K�v�Ȃ�Override��
    /// </summary>
    protected virtual void MoveProjectile(float deltaTime)
    {
        transform.position += deltaTime * spawnParams.Speed * spawnParams.Direction;
    }

    /// <summary>
    /// �͈͓��̓G�����m���ăR�[���o�b�N����
    /// </summary>
    private void DetectEnemies()
    {
        // 2D�̏ꍇ�A��������g�p��������ǂ�
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, spawnParams.DetectionRadius, enemyLayer);

        foreach (var hit in hits)
        {
            // �q�b�g�����I�u�W�F�N�g�����łȂ��A���̐e�I�u�W�F�N�g���T������IDamageable���擾����
            IDamageable damageable = hit.GetComponentInParent<IDamageable>();

            if (damageable == null)
            {
                Debug.LogWarning($"Collider on '{hit.gameObject.name}' or its parent does not have an IDamageable component.");
                continue; // ���̃q�b�g��
            }

            // IDamageable����ɏd���q�b�g���`�F�b�N
            if (!alreadyDetectedDamageables.Contains(damageable))
            {
                alreadyDetectedDamageables.Add(damageable);
                OnEnemyDetected(damageable);
            }
        }
    }

    /// <summary>
    /// �h���N���X�ŁA�G�����m�����Ƃ��̏������`
    /// </summary>
    protected virtual void OnEnemyDetected(IDamageable damageable)
    {
        damageable.TakeDamage(spawnParams.Damage);
    }

    /// <summary>
    /// �h���N���X�Ńt���[���P�ʂ̍X�V���s���ꍇ
    /// </summary>
    protected virtual void OnUpdate() { }

    /// <summary>
    /// �˒����z�����Ƃ��ɌĂ΂��i�����Ȃǁj
    /// </summary>
    protected virtual void OnRangeExceeded() { }

    /// <summary>
    /// ���Ƀq�b�g�����G�̃��X�g�����Z�b�g����
    /// </summary>
    protected void ResetDetectedEnemies()
    {
        alreadyDetectedDamageables.Clear();
    }


#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnParams.DetectionRadius);
    }
#endif
}