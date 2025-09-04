using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScreenFader : MonoBehaviour
{
    private Image _panelImage;

    [Header("Fade Settings")]
    [Tooltip("フェードイン時のアルファ値。0.0f～1.0f")]
    [SerializeField]
    private float _flashAlpha = 0.5f;

    [Tooltip("フェードインにかかる時間（秒）")]
    [SerializeField]
    private float _fadeInDuration = 0.2f;

    [Tooltip("色が維持される時間（秒）")]
    [SerializeField]
    private float _stayDuration = 0.3f;

    [Tooltip("フェードアウトにかかる時間（秒）")]
    [SerializeField]
    private float _fadeOutDuration = 0.5f;

    [Tooltip("アニメーションのイージングタイプ")]
    [SerializeField]
    private Ease _easeType = Ease.Linear;

    void Awake()
    {
        _panelImage = GetComponent<Image>();
    }

    /// <summary>
    /// 設定されたパラメーターに基づき、画面をフラッシュさせる。
    /// </summary>
    public void Flash()
    {
        // 現在のパネルの色を取得し、アルファ値を0に設定
        Color startColor = _panelImage.color;
        startColor.a = 0;
        _panelImage.color = startColor;

        // フェードイン用の目標色を作成
        Color targetColor = _panelImage.color;
        targetColor.a = _flashAlpha;

        // フェードインアニメーション
        _panelImage.DOColor(targetColor, _fadeInDuration)
            .SetEase(_easeType)
            .OnComplete(() =>
            {
                // フェードイン完了後、指定時間待機
                DOVirtual.DelayedCall(_stayDuration, () =>
                {
                    // フェードアウトアニメーション
                    _panelImage.DOFade(0, _fadeOutDuration).SetEase(_easeType);
                });
            });
    }

    /// <summary>
    /// フェードインアニメーションを再生する。
    /// </summary>
    public void FadeIn()
    {
        Color startColor = _panelImage.color;
        startColor.a = 0;
        _panelImage.color = startColor;

        Color targetColor = _panelImage.color;
        targetColor.a = 1.0f;

        _panelImage.DOColor(targetColor, _fadeInDuration).SetEase(_easeType);
    }

    /// <summary>
    /// フェードアウトアニメーションを再生する。
    /// </summary>
    public void FadeOut()
    {
        _panelImage.DOFade(0, _fadeOutDuration).SetEase(_easeType);
    }
}