
using UnityEngine;

/// <summary>
/// IBossAttackインターフェースを実装する抽象基底クラス。
/// すべての具体的な攻撃ロジックの共通機能を定義します。
/// </summary>
public abstract class BossAttackBase : MonoBehaviour, IBossAttack
{
    public abstract BossAttackType AttackType { get; }

    // 抽象メソッドとして定義し、子クラスでの実装を強制します。
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
        // ボス自身にダメージを与える
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
