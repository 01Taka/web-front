using UnityEngine;

/// <summary>
/// ��ʏ�ł̉~�`�W�F�X�`���[�ʁi��]�Ƌ����j�𑪒肷��ėp�g���b�J�[�B
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
    /// ����]�p�ideg�j
    /// </summary>
    public float TotalRotation => accumulatedAngle;

    /// <summary>
    /// ���ړ������i�s�N�Z���j
    /// </summary>
    public float TotalDistance => accumulatedDistance;

    /// <summary>
    /// ���S���W�ɑ΂��ėݐϊp�Ƌ����𑪂�g���b�J�[���������B
    /// </summary>
    public CircularGestureTracker(float deadZoneRadius = 3f)
    {
        deadZoneThresholdSqr = deadZoneRadius * deadZoneRadius;
    }

    /// <summary>
    /// �g���b�L���O�������i�^�b�`�J�n���ɌĂԁj
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
    /// ���t���[���A���݈ʒu�ƒ��S��n�����ƂŃg���b�L���O�X�V�B
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
            // �W�����v�h�~�F�f�b�h�]�[����������͍����𖳎�
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
    /// �g���b�L���O��Ԃ��蓮�ŃN���A�B
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
