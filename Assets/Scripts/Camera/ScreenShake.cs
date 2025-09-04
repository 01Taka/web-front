using UnityEngine;
using System.Collections;

public class ScreenShake : MonoBehaviour
{
    [SerializeField]
    private float _shakeDuration = 0.5f;
    [SerializeField]
    private float _shakeMagnitude = 0.1f;
    private Vector3 _originalPosition;

    void Start()
    {
        _originalPosition = transform.localPosition;
    }

    public void StartShake()
    {
        StartCoroutine(ShakeCoroutine());
    }

    private IEnumerator ShakeCoroutine()
    {
        float timer = 0f;
        while (timer < _shakeDuration)
        {
            float x = Random.Range(-1f, 1f) * _shakeMagnitude;
            float y = Random.Range(-1f, 1f) * _shakeMagnitude;
            transform.localPosition = _originalPosition + new Vector3(x, y, 0f);

            timer += Time.deltaTime;
            yield return null;
        }
        transform.localPosition = _originalPosition;
    }
}