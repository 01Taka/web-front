using System.Collections;
using UnityEngine;

/// <summary>
/// �{�X�̍U���p�^�[���Ǝ��s���Ǘ�����N���X�B
/// </summary>
public class BossAttackController : MonoBehaviour
{
    private BaseBossManager _bossManager;
    private Coroutine _attackRoutine;

    private BossPhaseSettings _currentPhaseSettings;

    // �U�������s�����ǂ����������v���p�e�B
    public bool IsAttacking { get; private set; }

    public void Initialize(BaseBossManager bossManager, BossPhaseSettings phaseSettings)
    {
        _bossManager = bossManager;
        _currentPhaseSettings = phaseSettings;
    }

    public void UpdatePhaseSettings(BossPhaseSettings phaseSettings)
    {
        _currentPhaseSettings = phaseSettings;
    }

    /// <summary>
    /// �U�����[�`�����J�n����
    /// </summary>
    public void StartAttacking()
    {
        StopAttacking();
        _attackRoutine = StartCoroutine(AttackRoutine());
    }

    /// <summary>
    /// �U�����[�`�����~����
    /// </summary>
    public void StopAttacking()
    {
        if (_attackRoutine != null)
        {
            StopCoroutine(_attackRoutine);
            _attackRoutine = null;
            IsAttacking = false; // �U����~�����t���O�����Z�b�g
        }
    }

    private IEnumerator AttackRoutine()
    {
        // �ŏ��̍U���܂ł̑ҋ@����
        yield return new WaitForSeconds(Random.Range(_currentPhaseSettings.minAttackInterval, _currentPhaseSettings.maxAttackInterval));

        while (true)
        {
            IsAttacking = true; // �U���J�n���}�[�N
            AttackPattern executedAttack = PerformRandomAttack();

            float waitTime = Random.Range(_currentPhaseSettings.minAttackInterval, _currentPhaseSettings.maxAttackInterval);

            if (executedAttack != null)
            {
                waitTime += executedAttack.AttackPreparationTime + executedAttack.AttackDuration;
            }

            // �U���̎��s���I���܂ő҂�
            yield return new WaitForSeconds(executedAttack.AttackPreparationTime + executedAttack.AttackDuration);

            IsAttacking = false; // �U���I�����}�[�N

            // ���̍U���܂ł̑ҋ@����
            yield return new WaitForSeconds(waitTime);
        }
    }

    public AttackPattern PerformRandomAttack()
    {
        AttackPattern[] patterns = _currentPhaseSettings.attackPatterns;

        if (patterns == null || patterns.Length == 0)
        {
            Debug.LogWarning("No attack patterns found for the current phase.");
            return null;
        }

        int randomIndex = Random.Range(0, patterns.Length);
        AttackPattern selectedPattern = patterns[randomIndex];
        _bossManager.ExecuteAttack(selectedPattern);
        return selectedPattern;
    }
}