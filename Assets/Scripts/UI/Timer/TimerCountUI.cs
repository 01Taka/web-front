using TMPro;
using UnityEngine;

public class TimerCountUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText; // �V���A���C�Y�t�B�[���h��UI Text���󂯎��
    [SerializeField] private float colorChangeBorder = 60f;
    private float lastTime = -1f; // �Ō�ɍX�V�������ԁi�����l�͖����j

    // �^�C�}�[��UI���X�V���郁�\�b�h
    public void UpdateTimerUI(float time)
    {
        // �����_�ȉ��̕������قȂ����ꍇ�̂�UI���X�V
        if (Mathf.Approximately(time, lastTime))
            return;

        lastTime = time;

        // ������������\�� (�����_�ȉ��͐؂�̂�)
        timerText.text = Mathf.Floor(time).ToString(); // �����������\��

        // ���Ԃ�1�������ɂȂ�ƐԐF�ɕω�
        if (time <= 60f)
        {
            // �c�莞�Ԃɉ����ĐԐF�ɕω�������
            float t = Mathf.InverseLerp(0f, colorChangeBorder, time); // 0����60�b�̊ԂŐi����0�`1�ɐ��K��
            timerText.color = Color.Lerp(Color.white, Color.red, 1 - t); // ������Ԃ�
        }
        else
        {
            // 1���ȏ�̏ꍇ�͔��F�ɖ߂�
            timerText.color = Color.white;
        }
    }
}
