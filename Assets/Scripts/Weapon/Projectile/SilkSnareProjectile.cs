using UnityEngine;

public class SilkSnareProjectile : ProjectileBase
{
    protected override void OnEnemyDetected(GameObject enemy)
    {
        // �X���[���ʕt�^�i���j
        Debug.Log($"[SilkSnare] Slowing {enemy.name}");
        // TODO: �G�ɃX���[���ʂ�t�^���鏈����ǉ�
        Destroy(gameObject);
    }
}
