using UnityEngine;
using System.Collections.Generic;

public abstract class ProjectileBase : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] protected LayerMask enemyLayer; // Enemyレイヤー設定

    protected ProjectileSpawnParams spawnParams;

    // 敵のIDamageableを基準に検出状態を保持（重複呼び出し防止用）
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
    /// 通常の移動処理。必要ならOverride可
    /// </summary>
    protected virtual void MoveProjectile(float deltaTime)
    {
        transform.position += deltaTime * spawnParams.Speed * spawnParams.Direction;
    }

    /// <summary>
    /// 範囲内の敵を検知してコールバックする
    /// </summary>
    private void DetectEnemies()
    {
        // 2Dの場合、こちらを使用する方が良い
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, spawnParams.DetectionRadius, enemyLayer);

        foreach (var hit in hits)
        {
            // ヒットしたオブジェクトだけでなく、その親オブジェクトも探索してIDamageableを取得する
            IDamageable damageable = hit.GetComponentInParent<IDamageable>();

            if (damageable == null)
            {
                Debug.LogWarning($"Collider on '{hit.gameObject.name}' or its parent does not have an IDamageable component.");
                continue; // 次のヒットへ
            }

            // IDamageableを基準に重複ヒットをチェック
            if (!alreadyDetectedDamageables.Contains(damageable))
            {
                alreadyDetectedDamageables.Add(damageable);
                OnEnemyDetected(damageable);
            }
        }
    }

    /// <summary>
    /// 派生クラスで、敵を検知したときの処理を定義
    /// </summary>
    protected virtual void OnEnemyDetected(IDamageable damageable)
    {
        damageable.TakeDamage(spawnParams.Damage);
    }

    /// <summary>
    /// 派生クラスでフレーム単位の更新を行う場合
    /// </summary>
    protected virtual void OnUpdate() { }

    /// <summary>
    /// 射程を越えたときに呼ばれる（爆発など）
    /// </summary>
    protected virtual void OnRangeExceeded() { }

    /// <summary>
    /// 既にヒットした敵のリストをリセットする
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