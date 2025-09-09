using UnityEngine;

[CreateAssetMenu(fileName = "ScreenShakeSettings", menuName = "Camera/Screen Shake Settings", order = 1)]
public class ScreenShakeSettings : ScriptableObject
{
    public float shakeDuration = 0.5f;
    public float shakeMagnitude = 0.1f;
}