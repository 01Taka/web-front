using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PunchBarrierEffectManager : MonoBehaviour
{
    [Header("設定")]
    [SerializeField] private MechanicalSpiderLegSettings _settings;

    [Header("バリア")]
    [SerializeField, Tooltip("バリアのリスト（順序通りに設定）")]
    private List<BarrierScript> _barriers = new List<BarrierScript>();

    [System.Serializable]
    public struct PunchPhaseTimings
    {
        public float moveDuration;
        public float holdDuration;
    }

    private Coroutine _punchSequenceCoroutine;

    // MechanicalSpiderLegを引数として受け取るように変更
    public void StartPunchSequence(MechanicalSpiderLeg mechanicalSpiderLeg, Vector3 finalTargetPosition)
    {
        if (mechanicalSpiderLeg == null)
        {
            Debug.LogError("MechanicalSpiderLeg is not assigned.");
            return;
        }

        if (_punchSequenceCoroutine != null)
        {
            StopCoroutine(_punchSequenceCoroutine);
        }

        _punchSequenceCoroutine = StartCoroutine(PunchSequence(mechanicalSpiderLeg, finalTargetPosition));
    }

    /// <summary>
    /// 現在のパンチシーケンスを即座に停止し、足を元の位置に戻します。
    /// </summary>
    public void StopAndReturnImmediately(MechanicalSpiderLeg mechanicalSpiderLeg)
    {
        if (_punchSequenceCoroutine != null)
        {
            StopCoroutine(_punchSequenceCoroutine);
            _punchSequenceCoroutine = null; // コルーチン参照をクリア
        }

        if (mechanicalSpiderLeg != null)
        {
            mechanicalSpiderLeg.ReturnToIdleFromPunch(_settings.PunchReturningDuration);
        }
        StopAllCoroutines();
        StartCoroutine(RemoveBarriers());
    }

    private IEnumerator RemoveBarriers()
    {
        foreach (var barrier in _barriers)
        {
            yield return new WaitForSeconds(_settings.BarrierDeploymentDelay);
            barrier.DestroyBarrier(false);
        }
    }

    private IEnumerator DeployBarriers()
    {
        foreach (var barrier in _barriers)
        {
            barrier.ResetBarrier();
            yield return new WaitForSeconds(_settings.BarrierDeploymentDelay);
        }
    }

    private IEnumerator PunchSequence(MechanicalSpiderLeg mechanicalSpiderLeg, Vector3 finalTarget)
    {
        float totalMoveDuration = 0f;

        // 最後の要素を除いた各バリアのmoveDurationの合計を計算
        for (int i = 0; i < _settings.PunchPhases.Count - 1; i++)
        {
            totalMoveDuration += _settings.PunchPhases[i].moveDuration;
        }

        // PunchDurationからmoveDurationの合計を引いて、残りの時間をholdDurationに割り当てる
        float remainingHoldDuration = _settings.PunchDuration - totalMoveDuration - _settings.PunchLiftDuration +_settings.PunchDurationOffset;

        // 各PunchPhaseTimingsに新しいholdDurationを設定 (最後のフェーズを除く)
        for (int i = 0; i < _settings.PunchPhases.Count - 1; i++)
        {
            var phase = _settings.PunchPhases[i];

            // ここで割合で分ける
            float ratio = phase.moveDuration / totalMoveDuration;
            float newHoldDuration = remainingHoldDuration * ratio;

            // 更新したholdDurationを再設定
            _settings.PunchPhases[i] = new PunchPhaseTimings
            {
                moveDuration = phase.moveDuration,
                holdDuration = newHoldDuration
            };
        }

        // 振りかぶりフェーズ
        yield return StartCoroutine(HandleWindupPhase(mechanicalSpiderLeg));

        // 各バリアを順に処理
        foreach (var barrier in _barriers)
        {
            int index = _barriers.IndexOf(barrier);
            yield return StartCoroutine(HandleBarrierPunch(mechanicalSpiderLeg, barrier, index));
        }

        // 最終目標へ移動
        if (_barriers.Count < _settings.PunchPhases.Count)
        {
            yield return StartCoroutine(HandleFinalPunch(mechanicalSpiderLeg, finalTarget));
        }

        // 完了したら元の位置に戻る
        yield return StartCoroutine(ReturnToIdle(mechanicalSpiderLeg));

        _punchSequenceCoroutine = null;
    }


    private IEnumerator HandleWindupPhase(MechanicalSpiderLeg mechanicalSpiderLeg)
    {
        mechanicalSpiderLeg.StartPunchWindup(_settings.PunchLiftDuration);
        yield return new WaitForSeconds(_settings.PunchLiftDuration / 2);
        // バリアの遅延展開は待機しない
        StartCoroutine(DeployBarriers());
        yield return new WaitForSeconds(_settings.PunchLiftDuration / 2);
        TryPlaySound(_settings.PunchStartClip);
    }

    private IEnumerator HandleBarrierPunch(MechanicalSpiderLeg mechanicalSpiderLeg, BarrierScript barrier, int index)
    {
        Vector3 barrierPos = barrier.transform.position;
        Vector3 holdPos = barrierPos + _settings.PunchHoldOffset;
        float moveDuration = _settings.PunchPhases[index].moveDuration;

        mechanicalSpiderLeg.StartPunchingSequence(holdPos, moveDuration);
        yield return new WaitUntil(() => mechanicalSpiderLeg.IsPuncnHolding());
        TryPlaySound(_settings.PunchImpactToBarrierClip);

        float holdDuration = _settings.PunchPhases[index].holdDuration;
        float timePerStep = holdDuration / barrier.ColorStepsCount;

        mechanicalSpiderLeg.StartPunchShaking(holdDuration);

        for (int j = 0; j < barrier.ColorStepsCount; j++)
        {
            yield return new WaitForSeconds(timePerStep);
            barrier.NextStep();
        }
    }

    private IEnumerator HandleFinalPunch(MechanicalSpiderLeg mechanicalSpiderLeg, Vector3 finalTarget)
    {
        PunchPhaseTimings finalMovePhase = _settings.PunchPhases[_settings.PunchPhases.Count - 1];
        mechanicalSpiderLeg.StartPunchingSequence(finalTarget, finalMovePhase.moveDuration);
        yield return new WaitUntil(() => mechanicalSpiderLeg.IsPuncnHolding());
        TryPlaySound(_settings.PunchImpactClip);
        yield return new WaitForSeconds(finalMovePhase.holdDuration);
    }

    private IEnumerator ReturnToIdle(MechanicalSpiderLeg mechanicalSpiderLeg)
    {
        mechanicalSpiderLeg.ReturnToIdleFromPunch(_settings.PunchReturningDuration);
        yield return new WaitUntil(() => mechanicalSpiderLeg.IsPunchComplete());
    }

    /// <summary>
    /// Plays an audio clip if the SoundManager instance is available.
    /// </summary>
    private void TryPlaySound(AudioClip clip)
    {
        if (SoundManager.Instance != null && clip != null)
        {
            SoundManager.Instance.PlayEffect(clip);
        }
    }
}