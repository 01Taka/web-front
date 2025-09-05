using UnityEngine;
using System.Collections;

public interface IEffectPrefab
{
    void SetSizeRaito(float size);
}

public class ExplosionEffect : MonoBehaviour, IEffectPrefab
{
    // --- Fields ---
    [Header("Explosion Settings")]
    [Tooltip("Duration of the explosion animation in seconds.")]
    [SerializeField]
    private float _explosionDuration = 0.5f;

    [Tooltip("Final scale of the explosion.")]
    [SerializeField]
    private Vector3 _maxScale = new Vector3(2f, 2f, 1f);
    private float _sizeRaito = 1f;

    [Tooltip("Starting color of the explosion.")]
    [SerializeField]
    private Color _startColor = Color.yellow;

    [Tooltip("Final color of the explosion, typically transparent.")]
    [SerializeField]
    private Color _endColor = new Color(1f, 1f, 1f, 0f);

    [Header("Components")]
    [Tooltip("SpriteRenderer for the explosion effect.")]
    [SerializeField]
    private SpriteRenderer _spriteRenderer;

    // --- Methods ---
    private void Start()
    {
        // Get the SpriteRenderer component if not set in the Inspector.
        // This is a more robust way to handle component dependencies.
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }

        // If SpriteRenderer is still not found, log an error and destroy the object.
        if (_spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component is missing. The explosion effect will not work.", this);
            Destroy(gameObject);
            return;
        }

        StartCoroutine(AnimateExplosion());
    }

    public void SetSizeRaito(float size)
    {
        _sizeRaito = size;
    }

    /// <summary>
    /// Animates the explosion by scaling and fading the sprite.
    /// </summary>
    private IEnumerator AnimateExplosion()
    {
        // Cache the transform for performance.
        var transformCache = transform;
        transformCache.localScale = Vector3.zero;
        _spriteRenderer.color = _startColor;

        float timeElapsed = 0f;

        // Use a more readable while loop condition.
        while (timeElapsed < _explosionDuration)
        {
            // Calculate progress using a direct time-based approach.
            float t = timeElapsed / _explosionDuration;

            // Use Vector3.Lerp for scaling and Color.Lerp for color changes.
            // Lerp is a robust and clear way to handle interpolation.
            transformCache.localScale = Vector3.Lerp(Vector3.zero, _maxScale * _sizeRaito, t);
            _spriteRenderer.color = Color.Lerp(_startColor, _endColor, t);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Ensure the final state is reached precisely.
        transformCache.localScale = _maxScale * _sizeRaito;
        _spriteRenderer.color = _endColor;

        // Clean up the object after the animation.
        Destroy(gameObject);
    }
}