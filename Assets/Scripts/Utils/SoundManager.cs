using UnityEngine;

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
    /// 効果音を再生する
    /// </summary>
    /// <param name="clip">再生するオーディオクリップ</param>
    /// <param name="volume">再生時の音量（0.0fから1.0f）</param>
    public void PlayEffect(AudioClip clip, float volume = 1.0f)
    {
        if (clip != null && _effectAudioSource != null)
        {
            // PlayOneShotの第二引数で音量を指定
            _effectAudioSource.PlayOneShot(clip, volume);
        }
    }
}