using UnityEngine;

public static class RectTransformExtensions
{
    /// <summary>
    /// 現在のRectTransformの値を、アンカーが(0,0)かつピボットも(0,0)だった場合の座標とサイズに変換します。
    /// </summary>
    /// <param name="rectTransform">変換したいRectTransform</param>
    /// <returns>変換されたRect (ローカル座標とサイズ)</returns>
    public static Rect GetLocalRectInParentSpace(this RectTransform rectTransform)
    {
        if (rectTransform.parent == null)
        {
            Debug.LogError("このRectTransformには親がありません。");
            return Rect.zero;
        }

        RectTransform parentRectTransform = rectTransform.parent.GetComponent<RectTransform>();
        if (parentRectTransform == null)
        {
            Debug.LogError("親がRectTransformではありません。");
            return Rect.zero;
        }

        // 親のRectTransformのサイズ
        Vector2 parentSize = parentRectTransform.rect.size;

        // アンカーが親の矩形内で占める領域のサイズを計算
        float anchorWidth = parentSize.x * (rectTransform.anchorMax.x - rectTransform.anchorMin.x);
        float anchorHeight = parentSize.y * (rectTransform.anchorMax.y - rectTransform.anchorMin.y);

        // 親の左下を基準としたアンカーの左下隅のオフセットを計算
        float anchorOffsetX = parentSize.x * rectTransform.anchorMin.x;
        float anchorOffsetY = parentSize.y * rectTransform.anchorMin.y;

        // アンカーを基準とするローカル座標を、親の左下を基準とする座標に変換
        float x = rectTransform.anchoredPosition.x + anchorOffsetX;
        float y = rectTransform.anchoredPosition.y + anchorOffsetY;

        // アンカーが占める領域にsizeDeltaを加算して、絶対的な幅と高さを計算
        float width = anchorWidth + rectTransform.sizeDelta.x;
        float height = anchorHeight + rectTransform.sizeDelta.y;

        // ピボットを(0,0)に変更した場合の座標を計算
        // 現在のピボット値とサイズからオフセットを求め、それを座標から差し引く
        x -= width * rectTransform.pivot.x;
        y -= height * rectTransform.pivot.y;

        return new Rect(x, y, width, height);
    }
}