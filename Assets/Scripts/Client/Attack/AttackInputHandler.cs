using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class AttackInputHandler : MonoBehaviour
{
    public struct AttackInputClossHistory
    {
        public bool IsEnter;
        public float Angle;
    }

    private TouchInputState touchState = new TouchInputState();

    // Dependencies
    private AttackVisualizer attackVisualizer;
    private AttackInputSettings settings;
    private IAttackSender attackSender;
    private InputAction pressAction;
    private InputAction holdAction;
    private Camera mainCam;

    // State management
    private bool wasInside;
    private bool isInside;
    private float entryAngle;
    private float exitAngle;
    private List<AttackInputClossHistory> clossHistory = new List<AttackInputClossHistory>();
    private AttackType? pendingAttackType;
    private Vector2 startPosition;

    /// <summary>
    /// Initializes the input handler and its dependencies.
    /// </summary>
    public void Initialize(
        AttackVisualizer visualizer,
        AttackInputSettings settings,
        IAttackSender attackSender,
        InputAction pressAction,
        InputAction holdAction,
        Camera mainCam
    )
    {
        this.attackVisualizer = visualizer;
        this.settings = settings;
        this.attackSender = attackSender;
        this.pressAction = pressAction;
        this.holdAction = holdAction;
        this.mainCam = mainCam;

        if (pressAction != null)
        {
            pressAction.started += OnPressStarted;
            pressAction.canceled += OnPressCanceled;
            pressAction.Enable();
        }

        if (holdAction != null)
        {
            holdAction.performed += OnPressHeld;
            holdAction.Enable();
        }
    }

    private void OnDisable()
    {
        if (pressAction != null)
        {
            pressAction.started -= OnPressStarted;
            pressAction.canceled -= OnPressCanceled;
            pressAction.Disable();
        }
        if (holdAction != null)
        {
            holdAction.performed -= OnPressHeld;
            holdAction.Disable();
        }

        // Clean up the visualizer's objects
        attackVisualizer?.Cleanup();
    }

    // ---- Input Events ----
    private void OnPressStarted(InputAction.CallbackContext ctx)
    {
        startPosition = GetPointerPosition();
        touchState.StartTouch(startPosition);

        attackVisualizer.SetPointerActive(true);
        attackVisualizer.UpdatePointerPosition(startPosition);

        wasInside = IsInsideDetection(startPosition);
        isInside = wasInside;
        entryAngle = 0f;
        exitAngle = 0f;
        clossHistory.Clear();
        pendingAttackType = null;
    }

    private void OnPressHeld(InputAction.CallbackContext ctx)
    {
        if (!touchState.IsTouching) return;

        Vector2 currentPos = GetPointerPosition();
        attackVisualizer.UpdatePointerPosition(currentPos);
        isInside = IsInsideDetection(currentPos);

        if (!wasInside && isInside)
        {
            entryAngle = GetAngleFromCenterFromPointer();
            pendingAttackType = AttackType.ChargedPierce;
            clossHistory.Add(new AttackInputClossHistory { IsEnter = true, Angle = entryAngle });
        }
        else if (wasInside && !isInside)
        {
            exitAngle = GetAngleFromCenterFromPointer();
            pendingAttackType = AttackType.SilkSnare;
            clossHistory.Add(new AttackInputClossHistory { IsEnter = false, Angle = exitAngle });
        }

        wasInside = isInside;
    }

    private void OnPressCanceled(InputAction.CallbackContext ctx)
    {
        if (!touchState.IsTouching) return;

        attackVisualizer.SetPointerActive(false);
        Vector2 releasePos = GetPointerPosition();
        touchState.EndTouch(releasePos);

        AttackInputData attackData = new AttackInputData();
        bool shouldSend = false;

        if (clossHistory.Count >= 2)
        {
            float angle = GetAngleFromCenterFromPointer();
            float charge = CalculateWebMineCharge();

            if (charge < settings.webMineFireBorder)
            {
                return;
            }

            attackData = new AttackInputData(
                AttackType.WebMine,
                GetDirection(angle),
                charge
            );
            shouldSend = true;
        }
        else if (pendingAttackType == AttackType.SilkSnare)
        {
            attackData = new AttackInputData(
                AttackType.SilkSnare,
                GetDirection(exitAngle),
                0f
            );
            shouldSend = true;
        }
        else if (pendingAttackType == AttackType.ChargedPierce)
        {
            float charge = touchState.HoldDuration;
            attackData = new AttackInputData(
                AttackType.ChargedPierce,
                GetDirection(entryAngle),
                charge
            );
            shouldSend = true;
        }
        else if (!isInside && touchState.HoldDuration < settings.volleyBurstHoldDuration)
        {
            float angle = GetAngleFromCenterFromPointer();
            attackData = new AttackInputData(
                AttackType.VolleyBurst,
                GetDirection(angle),
                0f
            );
            shouldSend = true;
        }

        if (shouldSend) attackSender?.SendAttack(attackData);
    }

    // ---- Utility Methods (remain in AttackInputHandler as they relate to input/logic) ----
    private Vector2 GetPointerPosition()
    {
        if (Touchscreen.current != null && Touchscreen.current.primaryTouch.press.isPressed)
            return Touchscreen.current.primaryTouch.position.ReadValue();

        return Mouse.current.position.ReadValue();
    }

    private bool IsInsideDetection(Vector2 screenPos)
    {
        Vector3 worldPos = mainCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
        worldPos.z = 0;

        Vector3 circleCenter = attackVisualizer.GetCircleCenterWorldPosition();
        float distance = Vector3.Distance(worldPos, circleCenter);
        if (distance > settings.radius) return false;
        return true;
    }
    
    private float GetAngleFromCenterFromPointer()
    {
        Vector3 worldPos = attackVisualizer.GetPointerWorldPosition();
        Vector3 circleCenter = attackVisualizer.GetCircleCenterWorldPosition();
        Vector2 localPos = (Vector2)(worldPos - circleCenter);
        return Mathf.Atan2(localPos.y, localPos.x) * Mathf.Rad2Deg;
    }

    private Vector3 GetDirection(float angle)
    {
        float rad = angle * Mathf.Deg2Rad;
        return new Vector3(Mathf.Cos(rad), Mathf.Sin(rad), 0f);
    }

    private float CalculateWebMineCharge()
    {
        float charge = 0f;
        AttackInputClossHistory? prevCloss = null;

        foreach (var cross in clossHistory)
        {
            if (prevCloss.HasValue)
            {
                bool prevIsBelow90 = prevCloss.Value.Angle <= 90f;
                bool currentIsBelow90 = cross.Angle <= 90f;

                if (prevIsBelow90 != currentIsBelow90 && cross.IsEnter != prevCloss.Value.IsEnter)
                {
                    charge += Mathf.Min(settings.webMineChargeMaxAngle, Mathf.Abs(cross.Angle - 90f));
                }
            }
            prevCloss = cross;
        }
        return charge;
    }

    private void OnDrawGizmos()
    {
        // Gizmos for visualization can be moved to AttackVisualizer if desired,
        // but for debugging purposes, it's often useful to keep it here or duplicate it.
        if (attackVisualizer == null)
        {
            return;
        }

        Vector3 center = attackVisualizer.GetCircleCenterWorldPosition();
        if (center == Vector3.zero) return;

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(center, settings.radius);

        Gizmos.color = Color.red;
        int segments = 32;

        float startAngle = -90f;
        float endAngle = 90f;

        Vector3 prevPoint = center + GetDirection(startAngle) * settings.radius;

        for (int i = 1; i <= segments; i++)
        {
            float angle = Mathf.Lerp(startAngle, endAngle, (float)i / segments);
            Vector3 currentPoint = center + GetDirection(angle) * settings.radius;
            Gizmos.DrawLine(prevPoint, currentPoint);
            prevPoint = currentPoint;
        }
    }
}