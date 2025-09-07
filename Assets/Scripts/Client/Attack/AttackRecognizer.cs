using UnityEngine;
using System.Collections.Generic;

public class AttackRecognizer : MonoBehaviour, IAttackRecognizer
{
    public List<AttackInputData> Recognize(TouchInputData touchData, Vector2 screenCenter, InputAttackConfig inputConfig)
    {
        List<AttackInputData> results = new();

        Debug.Log("Start recognizing attacks.");

        if (touchData.CicularGestureAmount > inputConfig.minOrbWeaverAmount)
        {
            results.Add(new AttackInputData
            {
                Type = AttackType.WebMine,
                Direction = touchData.FromBottomCenterDirection,
                ChargeAmount = touchData.CicularGestureAmount
            });
            return results;
        }

        if (touchData.SwipeDistance < inputConfig.minSwipeDistance)
        {
            results.Add(new AttackInputData
            {
                Type = AttackType.VolleyBurst,
                Direction = touchData.FromBottomCenterDirection,
            });
            return results;
        }

        if (touchData.IsSwipeForward && touchData.SwipeDistance > inputConfig.minSwipeDistance)
        {
            results.Add(new AttackInputData
            {
                Type = AttackType.SilkSnare,
                Direction = touchData.SwipeDirection
            });
        }
        else if (!touchData.IsSwipeForward && touchData.SwipeDistance > inputConfig.minSwipeDistance && touchData.HoldDuration > inputConfig.bowstringMinHold)
        {
            results.Add(new AttackInputData
            {
                Type = AttackType.ChargedPierce,
                Direction = -touchData.SwipeDirection,
                ChargeAmount = touchData.HoldDuration
            });
        }
        return results;
    }
}
