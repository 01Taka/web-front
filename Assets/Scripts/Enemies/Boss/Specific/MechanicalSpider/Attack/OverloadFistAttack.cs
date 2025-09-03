using UnityEngine;

public class OverloadFistAttack : BossAttackBase
{
    // 攻撃タイプを定義
    public override BossAttackType AttackType => BossAttackType.OverloadFist;

    [Header("攻撃に固有のパラメータ")]
    [SerializeField] private GameObject explosionPrefab;
    [SerializeField] private float explosionRadius = 5f;

    /// <summary>
    /// 攻撃実行時に呼び出される
    /// </summary>
    public override void ExecuteAttack(BossAttackContext context)
    {
        Debug.Log("OverloadFistAttack: 爆発ダメージを与えます。");

        // ポートの位置から爆発エフェクトを生成
        Vector3 explosionPosition = Vector3.zero;
        if (explosionPrefab != null)
        {
            GameObject.Instantiate(explosionPrefab, explosionPosition, Quaternion.identity);
        }

        base.ExecuteAttack(context);
    }
}