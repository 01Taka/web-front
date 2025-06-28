using UnityEngine;

public class InputAttackHandler : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private BetterTouchHandler touchHandler;
    [SerializeField] private Vector3 centerPosition;

    [Header("Config")]
    [SerializeField] private float centerRadius = 100f;

    private IAttackSender attackSender;
    private Camera gameCamera;

    private readonly TouchInputState touchState = new TouchInputState();
    private readonly OrbChargeTracker orbTracker = new OrbChargeTracker();
    private readonly VolleyQueue volleyQueue = new VolleyQueue();

    private const float MinSwipeDistance = 60f;
    private const float BowstringMinHold = 0.0f;

    public void Setup(IAttackSender attackSender, Camera gameCamera)
    {
        if (attackSender == null)
        {
            Debug.LogError("Setup failed: PlayerNetwork is null.");
            enabled = false;
            return;
        }

        if (gameCamera == null)
        {
            Debug.LogError("Setup failed: GameCamera is null.");
            enabled = false;
            return;
        }

        this.attackSender = attackSender;
        this.gameCamera = gameCamera;
    }

    void OnEnable()
    {
        if (touchHandler == null)
        {
            Debug.LogError("touchHandler is not assigned. Disabling InputAttackHandler.");
            enabled = false;
            return;
        }

        touchHandler.OnTouchStartEvent += OnTouchStart;
        touchHandler.OnTouchEndEvent += OnTouchEnd;
    }

    void OnDisable()
    {
        if (touchHandler != null)
        {
            touchHandler.OnTouchStartEvent -= OnTouchStart;
            touchHandler.OnTouchEndEvent -= OnTouchEnd;
        }
    }

    void Update()
    {
        if (!touchState.IsTouching || volleyQueue.HasQueuedShots) return;

        Vector3? centerScreen = GetCenterScreenPosition();
        if (centerScreen == null) return;

        orbTracker.Update(touchState.CurrentTouchPos, centerScreen.Value);
    }

    private void SendAttack(AttackType type, Vector3 direction, float chargeAmount = 0f, int shotCount = 0)
    {
        AttackInputData data = new()
        {
            Type = type,
            Direction = direction,
            ChargeAmount = chargeAmount,
            ShotCount = shotCount
        };
        attackSender.SendAttack(data);
    }

    void OnTouchStart(Vector2 current)
    {
        if (attackSender == null)
        {
            Debug.LogError("attackSender is null. Cannot send attack.");
            return;
        }

        touchState.StartTouch(current);
        orbTracker.Reset(current);

        Debug.Log($"Touch started. Within center: {IsWithinCenter(current)}");

        if (volleyQueue.HasQueuedShots || !IsWithinCenter(current))
        {
            volleyQueue.QueueShot();
        }
    }

    void OnTouchEnd(Vector2 touchEnd)
    {
        if (!touchState.IsTouching || attackSender == null) return;

        touchState.EndTouch(touchEnd);

        Vector2 start = touchState.TouchStart;

        Vector3? centerScreen = GetCenterScreenPosition();
        if (centerScreen == null) return;

        Vector2 swipeDir = (touchEnd - (Vector2)centerScreen.Value).normalized;
        Vector2 movement = touchEnd - start;
        float swipeLength = movement.magnitude;
        float holdDuration = touchState.HoldDuration;

        if (orbTracker.IsCharged)
        {
            SendAttack(AttackType.OrbWeaver, ScreenDirectionToWorld(swipeDir), orbTracker.ChargeAmount);
            orbTracker.Clear();
            return;
        }

        if (volleyQueue.ShouldFireVolley(swipeLength, MinSwipeDistance))
        {
            SendAttack(AttackType.WebVolley, ScreenDirectionToWorld(swipeDir), 0f, volleyQueue.Consume());
            return;
        }

        if (IsWithinCenter(start))
        {
            Direction upDir = Direction.Up;
            Vector2 toEnd = touchEnd - (Vector2)centerScreen.Value;
            bool isToForward = Vector2.Dot(toEnd, upDir.ToVector2()) > 0;

            if (isToForward && swipeLength > MinSwipeDistance)
            {
                SendAttack(AttackType.SilkSnare, ScreenDirectionToWorld(swipeDir));
            }
            else if (!isToForward && swipeLength > MinSwipeDistance && holdDuration > BowstringMinHold)
            {
                SendAttack(AttackType.BowstringPiercer, ScreenDirectionToWorld(-swipeDir), holdDuration);
            }
        }
    }

    bool IsWithinCenter(Vector2 screenPos)
    {
        Vector3? center = GetCenterScreenPosition();
        if (center == null) return false;

        return Vector2.Distance(screenPos, center.Value) <= centerRadius;
    }

    Vector3? GetCenterScreenPosition()
    {
        if (gameCamera == null)
        {
            Debug.LogError("Main Camera not found.");
            return null;
        }

        return gameCamera.WorldToScreenPoint(centerPosition);
    }

    Vector3 ScreenDirectionToWorld(Vector2 screenDirection)
    {
        if (gameCamera == null)
        {
            Debug.LogError("Main Camera not found. Cannot calculate direction.");
            return Vector3.forward;
        }

        // スクリーン中心からの相対スワイプ方向 → スクリーン座標の一部として利用
        Vector3 screenFrom = new Vector3(Screen.width * 0.5f, Screen.height * 0.5f, gameCamera.nearClipPlane);
        Vector3 screenTo = screenFrom + new Vector3(screenDirection.x, screenDirection.y, 0f);

        // ワールド空間に変換
        Vector3 worldFrom = gameCamera.ScreenToWorldPoint(screenFrom);
        Vector3 worldTo = gameCamera.ScreenToWorldPoint(screenTo);
        Vector3 worldDirection = (worldTo - worldFrom).normalized;

        // Y方向を無視してXZ平面での方向に限定（任意）
        worldDirection.y = 0;
        return worldDirection.normalized;
    }
}
