using UnityEngine;
using System.Collections.Generic;

public abstract class ProjectileBase : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] protected LayerMask enemyLayer; // Enemy���C���[�ݒ�

    protected ProjectileSpawnParams spawnParams;

    // �G�̌��o��Ԃ�ێ��i�d���Ăяo���h�~�p�j
    private HashSet<GameObject> alreadyDetectedEnemies = new HashSet<GameObject>();

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
        Collider[] hits = Physics.OverlapSphere(transform.position, spawnParams.DetectionRadius, enemyLayer);
        foreach (var hit in hits)
        {
            if (hit.CompareTag("Enemy") && !alreadyDetectedEnemies.Contains(hit.gameObject))
            {
                alreadyDetectedEnemies.Add(hit.gameObject);
                OnEnemyDetected(hit.gameObject);
            }
        }
    }

    /// <summary>
    /// �h���N���X�ŁA�G�����m�����Ƃ��̏������`
    /// </summary>
    protected virtual void OnEnemyDetected(GameObject enemy)
    {
        // ��FDebug.Log($"{gameObject.name} detected enemy: {enemy.name}");
    }

    /// <summary>
    /// �h���N���X�Ńt���[���P�ʂ̍X�V���s���ꍇ
    /// </summary>
    protected virtual void OnUpdate() { }

    /// <summary>
    /// �˒����z�����Ƃ��ɌĂ΂��i�����Ȃǁj
    /// </summary>
    protected virtual void OnRangeExceeded() { }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnParams.DetectionRadius);
    }
#endif
}
