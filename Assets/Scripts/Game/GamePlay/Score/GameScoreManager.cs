using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameScoreManager : MonoBehaviour
{
    // GameResultDetailのプレハブ、またはプールなどから取得したインスタンスを想定
    [SerializeField] private GameResultDetail _gameResultDetailPrefab;

    // スコア表示のコンテナとなるTransform
    [SerializeField] private Transform _scoreDetailContainer;
    [SerializeField] private UIComplexAnimator _totalScoreAnimator;

    // アニメーション間の遅延時間
    private const float ANIMATION_DELAY = 0.8f;

    public void ShowScore(ScoreBreakdown scoreBreakdown)
    {
        // 既存のスコア詳細をクリア
        foreach (Transform child in _scoreDetailContainer)
        {
            Destroy(child.gameObject);
        }

        // スコア表示のコルーチンを開始
        StartCoroutine(ShowScoreBreakdownCoroutine(scoreBreakdown));
    }

    private IEnumerator ShowScoreBreakdownCoroutine(ScoreBreakdown scoreBreakdown)
    {
        // ScoreBreakdownからキーと値のペアを取得
        var scores = new Dictionary<ScoreType, int>
        {
            { ScoreType.BossDefeatBonus, scoreBreakdown.scores[ScoreType.BossDefeatBonus] },
            { ScoreType.BossDamageBonus, scoreBreakdown.scores[ScoreType.BossDamageBonus] },
            { ScoreType.HordeEnemyScore, scoreBreakdown.scores[ScoreType.HordeEnemyScore] },
            { ScoreType.TimeRemainingBonus, scoreBreakdown.scores[ScoreType.TimeRemainingBonus] },
            { ScoreType.DamageTakenPenalty, scoreBreakdown.scores[ScoreType.DamageTakenPenalty] }
        };


        // 各スコア項目を順に処理
        foreach (var score in scores)
        {
            // GameResultDetailのインスタンスを生成
            GameResultDetail detailInstance = Instantiate(_gameResultDetailPrefab, _scoreDetailContainer);

            // GameResultDetailを初期化
            detailInstance.Initialize(score.Key, score.Value);
            detailInstance.PlayAnimation();

            // 指定された間隔だけ待機
            yield return new WaitForSeconds(ANIMATION_DELAY);
        }

        _totalScoreAnimator.Show();
    }
}