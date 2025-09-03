using UnityEngine;
using System;

[Serializable]
public class ScoreBreakdown
{
    public int BossDefeatBonus = 0;
    public int BossDamageBonus = 0;
    public int HordeEnemyScore = 0;
    public int TimeRemainingBonus = 0;
    public int DamageTakenPenalty = 0;

    public int TotalScore => BossDefeatBonus + BossDamageBonus + HordeEnemyScore + TimeRemainingBonus + DamageTakenPenalty;
}

public class ScoreManager : MonoBehaviour
{
    [SerializeField] private ScoreData _scoreData;

    private int _hordeEnemyKilledCount = 0;
    private int _totalDamageTaken = 0;
    private bool _isBossDefeated = false;

    public void Initialize()
    {
        _hordeEnemyKilledCount = 0;
        _totalDamageTaken = 0;
        _isBossDefeated = false;
    }

    public void AddDamageTaken(int damage)
    {
        _totalDamageTaken += damage;
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

    public ScoreBreakdown CalculateFinalScore(float remainingTime, int bossPhaseIndex, float currentPhaseHpPercent)
    {
        var breakdown = new ScoreBreakdown();

        // ボス撃破ボーナス
        if (_isBossDefeated)
        {
            breakdown.BossDefeatBonus = _scoreData.ScoreOnBossDefeat;
        }

        // ボスHP削りボーナス
        breakdown.BossDamageBonus = GetBossHealthScore(bossPhaseIndex, currentPhaseHpPercent);

        // 雑魚敵撃破スコア
        breakdown.HordeEnemyScore = _hordeEnemyKilledCount * _scoreData.ScorePerHordeEnemyDefeat;

        // 残り時間ボーナス
        if (remainingTime > 0)
        {
            breakdown.TimeRemainingBonus = Mathf.FloorToInt(remainingTime) * _scoreData.ScorePerSecondRemaining;
        }

        // ダメージ減点
        breakdown.DamageTakenPenalty = _totalDamageTaken * _scoreData.ScorePerDamageTaken;

        // 合計スコアがマイナスにならないように調整
        if (breakdown.TotalScore < 0)
        {
            breakdown.DamageTakenPenalty = -breakdown.TotalScore;
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

        // 完了したフェーズのスコアを加算
        for (int i = 0; i < currentPhaseIndex; i++)
        {
            if (i < _scoreData.ScorePerBossHpPercentByPhase.Length)
            {
                score += _scoreData.ScorePerBossHpPercentByPhase[i] * 100f;
            }
        }

        // 現在のフェーズで削ったHP割合のスコアを加算
        if (currentPhaseIndex >= 0 && currentPhaseIndex < _scoreData.ScorePerBossHpPercentByPhase.Length)
        {
            float hpDamagedPercent = 100f - (currentPhaseHpPercent * 100f);
            score += hpDamagedPercent * _scoreData.ScorePerBossHpPercentByPhase[currentPhaseIndex];
        }

        return Mathf.FloorToInt(score);
    }
}