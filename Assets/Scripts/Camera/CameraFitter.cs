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
        // �^�[�Q�b�g�̃��[���h���W�ł̃T�C�Y���擾
        Vector3[] corners = new Vector3[4];
        _targetRectangle.GetWorldCorners(corners);
        float width = Vector3.Distance(corners[0], corners[2]);
        float height = Vector3.Distance(corners[0], corners[1]);

        // ��ʂ̃A�X�y�N�g��ƃ^�[�Q�b�g�̃A�X�y�N�g����r
        float screenAspect = (float)Screen.width / (float)Screen.height;
        float targetAspect = width / height;

        if (targetAspect > screenAspect)
        {
            // �^�[�Q�b�g�������̏ꍇ�A���ɍ��킹�ăJ������Orthographic Size�𒲐�
            _mainCamera.orthographicSize = width / (2f * screenAspect);
        }
        else
        {
            // �^�[�Q�b�g���c���̏ꍇ�A�����ɍ��킹�ăJ������Orthographic Size�𒲐�
            _mainCamera.orthographicSize = height / 2f;
        }
    }
}