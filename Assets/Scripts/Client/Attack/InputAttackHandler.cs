//using UnityEngine;

//public class InputAttackHandler : MonoBehaviour
//{
//    [Header("Config")]
//    private InputAttackConfig inputConfig;
//    private BetterTouchHandler touchHandler;
//    private Vector3 centerPosition;
//    private IAttackRecognizer attackRecognizer;
//    private IAttackSender attackSender;
//    private Camera gameCamera;

//    private Vector3 bottomCenterPosition;

//    private readonly TouchInputState touchState = new TouchInputState();
//    private readonly CircularGestureTracker circularGestureTracker = new CircularGestureTracker(3f);

//    // スワイプ入力を「上方向基準」に変換するための回転行列
//    private Matrix4x4 screenRotationMatrix = Matrix4x4.identity;

//    #region Public API
//    public void Setup(
//        InputAttackConfig inputConfig,
//        IAttackSender attackSender,
//        BetterTouchHandler touchHandler,
//        AttackRecognizer attackRecognizer,
//        Camera gameCamera,
//        Vector3 centerPosition,
//        Direction inputDirection)
//    {
//        CleanupSubscriptions();

//        if (!ValidateDependencies(attackSender, attackRecognizer, gameCamera, touchHandler))
//        {
//            enabled = false;
//            return;
//        }

//        SubscribeToTouchEvents(touchHandler);

//        this.enabled = true;
//        this.inputConfig = inputConfig;
//        this.touchHandler = touchHandler;
//        this.attackRecognizer = attackRecognizer;
//        this.attackSender = attackSender;
//        this.gameCamera = gameCamera;
//        this.centerPosition = centerPosition;
//        this.screenRotationMatrix = GetScreenRotationMatrix(inputDirection);

//        this.bottomCenterPosition = CalculateBottomCenterFromInputDirection(inputDirection);

//#if UNITY_EDITOR
//        Debug.Log($"[InputAttackHandler] Bottom center world position: {bottomCenterPosition}");
//#endif
//    }
//    #endregion

//    #region Unity Lifecycle
//    private void OnDisable()
//    {
//        CleanupSubscriptions();
//    }
//    #endregion

//    #region Event Handlers
//    private void OnTouchStart(Vector2 screenPos)
//    {
//        touchState.StartTouch(screenPos);
//        circularGestureTracker.StartTracking(screenPos, centerPosition);
//    }

//    private void OnTouchMove(Vector2 screenPos)
//    {
//        if (TryGetCenterScreenPosition(out var centerScreen))
//        {
//            circularGestureTracker.UpdateTracking(screenPos, centerScreen);
//        }
//    }

//    private void OnTouchEnd(Vector2 screenPos)
//    {
//        if (!touchState.IsTouching || attackSender == null) return;

//        touchState.EndTouch(screenPos);

//        if (!TryGetCenterScreenPosition(out var centerScreen)) return;

//        var inputData = BuildTouchInputData(screenPos, centerScreen);

//        foreach (var attack in attackRecognizer.Recognize(inputData, centerScreen, inputConfig))
//        {
//            SendAttack(attack);
//        }

//        circularGestureTracker.Reset();
//    }
//    #endregion

//    #region Core Logic
//    private TouchInputData BuildTouchInputData(Vector2 endPos, Vector2 centerScreen)
//    {
//        // 入力座標を「上方向基準」に回転して扱う
//        Vector2 start = RotateScreenPos(touchState.TouchStart);
//        Vector2 end = RotateScreenPos(endPos);
//        Vector2 center = RotateScreenPos(centerScreen);
//        Vector2 bottomCenter = RotateScreenPos(GetBottomCenterScreenPosition());

//        Vector2 movement = end - start;
//        Vector2 centerDir = (end - center).normalized;
//        Vector2 bottomDir = (end - bottomCenter).normalized;

//        // forward 判定は常に「上方向基準」
//        bool isForward = Vector2.Dot(movement.normalized, Vector2.up) > 0f;

//        bool isCircularAttack = Mathf.Abs(circularGestureTracker.TotalDistance - movement.magnitude)
//                                > inputConfig.circularGestureDistanceThreshold;

//        float dpi = Screen.dpi > 0 ? Screen.dpi : inputConfig.fallbackDpi;

//        float gestureAmount = isCircularAttack
//            ? GameMath.WeightedGeometricMean(
//                circularGestureTracker.TotalRotation,
//                circularGestureTracker.TotalDistance,
//                inputConfig.circularRotationWeight,
//                inputConfig.circularDistanceWeight,
//                false)
//            : 0;

//        return new TouchInputData
//        {
//            StartWithInCenter = IsWithinCenter(start),
//            EndWithInCenter = IsWithinCenter(end),
//            HoldDuration = touchState.HoldDuration,
//            SwipeDistance = movement.magnitude,
//            SwipeDirection = movement.normalized,
//            FromCenterDirection = centerDir,
//            FromBottomCenterDirection = bottomDir,
//            IsSwipeForward = isForward,
//            CircularGestureAmount = gestureAmount / dpi
//        };
//    }

//    private void SendAttack(AttackInputData attack)
//    {
//        attackSender?.SendAttack(attack);
//    }

//    private bool IsWithinCenter(Vector2 screenPos)
//    {
//        return TryGetCenterScreenPosition(out var center) &&
//               Vector2.Distance(screenPos, center) <= inputConfig.centerRadius;
//    }
//    #endregion

//    #region Helpers
//    private bool TryGetCenterScreenPosition(out Vector2 screenPos)
//    {
//        if (gameCamera == null)
//        {
//            screenPos = default;
//            return false;
//        }

//        var worldToScreen = gameCamera.WorldToScreenPoint(centerPosition);
//        screenPos = new Vector2(worldToScreen.x, worldToScreen.y);
//        return true;
//    }

//    private Vector2 GetBottomCenterScreenPosition()
//    {
//        var worldToScreen = gameCamera.WorldToScreenPoint(bottomCenterPosition);
//        return new Vector2(worldToScreen.x, worldToScreen.y);
//    }

//    private Vector3 CalculateBottomCenterFromInputDirection(Direction inputDirection)
//    {
//        if (gameCamera == null) return Vector3.zero;

//        Vector2 screenCenter = new Vector2(Screen.width / 2f, Screen.height / 2f);
//        Vector2 offset = Vector2.zero;

//        switch (inputDirection)
//        {
//            case Direction.Up:
//                offset = Vector2.down * (Screen.height / 2f);
//                break;
//            case Direction.Down:
//                offset = Vector2.up * (Screen.height / 2f);
//                break;
//            case Direction.Left:
//                offset = Vector2.left * (Screen.width / 2f);
//                break;
//            case Direction.Right:
//                offset = Vector2.right * (Screen.width / 2f);
//                break;
//        }

//        Vector2 bottomScreen = screenCenter + offset;

//        float zDistance = centerPosition.z - gameCamera.transform.position.z;
//        return gameCamera.ScreenToWorldPoint(new Vector3(bottomScreen.x, bottomScreen.y, zDistance));
//    }


//    private bool ValidateDependencies(
//        IAttackSender sender,
//        IAttackRecognizer recognizer,
//        Camera cam,
//        BetterTouchHandler touchHandler)
//    {
//        if (sender == null || recognizer == null || cam == null)
//        {
//            Debug.LogError("Setup failed: Required references are null.");
//            return false;
//        }

//        if (touchHandler == null)
//        {
//            Debug.LogError("touchHandler is not assigned. Disabling InputAttackHandler.");
//            return false;
//        }

//        return true;
//    }

//    private void SubscribeToTouchEvents(BetterTouchHandler handler)
//    {
//        handler.OnTouchStartEvent += OnTouchStart;
//        handler.OnTouchEndEvent += OnTouchEnd;
//        handler.OnTouchMoveEvent += OnTouchMove;
//    }

//    private void CleanupSubscriptions()
//    {
//        if (touchHandler != null)
//        {
//            touchHandler.OnTouchStartEvent -= OnTouchStart;
//            touchHandler.OnTouchEndEvent -= OnTouchEnd;
//            touchHandler.OnTouchMoveEvent -= OnTouchMove;
//        }
//    }
//    #endregion

//    #region Rotation Helpers
//    private Matrix4x4 GetScreenRotationMatrix(Direction dir)
//    {
//        switch (dir)
//        {
//            case Direction.Up: return Matrix4x4.Rotate(Quaternion.identity);
//            case Direction.Down: return Matrix4x4.Rotate(Quaternion.Euler(0, 0, 180f));
//            case Direction.Left: return Matrix4x4.Rotate(Quaternion.Euler(0, 0, 90f));
//            case Direction.Right: return Matrix4x4.Rotate(Quaternion.Euler(0, 0, -90f));
//            default: return Matrix4x4.identity;
//        }
//    }

//    private Vector2 RotateScreenPos(Vector2 pos)
//    {
//        return screenRotationMatrix.MultiplyPoint3x4(pos);
//    }
//    #endregion
//}
