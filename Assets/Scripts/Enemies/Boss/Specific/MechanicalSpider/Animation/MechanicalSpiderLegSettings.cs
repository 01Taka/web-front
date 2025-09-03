using UnityEngine;

[CreateAssetMenu(menuName = "Boss/MechanicalSpider/LegSettings")]
public class MechanicalSpiderLegSettings : ScriptableObject
{
    [Header("Base Motion")]
    public float returningMoveDuration = 1.0f;

    [Header("Idle Motion")]
    public float idleAmplitude = 0.1f;
    public float idleSpeed = 2f;

    [Header("Punch Motion")]
    public float punchLiftHeight = 1.0f;
    public float punchLiftDuration = 0.5f;
    public float punchMoveDuration = 0.2f;

    [Header("Hold Motion")]
    public float holdJitterAmplitude = 0.05f;
    public float holdJitterSpeed = 10f;

    [Header("Swing Motion")]
    public float swingWindupDistance = 0.5f;
    public float swingWindupDuration = 0.3f;
    public float swingWindupHoldDuration = 0.1f; // êVÇµÇ≠í«â¡
    public float swingDistance = 1.5f;
    public float swingDuration = 0.5f;
    public float swingHoldDuration = 0.5f;
    public float returningFromSwingDuration = 0.5f;

    [Header("Audio Clips")]
    public AudioClip punchStartClip;
    public AudioClip punchImpactClip;
    public AudioClip swingStartClip;
}