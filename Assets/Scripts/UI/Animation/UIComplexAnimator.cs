using UnityEngine;
using DG.Tweening;

public class UIComplexAnimator : MonoBehaviour
{
    // �A�j���[�V�����̎��
    public enum AnimationType
    {
        SlideInFromLeft,
        FadeIn,
        ScaleIn,
        LoopScale,
        LoopFloat
    }

    // UI�̏�����Ԃ̎��
    public enum InitialState
    {
        Show,
        Hide
    }

    [System.Serializable]
    public class AnimationSettings
    {
        public bool isEnabled = false;
        public AnimationType type;
        public float duration = 0.5f;
        public Ease easeType = Ease.OutQuad;
        public float delay = 0f;

        [Header("���[�v�A�j���[�V�����ݒ�")]
        public float loopStrength = 0.2f;
    }

    [Header("�A�j���[�V�����ݒ�")]
    public AnimationSettings[] animations;
    [Header("�J�n���̏�Ԑݒ�")]
    public InitialState initialState = InitialState.Show;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalAnchoredPosition;
    private bool _isShowing = false;

    // ����UI���\�������ǂ������O������m�F�ł���v���p�e�B
    public bool IsShowing => _isShowing;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("RectTransform�R���|�[�l���g��������܂���B");
            return;
        }

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // ���̈ʒu��ۑ�
        originalAnchoredPosition = rectTransform.anchoredPosition;

        // ������Ԃ�ݒ�
        if (initialState == InitialState.Show)
        {
            Show();
        }
        else
        {
            SetStateToHide();
        }
    }

    /// <summary>
    /// UI���\����ԁiOut��ԁj�ɑ����ɐݒ肵�܂��B
    /// </summary>
    private void SetStateToHide()
    {
        DOTween.Kill(this.transform);
        _isShowing = false;

        // �e�A�j���[�V�����^�C�v�ɉ����ď�����Ԃ�ݒ�
        foreach (var anim in animations)
        {
            if (!anim.isEnabled) continue;

            switch (anim.type)
            {
                case AnimationType.SlideInFromLeft:
                    // ��ʍ��[�̊O���ɔz�u
                    float startX = GetElementRightEdgeX();
                    rectTransform.anchoredPosition = new Vector2(startX, rectTransform.anchoredPosition.y);
                    break;
                case AnimationType.FadeIn:
                    // �����x��0�ɐݒ�
                    canvasGroup.alpha = 0;
                    break;
                case AnimationType.ScaleIn:
                    // �X�P�[����0�ɐݒ�
                    rectTransform.localScale = Vector3.zero;
                    break;
            }
        }
    }

    /// <summary>
    /// UI��\����ԁiIn��ԁj�ɑ����ɐݒ肵�܂��B
    /// </summary>
    private void SetStateToShow()
    {
        DOTween.Kill(this.transform);
        _isShowing = true;

        // �ŏI��Ԃɑ����ɐݒ�
        rectTransform.anchoredPosition = originalAnchoredPosition;
        canvasGroup.alpha = 1;
        rectTransform.localScale = Vector3.one;
    }

    /// <summary>
    /// �A�j���[�V�������Đ�����UI��\����Ԃɂ��܂��B
    /// </summary>
    public void Show()
    {
        SetStateToShow();

        // ������Tween�����ׂĒ�~
        DOTween.Kill(this.transform);

        // �A�j���[�V�����J�n���̏�����Ԃɐݒ�
        SetStateToHide();

        Sequence sequence = DOTween.Sequence();
        float firstTweenDuration = 0f;

        foreach (var anim in animations)
        {
            if (!anim.isEnabled) continue;

            // �ŏ��̗L���ȃA�j���[�V�����̒������擾
            if (firstTweenDuration == 0f)
            {
                firstTweenDuration = anim.duration;
            }

            switch (anim.type)
            {
                case AnimationType.SlideInFromLeft:
                    sequence.Join(rectTransform.DOAnchorPos(originalAnchoredPosition, anim.duration).SetEase(anim.easeType));
                    break;
                case AnimationType.FadeIn:
                    sequence.Join(canvasGroup.DOFade(1, anim.duration).SetEase(anim.easeType));
                    break;
                case AnimationType.ScaleIn:
                    sequence.Join(rectTransform.DOScale(Vector3.one, anim.duration).SetEase(anim.easeType));
                    break;
                case AnimationType.LoopScale:
                    // ���[�v�A�j���[�V������Intro�A�j���[�V�����̌�ɊJ�n
                    float targetScale = 1.0f + anim.loopStrength;
                    sequence.Insert(firstTweenDuration, rectTransform.DOScale(targetScale, anim.duration).SetLoops(-1, LoopType.Yoyo).SetEase(anim.easeType));
                    break;
                case AnimationType.LoopFloat:
                    // ���[�v�A�j���[�V������Intro�A�j���[�V�����̌�ɊJ�n
                    float targetY = originalAnchoredPosition.y + anim.loopStrength * 100;
                    sequence.Insert(firstTweenDuration, rectTransform.DOAnchorPosY(targetY, anim.duration).SetLoops(-1, LoopType.Yoyo).SetEase(anim.easeType));
                    break;
            }
        }
        _isShowing = true;
    }

    /// <summary>
    /// �A�j���[�V�������t�Đ�����UI���\����Ԃɂ��܂��B
    /// </summary>
    public void Hide()
    {
        if (!_isShowing) return;

        // ������Tween�����ׂĒ�~
        DOTween.Kill(this.transform);

        Sequence sequence = DOTween.Sequence();

        foreach (var anim in animations)
        {
            if (!anim.isEnabled) continue;

            switch (anim.type)
            {
                case AnimationType.SlideInFromLeft:
                    float startX = -Screen.width;
                    sequence.Join(rectTransform.DOAnchorPos(new Vector2(startX, rectTransform.anchoredPosition.y), anim.duration).SetEase(anim.easeType));
                    break;
                case AnimationType.FadeIn:
                    sequence.Join(canvasGroup.DOFade(0, anim.duration).SetEase(anim.easeType));
                    break;
                case AnimationType.ScaleIn:
                    sequence.Join(rectTransform.DOScale(Vector3.zero, anim.duration).SetEase(anim.easeType));
                    break;
            }
        }
        _isShowing = false;
    }

    /// <summary>
    /// ���ׂẴA�j���[�V�����𑦍��ɒ�~���AUI��������Ԃɖ߂��܂��B
    /// </summary>
    public void Stop()
    {
        SetStateToHide();
    }

    private float GetElementRightEdgeX()
    {
        RectTransform rectTransform = GetComponent<RectTransform>();
        Canvas canvas = rectTransform.GetComponentInParent<Canvas>();

        Rect rectInParentSpace = RectTransformExtensions.GetLocalRectInParentSpace(rectTransform);
        float elementLeftEdgeX = rectInParentSpace.x + rectInParentSpace.width;
        return -elementLeftEdgeX;
    }
}
