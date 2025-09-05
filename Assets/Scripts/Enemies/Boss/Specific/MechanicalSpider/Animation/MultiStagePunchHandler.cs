using DG.Tweening;
using UnityEngine;

public class MultiStagePunchHandler
{
    private enum State { Idle, Moving, Holding, Returning }

    private State currentState = State.Idle;
    private Sequence currentSequence;

    private readonly MechanicalSpiderLeg leg;
    private bool _isShaking = false;

    public MultiStagePunchHandler(MechanicalSpiderLeg leg)
    {
        this.leg = leg;
    }

    /// <summary>
    /// ����U�肩�Ԃ铮�����J�n���܂��B
    /// </summary>
    /// <param name="target">�U�肩�Ԃ�̖ڕW�ʒu</param>
    /// <param name="duration">�U�肩�Ԃ�ɂ����鎞��</param>
    public void Windup(Vector3 target, float duration)
    {
        if (currentState != State.Idle) return;

        // DOTween�ɂ��ړ��A�j���[�V����
        currentSequence?.Kill();  // �����̃V�[�P���X������΃L�����Z��

        currentSequence = DOTween.Sequence()
            .Append(DOTween.To(() => leg.GetIKTargetLocalPosition(),
                               pos => leg.SetIKTargetLocalPosition(pos),
                               target,
                               duration))  // �ڕW�ʒu�ֈړ�
            .OnStart(() => currentState = State.Moving)
            .OnComplete(() =>
            {
                currentState = State.Holding;  // �ړ�������AHolding��Ԃ�
            });
    }

    /// <summary>
    /// ���̖ڕW�ʒu�ֈړ����J�n���܂��B
    /// </summary>
    /// <param name="target">���̖ڕW�ʒu</param>
    /// <param name="duration">�ړ��ɂ����鎞��</param>
    public void MoveToTarget(Vector3 target, float duration)
    {
        if (currentState != State.Holding) return; // �ҋ@��Ԃ���̂݊J�n

        // DOTween�ɂ��ړ��A�j���[�V����
        currentSequence?.Kill();  // �����̃V�[�P���X������΃L�����Z��

        currentSequence = DOTween.Sequence()
            .Append(DOTween.To(() => leg.GetIKTargetLocalPosition(),
                               pos => leg.SetIKTargetLocalPosition(pos),
                               target,
                               duration))  // �ڕW�ʒu�ֈړ�
            .OnStart(() => currentState = State.Moving)
            .OnComplete(() =>
            {
                currentState = State.Holding;  // �ړ�������AHolding��Ԃ�
            });
    }

    /// <summary>
    /// ����̍��W�Řr��U��������A�j���[�V�������J�n���܂��B
    /// </summary>
    /// <param name="shakePosition">�U���̒��S�ƂȂ郏�[���h���W</param>
    /// <param name="duration">�U�������鎞��</param>
    /// <param name="strength">�U���̋���</param>
    /// <param name="vibrato">�U���ׂ̍����i�p�x�j</param>
    public void StartShake(Vector3 shakePosition, float duration, float strength = 0.1f, int vibrato = 20)
    {
        // �z�[���h���̂݌Ăяo����
        if (currentState != State.Holding || _isShaking) return;

        // �����̃V�[�P���X���~
        currentSequence?.Kill(true);

        Vector3 localShakePosition = leg.GetIKTargetParent().InverseTransformPoint(shakePosition);

        currentSequence = DOTween.Sequence()
            .Append(DOTween.Shake(() => leg.GetIKTargetLocalPosition(),
                                  pos => leg.SetIKTargetLocalPosition(pos),
                                  duration,
                                  strength,
                                  vibrato,
                                  90f, // �����_���ȕ�����90�x�ȓ��ɂ���
                                  false))
            .OnStart(() => _isShaking = true)
            .OnComplete(() =>
            {
                // �U��������A���̒��S�ʒu�ɖ߂�
                leg.SetIKTargetLocalPosition(localShakePosition);
                _isShaking = false;
            });
    }

    /// <summary>
    /// ���̃x�[�X�ʒu�ɖ߂铮�����J�n���܂��B
    /// </summary>
    /// <param name="duration">�߂�̂ɂ����鎞��</param>
    public void ReturnToIdle(float duration)
    {
        // �U�肩�Ԃ��p���`������̏�Ԃ���J�n
        if (currentState != State.Holding && currentState != State.Moving) return;

        // DOTween�ɂ��ړ��A�j���[�V����
        currentSequence?.Kill();  // �����̃V�[�P���X������΃L�����Z��

        currentSequence = DOTween.Sequence()
            .Append(DOTween.To(() => leg.GetIKTargetLocalPosition(),
                               pos => leg.SetIKTargetLocalPosition(pos),
                               leg.BasePosition,
                               duration))  // ���̈ʒu�ɖ߂�
            .OnStart(() => currentState = State.Returning)
            .OnComplete(() =>
            {
                currentState = State.Idle;  // �߂�����Idle��Ԃ�
            });
    }

    /// <summary>
    /// ���݂̈ړ��������������ǂ�����Ԃ��܂��B
    /// </summary>
    public bool IsHolding()
    {
        return currentState == State.Holding;
    }

    /// <summary>
    /// �����A�C�h����Ԃɖ߂������ǂ�����Ԃ��܂��B
    /// </summary>
    public bool IsComplete()
    {
        return currentState == State.Idle;
    }
}
