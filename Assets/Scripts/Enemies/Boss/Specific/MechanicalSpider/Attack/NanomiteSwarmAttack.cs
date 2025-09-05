using System.Collections.Generic;
using UnityEngine;

public class NanomiteSwarmAttack : BossAttackBase
{
    public override BossAttackType AttackType => BossAttackType.NanomiteSwarm;

    [Header("ボム生成設定")]
    [Tooltip("生成する爆弾の数")]
    [SerializeField]
    private int _numberOfBoms = 5;
    [Tooltip("爆弾を生成する範囲の半径")]
    [SerializeField]
    private float _spawnRadius = 5f;
    [Tooltip("爆弾のPrefab")]
    [SerializeField]
    private GameObject _bomPrefab;
    [SerializeField]
    private float _bomSize = 1f;
    [Tooltip("爆弾が爆発するまでの時間")]
    [SerializeField]
    private float _explosionDuration = 3f;
    [SerializeField]
    private Transform[] _bomParents;
    [SerializeField] private Transform _bomMoveTarget;

    public override void ExecuteAttack(BossAttackContext context)
    {
        int parentIndex = MechanicalSpiderUtils.ConvertToPortToIndex(context.SinglePort);
        if (parentIndex < 0 || parentIndex >= _bomParents.Length || _bomPrefab == null)
        {
            Debug.LogWarning("Invalid parent index, missing parent transform, or missing bom prefab. Cannot spawn bombs.");
            return;
        }

        Transform parentTransform = _bomParents[parentIndex];

        // 指定された数の爆弾を生成
        for (int i = 0; i < _numberOfBoms; i++)
        {
            SpawnBom(parentTransform, context.BossManager.Position, context);
        }

        // アニメーションは自動で終わり、ダメージは爆弾が与えるのでbaseは実行しない
    }

    /// <summary>
    /// 単一の爆弾を生成します。
    /// </summary>
    /// <param name="parentTransform">爆弾の生成位置の親となるTransform</param>
    /// <param name="bossPosition">ボスの位置</param>
    private void SpawnBom(Transform parentTransform, Vector3 bossPosition, BossAttackContext context)
    {
        // ランダムな位置を計算
        Vector2 randomPos = Random.insideUnitCircle * _spawnRadius;
        Vector3 spawnPosition = parentTransform.position + new Vector3(randomPos.x, randomPos.y, 0);

        // 爆弾をインスタンス化
        GameObject bomInstance = Instantiate(_bomPrefab, spawnPosition, Quaternion.identity, parentTransform);

        bomInstance.transform.localScale = Vector3.one * _bomSize;

        // MechanicalSpiderBomコンポーネントを取得し、アクティベート
        if (bomInstance.TryGetComponent<MechanicalSpiderBom>(out var bomComponent))
        {
            bomComponent.Activate(_explosionDuration, context.Pattern.CancelDamageThreshold, _bomMoveTarget.position,  bossPosition, () => DamagePlayer(context), () => DamageBoss(context));
        }
        else
        {
            Debug.LogWarning($"The prefab '{_bomPrefab.name}' is missing the 'MechanicalSpiderBom' component.");
        }
    }
}