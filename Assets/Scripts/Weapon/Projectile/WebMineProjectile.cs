using UnityEngine;

public class WebMineProjectile : ProjectileBase
{
    [SerializeField] private WebArea webAreaPrefab;
    [SerializeField] private float rotationSpeed = 90f;

    protected override void OnUpdate()
    {
        transform.rotation *= Quaternion.Euler(0f, 0f, rotationSpeed * Time.deltaTime);
    }

    protected override void OnEnemyDetected(IDamageable enemy)
    {

    }

    protected override void OnRangeExceeded()
    {
        DeployWeb();
    }

    private void DeployWeb()
    {
        if (webAreaPrefab != null)
        {
            WebArea webArea = Instantiate(webAreaPrefab, transform.position, Quaternion.identity);
            webArea.Initialize(spawnParams.EffectDuration, 0, spawnParams.Damage, spawnParams.EffectInterval, spawnParams.EffectRadius);
        }

        Destroy(gameObject);
    }
}
