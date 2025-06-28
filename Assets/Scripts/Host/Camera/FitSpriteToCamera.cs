using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SpriteCameraFitter : MonoBehaviour
{
    public enum FitMode
    {
        AspectFit,   // アスペクト比を維持して全体が収まるように拡大（レターボックス的）
        AspectFill,  // アスペクト比を維持して短辺に合わせる（トリミングされる可能性あり）
        Stretch      // アスペクト比を無視してカメラにピッタリ合わせる
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

        // スプライトのサイズ（world単位）
        Vector2 spriteSize = sprite.bounds.size;

        // カメラのサイズ（world単位）
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
