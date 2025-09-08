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

    // 外部から受け取る依存
    private GameObject circleInstance;
    private Transform circleCenter;
    private AttackInputSettings settings;
    private IAttackSender attackSender;
    private InputAction pressAction;
    private InputAction holdAction;
    private Camera mainCam;

    // 状態管理
    private bool wasInside;
    private bool isInside;
    private float insideTime;
    private float entryAngle;
    private float exitAngle;
    private List<AttackInputClossHistory> clossHistory = new List<AttackInputClossHistory>();
    private AttackType? pendingAttackType;
    private Vector2 startPosition;

    /// <summary>
    /// 外部から初期化する
    /// </summary>
    public void Initialize(
        AttackInputSettings settings,
        IAttackSender attackSender,
        InputAction pressAction,
        InputAction holdAction,
        Camera mainCam
    )
    {
        this.settings = settings;
        this.attackSender = attackSender;
        this.pressAction = pressAction;
        this.holdAction = holdAction;
        this.mainCam = mainCam;

        // 設定から値を取得
        SpawnPrefab(settings.circlePrefab);

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

    private void SpawnPrefab(GameObject circlePrefab)
    {
        if (circlePrefab != null && mainCam != null)
        {
            Vector3 screenBottomCenter = new Vector3(Screen.width / 2f, 0, 0f);
            Vector3 worldBottomCenter = mainCam.ScreenToWorldPoint(screenBottomCenter);
            worldBottomCenter.z = 0f;

            circleInstance = Instantiate(circlePrefab, worldBottomCenter, Quaternion.identity);
            circleCenter = circleInstance.transform;

            if (circleInstance != null)
            {
                circleInstance.transform.localScale = new Vector3(settings.radius * 2, settings.radius * 2, 1);
            }
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
    }

    // ---- 入力イベント ----
    private void OnPressStarted(InputAction.CallbackContext ctx)
    {
        startPosition = GetPointerPosition();
        touchState.StartTouch(startPosition);

        wasInside = IsInsideDetection(startPosition);
        isInside = wasInside;
        insideTime = 0f;
        entryAngle = 0f;
        exitAngle = 0f;
        clossHistory.Clear();
        pendingAttackType = null;
    }

    private void OnPressHeld(InputAction.CallbackContext ctx)
    {
        if (!touchState.IsTouching) return;

        Vector2 currentPos = GetPointerPosition();
        isInside = IsInsideDetection(currentPos);

        if (!wasInside && isInside)
        {
            entryAngle = GetAngleFromCenter(currentPos);
            pendingAttackType = AttackType.ChargedPierce;
            clossHistory.Add(new AttackInputClossHistory { IsEnter = true, Angle = entryAngle });
        }
        else if (wasInside && !isInside)
        {
            exitAngle = GetAngleFromCenter(currentPos);
            pendingAttackType = AttackType.SilkSnare;
            clossHistory.Add(new AttackInputClossHistory { IsEnter = false, Angle = exitAngle });
        }

        wasInside = isInside;

        if (isInside) insideTime += Time.deltaTime;
    }

    private void OnPressCanceled(InputAction.CallbackContext ctx)
    {
        if (!touchState.IsTouching) return;

        Vector2 releasePos = GetPointerPosition();
        touchState.EndTouch(releasePos);

        AttackInputData attackData = new AttackInputData();
        bool shouldSend = false;

        if (clossHistory.Count >= 2)
        {
            float angle = GetAngleFromCenter(releasePos);
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
            Vector2 direction = (startPosition - releasePos).normalized;
            attackData = new AttackInputData(
                AttackType.ChargedPierce,
                direction,
                charge
            );
            shouldSend = true;
        }
        else if (!isInside && touchState.HoldDuration < settings.volleyBurstHoldDuration)
        {
            float angle = GetAngleFromCenter(releasePos);
            attackData = new AttackInputData(
                AttackType.VolleyBurst,
                GetDirection(angle),
                0f
            );
            shouldSend = true;
        }

        if (shouldSend) attackSender?.SendAttack(attackData);
    }

    // ---- Utility ----
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

        float distance = Vector3.Distance(worldPos, circleCenter.position);
        if (distance > settings.radius) return false;

        if (worldPos.y < circleCenter.position.y) return false;

        Vector2 localPos = worldPos - circleCenter.position;
        float angle = Vector2.SignedAngle(Vector2.up, localPos);
        bool inSemi = angle >= -90f && angle <= 90f;
        return settings.mirrorHorizontally ? !inSemi : inSemi;
    }

    private float GetAngleFromCenter(Vector2 screenPos)
    {
        Vector3 worldPos = mainCam.ScreenToWorldPoint(new Vector3(screenPos.x, screenPos.y, 0));
        worldPos.z = 0;

        Vector2 localPos = worldPos - circleCenter.position;
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
        if (circleCenter == null || mainCam == null)
        {
            return;
        }

        Gizmos.color = Color.cyan;
        Gizmos.DrawWireSphere(circleCenter.position, settings.radius);

        Gizmos.color = Color.red;
        Vector3 center = circleCenter.position;
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