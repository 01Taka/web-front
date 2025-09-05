using DG.Tweening;
using UnityEngine;

public class MultiStagePunchHandler
{
    private enum State { Idle, Moving, Holding, Returning }

    private State currentState = State.Idle;
    private Sequence currentSequence;

    private readonly MechanicalSpiderLeg leg;
    private bool _isShaking = false;

    public MultiStagePunchHandler(MechanicalSpiderLeg leg)
    {
        this.leg = leg;
    }

    /// <summary>
    /// 足を振りかぶる動きを開始します。
    /// </summary>
    /// <param name="target">振りかぶりの目標位置</param>
    /// <param name="duration">振りかぶりにかかる時間</param>
    public void Windup(Vector3 target, float duration)
    {
        if (currentState != State.Idle) return;

        // DOTweenによる移動アニメーション
        currentSequence?.Kill();  // 既存のシーケンスがあればキャンセル

        currentSequence = DOTween.Sequence()
            .Append(DOTween.To(() => leg.GetIKTargetLocalPosition(),
                               pos => leg.SetIKTargetLocalPosition(pos),
                               target,
                               duration))  // 目標位置へ移動
            .OnStart(() => currentState = State.Moving)
            .OnComplete(() =>
            {
                currentState = State.Holding;  // 移動完了後、Holding状態に
            });
    }

    /// <summary>
    /// 次の目標位置へ移動を開始します。
    /// </summary>
    /// <param name="target">次の目標位置</param>
    /// <param name="duration">移動にかかる時間</param>
    public void MoveToTarget(Vector3 target, float duration)
    {
        if (currentState != State.Holding) return; // 待機状態からのみ開始

        // DOTweenによる移動アニメーション
        currentSequence?.Kill();  // 既存のシーケンスがあればキャンセル

        currentSequence = DOTween.Sequence()
            .Append(DOTween.To(() => leg.GetIKTargetLocalPosition(),
                               pos => leg.SetIKTargetLocalPosition(pos),
                               target,
                               duration))  // 目標位置へ移動
            .OnStart(() => currentState = State.Moving)
            .OnComplete(() =>
            {
                currentState = State.Holding;  // 移動完了後、Holding状態に
            });
    }

    /// <summary>
    /// 特定の座標で腕を振動させるアニメーションを開始します。
    /// </summary>
    /// <param name="shakePosition">振動の中心となるワールド座標</param>
    /// <param name="duration">振動させる時間</param>
    /// <param name="strength">振動の強さ</param>
    /// <param name="vibrato">振動の細かさ（頻度）</param>
    public void StartShake(Vector3 shakePosition, float duration, float strength = 0.1f, int vibrato = 20)
    {
        // ホールド中のみ呼び出せる
        if (currentState != State.Holding || _isShaking) return;

        // 既存のシーケンスを停止
        currentSequence?.Kill(true);

        Vector3 localShakePosition = leg.GetIKTargetParent().InverseTransformPoint(shakePosition);

        currentSequence = DOTween.Sequence()
            .Append(DOTween.Shake(() => leg.GetIKTargetLocalPosition(),
                                  pos => leg.SetIKTargetLocalPosition(pos),
                                  duration,
                                  strength,
                                  vibrato,
                                  90f, // ランダムな方向を90度以内にする
                                  false))
            .OnStart(() => _isShaking = true)
            .OnComplete(() =>
            {
                // 振動完了後、元の中心位置に戻る
                leg.SetIKTargetLocalPosition(localShakePosition);
                _isShaking = false;
            });
    }

    /// <summary>
    /// 元のベース位置に戻る動きを開始します。
    /// </summary>
    /// <param name="duration">戻るのにかかる時間</param>
    public void ReturnToIdle(float duration)
    {
        // 振りかぶりやパンチ完了後の状態から開始
        if (currentState != State.Holding && currentState != State.Moving) return;

        // DOTweenによる移動アニメーション
        currentSequence?.Kill();  // 既存のシーケンスがあればキャンセル

        currentSequence = DOTween.Sequence()
            .Append(DOTween.To(() => leg.GetIKTargetLocalPosition(),
                               pos => leg.SetIKTargetLocalPosition(pos),
                               leg.BasePosition,
                               duration))  // 元の位置に戻る
            .OnStart(() => currentState = State.Returning)
            .OnComplete(() =>
            {
                currentState = State.Idle;  // 戻ったらIdle状態に
            });
    }

    /// <summary>
    /// 現在の移動が完了したかどうかを返します。
    /// </summary>
    public bool IsHolding()
    {
        return currentState == State.Holding;
    }

    /// <summary>
    /// 足がアイドル状態に戻ったかどうかを返します。
    /// </summary>
    public bool IsComplete()
    {
        return currentState == State.Idle;
    }
}
