using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PunchBarrierEffectManager : MonoBehaviour
{
    [Header("�ݒ�")]
    [SerializeField] private MechanicalSpiderLegSettings _settings;

    [Header("�o���A")]
    [SerializeField, Tooltip("�o���A�̃��X�g�i�����ʂ�ɐݒ�j")]
    private List<BarrierScript> _barriers = new List<BarrierScript>();

    [System.Serializable]
    public struct PunchPhaseTimings
    {
        public float moveDuration;
        public float holdDuration;
    }

    private Coroutine _punchSequenceCoroutine;

    // MechanicalSpiderLeg�������Ƃ��Ď󂯎��悤�ɕύX
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
    /// ���݂̃p���`�V�[�P���X�𑦍��ɒ�~���A�������̈ʒu�ɖ߂��܂��B
    /// </summary>
    public void StopAndReturnImmediately(MechanicalSpiderLeg mechanicalSpiderLeg)
    {
        if (_punchSequenceCoroutine != null)
        {
            StopCoroutine(_punchSequenceCoroutine);
            _punchSequenceCoroutine = null; // �R���[�`���Q�Ƃ��N���A
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

        // �Ō�̗v�f���������e�o���A��moveDuration�̍��v���v�Z
        for (int i = 0; i < _settings.PunchPhases.Count - 1; i++)
        {
            totalMoveDuration += _settings.PunchPhases[i].moveDuration;
        }

        // PunchDuration����moveDuration�̍��v�������āA�c��̎��Ԃ�holdDuration�Ɋ��蓖�Ă�
        float remainingHoldDuration = _settings.PunchDuration - totalMoveDuration - _settings.PunchLiftDuration +_settings.PunchDurationOffset;

        // �ePunchPhaseTimings�ɐV����holdDuration��ݒ� (�Ō�̃t�F�[�Y������)
        for (int i = 0; i < _settings.PunchPhases.Count - 1; i++)
        {
            var phase = _settings.PunchPhases[i];

            // �����Ŋ����ŕ�����
            float ratio = phase.moveDuration / totalMoveDuration;
            float newHoldDuration = remainingHoldDuration * ratio;

            // �X�V����holdDuration���Đݒ�
            _settings.PunchPhases[i] = new PunchPhaseTimings
            {
                moveDuration = phase.moveDuration,
                holdDuration = newHoldDuration
            };
        }

        // �U�肩�Ԃ�t�F�[�Y
        yield return StartCoroutine(HandleWindupPhase(mechanicalSpiderLeg));

        // �e�o���A�����ɏ���
        foreach (var barrier in _barriers)
        {
            int index = _barriers.IndexOf(barrier);
            yield return StartCoroutine(HandleBarrierPunch(mechanicalSpiderLeg, barrier, index));
        }

        // �ŏI�ڕW�ֈړ�
        if (_barriers.Count < _settings.PunchPhases.Count)
        {
            yield return StartCoroutine(HandleFinalPunch(mechanicalSpiderLeg, finalTarget));
        }

        // ���������猳�̈ʒu�ɖ߂�
        yield return StartCoroutine(ReturnToIdle(mechanicalSpiderLeg));

        _punchSequenceCoroutine = null;
    }


    private IEnumerator HandleWindupPhase(MechanicalSpiderLeg mechanicalSpiderLeg)
    {
        mechanicalSpiderLeg.StartPunchWindup(_settings.PunchLiftDuration);
        yield return new WaitForSeconds(_settings.PunchLiftDuration / 2);
        // �o���A�̒x���W�J�͑ҋ@���Ȃ�
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