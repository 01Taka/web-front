using UnityEngine;
using DG.Tweening;

public class UIComplexAnimator : MonoBehaviour
{
    // アニメーションの種類
    public enum AnimationType
    {
        SlideInFromLeft,
        FadeIn,
        ScaleIn,
        LoopScale,
        LoopFloat
    }

    // UIの初期状態の種類
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

        [Header("ループアニメーション設定")]
        public float loopStrength = 0.2f;
    }

    [Header("アニメーション設定")]
    public AnimationSettings[] animations;
    [Header("開始時の状態設定")]
    public InitialState initialState = InitialState.Show;

    private RectTransform rectTransform;
    private CanvasGroup canvasGroup;
    private Vector2 originalAnchoredPosition;
    private bool _isShowing = false;

    // 現在UIが表示中かどうかを外部から確認できるプロパティ
    public bool IsShowing => _isShowing;

    void Awake()
    {
        rectTransform = GetComponent<RectTransform>();
        if (rectTransform == null)
        {
            Debug.LogError("RectTransformコンポーネントが見つかりません。");
            return;
        }

        canvasGroup = GetComponent<CanvasGroup>();
        if (canvasGroup == null)
        {
            canvasGroup = gameObject.AddComponent<CanvasGroup>();
        }

        // 元の位置を保存
        originalAnchoredPosition = rectTransform.anchoredPosition;

        // 初期状態を設定
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
    /// UIを非表示状態（Out状態）に即座に設定します。
    /// </summary>
    private void SetStateToHide()
    {
        DOTween.Kill(this.transform);
        _isShowing = false;

        // 各アニメーションタイプに応じて初期状態を設定
        foreach (var anim in animations)
        {
            if (!anim.isEnabled) continue;

            switch (anim.type)
            {
                case AnimationType.SlideInFromLeft:
                    // 画面左端の外側に配置
                    float startX = GetElementRightEdgeX();
                    rectTransform.anchoredPosition = new Vector2(startX, rectTransform.anchoredPosition.y);
                    break;
                case AnimationType.FadeIn:
                    // 透明度を0に設定
                    canvasGroup.alpha = 0;
                    break;
                case AnimationType.ScaleIn:
                    // スケールを0に設定
                    rectTransform.localScale = Vector3.zero;
                    break;
            }
        }
    }

    /// <summary>
    /// UIを表示状態（In状態）に即座に設定します。
    /// </summary>
    private void SetStateToShow()
    {
        DOTween.Kill(this.transform);
        _isShowing = true;

        // 最終状態に即座に設定
        rectTransform.anchoredPosition = originalAnchoredPosition;
        canvasGroup.alpha = 1;
        rectTransform.localScale = Vector3.one;
    }

    /// <summary>
    /// アニメーションを再生してUIを表示状態にします。
    /// </summary>
    public void Show()
    {
        SetStateToShow();

        // 既存のTweenをすべて停止
        DOTween.Kill(this.transform);

        // アニメーション開始時の初期状態に設定
        SetStateToHide();

        Sequence sequence = DOTween.Sequence();
        float firstTweenDuration = 0f;

        foreach (var anim in animations)
        {
            if (!anim.isEnabled) continue;

            // 最初の有効なアニメーションの長さを取得
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
                    // ループアニメーションはIntroアニメーションの後に開始
                    float targetScale = 1.0f + anim.loopStrength;
                    sequence.Insert(firstTweenDuration, rectTransform.DOScale(targetScale, anim.duration).SetLoops(-1, LoopType.Yoyo).SetEase(anim.easeType));
                    break;
                case AnimationType.LoopFloat:
                    // ループアニメーションはIntroアニメーションの後に開始
                    float targetY = originalAnchoredPosition.y + anim.loopStrength * 100;
                    sequence.Insert(firstTweenDuration, rectTransform.DOAnchorPosY(targetY, anim.duration).SetLoops(-1, LoopType.Yoyo).SetEase(anim.easeType));
                    break;
            }
        }
        _isShowing = true;
    }

    /// <summary>
    /// アニメーションを逆再生してUIを非表示状態にします。
    /// </summary>
    public void Hide()
    {
        if (!_isShowing) return;

        // 既存のTweenをすべて停止
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
    /// すべてのアニメーションを即座に停止し、UIを初期状態に戻します。
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
