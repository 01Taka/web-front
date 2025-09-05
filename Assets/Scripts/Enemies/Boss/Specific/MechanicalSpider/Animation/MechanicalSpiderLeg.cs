using DG.Tweening;
using TMPro;
using UnityEngine;

public class MechanicalSpiderLeg
{
    private enum State { Idle, Swinging, ReturningFromSwing }

    private State currentState = State.Idle;
    private readonly MultiStagePunchHandler _punchHandler;

    private readonly Transform ikTarget;
    private readonly MechanicalSpiderLegSettings settings;
    private readonly bool isRightLeg;

    private Vector3 basePosition;
    private float timer;

    public Vector3 BasePosition => basePosition;

    public MechanicalSpiderLeg(Transform target, MechanicalSpiderLegSettings settings, bool isRightLeg)
    {
        this.ikTarget = target;
        this.settings = settings;
        this.isRightLeg = isRightLeg;
        this.basePosition = target.localPosition;
        this._punchHandler = new MultiStagePunchHandler(this);
    }

    public void UpdateLeg()
    {
        // Swing logic is now handled by a single DOTween sequence,
        // so no state-based Update logic is needed for it.
        timer += Time.deltaTime;

        if (currentState == State.Idle && IsPunchComplete())
        {
            UpdateIdle();
        }
    }

    private void UpdateIdle()
    {
        float direction = isRightLeg ? 1f : -1f;
        Vector3 offset = new Vector3(
            Mathf.Sin(timer * settings.idleSpeed) * settings.idleAmplitude * direction,
            0f,
            0f
        );
        ikTarget.localPosition = basePosition + offset;
    }

    // --- Public Methods for External Control ---
    public void StartPunchWindup(float duration)
    {
        Vector3 targetPosition = ikTarget.localPosition;
        targetPosition.y = ikTarget.localPosition.y + settings.PunchLiftHeight;
        _punchHandler.Windup(targetPosition, duration);
    }

    /// <summary>
    /// パンチシーケンスを開始し、振りかぶりから目標位置に移動します。
    /// </summary>
    /// <param name="targetWorldPosition">パンチの目標位置（ワールド座標）</param>
    /// <param name="duration">パンチにかかる時間</param>
    public void StartPunchingSequence(Vector3 targetWorldPosition, float duration)
    {
        Vector3 localTarget = ikTarget.parent.InverseTransformPoint(targetWorldPosition);
        _punchHandler.MoveToTarget(localTarget, duration);
    }

    public void StartPunchShaking(float duration)
    {
        Vector3 targetPosition = ikTarget.localPosition;
        targetPosition.y = ikTarget.localPosition.y + settings.PunchLiftHeight;
        _punchHandler.StartShake(targetPosition, duration, settings.PunchShakeStrength, settings.PunchShaleVibrato);
    }

    /// <summary>
    /// 現在のパンチシーケンスを完了し、元の位置に戻ります。
    /// </summary>
    /// <param name="duration">戻るのにかかる時間</param>
    public void ReturnToIdleFromPunch(float duration)
    {
        _punchHandler.ReturnToIdle(duration);
    }

    /// <summary>
    /// スイングシーケンス全体を開始します。
    /// </summary>
    public void StartSwing()
    {
        if (currentState != State.Idle) return;

        // Start the combined swing sequence
        Vector3 windupTarget = basePosition + new Vector3(-(isRightLeg ? 1f : -1f) * settings.swingWindupDistance, 0f, 0f);
        Vector3 finalSwingTarget = basePosition + new Vector3((isRightLeg ? 1f : -1f) * settings.swingDistance, 0f, 0f);

        // Sequence creation
        var sequence = DOTween.Sequence();

        // 1. Windup
        sequence.Append(DOTween.To(() => ikTarget.localPosition,
                pos => ikTarget.localPosition = pos,
                windupTarget,
                settings.swingWindupDuration)
            .SetEase(Ease.OutQuad));

        // 2. Windup Hold
        sequence.AppendInterval(settings.swingWindupHoldDuration);

        // 3. Swinging
        sequence.Append(DOTween.To(() => ikTarget.localPosition,
                pos => ikTarget.localPosition = pos,
                finalSwingTarget,
                settings.swingDuration)
            .SetEase(Ease.InSine)
            .OnStart(() =>
            {
                if (settings.swingStartClip != null && SoundManager.Instance != null)
                {
                    SoundManager.Instance.PlayEffect(settings.swingStartClip);
                }
            }));

        // 4. Swinging Hold
        sequence.AppendInterval(settings.swingHoldDuration);

        // 5. Return to Idle
        sequence.Append(DOTween.To(() => ikTarget.localPosition,
                pos => ikTarget.localPosition = pos,
                basePosition,
                settings.returningFromSwingDuration)
            .SetEase(Ease.OutSine));

        // On complete, reset to Idle state
        sequence.OnComplete(() => ResetToIdle());

        // Set the state to Swinging as soon as the sequence starts
        currentState = State.Swinging;
    }

    // --- Public Helper Methods for Handlers ---

    public Transform GetIKTargetParent()
    {
        return ikTarget.parent;
    }

    public Vector3 GetIKTargetLocalPosition()
    {
        return ikTarget.localPosition;
    }

    public void SetIKTargetLocalPosition(Vector3 newPosition)
    {
        ikTarget.localPosition = newPosition;
    }

    public MechanicalSpiderLegSettings GetSettings()
    {
        return settings;
    }

    public bool IsPuncnHolding()
    {
        return _punchHandler.IsHolding();
    }

    public bool IsPunchComplete()
    {
        return _punchHandler.IsComplete();
    }

    public void ResetToIdle()
    {
        currentState = State.Idle;
        timer = 0f;
    }
}