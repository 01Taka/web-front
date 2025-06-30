using UnityEngine;

public class SilkSnareProjectile : ProjectileBase
{
    protected override void OnEnemyDetected(GameObject enemy)
    {
        // スロー効果付与（仮）
        Debug.Log($"[SilkSnare] Slowing {enemy.name}");
        // TODO: 敵にスロー効果を付与する処理を追加
        Destroy(gameObject);
    }
}
