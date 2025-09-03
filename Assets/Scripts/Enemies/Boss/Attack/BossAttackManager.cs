using UnityEngine;
using System.Collections.Generic;

public class BossAttackManager
{
    private BossAttackPortsManager _portsManager;

    public void Initialize(MonoBehaviour coroutineRunner, IBossAttackLogic bossAttackLogic, BossFiringPort[] firingPorts)
    {
        // FiringPort��Enum�z������BossAttackPort�̎����𐶐����A�t�B�[���h�ϐ��ɕۑ�
        Dictionary<BossFiringPort, BossAttackPort>  portsDictionary = new Dictionary<BossFiringPort, BossAttackPort>();
        foreach (BossFiringPort portType in firingPorts)
        {
            if (!portsDictionary.ContainsKey(portType))
            {
                portsDictionary.Add(portType, new BossAttackPort(portType));
            }
        }

        _portsManager = new BossAttackPortsManager(coroutineRunner, portsDictionary, bossAttackLogic);
    }

    public void ExecuteAttack(AttackPattern pattern)
    {
        switch (pattern.FiringPortType)
        {
            case FiringPortType.All:
                StartUltimateAttack(pattern);
                break;
            case FiringPortType.Single:
                StartRandomAttackFromAvailablePort(pattern);
                break;
        }
    }

    /// <summary>
    /// �J���Ă���|�[�g���烉���_���ɒʏ�U�����J�n���܂��B
    /// </summary>
    public void StartRandomAttackFromAvailablePort(AttackPattern randomPattern)
    {
        List<BossAttackPort> availablePorts = _portsManager.GetAvailablePorts();

        if (availablePorts.Count == 0)
        {
            MyLogger.LogWarning("���p�\�ȍU���|�[�g������܂���B");
            return;
        }

        // �����_���Ɉ�̃|�[�g��I��
        BossAttackPort randomPort = availablePorts[Random.Range(0, availablePorts.Count)];

        // �I�����ꂽ�|�[�g�ɍU���J�n���w��
        StartAttack(randomPort.FiringPortType, randomPattern);
    }

    public void StartAttack(BossFiringPort portType, AttackPattern attackPattern)
    {
        _portsManager.StartAttack(portType, attackPattern);
    }

    /// <summary>
    /// ���ׂẴ|�[�g���g����Z���J�n���܂��B
    /// </summary>
    public void StartUltimateAttack(AttackPattern attackPattern)
    {
        _portsManager.StartUltimateAttack(attackPattern);
    }

    /// <summary>
    /// ���ׂĂ̍U���������I�ɃL�����Z�����܂��B
    /// </summary>
    public void CancelAllAttacks()
    {
        _portsManager.CancelAllAttacks();
    }
}