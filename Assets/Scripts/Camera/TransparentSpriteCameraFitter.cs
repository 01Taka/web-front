using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TransparentSpriteCameraFitter : MonoBehaviour
{
    public enum FitMode
    {
        AspectFit,   // �A�X�y�N�g����ێ����đS�̂����܂�悤�Ɋg��i���^�[�{�b�N�X�I�j
        AspectFill,  // �A�X�y�N�g����ێ����ĒZ�ӂɍ��킹��i�g���~���O�����\������j
        Stretch      // �A�X�y�N�g��𖳎����ăJ�����Ƀs�b�^�����킹��
    }

    [SerializeField] private Camera targetCamera;
    [SerializeField] private FitMode fitMode = FitMode.AspectFit;

    private SpriteRenderer spriteRenderer;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        spriteRenderer = GetComponent<SpriteRenderer>();

        if (spriteRenderer == null)
        {
            Debug.LogError("No SpriteRenderer found on the object.");
            return;
        }

        // �����ȃX�v���C�g��ݒ�
        spriteRenderer.sprite = null; // �����X�v���C�g

        ApplyFit();
    }

    void ApplyFit()
    {
        if (spriteRenderer == null || targetCamera == null) return;

        // �X�v���C�g�̃T�C�Y�ibounds����擾�j
        Vector2 objectSize = spriteRenderer.bounds.size;

        // �J�����̃T�C�Y�iworld�P�ʁj
        float camHeight = 2f * targetCamera.orthographicSize;
        float camWidth = camHeight * targetCamera.aspect;

        Vector3 scale = transform.localScale;

        switch (fitMode)
        {
            case FitMode.AspectFit:
                {
                    float scaleFactor = Mathf.Min(camWidth / objectSize.x, camHeight / objectSize.y);
                    scale = Vector3.one * scaleFactor;
                    break;
                }

            case FitMode.AspectFill:
                {
                    float scaleFactor = Mathf.Max(camWidth / objectSize.x, camHeight / objectSize.y);
                    scale = Vector3.one * scaleFactor;
                    break;
                }

            case FitMode.Stretch:
                {
                    scale = new Vector3(camWidth / objectSize.x, camHeight / objectSize.y, 1f);
                    break;
                }
        }

        // ������̃X�P�[����ݒ肵�Ȃ��悤�ɖh�~
        if (float.IsInfinity(scale.x) || float.IsInfinity(scale.y) || float.IsInfinity(scale.z))
        {
            Debug.LogError("Calculated scale is infinite: " + scale);
            return;
        }

        transform.localScale = scale;
    }
}
