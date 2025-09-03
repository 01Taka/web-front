using UnityEngine;
using UnityEngine.Events;
using System.Collections;

public class MechanicalSpiderBom : MonoBehaviour, IDamageable
{
    // --- �t�B�[���h ---
    [Header("��]�ݒ�")]
    [Tooltip("Z������̉�]���x (�x/�b)")]
    [SerializeField]
    private float _rotationSpeed = 100f;

    [Header("�_�Őݒ�")]
    [Tooltip("�_�ł̋K��F")]
    [SerializeField]
    private Color _blinkColor = Color.red;
    [Tooltip("�_�ł̊J�n�p�x (�b)")]
    [SerializeField]
    private float _startBlinkInterval = 0.5f;
    [Tooltip("�_�ł̏I���p�x (�b)")]
    [SerializeField]
    private float _endBlinkInterval = 0.1f;
    [Header("�X�v���C�g�ݒ�")]
    [SerializeField]
    private SpriteRenderer _spriteRenderer;
    private Color _originalColor;

    [Header("�����ݒ�")]
    [SerializeField]
    private AudioClip _bomClip;
    [Tooltip("�������ɌĂ΂��C�x���g")]
    [SerializeField]
    private UnityEvent _onExplosion;
    private float _explosionTime;
    private float _timeElapsed;

    // �V�����t�B�[���h: �����v���n�u
    [Tooltip("�������ɐ�������v���n�u")]
    [SerializeField]
    private GameObject _explosionPrefab;

    // �V�����t�B�[���h: �����܂ł̎��ԃ����_����
    [Tooltip("�����܂ł̎��Ԃɉ��Z���郉���_���l�͈̔� (�b)")]
    [SerializeField]
    private float _explosionTimeRandomRange = 0.5f;

    // �V�����t�B�[���h: �����ʒu�����_����
    [Tooltip("�������Ƀ^�[�Q�b�g���W���烉���_���ɂ��炷���a")]
    [SerializeField]
    private float _explosionRadius = 0.5f;

    // �O������n���ꂽ���X�i�[��ێ����邽�߂̃v���C�x�[�g�t�B�[���h
    private UnityAction _explosionAction;

    // �V�����t�B�[���h
    [Header("�ǐՂ�HP�ݒ�")]
    [Tooltip("�ǐՑ��x")]
    [SerializeField]
    private float _seekSpeed = 5f;

    private Vector2 _targetPosition;

    // �V����UnityEvent
    [Header("�ڕW���B�C�x���g")]
    [Tooltip("�ڕW�ɓ��B�������ɌĂ΂��C�x���g")]
    [SerializeField]
    private UnityEvent _onTargetReached;

    // �O������n���ꂽ���X�i�[��ێ����邽�߂̃v���C�x�[�g�t�B�[���h
    private UnityAction _targetReachedAction;

    // HealthManager�̃C���X�^���X
    private HealthManager _healthManager;

    // --- ���\�b�h ---

    /// <summary>
    /// ���e���N�����郁�\�b�h�B
    /// </summary>
    /// <param name="explosionDuration">�����܂ł̎���</param>
    /// <param name="maxHealth">�ő�HP</param>
    /// <param name="target">�ǐՖڕW</param>
    /// <param name="onExplosionAction">�������Ɏ��s�����A�N�V����</param>
    /// <param name="onTargetReachedAction">�ڕW���B���Ɏ��s�����A�N�V����</param>
    public void Activate(float explosionDuration, float maxHealth, Vector2 target,
                         UnityAction onExplosionAction, UnityAction onTargetReachedAction)
    {
        // �����܂ł̎��ԂɃ����_���Ȓl�����Z
        _explosionTime = explosionDuration + Random.Range(0, _explosionTimeRandomRange);
        _targetPosition = target;
        _timeElapsed = 0f;

        _healthManager.SetMaxHealth(maxHealth);

        // ���X�i�[���t�B�[���h�ɕێ�
        _explosionAction = onExplosionAction;
        _targetReachedAction = onTargetReachedAction;

        // �O������n���ꂽ���X�i�[���C�x���g�ɓo�^
        _onExplosion.AddListener(_explosionAction);
        _onTargetReached.AddListener(_targetReachedAction);

        // �_�łƔ����̃R���[�`�����J�n
        StartCoroutine(BlinkEffectCoroutine());
        StartCoroutine(ExplosionCoroutine());
    }

    private void Awake()
    {
        // HealthManager��������
        _healthManager = new HealthManager(100);
        // HP��0�ɂȂ�����^�[�Q�b�g��ǐՂ��鏈����o�^
        _healthManager.AddOnDeathAction(StartSeeking);
    }

    private void Start()
    {
        if (_spriteRenderer != null)
        {
            _originalColor = _spriteRenderer.color;
        }
    }

    private void Update()
    {
        // Z������̉�]
        transform.Rotate(0, 0, _rotationSpeed * Time.deltaTime);

        // �ǐՏ�Ԃ̏ꍇ�A�^�[�Q�b�g�Ɍ������Ĉړ�
        if (!_healthManager.IsAlive)
        {
            // �^�[�Q�b�g�����݂���ꍇ
            if (_targetPosition != null)
            {
                // �^�[�Q�b�g�Ɍ������Ĉړ�
                transform.position = Vector2.MoveTowards(transform.position, _targetPosition, _seekSpeed * Time.deltaTime);

                // �^�[�Q�b�g�ɏ\���ɋ߂Â�����C�x���g���N�����A�I�u�W�F�N�g��j��
                if (Vector2.Distance(transform.position, _targetPosition) < 0.1f)
                {
                    // �����ʒu�Ƀ����_���ȃI�t�Z�b�g��������
                    Vector3 explosionPosition = (Vector3)_targetPosition + (Vector3)Random.insideUnitCircle * _explosionRadius;

                    // �����v���n�u�𐶐�
                    if (_explosionPrefab != null)
                    {
                        Instantiate(_explosionPrefab, explosionPosition, Quaternion.identity);
                    }

                    _onTargetReached.Invoke();
                    PlayExplosionSound();
                    Destroy(gameObject);
                }
            }
        }
    }

    private void PlayExplosionSound()
    {
        SoundManager.Instance.PlayEffect(_bomClip);
    }

    /// <summary>
    /// �_���[�W���󂯂郁�\�b�h
    /// </summary>
    /// <param name="amount">�_���[�W��</param>
    public void TakeDamage(float amount)
    {
        
        _healthManager.TakeDamage(amount);
    }

    private void StartSeeking()
    {
        // �����̃R���[�`�����~
        StopCoroutine(ExplosionCoroutine());
        // �_�ł̃R���[�`�����~
        StopCoroutine(BlinkEffectCoroutine());
        // ���o�I�ȕω�
        _spriteRenderer.color = _blinkColor;
        _rotationSpeed = 300f; // ��]���x���グ��Ȃ�
    }

    /// <summary>
    /// �_�ŃG�t�F�N�g�𐧌䂷��R���[�`���B
    /// </summary>
    private IEnumerator BlinkEffectCoroutine()
    {
        // SpriteRenderer�����݂��Ȃ��ꍇ�͏I��
        if (_spriteRenderer == null) yield break;

        while (_timeElapsed < _explosionTime && _healthManager.IsAlive)
        {
            // �o�ߎ��ԂɊ�Â��ē_�ŕp�x���v�Z
            float normalizedTime = _timeElapsed / _explosionTime;
            float currentBlinkInterval = Mathf.Lerp(_startBlinkInterval, _endBlinkInterval, normalizedTime);

            // �F��_blinkColor�ɕύX
            _spriteRenderer.color = _blinkColor;
            yield return new WaitForSeconds(currentBlinkInterval);

            // �F��(#FFFFFF)�ɕύX
            _spriteRenderer.color = _originalColor;
            yield return new WaitForSeconds(currentBlinkInterval);
        }
    }

    /// <summary>
    /// �����܂ł̎��Ԃ��J�E���g�_�E�����A�C�x���g���N������R���[�`���B
    /// </summary>
    private IEnumerator ExplosionCoroutine()
    {
        // �o�ߎ��Ԃ��J�E���g
        while (_timeElapsed < _explosionTime && _healthManager.IsAlive)
        {
            _timeElapsed += Time.deltaTime;
            yield return null;
        }

        // ���e�̗̑͂��c���Ă���ꍇ�͔���
        if (_healthManager.IsAlive)
        {
            // �����v���n�u�𐶐�
            if (_explosionPrefab != null)
            {
                Instantiate(_explosionPrefab, transform.position, Quaternion.identity);
            }

            PlayExplosionSound();
            _onExplosion.Invoke();
            
            Destroy(gameObject);
        }
    }

    // �I�u�W�F�N�g���j�������Ƃ��ɌĂ΂��
    private void OnDestroy()
    {
        // UnityEvent���烊�X�i�[������
        // Null�`�F�b�N�́A�I�u�W�F�N�g����ɔj�������ꍇ�����邽�ߕK�{
        if (_onExplosion != null && _explosionAction != null)
        {
            _onExplosion.RemoveListener(_explosionAction);
        }

        if (_onTargetReached != null && _targetReachedAction != null)
        {
            _onTargetReached.RemoveListener(_targetReachedAction);
        }

        // HealthManager�̃��X�i�[������
        if (_healthManager != null)
        {
            _healthManager.ClearEvents();
        }
    }
}