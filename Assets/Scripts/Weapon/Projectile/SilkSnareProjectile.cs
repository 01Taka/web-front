using UnityEngine;

public class SilkSnareProjectile : ProjectileBase
{
    protected override void OnEnemyDetected(IDamageable enemy)
    {
        base.OnEnemyDetected(enemy);
        ReturnToPool();
    }
}
