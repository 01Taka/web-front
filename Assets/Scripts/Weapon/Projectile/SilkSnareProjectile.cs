using UnityEngine;

public class SilkSnareProjectile : ProjectileBase
{
    protected override void OnEnemyDetected(IDamageable enemy)
    {
        base.OnEnemyDetected(enemy);
        // スロー効果付与（仮）
        // TODO: 敵にスロー効果を付与する処理を追加
        Destroy(gameObject);
    }
}
