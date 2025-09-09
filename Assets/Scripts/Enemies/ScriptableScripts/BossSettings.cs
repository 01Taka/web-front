using UnityEngine;

[CreateAssetMenu(fileName = "NewBossSettings", menuName = "Boss/BossSettings")]
public class BossSettings : ScriptableObject
{
    public BossId id;
    public GameObject bossInstance;
    public BossEntranceSettings entranceSettings;

    [Tooltip("ボスの総体力。")]
    public float maxHealth;

    [Tooltip("フェーズごとの設定。")]
    public BossPhaseSettings[] phaseSettings;

    // フェーズ数を取得
    public int GetPhaseCount()
    {
        return phaseSettings.Length;
    }

    // OnValidateはエディタで値が変更されたときに呼び出される
    private void OnValidate()
    {
        if (phaseSettings == null || phaseSettings.Length == 0)
        {
            return;
        }

        // 最初のフェーズの移行パーセンテージが100%未満であることを検証
        if (phaseSettings[0].phaseTransitionHealthPercentage >= 1.0f)
        {
            Debug.LogError("最初のフェーズの移行パーセンテージは100%未満に設定してください。");
        }

        // フェーズごとの移行パーセンテージが前のフェーズより高くないことを検証
        for (int i = 1; i < phaseSettings.Length; i++)
        {
            if (phaseSettings[i].phaseTransitionHealthPercentage > phaseSettings[i - 1].phaseTransitionHealthPercentage)
            {
                Debug.LogError($"フェーズ {i + 1} の移行パーセンテージ ({phaseSettings[i].phaseTransitionHealthPercentage * 100}%) が、前のフェーズ {i} のパーセンテージ ({phaseSettings[i - 1].phaseTransitionHealthPercentage * 100}%) より高く設定されています。パーセンテージは常に減少する必要があります。");
            }
        }
    }
}