using System.Collections.Generic;
using UnityEngine;
using static PunchBarrierEffectManager;

[CreateAssetMenu(menuName = "Boss/MechanicalSpider/LegSettings")]
public class MechanicalSpiderLegSettings : ScriptableObject
{
    [Header("Base Motion")]
    public float returningMoveDuration = 1.0f;

    [Header("Idle Motion")]
    public float idleAmplitude = 0.1f;
    public float idleSpeed = 2f;

    [Header("Punch Motion")]
    public float PunchLiftHeight = 1.0f;
    public float PunchLiftDuration = 0.5f;
    public float PunchDurationOffset = 0.14f;
    public float PunchDuration = 7.5f;
    public List<PunchPhaseTimings> PunchPhases = new List<PunchPhaseTimings>();
    public float PunchReturningDuration = 0.5f;
    public Vector3 PunchHoldOffset = Vector3.zero;
    public float PunchShakeStrength = 0.1f;
    public int PunchShaleVibrato = 20;
    public float BarrierDeploymentDelay = 0.1f;

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
    public AudioClip PunchStartClip;
    public AudioClip PunchImpactClip;
    public AudioClip swingStartClip;
    public AudioClip PunchImpactToBarrierClip;
}