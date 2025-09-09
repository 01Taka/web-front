using UnityEngine;

[CreateAssetMenu(fileName = "BossEntranceSettings", menuName = "Boss/Boss Entrance Settings", order = 1)]
public class BossEntranceSettings : ScriptableObject
{
    public ScreenShakeSettings ScreenShakeSettings;

    [Header("足音の設定")]
    [Tooltip("足音のオーディオクリップ配列。複数設定するとランダムに再生されます。")]
    public AudioClip[] FootstepClips;

    [Tooltip("足音を鳴らす歩数")]
    public int NumberOfFootsteps = 3;

    [Tooltip("足音を鳴らす間隔（秒）")]
    public float FootstepInterval = 0.5f;

    [Header("ボスの着地設定")]
    [Tooltip("ボスの着地音のオーディオクリップ")]
    public AudioClip LandingClip;

    [Tooltip("ジャンプの高さ")]
    public float JumpHeight = 5f;

    [Tooltip("ジャンプの所要時間（秒）。着地音を鳴らすまでの時間")]
    public float JumpDuration = 1.0f;

    [Tooltip("ジャンプの移動にかける時間（秒）。JumpDurationよりも短い必要があります")]
    public float JumpMoveDuration = 0.5f;
}