using UnityEngine;
using System.Collections;

public class SoundManager : MonoBehaviour
{
    // シングルトンのインスタンス
    public static SoundManager Instance { get; private set; }

    [SerializeField]
    private AudioSource _effectAudioSource;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// 効果音を再生する（通常版）
    /// </summary>
    /// <param name="clip">再生するオーディオクリップ</param>
    /// <param name="volume">再生時の音量（0.0fから1.0f）</param>
    public void PlayEffect(AudioClip clip, float volume = 1.0f)
    {
        if (clip != null && _effectAudioSource != null)
        {
            _effectAudioSource.PlayOneShot(clip, volume);
        }
    }

    /// <summary>
    /// 効果音を再生する（ピッチと音量をランダムに調整可能）
    /// </summary>
    /// <param name="clip">再生するオーディオクリップ</param>
    /// <param name="minVolume">音量の最小値</param>
    /// <param name="maxVolume">音量の最大値</param>
    /// <param name="minPitch">ピッチの最小値</param>
    /// <param name="maxPitch">ピッチの最大値</param>
    public void PlayEffect(AudioClip clip, float minVolume, float maxVolume, float minPitch, float maxPitch)
    {
        if (clip != null && _effectAudioSource != null)
        {
            // ランダムなピッチと音量を設定
            float randomPitch = Random.Range(minPitch, maxPitch);
            float randomVolume = Random.Range(minVolume, maxVolume);

            // PlayOneShotの前にAudioSourceのピッチを設定
            _effectAudioSource.pitch = randomPitch;

            // PlayOneShotで再生
            _effectAudioSource.PlayOneShot(clip, randomVolume);

            // 再生後、ピッチを元の状態に戻す（この処理は次のPlayOneShotまで有効）
            _effectAudioSource.pitch = 1.0f;
        }
    }
}