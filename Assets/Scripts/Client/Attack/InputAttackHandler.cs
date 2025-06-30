using UnityEngine;

public class InputAttackHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BetterTouchHandler touchHandler;
    [SerializeField] private Vector3 centerPosition;
    private Vector3 bottomCenterPosition;

    [Header("Config")]
    [SerializeField] private InputAttackConfig inputConfig;

    private IAttackRecognizer attackRecognizer;
    private IAttackSender attackSender;
    private Camera gameCamera;
    private Vector2 swipeForwardDirection;

    private readonly TouchInputState touchState = new TouchInputState();
    private readonly CircularGestureTracker circularGestureTracker = new CircularGestureTracker(3f);

    public void Setup(IAttackSender attackSender, AttackRecognizer attackRecognizer, Camera gameCamera, Vector2 swipeForwardDirection)
    {
        if (attackSender == null || attackRecognizer == null || gameCamera == null)
        {
            Debug.LogError("Setup failed: Required references are null.");
            enabled = false;
            return;
        }

        this.attackRecognizer = attackRecognizer;
        this.attackSender = attackSender;
        this.gameCamera = gameCamera;
        this.swipeForwardDirection = swipeForwardDirection;
        this.bottomCenterPosition = CalculateBottomCenterFromSwipeDirection();

        Debug.Log($"[InputAttackHandler] Bottom center world position: {bottomCenterPosition}");
    }

    private void OnEnable()
    {
        if (touchHandler == null)
        {
            Debug.LogError("touchHandler is not assigned. Disabling InputAttackHandler.");
            enabled = false;
            return;
        }

        touchHandler.OnTouchStartEvent += OnTouchStart;
        touchHandler.OnTouchEndEvent += OnTouchEnd;
        touchHandler.OnTouchMoveEvent += OnTouchMove;
    }

    private void OnDisable()
    {
        if (touchHandler != null)
        {
            touchHandler.OnTouchStartEvent -= OnTouchStart;
            touchHandler.OnTouchEndEvent -= OnTouchEnd;
            touchHandler.OnTouchMoveEvent -= OnTouchMove;
        }
    }

    private void OnTouchStart(Vector2 screenPos)
    {
        touchState.StartTouch(screenPos);
        circularGestureTracker.StartTracking(screenPos, centerPosition);
    }

    private void OnTouchMove(Vector2 screenPos)
    {
        var centerScreen = GetCenterScreenPosition();
        if (centerScreen.HasValue)
        {
            circularGestureTracker.UpdateTracking(screenPos, centerScreen.Value);
        }
    }

    private void OnTouchEnd(Vector2 screenPos)
    {
        if (!touchState.IsTouching || attackSender == null) return;

        touchState.EndTouch(screenPos);

        var centerScreen = GetCenterScreenPosition();
        if (!centerScreen.HasValue) return;

        Vector2 start = touchState.TouchStart;
        Vector2 movement = screenPos - start;
        Vector2 centerDir = (screenPos - centerScreen.Value).normalized;
        Vector2 bottomDir = (screenPos - GetBottomCenterScreenPosition()).normalized;
        bool isForward = Vector2.Dot(movement.normalized, swipeForwardDirection.normalized) > 0f;

        bool isCircularAttack = Mathf.Abs(circularGestureTracker.TotalDistance - movement.magnitude)
                                > inputConfig.circularGestureDistanceThreshold;

        float dpi = Screen.dpi > 0 ? Screen.dpi : inputConfig.fallbackDpi;

        float gestureAmount = isCircularAttack
            ? GameMath.WeightedGeometricMean(
                circularGestureTracker.TotalRotation,
                circularGestureTracker.TotalDistance,
                inputConfig.circularRotationWeight,
                inputConfig.circularDistanceWeight,
                false)
            : 0;

        var inputData = new TouchInputData
        {
            StartWithInCenter = IsWithinCenter(start),
            EndWithInCenter = IsWithinCenter(screenPos),
            HoldDuration = touchState.HoldDuration,
            SwipeDistance = movement.magnitude,
            SwipeDirection = movement.normalized,
            FromCenterDirection = centerDir,
            FromBottomCenterDirection = bottomDir,
            IsSwipeForward = isForward,
            CicularGestureAmount = gestureAmount / dpi
        };

        foreach (var attack in attackRecognizer.Recognize(inputData, centerScreen.Value, inputConfig))
        {
            SendAttack(attack);
        }

        circularGestureTracker.Reset();
    }

    private void SendAttack(AttackInputData attack)
    {
        attackSender.SendAttack(attack);
    }

    private bool IsWithinCenter(Vector2 screenPos)
    {
        var center = GetCenterScreenPosition();
        return center.HasValue && Vector2.Distance(screenPos, center.Value) <= inputConfig.centerRadius;
    }

    private Vector2? GetCenterScreenPosition()
    {
        return gameCamera?.WorldToScreenPoint(centerPosition);
    }

    private Vector2 GetBottomCenterScreenPosition()
    {
        return gameCamera.WorldToScreenPoint(bottomCenterPosition);
    }

    private Vector3 CalculateBottomCenterFromSwipeDirection()
    {
        if (gameCamera == null) return Vector3.zero;

        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
        Vector2 bottomScreen = screenCenter - swipeForwardDirection.normalized * (Screen.height / 2f);

        float zDistance = Mathf.Abs(gameCamera.transform.position.z); // or desired depth
        return gameCamera.ScreenToWorldPoint(new Vector3(bottomScreen.x, bottomScreen.y, zDistance));
    }
}
