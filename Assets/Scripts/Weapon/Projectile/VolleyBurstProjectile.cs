using UnityEngine;

public class VolleyBurstProjectile : ProjectileBase
{
    [SerializeField] private VolleyExplosion explosionPrefab;

    protected override void OnRangeExceeded()
    {
        Explode();
    }

    private void Explode()
    {
        if (explosionPrefab != null)
        {
            VolleyExplosion explosion = Instantiate(explosionPrefab, transform.position, Quaternion.identity);
            explosion.Initialize(spawnParams.EffectRadius, spawnParams.Damage);
        }

        // TODO: �����͈͓��Ƀ_���[�W�t�^���鏈��
    }
}
