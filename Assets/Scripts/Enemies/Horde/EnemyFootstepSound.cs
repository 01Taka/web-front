using UnityEngine;
using System.Collections;

public class EnemyFootstepSound : MonoBehaviour
{
    // === Public Variables ===
    public AudioClip[] footstepClips; // 設定可能な複数の足音クリップ
    public float minPitch = 0.9f;     // ピッチの最小値
    public float maxPitch = 1.1f;     // ピッチの最大値
    public float minVolume = 0.8f;    // 音量の最小値
    public float maxVolume = 1.0f;    // 音量の最大値
    public float minInterval = 0.4f;  // 足音の間隔の最小値
    public float maxInterval = 0.6f;  // 足音の間隔の最大値

    // === Private Variables ===
    private Coroutine footstepCoroutine;

    /// <summary>
    /// 足音の再生を開始する
    /// </summary>
    public void StartFootsteps()
    {
        // 既にコルーチンが実行中なら、いったん停止して再開する
        if (footstepCoroutine != null)
        {
            StopCoroutine(footstepCoroutine);
        }
        footstepCoroutine = StartCoroutine(PlayFootstepsLoop());
    }

    /// <summary>
    /// 足音の再生を停止する
    /// </summary>
    public void StopFootsteps()
    {
        if (footstepCoroutine != null)
        {
            StopCoroutine(footstepCoroutine);
            footstepCoroutine = null;
        }
    }

    /// <summary>
    /// 足音をランダムな間隔で繰り返し再生するコルーチン
    /// </summary>
    private IEnumerator PlayFootstepsLoop()
    {
        while (true)
        {
            // 足音の再生間隔をランダムに決定
            float interval = Random.Range(minInterval, maxInterval);
            yield return new WaitForSeconds(interval);

            // 足音クリップが設定されているか確認
            if (footstepClips.Length == 0)
            {
                Debug.LogWarning("Footstep clips are not assigned in the inspector.");
                yield break; // 警告を表示してコルーチンを終了
            }

            // ランダムな足音クリップを選択
            AudioClip randomClip = footstepClips[Random.Range(0, footstepClips.Length)];

            // SoundManagerを通じて足音を再生
            if (SoundManager.Instance != null)
            {
                SoundManager.Instance.PlayEffect(randomClip, minVolume, maxVolume, minPitch, maxPitch);
            }
        }
    }
}