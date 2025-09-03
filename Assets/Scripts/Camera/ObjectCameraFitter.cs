using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ObjectCameraFitter : MonoBehaviour
{
    public enum FitMode
    {
        AspectFit,   // �A�X�y�N�g����ێ����đS�̂����܂�悤�Ɋg��i���^�[�{�b�N�X�I�j
        AspectFill,  // �A�X�y�N�g����ێ����ĒZ�ӂɍ��킹��i�g���~���O�����\������j
        Stretch      // �A�X�y�N�g��𖳎����ăJ�����Ƀs�b�^�����킹��
    }

    [SerializeField] private Camera targetCamera;
    [SerializeField] private FitMode fitMode = FitMode.AspectFit;

    private BoxCollider2D boxCollider;

    void Start()
    {
        if (targetCamera == null)
            targetCamera = Camera.main;

        boxCollider = GetComponent<BoxCollider2D>();

        if (boxCollider == null)
        {
            Debug.LogError("No BoxCollider2D found on the object.");
            return;
        }

        ApplyFit();
    }

    void ApplyFit()
    {
        if (boxCollider == null || targetCamera == null) return;

        // �I�u�W�F�N�g�̃T�C�Y�iCollider2D�̃T�C�Y�j
        Vector2 objectSize = boxCollider.size;

        // �J�����̃T�C�Y�iworld�P�ʁj
        float camHeight = 2f * targetCamera.orthographicSize;
        float camWidth = camHeight * targetCamera.aspect;

        Vector3 scale = transform.localScale;

        switch (fitMode)
        {
            case FitMode.AspectFit:
                {
                    // �A�X�y�N�g����ێ����đS�̂����܂�悤�Ɋg��
                    float scaleFactor = Mathf.Min(camWidth / objectSize.x, camHeight / objectSize.y);
                    scale = Vector3.one * scaleFactor;
                    break;
                }

            case FitMode.AspectFill:
                {
                    // �A�X�y�N�g����ێ����ĒZ�ӂɍ��킹��i�g���~���O�����ꍇ������j
                    float scaleFactor = Mathf.Max(camWidth / objectSize.x, camHeight / objectSize.y);
                    scale = Vector3.one * scaleFactor;
                    break;
                }

            case FitMode.Stretch:
                {
                    // �A�X�y�N�g��𖳎����ăJ�����Ƀs�b�^�����킹��
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
