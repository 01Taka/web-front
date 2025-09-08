using UnityEngine;
using System.Collections;

public class WebArea : MonoBehaviour
{
    [SerializeField] private LayerMask enemyLayer;
    [SerializeField] private float expandDuration = 0.3f;
    [SerializeField] private float shrinkDuration = 0.3f;

    private float destroyTime;
    private float slowMultiplier;
    private float tickDamage;
    private float tickInterval;
    private float effectRadius;

    private Vector3 originalScale;
    private bool isShrinking = false;

    public void Initialize(float duration, float slow, float damagePerTick, float interval, float radius)
    {
        destroyTime = Time.time + duration;
        slowMultiplier = slow;
        tickDamage = damagePerTick;
        tickInterval = interval;
        effectRadius = radius;

        originalScale = transform.localScale * radius;
        transform.localScale = Vector3.zero;

        StartCoroutine(ExpandAndStartTick());
    }

    private IEnumerator ExpandAndStartTick()
    {
        float timer = 0f;
        while (timer < expandDuration)
        {
            float t = timer / expandDuration;
            transform.localScale = Vector3.Lerp(Vector3.zero, originalScale, t);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = originalScale;
        InvokeRepeating(nameof(DamageAndSlow), 0f, tickInterval);
    }

    private void Update()
    {
        if (!isShrinking && Time.time >= destroyTime)
        {
            isShrinking = true;
            CancelInvoke(nameof(DamageAndSlow));
            StartCoroutine(ShrinkAndDestroy());
        }
    }

    private IEnumerator ShrinkAndDestroy()
    {
        float timer = 0f;
        Vector3 startScale = transform.localScale;

        while (timer < shrinkDuration)
        {
            float t = timer / shrinkDuration;
            transform.localScale = Vector3.Lerp(startScale, Vector3.zero, t);
            timer += Time.deltaTime;
            yield return null;
        }

        transform.localScale = Vector3.zero;
        Destroy(gameObject);
    }

    private void DamageAndSlow()
    {
        Collider2D[] enemies = Physics2D.OverlapCircleAll(transform.position, effectRadius, enemyLayer);
        foreach (var enemy in enemies)
        {
            if (enemy.TryGetComponent<IDamageable>(out var damageable))
            {
                damageable.TakeDamage(tickDamage);
            }

            if (enemy.TryGetComponent<IMovable>(out var movable))
            {
                movable.ApplySlow(slowMultiplier, tickInterval + 0.1f);
            }
        }
    }

#if UNITY_EDITOR
    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(transform.position, effectRadius);
    }
#endif
}
