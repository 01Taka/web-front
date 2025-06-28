using UnityEngine;

public class OrbChargeTracker
{
    private float accumulatedAngle = 0f;
    private float accumulatedDistance = 0f;
    private float lastAngle = 0f;
    private Vector2 lastScreenPosition;
    private bool wasInDeadZoneLastFrame = false;

    private const float DeadZoneThresholdSqr = 10f;
    private const float RequiredTotalRotation = 180f;
    private const float RequiredTotalDistance = 200f;

    public bool IsCharged => accumulatedAngle >= RequiredTotalRotation && accumulatedDistance >= RequiredTotalDistance;

    public float TotalRotation => accumulatedAngle;
    public float TotalDistance => accumulatedDistance;
    public float ChargeAmount => TotalDistance;

    public void Reset(Vector2 currentScreenPosition)
    {
        accumulatedAngle = 0f;
        accumulatedDistance = 0f;
        lastScreenPosition = currentScreenPosition;
        lastAngle = GetAngleFromCenter(currentScreenPosition);
        wasInDeadZoneLastFrame = false;
    }

    public void Update(Vector2 currentPosition, Vector2 center)
    {
        Vector2 direction = currentPosition - center;
        float magnitudeSqr = direction.sqrMagnitude;

        bool isInDeadZone = magnitudeSqr < DeadZoneThresholdSqr;
        if (isInDeadZone)
        {
            wasInDeadZoneLastFrame = true;
            return; // 無視：デッドゾーン内
        }

        if (wasInDeadZoneLastFrame)
        {
            // デッドゾーンを抜けた直後は無視（ジャンプ防止）
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

    public void Clear()
    {
        accumulatedAngle = 0f;
        accumulatedDistance = 0f;
        wasInDeadZoneLastFrame = false;
    }

    private float GetAngleFromDirection(Vector2 dir)
    {
        return Mathf.Atan2(dir.y, dir.x) * Mathf.Rad2Deg;
    }

    private float GetAngleFromCenter(Vector2 screenPos)
    {
        return GetAngleFromDirection(screenPos); // 原点中心の角度を計算
    }
}
