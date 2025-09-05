using System.Collections.Generic;
using UnityEngine;

public class NanomiteSwarmAttack : BossAttackBase
{
    public override BossAttackType AttackType => BossAttackType.NanomiteSwarm;

    [Header("�{�������ݒ�")]
    [Tooltip("�������锚�e�̐�")]
    [SerializeField]
    private int _numberOfBoms = 5;
    [Tooltip("���e�𐶐�����͈͂̔��a")]
    [SerializeField]
    private float _spawnRadius = 5f;
    [Tooltip("���e��Prefab")]
    [SerializeField]
    private GameObject _bomPrefab;
    [SerializeField]
    private float _bomSize = 1f;
    [Tooltip("���e����������܂ł̎���")]
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

        // �w�肳�ꂽ���̔��e�𐶐�
        for (int i = 0; i < _numberOfBoms; i++)
        {
            SpawnBom(parentTransform, context.BossManager.Position, context);
        }

        // �A�j���[�V�����͎����ŏI���A�_���[�W�͔��e���^����̂�base�͎��s���Ȃ�
    }

    /// <summary>
    /// �P��̔��e�𐶐����܂��B
    /// </summary>
    /// <param name="parentTransform">���e�̐����ʒu�̐e�ƂȂ�Transform</param>
    /// <param name="bossPosition">�{�X�̈ʒu</param>
    private void SpawnBom(Transform parentTransform, Vector3 bossPosition, BossAttackContext context)
    {
        // �����_���Ȉʒu���v�Z
        Vector2 randomPos = Random.insideUnitCircle * _spawnRadius;
        Vector3 spawnPosition = parentTransform.position + new Vector3(randomPos.x, randomPos.y, 0);

        // ���e���C���X�^���X��
        GameObject bomInstance = Instantiate(_bomPrefab, spawnPosition, Quaternion.identity, parentTransform);

        bomInstance.transform.localScale = Vector3.one * _bomSize;

        // MechanicalSpiderBom�R���|�[�l���g���擾���A�A�N�e�B�x�[�g
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