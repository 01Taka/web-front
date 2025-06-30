using UnityEngine;

[CreateAssetMenu(fileName = "InputAttackConfig", menuName = "Attack/Input Attack Config", order = 1)]
public class InputAttackConfig : ScriptableObject
{
    [Header("Center Detection")]
    public float centerRadius = 100f;

    [Header("Circular Gesture Recognition")]
    public float circularRotationWeight = 0.5f;
    public float circularDistanceWeight = 0.5f;
    public float circularGestureDistanceThreshold = 150f;

    [Header("Fallback")]
    public float fallbackDpi = 160f;

    public float minSwipeDistance;
    public float bowstringMinHold;
    public float minOrbWeaverAmount;
}
