using UnityEngine;
using System.Collections.Generic;

public enum ScoreType
{
    BossDefeatBonus,
    BossDamageBonus,
    HordeEnemyScore,
    TimeRemainingBonus,
    DamageTakenPenalty,
}

public class ScoreBreakdown
{
    public Dictionary<ScoreType, int> scores = new Dictionary<ScoreType, int>();

    public int TotalScore
    {
        get
        {
            int total = 0;
            foreach (var score in scores.Values)
            {
                total += score;
            }
            return total;
        }
    }
}

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private ScoreData _scoreData;

    private int _hordeEnemyKilledCount = 0;
    private bool _isBossDefeated = false;

    public void Initialize()
    {
        _hordeEnemyKilledCount = 0;
        _isBossDefeated = false;
    }

    public void AddHordeEnemyKill()
    {
        _hordeEnemyKilledCount++;
    }

    public void OnBossDefeat()
    {
        if (!_isBossDefeated)
        {
            _isBossDefeated = true;
        }
    }

    public ScoreBreakdown CalculateFinalScore(float remainingTime, int bossPhaseIndex, float currentPhaseHpPercent, float totalDamageTaken)
    {
        var breakdown = new ScoreBreakdown();

        // �{�X���j�{�[�i�X
        if (_isBossDefeated)
        {
            breakdown.scores[ScoreType.BossDefeatBonus] = _scoreData.ScoreOnBossDefeat;
        }

        // �{�XHP���{�[�i�X
        breakdown.scores[ScoreType.BossDamageBonus] = GetBossHealthScore(bossPhaseIndex, currentPhaseHpPercent);

        // �G���G���j�X�R�A
        breakdown.scores[ScoreType.HordeEnemyScore] = _hordeEnemyKilledCount * _scoreData.ScorePerHordeEnemyDefeat;

        // �c�莞�ԃ{�[�i�X
        if (remainingTime > 0)
        {
            breakdown.scores[ScoreType.TimeRemainingBonus] = Mathf.FloorToInt(remainingTime) * _scoreData.ScorePerSecondRemaining;
        }

        // �_���[�W���_
        breakdown.scores[ScoreType.DamageTakenPenalty] = Mathf.FloorToInt(totalDamageTaken) * _scoreData.ScorePerDamageTaken;

        // ���v�X�R�A���}�C�i�X�ɂȂ�Ȃ��悤�ɒ���
        if (breakdown.TotalScore < 0)
        {
            int penalty = breakdown.scores[ScoreType.DamageTakenPenalty];
            breakdown.scores[ScoreType.DamageTakenPenalty] = penalty - breakdown.TotalScore;
        }

        return breakdown;
    }

    private int GetBossHealthScore(int currentPhaseIndex, float currentPhaseHpPercent)
    {
        if (_scoreData == null || _scoreData.ScorePerBossHpPercentByPhase == null || _scoreData.ScorePerBossHpPercentByPhase.Length == 0)
        {
            Debug.LogError("ScoreData or ScorePerBossHpPercentByPhase is not set or empty.");
            return 0;
        }

        float score = 0f;

        // ���������t�F�[�Y�̃X�R�A�����Z
        for (int i = 0; i < currentPhaseIndex; i++)
        {
            if (i < _scoreData.ScorePerBossHpPercentByPhase.Length)
            {
                score += _scoreData.ScorePerBossHpPercentByPhase[i] * 100f;
            }
        }

        // ���݂̃t�F�[�Y�ō����HP�����̃X�R�A�����Z
        if (currentPhaseIndex >= 0 && currentPhaseIndex < _scoreData.ScorePerBossHpPercentByPhase.Length)
        {
            float hpDamagedPercent = 100f - (currentPhaseHpPercent * 100f);
            score += hpDamagedPercent * _scoreData.ScorePerBossHpPercentByPhase[currentPhaseIndex];
        }

        return Mathf.FloorToInt(score);
    }
}