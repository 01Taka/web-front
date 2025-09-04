using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class ScreenFader : MonoBehaviour
{
    private Image _panelImage;

    [Header("Fade Settings")]
    [Tooltip("�t�F�[�h�C�����̃A���t�@�l�B0.0f�`1.0f")]
    [SerializeField]
    private float _flashAlpha = 0.5f;

    [Tooltip("�t�F�[�h�C���ɂ����鎞�ԁi�b�j")]
    [SerializeField]
    private float _fadeInDuration = 0.2f;

    [Tooltip("�F���ێ�����鎞�ԁi�b�j")]
    [SerializeField]
    private float _stayDuration = 0.3f;

    [Tooltip("�t�F�[�h�A�E�g�ɂ����鎞�ԁi�b�j")]
    [SerializeField]
    private float _fadeOutDuration = 0.5f;

    [Tooltip("�A�j���[�V�����̃C�[�W���O�^�C�v")]
    [SerializeField]
    private Ease _easeType = Ease.Linear;

    void Awake()
    {
        _panelImage = GetComponent<Image>();
    }

    /// <summary>
    /// �ݒ肳�ꂽ�p�����[�^�[�Ɋ�Â��A��ʂ��t���b�V��������B
    /// </summary>
    public void Flash()
    {
        // ���݂̃p�l���̐F���擾���A�A���t�@�l��0�ɐݒ�
        Color startColor = _panelImage.color;
        startColor.a = 0;
        _panelImage.color = startColor;

        // �t�F�[�h�C���p�̖ڕW�F���쐬
        Color targetColor = _panelImage.color;
        targetColor.a = _flashAlpha;

        // �t�F�[�h�C���A�j���[�V����
        _panelImage.DOColor(targetColor, _fadeInDuration)
            .SetEase(_easeType)
            .OnComplete(() =>
            {
                // �t�F�[�h�C��������A�w�莞�ԑҋ@
                DOVirtual.DelayedCall(_stayDuration, () =>
                {
                    // �t�F�[�h�A�E�g�A�j���[�V����
                    _panelImage.DOFade(0, _fadeOutDuration).SetEase(_easeType);
                });
            });
    }

    /// <summary>
    /// �t�F�[�h�C���A�j���[�V�������Đ�����B
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
    /// �t�F�[�h�A�E�g�A�j���[�V�������Đ�����B
    /// </summary>
    public void FadeOut()
    {
        _panelImage.DOFade(0, _fadeOutDuration).SetEase(_easeType);
    }
}