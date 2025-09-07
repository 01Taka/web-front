using UnityEngine;
using System;

public class ExplosionEffect : MonoBehaviour
{
    private ExplosionSettings _settings;
    private float _timer;
    private float _duration;
    private float _scaleRaito = 1f;

    public Action OnReturnToPool { get; set; }

    public void Initialize(ExplosionSettings settings)
    {
        _settings = settings;
        _duration = settings.explosionDuration;
        _timer = 0f;

        // ������Ԃ̐ݒ�i�F�A�X�v���C�g�A�T�C�Y�j
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null && settings.explosionSprite != null)
        {
            sr.sprite = settings.explosionSprite;
            sr.color = settings.startColor;
        }

        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
    }

    public void SetSizeRaito(float ratio)
    {
        _scaleRaito = ratio;
    }

    private void Update()
    {
        if (_settings == null) return;

        _timer += Time.deltaTime;
        float t = _timer / _duration;

        // �F�̕��
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.Lerp(_settings.startColor, _settings.endColor, t);
        }

        // �T�C�Y�A�j���[�V����
        transform.localScale = Vector3.Lerp(Vector3.zero, _settings.maxScale * _scaleRaito, t);

        if (_timer >= _duration)
        {
            gameObject.SetActive(false);
            OnReturnToPool?.Invoke(); // �v�[���ɕԋp
        }
    }
}
