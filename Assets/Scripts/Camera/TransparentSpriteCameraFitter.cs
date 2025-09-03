using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class TransparentSpriteCameraFitter : MonoBehaviour
{
    public enum FitMode
    {
        AspectFit,   // アスペクト比を維持して全体が収まるように拡大（レターボックス的）
        AspectFill,  // アスペクト比を維持して短辺に合わせる（トリミングされる可能性あり）
        Stretch      // アスペクト比を無視してカメラにピッタリ合わせる
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

        // 透明なスプライトを設定
        spriteRenderer.sprite = null; // 透明スプライト

        ApplyFit();
    }

    void ApplyFit()
    {
        if (spriteRenderer == null || targetCamera == null) return;

        // スプライトのサイズ（boundsから取得）
        Vector2 objectSize = spriteRenderer.bounds.size;

        // カメラのサイズ（world単位）
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

        // 無限大のスケールを設定しないように防止
        if (float.IsInfinity(scale.x) || float.IsInfinity(scale.y) || float.IsInfinity(scale.z))
        {
            Debug.LogError("Calculated scale is infinite: " + scale);
            return;
        }

        transform.localScale = scale;
    }
}
