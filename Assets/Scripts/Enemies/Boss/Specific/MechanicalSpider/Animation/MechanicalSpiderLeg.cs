using UnityEngine;

public class MechanicalSpiderLeg
{
    private enum State { Idle, Lifting, Punching, Holding, Returning, SwingWindup, SwingWindupHold, Swinging, SwingingHold, ReturningFromSwing }

    private State currentState = State.Idle;

    private readonly Transform ikTarget;
    private readonly MechanicalSpiderLegSettings settings;
    private readonly bool isRightLeg;

    private Vector3 basePosition;
    private Vector3 punchTarget;
    private float timer;

    private Vector3 startPosition;
    private Vector3 swingTargetPosition;

    public MechanicalSpiderLeg(Transform target, MechanicalSpiderLegSettings settings, bool isRightLeg)
    {
        this.ikTarget = target;
        this.settings = settings;
        this.isRightLeg = isRightLeg;
        this.basePosition = target.localPosition;
    }

    public void UpdateLeg()
    {
        timer += Time.deltaTime;

        switch (currentState)
        {
            case State.Idle:
                UpdateIdle();
                break;
            case State.Lifting:
                UpdateLifting();
                break;
            case State.Punching:
                UpdatePunching();
                break;
            case State.Holding:
                UpdateHolding();
                break;
            case State.Returning:
                UpdateReturning();
                break;
            case State.SwingWindup:
                UpdateSwingWindup();
                break;
            case State.SwingWindupHold:
                UpdateSwingWindupHold();
                break;
            case State.Swinging:
                UpdateSwinging();
                break;
            case State.SwingingHold:
                UpdateSwingingHold();
                break;
            case State.ReturningFromSwing:
                UpdateReturningFromSwing();
                break;
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

    private void UpdateLifting()
    {
        float t = Mathf.Clamp01(timer / settings.punchLiftDuration);
        Vector3 targetPos = basePosition + Vector3.up * settings.punchLiftHeight;
        ikTarget.localPosition = Vector3.Lerp(startPosition, targetPos, t);

        if (t >= 1.0f)
        {
            // Play sound at the start of the punch.
            if (settings.punchStartClip != null && SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayEffect(settings.punchStartClip);
            }

            currentState = State.Punching;
            timer = 0f;
            startPosition = ikTarget.localPosition;
        }
    }

    private void UpdatePunching()
    {
        float t = Mathf.Clamp01(timer / settings.punchMoveDuration);
        ikTarget.localPosition = Vector3.Lerp(startPosition, punchTarget, t);

        if (t >= 1.0f)
        {
            // Play sound when the punch reaches its destination.
            if (settings.punchImpactClip != null && SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayEffect(settings.punchImpactClip);
            }

            currentState = State.Holding;
            timer = 0f;
        }
    }

    private void UpdateHolding()
    {
        float direction = isRightLeg ? 1f : -1f;
        Vector3 jitter = new Vector3(
            Mathf.Sin(timer * settings.holdJitterSpeed) * settings.holdJitterAmplitude * direction,
            0f,
            0f
        );
        ikTarget.localPosition = punchTarget + jitter;
    }

    private void UpdateReturning()
    {
        float t = Mathf.Clamp01(timer / settings.returningMoveDuration);
        ikTarget.localPosition = Vector3.Lerp(startPosition, basePosition, t);

        if (t >= 1.0f)
        {
            ResetToIdle();
        }
    }

    private void UpdateSwingWindup()
    {
        float t = Mathf.Clamp01(timer / settings.swingWindupDuration);
        float direction = isRightLeg ? 1f : -1f;
        Vector3 windupTarget = basePosition + new Vector3(-direction * settings.swingWindupDistance, 0f, 0f);
        ikTarget.localPosition = Vector3.Lerp(startPosition, windupTarget, t);

        if (t >= 1.0f)
        {
            currentState = State.SwingWindupHold;
            timer = 0f;
            startPosition = ikTarget.localPosition; // ŽŸ‚Ìó‘Ô‚Ì‚½‚ß‚ÉŒ»Ý‚ÌˆÊ’u‚ð•Û‘¶
        }
    }

    private void UpdateSwingWindupHold()
    {
        if (timer >= settings.swingWindupHoldDuration)
        {
            // Play sound at the start of the swing.
            if (settings.swingStartClip != null && SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayEffect(settings.swingStartClip);
            }

            currentState = State.Swinging;
            timer = 0f;
            startPosition = ikTarget.localPosition;
        }
    }


    private void UpdateSwinging()
    {
        float t = Mathf.Clamp01(timer / settings.swingDuration);
        float direction = isRightLeg ? 1f : -1f;
        Vector3 finalSwingTarget = basePosition + new Vector3(direction * settings.swingDistance, 0f, 0f);
        ikTarget.localPosition = Vector3.Lerp(startPosition, finalSwingTarget, t);

        if (t >= 1.0f)
        {
            currentState = State.SwingingHold;
            timer = 0f;
        }
    }

    private void UpdateSwingingHold()
    {
        if (timer >= settings.swingHoldDuration)
        {
            currentState = State.ReturningFromSwing;
            timer = 0f;
            startPosition = ikTarget.localPosition;
        }
    }

    private void UpdateReturningFromSwing()
    {
        float t = Mathf.Clamp01(timer / settings.returningFromSwingDuration);
        ikTarget.localPosition = Vector3.Lerp(startPosition, basePosition, t);
        if (t >= 1.0f)
        {
            ResetToIdle();
        }
    }

    public void StartPunch(Vector3 targetWorldPosition)
    {
        if (currentState != State.Idle) return;
        punchTarget = ikTarget.parent.InverseTransformPoint(targetWorldPosition);
        currentState = State.Lifting;
        timer = 0f;
        startPosition = ikTarget.localPosition;
    }

    public void EndPunchAndReturn()
    {
        if (currentState != State.Holding && currentState != State.Punching) return;
        currentState = State.Returning;
        timer = 0f;
        startPosition = ikTarget.localPosition;
    }

    public void StartSwing()
    {
        if (currentState != State.Idle) return;
        currentState = State.SwingWindup;
        timer = 0f;
        startPosition = ikTarget.localPosition;
    }

    public void ResetToIdle()
    {
        currentState = State.Idle;
        timer = 0f;
    }
}