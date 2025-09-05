
using UnityEngine;

/// <summary>
/// IBossAttackインターフェースを実装する抽象基底クラス。
/// すべての具体的な攻撃ロジックの共通機能を定義します。
/// </summary>
public abstract class BossAttackBase : MonoBehaviour, IBossAttack
{
    public abstract BossAttackType AttackType { get; }

    protected IBossAttacdkLogicCallbasks _callbacks;

    public void InitializeAttackCallbacks(IBossAttacdkLogicCallbasks callbasks)
    {
        _callbacks = callbasks;
    }

    // 抽象メソッドとして定義し、子クラスでの実装を強制します。
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
