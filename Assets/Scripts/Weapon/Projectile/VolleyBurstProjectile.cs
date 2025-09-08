using UnityEngine;

public class VolleyBurstProjectile : ProjectileBase
{
    [SerializeField] private VolleyExplosion explosionPrefab;
    [SerializeField] private float prefabScaleRatio = 0.5f;
    [SerializeField] private int _preloadCount = 10;

    protected override void OnRangeExceeded()
    {
        Explode();
    }

    protected override void OnEnemyDetected(IDamageable damageable)
    {
        Explode();
    }

    private void Explode()
    {
        if (explosionPrefab == null || _poolManager == null) return;

        VolleyExplosion explosion = _poolManager.Get(explosionPrefab, _poolParent, _preloadCount);
        explosion.transform.position = transform.position;
        explosion.transform.localScale = explosionPrefab.transform.localScale * prefabScaleRatio;
        explosion.Initialize(spawnParams.EffectRadius, spawnParams.Damage, spawnParams.ProjectileColor);
    }
}
