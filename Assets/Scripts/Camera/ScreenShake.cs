using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{
    private Vector3 _originalPosition;
    private Coroutine _shakeCoroutine;

    private void Awake()
    {
        _originalPosition = transform.localPosition;
    }

    public void StartShake(ScreenShakeSettings settings)
    {
        if (_shakeCoroutine != null)
        {
            StopCoroutine(_shakeCoroutine);
        }
        _shakeCoroutine = StartCoroutine(ShakeCoroutine(settings));
    }

    private IEnumerator ShakeCoroutine(ScreenShakeSettings settings)
    {
        float timer = 0f;
        while (timer < settings.shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * settings.shakeMagnitude;
            float y = Random.Range(-1f, 1f) * settings.shakeMagnitude;
            transform.localPosition = _originalPosition + new Vector3(x, y, 0f);

            timer += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = _originalPosition;
        _shakeCoroutine = null;
    }
}