using UnityEngine;

public class ChargedPierceProjectile : ProjectileBase
{
    protected override void OnEnemyDetected(GameObject enemy)
    {
        // スタン効果付与（仮）
        Debug.Log($"[ChargedPierce] Stunning {enemy.name}");
    }
}
