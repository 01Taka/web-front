using TMPro;
using UnityEngine;

public class TimerCountUI : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI timerText; // シリアライズフィールドでUI Textを受け取る
    [SerializeField] private float colorChangeBorder = 60f;
    private float lastTime = -1f; // 最後に更新した時間（初期値は無効）

    // タイマーのUIを更新するメソッド
    public void UpdateTimerUI(float time)
    {
        // 小数点以下の部分が異なった場合のみUIを更新
        if (Mathf.Approximately(time, lastTime))
            return;

        lastTime = time;

        // 整数部だけを表示 (小数点以下は切り捨て)
        timerText.text = Mathf.Floor(time).ToString(); // 整数部だけ表示

        // 時間が1分未満になると赤色に変化
        if (time <= 60f)
        {
            // 残り時間に応じて赤色に変化させる
            float t = Mathf.InverseLerp(0f, colorChangeBorder, time); // 0から60秒の間で進捗を0～1に正規化
            timerText.color = Color.Lerp(Color.white, Color.red, 1 - t); // 白から赤へ
        }
        else
        {
            // 1分以上の場合は白色に戻す
            timerText.color = Color.white;
        }
    }
}
