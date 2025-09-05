using UnityEngine;
using UnityEngine.Events;
using System.Collections;
using System.Collections.Generic;

public class MechanicalSpiderBom : MonoBehaviour, IDamageable
{
    // --- �t�B�[���h ---
    [Tooltip("���̃X�p�C�_�[�̐ݒ���`����ScriptableObject")]
    [SerializeField]
    private MechanicalSpiderBomSettings _settings;

    [Header("�C�x���g�ƎQ��")]
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    [Tooltip("�������ɌĂ΂��C�x���g")]
    [SerializeField]
    private UnityEvent _onExplosion;
    [Tooltip("�ڕW�ɓ��B�������ɌĂ΂��C�x���g")]
    [SerializeField]
    private UnityEvent _onTargetReached;

    private Color _originalColor = Color.white;
    private HealthManager _healthManager;
    private UnityAction _explosionAction;
    private UnityAction _targetReachedAction;

    private float _baseExplosionTime;
    private float _explosionTime;
    private float _timeElapsed;
    private Vector2 _targetPosition;
    private Vector2 _targetPositionOnDestroyed;
    private bool _isDestroyed = false;

    // --- ���\�b�h ---

    /// <summary>
    /// ���e���N�����郁�\�b�h�B
    /// </summary>
    /// <param name="explosionDuration">�����܂ł̎���</param>
    /// <param name="maxHealth">�ő�HP</param>
    /// <param name="target">�j��O�̒ǐՖڕW</param>
    /// <param name="targetOnDestroyed">�j���̒ǐՖڕW</param>
    /// <param name="onExplosionAction">�������Ɏ��s�����A�N�V����</param>
    /// <param name="onTargetReachedAction">�ڕW���B���Ɏ��s�����A�N�V����</param>
    public void Activate(float explosionDuration, float maxHealth, Vector2 target, Vector2 targetOnDestroyed,
                         UnityAction onExplosionAction, UnityAction onTargetReachedAction)
    {
        _baseExplosionTime = explosionDuration;
        _explosionTime = explosionDuration + Random.Range(0, _settings.ExplosionTimeRandomRange);
        _targetPosition = target;
        _targetPositionOnDestroyed = targetOnDestroyed;
        _timeElapsed = 0f;

        _explosionAction = onExplosionAction;
        _targetReachedAction = onTargetReachedAction;

        _onExplosion.AddListener(_explosionAction);
        _onTargetReached.AddListener(_targetReachedAction);

        _healthManager.SetMaxHealth(maxHealth);

        StartCoroutine(StartTimerAndEffects());
    }

    private void Awake()
    {
        if (_spriteRenderer != null)
        {
            _originalColor = _spriteRenderer.color;

            _healthManager = new HealthManager(100);
            _healthManager.AddOnDeathAction(StartSeekingOnDestroyed);
        }
    }

    private void Update()
    {
        if (!_isDestroyed)
        {
            transform.Rotate(0, 0, _settings.RotationSpeed * Time.deltaTime);
            transform.position = Vector2.MoveTowards(transform.position, _targetPosition, _settings.SeekSpeed * Time.deltaTime);
        }
        else
        {
            transform.Rotate(0, 0, _settings.RotationSpeedOnDestroyed * Time.deltaTime);
            transform.position = Vector2.MoveTowards(transform.position, _targetPositionOnDestroyed, _settings.SeekSpeedOnDestroyed * Time.deltaTime);

            if (Vector2.Distance(transform.position, _targetPositionOnDestroyed) < 0.1f)
            {
                InstantiateExplosion(_settings.ExplosionPrefabOnDestroy, _settings.ExplosionClipOnDestroy, _targetPositionOnDestroyed);
                _onTargetReached.Invoke();
                Destroy(gameObject);
            }
        }
    }

    /// <summary>
    /// �����G�t�F�N�g�ƃT�E���h�𐶐�����ėp���\�b�h
    /// </summary>
    private void InstantiateExplosion(GameObject explosionPrefab, AudioClip explosionClip, Vector2 position)
    {
        if (explosionPrefab != null)
        {
            Vector3 explosionPosition = (Vector3)position + (Vector3)Random.insideUnitCircle * _settings.ExplosionRadius;
            Instantiate(explosionPrefab, explosionPosition, Quaternion.identity);
        }

        if (explosionClip != null)
        {
            SoundManager.Instance.PlayEffect(explosionClip);
        }
    }

    public void TakeDamage(float amount)
    {
        _healthManager.TakeDamage(amount);
        if (_healthManager.CurrentHealth == 0)
        {
            StartSeekingOnDestroyed();
        }
    }

    private void StartSeekingOnDestroyed()
    {
        _isDestroyed = true;
        StopAllCoroutines();
        _spriteRenderer.color = _settings.BlinkColor;
    }

    private IEnumerator StartTimerAndEffects()
    {
        if (_spriteRenderer == null) yield break;

        _timeElapsed = 0f;
        int nextBlinkIndex = 0;
        List<float> blinkTimings = CalculateBlinkTimings();

        while (_timeElapsed < _explosionTime && _healthManager.IsAlive)
        {
            _timeElapsed += Time.deltaTime;

            if (nextBlinkIndex < blinkTimings.Count && _timeElapsed >= blinkTimings[nextBlinkIndex])
            {
                _spriteRenderer.color = (_spriteRenderer.color == _originalColor) ? _settings.BlinkColor : _originalColor;
                nextBlinkIndex++;
            }

            yield return null;
        }

        if (_healthManager.IsAlive)
        {
            InstantiateExplosion(_settings.ExplosionPrefab, _settings.BomClip, transform.position);
            _onExplosion.Invoke();
            Destroy(gameObject);
        }
    }

    /// <summary>
    /// �_�Ń^�C�~���O�����O�Ɍv�Z����w���p�[���\�b�h
    /// </summary>
    private List<float> CalculateBlinkTimings()
    {
        List<float> timings = new List<float>();
        float currentTime = 0f;

        while (currentTime < _baseExplosionTime)
        {
            float normalizedTime = currentTime / _baseExplosionTime;
            float blinkInterval = Mathf.Lerp(_settings.StartBlinkInterval, _settings.EndBlinkInterval, normalizedTime);

            timings.Add(currentTime);
            currentTime += blinkInterval;
            if (currentTime < _baseExplosionTime)
            {
                timings.Add(currentTime);
                currentTime += blinkInterval;
            }
        }
        return timings;
    }

    private void OnDestroy()
    {
        if (_onExplosion != null && _explosionAction != null)
        {
            _onExplosion.RemoveListener(_explosionAction);
        }

        if (_onTargetReached != null && _targetReachedAction != null)
        {
            _onTargetReached.RemoveListener(_targetReachedAction);
        }

        if (_healthManager != null)
        {
            _healthManager.ClearEvents();
        }
    }
}