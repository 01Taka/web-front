using UnityEngine;

public class ChargedPierceProjectile : ProjectileBase
{
    protected override void OnEnemyDetected(IDamageable enemy)
    {
        base.OnEnemyDetected(enemy);
        // スタン効果付与（仮）
    }
}
