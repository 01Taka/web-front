using UnityEngine;
using System.Collections.Generic;

public class BossAttackManager
{
    private BossAttackPortsManager _portsManager;

    public void Initialize(MonoBehaviour coroutineRunner, IBossAttackLogic bossAttackLogic, BossFiringPort[] firingPorts)
    {
        // FiringPortのEnum配列を基にBossAttackPortの辞書を生成し、フィールド変数に保存
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
    /// 開いているポートからランダムに通常攻撃を開始します。
    /// </summary>
    public void StartRandomAttackFromAvailablePort(AttackPattern randomPattern)
    {
        List<BossAttackPort> availablePorts = _portsManager.GetAvailablePorts();

        if (availablePorts.Count == 0)
        {
            MyLogger.LogWarning("利用可能な攻撃ポートがありません。");
            return;
        }

        // ランダムに一つのポートを選択
        BossAttackPort randomPort = availablePorts[Random.Range(0, availablePorts.Count)];

        // 選択されたポートに攻撃開始を指示
        StartAttack(randomPort.FiringPortType, randomPattern);
    }

    public void StartAttack(BossFiringPort portType, AttackPattern attackPattern)
    {
        _portsManager.StartAttack(portType, attackPattern);
    }

    /// <summary>
    /// すべてのポートを使う大技を開始します。
    /// </summary>
    public void StartUltimateAttack(AttackPattern attackPattern)
    {
        _portsManager.StartUltimateAttack(attackPattern);
    }

    /// <summary>
    /// すべての攻撃を強制的にキャンセルします。
    /// </summary>
    public void CancelAllAttacks()
    {
        _portsManager.CancelAllAttacks();
    }
}