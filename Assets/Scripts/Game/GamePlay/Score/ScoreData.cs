using UnityEngine;

[CreateAssetMenu(fileName = "ScoreData", menuName = "Score/ScoreData")]
public class ScoreData : ScriptableObject
{
    [Header("ボス関連のスコア")]
    public int ScoreOnBossDefeat = 10000;

    [Tooltip("各フェーズでHPを1%削るごとのスコア")]
    public float[] ScorePerBossHpPercentByPhase;

    [Header("雑魚敵関連のスコア")]
    public int ScorePerHordeEnemyDefeat = 100;

    [Header("時間とダメージのスコア")]
    public int ScorePerSecondRemaining = 300;
    public int ScorePerDamageTaken = -5;
}