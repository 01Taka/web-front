using UnityEngine;
using System.Collections.Generic;

public class OverloadFistAttack : BossAttackBase
{
    // 攻撃タイプを定義
    public override BossAttackType AttackType => BossAttackType.OverloadFist;

    [Header("Overload Fist Specific")]
    [SerializeField] private MechanicalSpiderLegPart[] _legParts; // インスペクターで設定

    // 爆発エフェクトとサウンドエフェクト
    [SerializeField] private ExplosionType _explosionType; // 爆発エフェクトの種類
    [SerializeField] protected float _explosionSize;
    [SerializeField] private AudioClip _explosionSound; // 爆発のAudioClip

    // 攻撃ごとにインクリメントされる静的変数
    private static int _recordIdCounter = 0;

    // 攻撃開始時に生成するレコードIDを管理する辞書
    private Dictionary<int, int> _recordIds;

    private void Awake()
    {
        _recordIds = new Dictionary<int, int>();
    }

    /// <summary>
    /// 攻撃準備開始時の処理。
    /// レコードを開始し、ダメージコールバックを登録します。
    /// </summary>
    public override void OnBeginPreparation(BossAttackContext context)
    {
        // 攻撃ごとに一意のIDを生成
        _recordIdCounter++;

        int index = MechanicalSpiderUtils.ConvertToPortToIndex(context.SinglePort);
        MechanicalSpiderLegPart part = _legParts[index];
        _recordIds[index] = _recordIdCounter;

        // レコードを開始し、コールバックを登録
        part.StartRecordAndRegisterCallback(_recordIdCounter, (amount) => OnDamageTakenDuringAttack(context));

        TargetMarkManager.Instance.StartLockOn(part.TargetMarkPosition);

        // 親クラスの処理を実行
        base.OnBeginPreparation(context);
    }

    /// <summary>
    /// ダメージを受けたときに呼び出されるコールバックメソッド。
    /// </summary>
    private void OnDamageTakenDuringAttack(BossAttackContext context)
    {
        // レコードIDと対応する脚部を取得
        if (TryGetRecordInfo(context, out int recordId, out MechanicalSpiderLegPart part))
        {
            float recordedDamage = part.GetRecordedDamage(recordId);

            // 閾値を超えたか確認
            if (recordedDamage > context.Pattern.CancelDamageThreshold)
            {
                // 攻撃をキャンセル
                _callbacks.CancelAttackOnPort(context.SinglePort.FiringPortType);
            }
        }
    }

    /// <summary>
    /// 攻撃がキャンセルされたときの処理。
    /// レコードを終了し、エフェクトとSEを再生します。
    /// </summary>
    public override void OnCanceledAttack(BossAttackContext context)
    {
        CleanUpAttack(context);

        // 爆発エフェクトとSEを再生
        SpawnExplosion(context.SinglePort);

        base.OnCanceledAttack(context);
    }

    /// <summary>
    /// 攻撃実行時の処理。
    /// レコードとコールバックを終了します。
    /// </summary>
    public override void ExecuteAttack(BossAttackContext context)
    {
        CleanUpAttack(context);
        base.ExecuteAttack(context);
    }

    /// <summary>
    /// 攻撃終了時のクリーンアップ処理を共通化
    /// </summary>
    /// <param name="context"></param>
    private void CleanUpAttack(BossAttackContext context)
    {
        TargetMarkManager.Instance.ReleaseLockOn();

        if (TryGetRecordInfo(context, out int recordId, out MechanicalSpiderLegPart part))
        {
            // レコードを終了し、コールバックを解除
            part?.EndRecordAndUnregisterCallback(recordId);

            // 辞書からレコードIDを削除
            int index = MechanicalSpiderUtils.ConvertToPortToIndex(context.SinglePort);
            _recordIds.Remove(index);
        }
    }

    /// <summary>
    /// コンテキストからレコードIDと対応する脚部を安全に取得します。
    /// </summary>
    /// <param name="context">ボス攻撃コンテキスト</param>
    /// <param name="recordId">取得したレコードID</param>
    /// <param name="part">取得した脚部</param>
    /// <returns>情報の取得に成功したかどうか</returns>
    private bool TryGetRecordInfo(BossAttackContext context, out int recordId, out MechanicalSpiderLegPart part)
    {
        recordId = -1;
        part = null;
        int index = MechanicalSpiderUtils.ConvertToPortToIndex(context.SinglePort);

        if (_recordIds.TryGetValue(index, out recordId))
        {
            part = _legParts[index];
            return true;
        }

        return false;
    }

    /// <summary>
    /// 爆発エフェクトとSEを生成・再生します。
    /// </summary>
    /// <param name="port">攻撃ポート</param>
    private void SpawnExplosion(BossAttackPort port)
    {
        int index = MechanicalSpiderUtils.ConvertToPortToIndex(port);
        Vector3 spawnPosition = _legParts[index].transform.position;
        ExplosionEffectPoolManager.Instance.PlayExplosion(spawnPosition, _explosionSize, _explosionType);
        SoundManager.Instance.PlayEffect(_explosionSound);
    }
}