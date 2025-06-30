using UnityEngine;
using System.Collections;

public class VolleyExplosion : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float animationDuration = 0.3f;

    private float explosionRadius;
    private float explosionDamage;
    private Vector3 originalScale;

    public void Initialize(float radius, float damage)
    {
        explosionRadius = radius;
        explosionDamage = damage;

        // スケールを半径に基づいて設定（直径 = radius * 2）
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
        Collider[] targets = Physics.OverlapSphere(transform.position, explosionRadius, enemyLayer);
        foreach (var target in targets)
        {
            if (target.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(explosionDamage);
            }
        }

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
