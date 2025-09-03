using UnityEngine;

[RequireComponent(typeof(BoxCollider2D))]
public class ObjectCameraFitter : MonoBehaviour
{
    public enum FitMode
    {
        AspectFit,   // アスペクト比を維持して全体が収まるように拡大（レターボックス的）
        AspectFill,  // アスペクト比を維持して短辺に合わせる（トリミングされる可能性あり）
        Stretch      // アスペクト比を無視してカメラにピッタリ合わせる
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

        // オブジェクトのサイズ（Collider2Dのサイズ）
        Vector2 objectSize = boxCollider.size;

        // カメラのサイズ（world単位）
        float camHeight = 2f * targetCamera.orthographicSize;
        float camWidth = camHeight * targetCamera.aspect;

        Vector3 scale = transform.localScale;

        switch (fitMode)
        {
            case FitMode.AspectFit:
                {
                    // アスペクト比を維持して全体が収まるように拡大
                    float scaleFactor = Mathf.Min(camWidth / objectSize.x, camHeight / objectSize.y);
                    scale = Vector3.one * scaleFactor;
                    break;
                }

            case FitMode.AspectFill:
                {
                    // アスペクト比を維持して短辺に合わせる（トリミングされる場合もあり）
                    float scaleFactor = Mathf.Max(camWidth / objectSize.x, camHeight / objectSize.y);
                    scale = Vector3.one * scaleFactor;
                    break;
                }

            case FitMode.Stretch:
                {
                    // アスペクト比を無視してカメラにピッタリ合わせる
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
