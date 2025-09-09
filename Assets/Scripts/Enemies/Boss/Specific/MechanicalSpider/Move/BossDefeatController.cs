using UnityEngine;
using System.Collections;
using System;
using UnityEngine.Events;

public class BossDefeatController : MonoBehaviour
{
    [Header("設定")]
    [Tooltip("撃破演出の設定をまとめたScriptableObject")]
    [SerializeField] private BossDefeatSettings _settings;

    private ScreenShake _screenShake;

    [Header("イベント")]
    [Tooltip("撃破演出が完了したときに呼び出されるイベント")]
    public UnityEvent OnDefeatCompleted = new UnityEvent();

    /// <summary>
    /// 撃破演出を開始する
    /// </summary>
    public void StartDefeatSequence()
    {
        if (SceneComponentManager.Instance.GameCamera.TryGetComponent<ScreenShake>(out var screenShake))
        {
            _screenShake = screenShake;
        }

        if (_settings == null)
        {
            Debug.LogError("BossDefeatSettingsが設定されていません。", this);
            return;
        }

        StartCoroutine(DefeatSequenceCoroutine());
    }

    private IEnumerator DefeatSequenceCoroutine()
    {
        // ボスの当たり判定と描画を無効にする
        if (TryGetComponent(out Collider2D collider))
        {
            collider.enabled = false;
        }
        if (TryGetComponent(out Renderer renderer))
        {
            renderer.enabled = false;
        }

        // 小さい爆発を連続で再生
        for (int i = 0; i < _settings.numberOfSmallExplosions; i++)
        {
            // ボスの周りのランダムな位置を取得
            float angle = UnityEngine.Random.Range(0f, Mathf.PI * 2f);
            float radius = UnityEngine.Random.Range(0f, _settings.explosionEffectRadius);

            Vector3 randomPosition = transform.position + new Vector3(
                radius * Mathf.Cos(angle),
                radius * Mathf.Sin(angle),
                _settings.explosionEffectZ
            );

            if (SoundManager.Instance != null && _settings.smallExplosionClip != null)
            {
                SoundManager.Instance.PlayEffect(_settings.smallExplosionClip);
            }
            // 爆発を再生
            ExplosionEffectPoolManager.Instance.PlayExplosion(randomPosition, 1.0f, _settings.smallExplosionType);
            yield return new WaitForSeconds(_settings.smallExplosionInterval);
        }

        // 最後の大きな爆発
        if (SoundManager.Instance != null && _settings.finalExplosionClip != null)
        {
            SoundManager.Instance.PlayEffect(_settings.finalExplosionClip);
        }

        // 最後の爆発を再生
        ExplosionEffectPoolManager.Instance.PlayExplosion(transform.position, 2.0f, _settings.finalExplosionType);

        // 画面を揺らす
        if (_screenShake != null && _settings.finalShakeSettings != null)
        {
            _screenShake.StartShake(_settings.finalShakeSettings);
        }

        // 爆発の演出時間分待機
        yield return new WaitForSeconds(_settings.finalExplosionDuration);

        // イベントを呼び出す
        OnDefeatCompleted?.Invoke();

        // オブジェクトを破棄
        Destroy(gameObject);
    }

    // オブジェクトが破棄される前にイベントリスナーを解除
    private void OnDestroy()
    {
        OnDefeatCompleted.RemoveAllListeners();
    }
}