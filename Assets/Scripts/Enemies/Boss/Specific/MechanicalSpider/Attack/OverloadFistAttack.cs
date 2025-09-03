using UnityEngine;

public class OverloadFistAttack : BossAttackBase
{
    // �U���^�C�v���`
    public override BossAttackType AttackType => BossAttackType.OverloadFist;

    [Header("�U���ɌŗL�̃p�����[�^")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionRadius = 5f;

    /// <summary>
    /// �U�����s���ɌĂяo�����
    /// </summary>
    public override void ExecuteAttack(BossAttackContext context)
    {
        Debug.Log("OverloadFistAttack: �����_���[�W��^���܂��B");

        // �|�[�g�̈ʒu���甚���G�t�F�N�g�𐶐�
        Vector3 explosionPosition = Vector3.zero;
        if (explosionPrefab != null)
        {
            GameObject.Instantiate(explosionPrefab, explosionPosition, Quaternion.identity);
        }

        base.ExecuteAttack(context);
    }
}