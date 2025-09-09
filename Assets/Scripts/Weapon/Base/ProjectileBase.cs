using UnityEngine;
using System.Collections.Generic;

public abstract class ProjectileBase : MonoBehaviour, IPoolable
{
    public bool IsReusable { get; set; }

    [Header("Projectile Settings")]
    [SerializeField] protected LayerMask enemyLayer;
    [SerializeField] protected SpriteRenderer _spriteRenderer;

    protected ProjectileSpawnParams spawnParams;
    private HashSet<IDamageable> alreadyDetectedDamageables = new HashSet<IDamageable>();

    private ObjectPool<ProjectileBase> _pool;
    protected PoolManager _poolManager;
    protected Transform _poolParent;
    

    public virtual void Initialize(ProjectileSpawnParams spawnParams, PoolManager poolManager, Transform poolParent)
    {
        this.spawnParams = spawnParams;
        this._poolManager = poolManager;
        this._poolParent = poolParent;
        SetColor(spawnParams.ProjectileColor);
        ResetDetectedEnemies();
    }

    public void SetPool<T>(ObjectPool<T> pool) where T : Component, IPoolable
    {
        _pool = pool as ObjectPool<ProjectileBase>;
    }

    private void SetColor(Color color)
    {
        if (_spriteRenderer != null)
        {
            _spriteRenderer.color = color;
        }
        else
        {
            Debug.LogError("SpriteRender not atached");
        }
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
            ReturnToPool();
        }
    }

    protected virtual void MoveProjectile(float deltaTime)
    {
        transform.position += deltaTime * spawnParams.Speed * spawnParams.Direction;
    }

    private void DetectEnemies()
    {
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, spawnParams.DetectionRadius, enemyLayer);

        foreach (var hit in hits)
        {
            IDamageable damageable = hit.GetComponentInParent<IDamageable>();
            if (damageable == null) continue;

            if (!alreadyDetectedDamageables.Contains(damageable))
            {
                alreadyDetectedDamageables.Add(damageable);
                OnEnemyDetected(damageable);
            }
        }
    }

    protected virtual void OnEnemyDetected(IDamageable damageable)
    {
        damageable.TakeDamage(spawnParams.Damage);
        // 例: ヒットしたら破棄するタイプなら ReturnToPool()
    }

    protected virtual void OnUpdate() { }
    protected virtual void OnRangeExceeded() { }

    protected void ResetDetectedEnemies()
    {
        alreadyDetectedDamageables.Clear();
    }

    public void ReturnToPool()
    {
        if (_pool != null)
        {
            _pool.ReturnToPool(this);
        }
        else
        {
            Debug.LogWarning($"No pool assigned for {gameObject.name}, destroying instead.");
            Destroy(gameObject);
        }
    }

#if UNITY_EDITOR
    protected virtual void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, spawnParams.DetectionRadius);
    }
#endif
}
