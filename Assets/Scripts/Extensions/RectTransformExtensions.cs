using UnityEngine;

public static class RectTransformExtensions
{
    /// <summary>
    /// ���݂�RectTransform�̒l���A�A���J�[��(0,0)���s�{�b�g��(0,0)�������ꍇ�̍��W�ƃT�C�Y�ɕϊ����܂��B
    /// </summary>
    /// <param name="rectTransform">�ϊ�������RectTransform</param>
    /// <returns>�ϊ����ꂽRect (���[�J�����W�ƃT�C�Y)</returns>
    public static Rect GetLocalRectInParentSpace(this RectTransform rectTransform)
    {
        if (rectTransform.parent == null)
        {
            Debug.LogError("����RectTransform�ɂ͐e������܂���B");
            return Rect.zero;
        }

        RectTransform parentRectTransform = rectTransform.parent.GetComponent<RectTransform>();
        if (parentRectTransform == null)
        {
            Debug.LogError("�e��RectTransform�ł͂���܂���B");
            return Rect.zero;
        }

        // �e��RectTransform�̃T�C�Y
        Vector2 parentSize = parentRectTransform.rect.size;

        // �A���J�[���e�̋�`���Ő�߂�̈�̃T�C�Y���v�Z
        float anchorWidth = parentSize.x * (rectTransform.anchorMax.x - rectTransform.anchorMin.x);
        float anchorHeight = parentSize.y * (rectTransform.anchorMax.y - rectTransform.anchorMin.y);

        // �e�̍�������Ƃ����A���J�[�̍������̃I�t�Z�b�g���v�Z
        float anchorOffsetX = parentSize.x * rectTransform.anchorMin.x;
        float anchorOffsetY = parentSize.y * rectTransform.anchorMin.y;

        // �A���J�[����Ƃ��郍�[�J�����W���A�e�̍�������Ƃ�����W�ɕϊ�
        float x = rectTransform.anchoredPosition.x + anchorOffsetX;
        float y = rectTransform.anchoredPosition.y + anchorOffsetY;

        // �A���J�[����߂�̈��sizeDelta�����Z���āA��ΓI�ȕ��ƍ������v�Z
        float width = anchorWidth + rectTransform.sizeDelta.x;
        float height = anchorHeight + rectTransform.sizeDelta.y;

        // �s�{�b�g��(0,0)�ɕύX�����ꍇ�̍��W���v�Z
        // ���݂̃s�{�b�g�l�ƃT�C�Y����I�t�Z�b�g�����߁A��������W���獷������
        x -= width * rectTransform.pivot.x;
        y -= height * rectTransform.pivot.y;

        return new Rect(x, y, width, height);
    }
}