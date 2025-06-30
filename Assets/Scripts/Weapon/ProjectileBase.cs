using UnityEngine;
using System.Collections.Generic;

public abstract class ProjectileBase : MonoBehaviour
{
    [Header("Projectile Settings")]
    [SerializeField] protected LayerMask enemyLayer; // Enemyレイヤー設定

    protected ProjectileSpawnParams spawnParams;

    // 敵の検出状態を保持（重複呼び出し防止用）
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
    /// 派生クラスで、敵を検知したときの処理を定義
    /// </summary>
    protected virtual void OnEnemyDetected(GameObject enemy)
    {
        // 例：Debug.Log($"{gameObject.name} detected enemy: {enemy.name}");
    }

    /// <summary>
    /// 派生クラスでフレーム単位の更新を行う場合
    /// </summary>
    protected virtual void OnUpdate() { }

    /// <summary>
    /// 射程を越えたときに呼ばれる（爆発など）
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
