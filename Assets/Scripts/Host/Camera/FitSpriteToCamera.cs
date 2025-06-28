using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteCameraFitter : MonoBehaviour
{
    public enum FitMode
    {
        AspectFit,   // �A�X�y�N�g����ێ����đS�̂����܂�悤�Ɋg��i���^�[�{�b�N�X�I�j
        AspectFill,  // �A�X�y�N�g����ێ����ĒZ�ӂɍ��킹��i�g���~���O�����\������j
        Stretch      // �A�X�y�N�g��𖳎����ăJ�����Ƀs�b�^�����킹��
    }

    [SerializeField] private Camera targetCamera;
    [SerializeField] private FitMode fitMode = FitMode.AspectFit;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        ApplyFit();
    }

    void ApplyFit()
    {
        SpriteRenderer sr = GetComponent<SpriteRenderer>();
        Sprite sprite = sr.sprite;

        if (sprite == null || targetCamera == null) return;

        // �X�v���C�g�̃T�C�Y�iworld�P�ʁj
        Vector2 spriteSize = sprite.bounds.size;

        // �J�����̃T�C�Y�iworld�P�ʁj
        float camHeight = 2f * targetCamera.orthographicSize;
        float camWidth = camHeight * targetCamera.aspect;

        Vector3 scale = transform.localScale;

        switch (fitMode)
        {
            case FitMode.AspectFit:
                {
                    float scaleFactor = Mathf.Min(camWidth / spriteSize.x, camHeight / spriteSize.y);
                    scale = Vector3.one * scaleFactor;
                    break;
                }

            case FitMode.AspectFill:
                {
                    float scaleFactor = Mathf.Max(camWidth / spriteSize.x, camHeight / spriteSize.y);
                    scale = Vector3.one * scaleFactor;
                    break;
                }

            case FitMode.Stretch:
                {
                    scale = new Vector3(camWidth / spriteSize.x, camHeight / spriteSize.y, 1f);
                    break;
                }
        }

        transform.localScale = scale;
    }
}
