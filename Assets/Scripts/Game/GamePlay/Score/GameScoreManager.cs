using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GameScoreManager : MonoBehaviour
{
    // GameResultDetail�̃v���n�u�A�܂��̓v�[���Ȃǂ���擾�����C���X�^���X��z��
    [SerializeField] private GameResultDetail _gameResultDetailPrefab;

    // �X�R�A�\���̃R���e�i�ƂȂ�Transform
    [SerializeField] private Transform _scoreDetailContainer;
    [SerializeField] private UIComplexAnimator _totalScoreAnimator;

    // �A�j���[�V�����Ԃ̒x������
    private const float ANIMATION_DELAY = 0.8f;

    public void ShowScore(ScoreBreakdown scoreBreakdown)
    {
        // �����̃X�R�A�ڍׂ��N���A
        foreach (Transform child in _scoreDetailContainer)
        {
            Destroy(child.gameObject);
        }

        // �X�R�A�\���̃R���[�`�����J�n
        StartCoroutine(ShowScoreBreakdownCoroutine(scoreBreakdown));
    }

    private IEnumerator ShowScoreBreakdownCoroutine(ScoreBreakdown scoreBreakdown)
    {
        // ScoreBreakdown����L�[�ƒl�̃y�A���擾
        var scores = new Dictionary<ScoreType, int>
        {
            { ScoreType.BossDefeatBonus, scoreBreakdown.scores[ScoreType.BossDefeatBonus] },
            { ScoreType.BossDamageBonus, scoreBreakdown.scores[ScoreType.BossDamageBonus] },
            { ScoreType.HordeEnemyScore, scoreBreakdown.scores[ScoreType.HordeEnemyScore] },
            { ScoreType.TimeRemainingBonus, scoreBreakdown.scores[ScoreType.TimeRemainingBonus] },
            { ScoreType.DamageTakenPenalty, scoreBreakdown.scores[ScoreType.DamageTakenPenalty] }
        };


        // �e�X�R�A���ڂ����ɏ���
        foreach (var score in scores)
        {
            // GameResultDetail�̃C���X�^���X�𐶐�
            GameResultDetail detailInstance = Instantiate(_gameResultDetailPrefab, _scoreDetailContainer);

            // GameResultDetail��������
            detailInstance.Initialize(score.Key, score.Value);
            detailInstance.PlayAnimation();

            // �w�肳�ꂽ�Ԋu�����ҋ@
            yield return new WaitForSeconds(ANIMATION_DELAY);
        }

        _totalScoreAnimator.Show();
    }
}