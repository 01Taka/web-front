
using UnityEngine;

/// <summary>
/// IBossAttack�C���^�[�t�F�[�X���������钊�ۊ��N���X�B
/// ���ׂĂ̋�̓I�ȍU�����W�b�N�̋��ʋ@�\���`���܂��B
/// </summary>
public abstract class BossAttackBase : MonoBehaviour, IBossAttack
{
    public abstract BossAttackType AttackType { get; }

    protected IBossAttacdkLogicCallbasks _callbacks;

    public void InitializeAttackCallbacks(IBossAttacdkLogicCallbasks callbasks)
    {
        _callbacks = callbasks;
    }

    // ���ۃ��\�b�h�Ƃ��Ē�`���A�q�N���X�ł̎������������܂��B
    public virtual void OnBeginPreparation(BossAttackContext context)
    {
        StartAnimation(context);
    }

    public virtual void ExecuteAttack(BossAttackContext context)
    {
        Debug.Log("ExecuteAttack");
        DamagePlayer(context);
        EndAnimation(context);
    }

    public virtual void OnCanceledAttack(BossAttackContext context)
    {
        Debug.Log("OnCanceledAttack");
        DamageBoss(context);
        EndAnimation(context);
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

    protected void StartAnimation(BossAttackContext context)
    {
        context.AnimationController.StartAnimation(CreateBossAnimationControllerContext(context));
    }

    protected void EndAnimation(BossAttackContext context)
    {
        context.AnimationController.EndAnimation(CreateBossAnimationControllerContext(context));
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
