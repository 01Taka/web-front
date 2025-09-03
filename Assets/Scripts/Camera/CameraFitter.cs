using UnityEngine;

public class CameraFitter : MonoBehaviour
{
    [SerializeField] private RectTransform _targetRectangle;
    [SerializeField] private Camera _mainCamera;

    void Start()
    {
        if (_mainCamera == null)
        {
            _mainCamera = Camera.main;
        }

        FitToTargetRectangle();
    }

    void FitToTargetRectangle()
    {
        // ターゲットのワールド座標でのサイズを取得
        Vector3[] corners = new Vector3[4];
        _targetRectangle.GetWorldCorners(corners);
        float width = Vector3.Distance(corners[0], corners[2]);
        float height = Vector3.Distance(corners[0], corners[1]);

        // 画面のアスペクト比とターゲットのアスペクト比を比較
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float targetAspect = width / height;

        if (targetAspect > screenAspect)
        {
            // ターゲットが横長の場合、幅に合わせてカメラのOrthographic Sizeを調整
            _mainCamera.orthographicSize = width / (2f * screenAspect);
        }
        else
        {
            // ターゲットが縦長の場合、高さに合わせてカメラのOrthographic Sizeを調整
            _mainCamera.orthographicSize = height / 2f;
        }
    }
}