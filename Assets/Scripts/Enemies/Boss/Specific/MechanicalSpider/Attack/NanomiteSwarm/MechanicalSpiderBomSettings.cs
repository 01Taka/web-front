using UnityEngine;

[CreateAssetMenu(fileName = "MechanicalSpiderBomSettings", menuName = "Boss/MechanicalSpider/Bom", order = 1)]
public class MechanicalSpiderBomSettings : ScriptableObject
{
    [Header("SpawnerSettings")]
    public int NumberOfBoms = 5;
    public float SpawnRadius = 5f;
    public float ExplosionTime = 3f;

    [Header("回転設定")]
    [Tooltip("Z軸周りの回転速度 (度/秒)")]
    public float RotationSpeed = 100f;
    [Tooltip("破壊後の回転速度 (度/秒)")]
    public float RotationSpeedOnDestroyed = 300f;

    [Header("点滅設定")]
    [Tooltip("点滅の規定色")]
    public Color BlinkColor = Color.red;
    [Tooltip("点滅の開始頻度 (秒)")]
    public float StartBlinkInterval = 0.5f;
    [Tooltip("点滅の終了頻度 (秒)")]
    public float EndBlinkInterval = 0.1f;

    [Header("爆発設定 (破壊後)")]
    [Tooltip("爆発までの時間に加算するランダム値の範囲 (秒)")]
    public float ExplosionTimeRandomRange = 0.5f;
    [Tooltip("爆発時にターゲット座標からランダムにずらす半径")]
    public float ExplosionRadius = 0.5f;

    [Header("追跡とHP設定")]
    [Tooltip("追跡速度")]
    public float SeekSpeed = 5f;
    [Tooltip("破壊後の追跡速度")]
    public float SeekSpeedOnDestroyed = 5f;

    [Header("サウンド設定")]
    public AudioClip BomClip;
    public AudioClip ExplosionClipOnDestroy;
    public AudioClip HitClip;

    public float HitClipVolume = 0.5f;
}