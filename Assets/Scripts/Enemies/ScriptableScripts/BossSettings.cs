using UnityEngine;

[CreateAssetMenu(fileName = "NewBossSettings", menuName = "Boss/BossSettings")]
public class BossSettings : ScriptableObject
{
    public BossId id;
    public GameObject bossInstance;

    // フェーズごとの設定（複数のフェーズを保持）
    public BossPhaseSettings[] phaseSettings;

    // フェーズ数を取得
    public int GetPhaseCount()
    {
        return phaseSettings.Length;
    }
}
