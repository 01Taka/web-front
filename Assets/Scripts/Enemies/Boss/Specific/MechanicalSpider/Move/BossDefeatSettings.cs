using UnityEngine;

[CreateAssetMenu(fileName = "BossDefeatSettings", menuName = "Boss/Boss Defeat Settings", order = 2)]
public class BossDefeatSettings : ScriptableObject
{
    [Header("Explosion Sequence")]
    [Tooltip("The number of small explosions to play before the final one.")]
    public int numberOfSmallExplosions = 3;

    [Tooltip("The time between each small explosion.")]
    public float smallExplosionInterval = 0.3f;

    [Tooltip("The type of small explosions to play.")]
    public ExplosionType smallExplosionType = ExplosionType.Default;

    public AudioClip smallExplosionClip;

    public float explosionEffectRadius = 10f;
    public float explosionEffectZ = -10f;

    [Tooltip("The type of the final, large explosion.")]
    public ExplosionType finalExplosionType = ExplosionType.Big;

    [Tooltip("The duration to wait before destroying the boss GameObject.")]
    public float finalExplosionDuration = 0.5f;

    [Header("Audio")]
    [Tooltip("The sound to play for the final explosion.")]
    public AudioClip finalExplosionClip;

    [Tooltip("The screen shake settings for the final explosion.")]
    public ScreenShakeSettings finalShakeSettings;
}