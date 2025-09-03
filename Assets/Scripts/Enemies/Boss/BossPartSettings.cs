using UnityEngine;

[CreateAssetMenu(fileName = "BossPartSettings", menuName = "Boss/Boss Part Settings", order = 1)]
public class BossPartSettings : ScriptableObject
{
    [Header("Part Properties")]
    public float damageMultiplier = 1.0f;

    [Header("Visual Effects")]
    public Color damageColor = Color.red;
    public float damageColorDuration = 0.1f;
    public float vibrationThreshold = 50f;
    public float vibrationDuration = 0.2f;
    public float vibrationMagnitude = 0.1f;
}