using UnityEngine;

public class SilkSnareProjectile : ProjectileBase
{
    protected override void OnEnemyDetected(IDamageable enemy)
    {
        base.OnEnemyDetected(enemy);
        // �X���[���ʕt�^�i���j
        // TODO: �G�ɃX���[���ʂ�t�^���鏈����ǉ�
        Destroy(gameObject);
    }
}
