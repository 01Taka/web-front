using UnityEngine;
using System.Collections;

public class VolleyExplosion : MonoBehaviour, IPoolable
{
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float animationDuration = 0.3f;

    private float explosionRadius;
    private float explosionDamage;
    private Vector3 originalScale;

    private ObjectPool<VolleyExplosion> _pool;

    public void SetPool<T>(ObjectPool<T> pool) where T : Component
    {
        _pool = pool as ObjectPool<VolleyExplosion>;
    }

    public void Initialize(float radius, float damage)
    {
        explosionRadius = radius;
        explosionDamage = damage;

        // スケールリセット
        originalScale = transform.localScale * explosionRadius;
        transform.localScale = Vector3.zero;

        StartCoroutine(ScaleUpAndExplode(animationDuration));
    }

    private IEnumerator ScaleUpAndExplode(float duration)
    {
        float timer = 0f;
        while (timer < duration)
        {
            float t = timer / duration;
            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
        Explode();
    }

    private void Explode()
    {
        Collider2D[] targets = Physics2D.OverlapCircleAll(transform.position, explosionRadius, enemyLayer);
        foreach (var target in targets)
        {
            if (target.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(explosionDamage);
            }
        }

        ReturnToPool();
    }

    public void ReturnToPool()
    {
        if (_pool != null)
            _pool.ReturnToPool(this);
        else
            Destroy(gameObject);
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, explosionRadius);
    }
#endif
}
