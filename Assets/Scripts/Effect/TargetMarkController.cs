using UnityEngine;
using UnityEngine.InputSystem;
using TMPro;
using DG.Tweening; // Import DOTween

public class TargetMarkController : MonoBehaviour
{
    public enum MarkState
    {
        Idle,
        RandomBurst,
        LockOn
    }

    [SerializeField] private MarkState _currentState = MarkState.Idle;

    // Burst state settings
    [SerializeField] private float _randomBurstDuration = 0.5f;
    [SerializeField] private float _burstAlphaScaleDuration = 0.3f; // Time for alpha and scale changes
    [SerializeField] private float _burstMoveSpeed = 20f;
    [SerializeField] private float _lockOnSpeed = 10f;
    [SerializeField] private float _lockOnDistance = 0.1f;

    // Max radius to move away from the target
    [SerializeField] private float _randomMoveRadius = 3f;
    [SerializeField] private float _burstMoveDistance = 1f;

    // New properties for scale and animation
    [SerializeField] private float _targetScale = 1.5f; // Target size for the burst state
    [SerializeField] private float _lockOnPulseScale = 1.2f; // Scale multiplier for the lock-on pulse
    [SerializeField] private float _lockOnPulseDuration = 0.5f; // Duration of one pulse loop

    // New serialized fields for DOTween's Ease
    [SerializeField] private Ease _lockOnPulseEase = Ease.InOutSine;

    // Component references exposed in the Inspector
    [SerializeField] private SpriteRenderer _spriteRenderer;
    [SerializeField] private TextMeshPro _textMeshPro;

    // Color settings
    [SerializeField] private Color _lockOnColor = Color.red; // Color when locked on
    [SerializeField] private Color _defaultColor = Color.white; // Default color
    
    private Transform _target;
    private Vector2 _randomTargetPosition;
    private float _burstTimer;
    private Vector3 _initialScale;

    // Public property to access the current state
    public MarkState CurrentState => _currentState;

    private Tween _scaleTween; // Variable to store the DOTween tween

    void Awake()
    {
        if (_spriteRenderer == null)
        {
            _spriteRenderer = GetComponent<SpriteRenderer>();
        }
        if (_textMeshPro == null)
        {
            _textMeshPro = GetComponentInChildren<TextMeshPro>();
        }

        if (_spriteRenderer == null)
        {
            Debug.LogError("SpriteRenderer component is missing on this GameObject or not assigned in the Inspector.");
        }
        _initialScale = transform.localScale;

        SetAlpha(0f);
        SetColor(_defaultColor);
    }

    public void SetTarget(Transform newTarget)
    {
        if (newTarget != null)
        {
            SetAlpha(1f);
            SetColor(_defaultColor);
            _target = newTarget;
            _currentState = MarkState.RandomBurst;
            _burstTimer = 0f;
            SetNewRandomPosition();

            transform.localScale = Vector3.zero;
            transform.position = newTarget.position;
        }
    }

    public void ReleaseLockOn()
    {
        _currentState = MarkState.Idle;
        SetAlpha(0f);

        // Stop the scale animation
        _scaleTween?.Kill();
        transform.localScale = _initialScale;
    }

    void Update()
    {
        switch (_currentState)
        {
            case MarkState.Idle:
                break;

            case MarkState.RandomBurst:
                transform.position = Vector2.MoveTowards(transform.position, _randomTargetPosition, _burstMoveSpeed * Time.deltaTime);

                _burstTimer += Time.deltaTime;

                // Control alpha and scale independently
                float t = Mathf.Clamp01(_burstTimer / _burstAlphaScaleDuration);
                transform.localScale = Vector3.Lerp(Vector3.zero, _initialScale * _targetScale, t);

                if (_spriteRenderer != null)
                {
                    Color currentColor = _spriteRenderer.color;
                    currentColor.a = Mathf.Lerp(0f, 1f, t);
                    _spriteRenderer.color = currentColor;
                }

                if (_textMeshPro != null)
                {
                    Color currentColor = _textMeshPro.color;
                    currentColor.a = Mathf.Lerp(0f, 1f, t);
                    _textMeshPro.color = currentColor;
                }

                if (_burstTimer >= _randomBurstDuration)
                {
                    _currentState = MarkState.LockOn;

                    transform.localScale = _initialScale * _targetScale;
                    SetAlpha(1f);
                    SetColor(_defaultColor); // Revert to default color before entering lock-on state
                }

                if (Vector2.Distance(transform.position, _randomTargetPosition) < 0.1f)
                {
                    SetNewRandomPosition();
                }
                break;

            case MarkState.LockOn:
                if (_target != null)
                {
                    transform.position = Vector2.MoveTowards(transform.position, _target.position, _lockOnSpeed * Time.deltaTime);

                    if (Vector2.Distance(transform.position, _target.position) < _lockOnDistance)
                    {
                        SetColor(_lockOnColor);
                    }
                    else
                    {
                        SetColor(_defaultColor);
                    }

                    // Start the pulsing scale animation
                    if (_scaleTween == null || !_scaleTween.IsPlaying())
                    {
                        _scaleTween = transform.DOScale(_initialScale * _targetScale * _lockOnPulseScale, _lockOnPulseDuration)
                            .SetLoops(-1, LoopType.Yoyo)
                            .SetEase(_lockOnPulseEase);
                    }
                }
                break;
        }
    }

    void SetNewRandomPosition()
    {
        if (_target != null)
        {
            Vector2 randomOffset = Random.insideUnitCircle * _randomMoveRadius;
            _randomTargetPosition = (Vector2)_target.position + randomOffset;

            Vector2 direction = (_randomTargetPosition - (Vector2)transform.position).normalized;
            _randomTargetPosition = (Vector2)transform.position + direction * _burstMoveDistance;
        }
    }

    private void SetAlpha(float alpha)
    {
        if (_spriteRenderer != null)
        {
            Color color = _spriteRenderer.color;
            color.a = alpha;
            _spriteRenderer.color = color;
        }
        if (_textMeshPro != null)
        {
            Color color = _textMeshPro.color;
            color.a = alpha;
            _textMeshPro.color = color;
        }
    }

    private void SetColor(Color color)
    {
        if (_spriteRenderer != null)
        {
            float alpha = _spriteRenderer.color.a;
            color.a = alpha;
            _spriteRenderer.color = color;
        }
        if (_textMeshPro != null)
        {
            float alpha = _textMeshPro.color.a;
            color.a = alpha;
            _textMeshPro.color = color;
        }
    }
}