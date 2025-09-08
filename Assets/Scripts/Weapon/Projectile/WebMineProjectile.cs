using UnityEngine;

public class WebMineProjectile : ProjectileBase
{
    [SerializeField] private WebArea webAreaPrefab;
    [SerializeField] private float prefabScaleRatio = 0.5f;
    [SerializeField] private int _preloadCount = 5;
    [SerializeField] private float rotationSpeed = 90f;

    protected override void OnUpdate()
    {
        transform.rotation *= Quaternion.Euler(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    protected override void OnEnemyDetected(IDamageable enemy)
    {
        // “G‚É“–‚½‚Á‚Ä‚à’Ê‰ß‚·‚é‚¾‚¯
    }

    protected override void OnRangeExceeded()
    {
        DeployWeb();
    }

    private void DeployWeb()
    {
        if (webAreaPrefab != null)
        {
            WebArea explosion = _poolManager.Get(webAreaPrefab, _poolParent, _preloadCount);
            explosion.transform.position = transform.position;
            explosion.transform.localScale = webAreaPrefab.transform.localScale * prefabScaleRatio;
            explosion.Initialize(spawnParams.EffectDuration, 0, spawnParams.Damage, spawnParams.EffectInterval, spawnParams.EffectRadius);
        }

        ReturnToPool();
    }
}
