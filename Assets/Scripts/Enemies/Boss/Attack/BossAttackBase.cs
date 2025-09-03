
using UnityEngine;

/// <summary>
/// IBossAttack�C���^�[�t�F�[�X���������钊�ۊ��N���X�B
/// ���ׂĂ̋�̓I�ȍU�����W�b�N�̋��ʋ@�\���`���܂��B
/// </summary>
public abstract class BossAttackBase : MonoBehaviour, IBossAttack
{
    public abstract BossAttackType AttackType { get; }

    // ���ۃ��\�b�h�Ƃ��Ē�`���A�q�N���X�ł̎������������܂��B
    public virtual void OnBeginPreparation(BossAttackContext context)
    {
        context.AnimationController.StartAnimation(CreateBossAnimationControllerContext(context));
    }

    public virtual void ExecuteAttack(BossAttackContext context)
    {
        if (context.PlayerDamageable != null)
        {
            float damageAmount = context.Pattern.BaseDamage;
            context.PlayerDamageable.TakeDamage(damageAmount);
        }

        context.AnimationController.EndAnimation(CreateBossAnimationControllerContext(context));
    }

    public virtual void OnCanceledAttack(BossAttackContext context)
    {
        // �{�X���g�Ƀ_���[�W��^����
        if (context.BossManager != null)
        {
            float selfDamage = context.Pattern.CancelSelfInflictedDamage;
            context.BossManager.TakeDamage(selfDamage);
        }

        context.AnimationController.EndAnimation(CreateBossAnimationControllerContext(context));
    }

    protected void DamageBoss(BossAttackContext context)
    {
        if (context.BossManager != null)
        {
            context.BossManager.TakeDamage(context.Pattern.CancelSelfInflictedDamage);
        }
    }

    protected void DamagePlayer(BossAttackContext context)
    {
        if (context.PlayerDamageable != null)
        {
            context.PlayerDamageable.TakeDamage(context.Pattern.BaseDamage);
        }
    }

    protected BossAnimationControllerContext CreateBossAnimationControllerContext(BossAttackContext context)
    {
        return new BossAnimationControllerContext
        {
            AttackType = context.Pattern.AttackType,
            AttackPort = context.SinglePort
        };
    }
}
