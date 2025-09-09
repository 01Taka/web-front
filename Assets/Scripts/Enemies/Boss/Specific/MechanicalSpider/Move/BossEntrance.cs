using UnityEngine;
using System.Collections;

public class BossEntrance : MonoBehaviour
{
    private BossEntranceSettings _settings;
    private Coroutine _entranceCoroutine;
    private float _landingYPosition;

    private ScreenShake _screenShake;

    /// <summary>
    /// ボスの出現演出を初期化して開始する
    /// </summary>
    /// <param name="settings">出現演出の設定</param>
    /// <param name="landingYPosition">ボスの着地位置のY座標</param>
    public void Initialize(BossEntranceSettings settings, float landingYPosition, ScreenShake screenShake)
    {
        _settings = settings;
        _landingYPosition = landingYPosition;
        _screenShake = screenShake;

        if (_settings == null)
        {
            Debug.LogError("BossEntranceSettingsが設定されていません。");
            return;
        }
        if (SoundManager.Instance == null)
        {
            Debug.LogError("SoundManagerのインスタンスが見つかりません。");
            return;
        }

        if (_entranceCoroutine != null)
        {
            StopCoroutine(_entranceCoroutine);
        }

        // ジャンプ開始位置を設定
        Vector3 jumpStartPosition = transform.position;
        jumpStartPosition.y = _landingYPosition + _settings.JumpHeight;
        transform.position = jumpStartPosition;

        _entranceCoroutine = StartCoroutine(EntranceSequenceCoroutine());
    }

    private IEnumerator EntranceSequenceCoroutine()
    {
        // 足音を複数回再生
        for (int i = 0; i < _settings.NumberOfFootsteps; i++)
        {
            if (_settings.FootstepClips != null && _settings.FootstepClips.Length > 0)
            {
                AudioClip randomFootstepClip = _settings.FootstepClips[Random.Range(0, _settings.FootstepClips.Length)];
                SoundManager.Instance.PlayEffect(randomFootstepClip);
            }
            yield return new WaitForSeconds(_settings.FootstepInterval);
        }

        float remainingTime = _settings.JumpDuration - _settings.JumpMoveDuration;
        if (remainingTime > 0)
        {
            yield return new WaitForSeconds(remainingTime);
        }

        // ジャンプの移動を開始
        float moveElapsedTime = 0f;
        Vector3 startPosition = transform.position;
        Vector3 endPosition = new Vector3(transform.position.x, _landingYPosition, transform.position.z);

        while (moveElapsedTime < _settings.JumpMoveDuration)
        {
            transform.position = Vector3.Lerp(startPosition, endPosition, moveElapsedTime / _settings.JumpMoveDuration);
            moveElapsedTime += Time.deltaTime;
            yield return null;
        }

        // 最終位置を保証
        transform.position = endPosition;

        // 着地音を再生
        if (_settings.LandingClip != null)
        {
            SoundManager.Instance.PlayEffect(_settings.LandingClip);
        }

        // 着地後の振動演出を開始
        if (_screenShake != null)
        {
            _screenShake.StartShake(_settings.ScreenShakeSettings);
        }
        StartCoroutine(BossShakeCoroutine(0.2f, 0.05f));
    }

    /// <summary>
    /// ボスのトランスフォームを揺らすコルーチン
    /// </summary>
    private IEnumerator BossShakeCoroutine(float shakeDuration, float shakeMagnitude)
    {
        Vector3 originalPosition = transform.localPosition;
        float timer = 0f;

        while (timer < shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * shakeMagnitude;
            float y = Random.Range(-1f, 1f) * shakeMagnitude;
            transform.localPosition = originalPosition + new Vector3(x, y, 0f);

            timer += Time.deltaTime;
            yield return null;
        }

        // 最終位置を元の位置に戻す
        transform.localPosition = originalPosition;
    }
}