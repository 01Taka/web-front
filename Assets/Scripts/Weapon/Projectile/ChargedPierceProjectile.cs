using UnityEngine;

public class ChargedPierceProjectile : ProjectileBase
{
    protected override void OnEnemyDetected(GameObject enemy)
    {
        // �X�^�����ʕt�^�i���j
        Debug.Log($"[ChargedPierce] Stunning {enemy.name}");
    }
}
