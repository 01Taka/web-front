using UnityEngine;

/// <summary>
/// 画面上での円形ジェスチャー量（回転と距離）を測定する汎用トラッカー。
/// </summary>
public class CircularGestureTracker
{
    private float accumulatedAngle = 0f;
    private float accumulatedDistance = 0f;
    private float lastAngle = 0f;
    private Vector2 lastScreenPosition;
    private bool wasInDeadZoneLastFrame = false;

    private readonly float deadZoneThresholdSqr;

    /// <summary>
    /// 総回転角（deg）
    /// </summary>
    public float TotalRotation => accumulatedAngle;

    /// <summary>
    /// 総移動距離（ピクセル）
    /// </summary>
    public float TotalDistance => accumulatedDistance;

    /// <summary>
    /// 中心座標に対して累積角と距離を測るトラッカーを初期化。
    /// </summary>
    public CircularGestureTracker(float deadZoneRadius = 3f)
    {
        deadZoneThresholdSqr = deadZoneRadius * deadZoneRadius;
    }

    /// <summary>
    /// トラッキング初期化（タッチ開始時に呼ぶ）
    /// </summary>
    public void StartTracking(Vector2 currentPosition, Vector2 center)
    {
        accumulatedAngle = 0f;
        accumulatedDistance = 0f;
        lastScreenPosition = currentPosition;
        lastAngle = GetAngleFromCenter(currentPosition, center);
        wasInDeadZoneLastFrame = false;
    }

    /// <summary>
    /// 毎フレーム、現在位置と中心を渡すことでトラッキング更新。
    /// </summary>
    public void UpdateTracking(Vector2 currentPosition, Vector2 center)
    {
        Vector2 direction = currentPosition - center;
        float magnitudeSqr = direction.sqrMagnitude;

        bool isInDeadZone = magnitudeSqr < deadZoneThresholdSqr;
        if (isInDeadZone)
        {
            wasInDeadZoneLastFrame = true;
            return;
        }

        if (wasInDeadZoneLastFrame)
        {
            // ジャンプ防止：デッドゾーン抜け直後は差分を無視
            wasInDeadZoneLastFrame = false;
            lastAngle = GetAngleFromDirection(direction);
            lastScreenPosition = currentPosition;
            return;
        }

        float currentAngle = GetAngleFromDirection(direction);
        float deltaAngle = Mathf.DeltaAngle(lastAngle, currentAngle);

        accumulatedAngle += Mathf.Abs(deltaAngle);
        lastAngle = currentAngle;

        accumulatedDistance += Vector2.Distance(currentPosition, lastScreenPosition);
        lastScreenPosition = currentPosition;
    }

    /// <summary>
    /// トラッキング状態を手動でクリア。
    /// </summary>
    public void Reset()
    {
        accumulatedAngle = 0f;
        accumulatedDistance = 0f;
        wasInDeadZoneLastFrame = false;
    }

    private float GetAngleFromDirection(Vector2 dir)
    {
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    private float GetAngleFromCenter(Vector2 screenPos, Vector2 center)
    {
        return GetAngleFromDirection(screenPos - center);
    }
}
