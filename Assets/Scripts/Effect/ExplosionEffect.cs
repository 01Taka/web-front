using UnityEngine;
using System;

public class ExplosionEffect : MonoBehaviour, IPoolable
{
    public bool IsReusable { get; set; }
    private ObjectPool<ExplosionEffect> _pool;

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

        // 初期状態の設定（色、スプライト、サイズ）
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null && settings.explosionSprite != null)
        {
            sr.sprite = settings.explosionSprite;
            sr.color = settings.startColor;
        }

        transform.localScale = Vector3.zero;
        gameObject.SetActive(true);
    }

    public void SetPool<T>(ObjectPool<T> pool) where T : Component, IPoolable
    {
        _pool = pool as ObjectPool<ExplosionEffect>;
    }

    public void ReturnToPool()
    {
        if (_pool != null)
        {
            _pool.ReturnToPool(this);
        }
        else
        {
            Debug.LogWarning($"No pool assigned for {gameObject.name}, destroying instead.");
            Destroy(gameObject);
        }
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

        // 色の補間
        var sr = GetComponent<SpriteRenderer>();
        if (sr != null)
        {
            sr.color = Color.Lerp(_settings.startColor, _settings.endColor, t);
        }

        // サイズアニメーション
        transform.localScale = Vector3.Lerp(Vector3.zero, _settings.maxScale * _scaleRaito, t);

        if (_timer >= _duration)
        {
            gameObject.SetActive(false);
            OnReturnToPool?.Invoke(); // プールに返却
        }
    }
}
