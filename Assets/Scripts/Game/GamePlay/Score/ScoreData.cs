using UnityEngine;

[CreateAssetMenu(fileName = "ScoreData", menuName = "Score/ScoreData")]
public class ScoreData : ScriptableObject
{
    [Header("�{�X�֘A�̃X�R�A")]
    public int ScoreOnBossDefeat = 10000;

    [Tooltip("�e�t�F�[�Y��HP��1%��邲�Ƃ̃X�R�A")]
    public float[] ScorePerBossHpPercentByPhase;

    [Header("�G���G�֘A�̃X�R�A")]
    public int ScorePerHordeEnemyDefeat = 100;

    [Header("���Ԃƃ_���[�W�̃X�R�A")]
    public int ScorePerSecondRemaining = 300;
    public int ScorePerDamageTaken = -5;
}