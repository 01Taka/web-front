using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class NanomiteSwarmAttack : BossAttackBase
{
    public override BossAttackType AttackType => BossAttackType.NanomiteSwarm;

    [SerializeField]
    private MechanicalSpiderBomSettings _settings;

    [SerializeField]
    private Transform[] _bomParents;
    [SerializeField] private Transform _bomMoveTarget;

    private BomPoolManager _poolManager;
    private readonly List<MechanicalSpiderBom> _spawnedBombs = new List<MechanicalSpiderBom>();
    private DefaultBomMovement _bomMovement = new DefaultBomMovement();
    private BomTargetMarker _currentMarker;

    private void Awake()
    {
        _poolManager = GetComponentInParent<BomPoolManager>();
        GameObject targetMarkerPositionObj = new GameObject("BomTargetMarkerPosition");
        _currentMarker = targetMarkerPositionObj.AddComponent<BomTargetMarker>();
    }

    private void OnDisable()
    {
        // �U�������f���ꂽ�ꍇ�A�c���Ă��邷�ׂẴ{�����v�[���ɖ߂�
        // ����ɂ��A�Q�Ƃ��N���A���A�R���[�`������~�����
        foreach (var bom in _spawnedBombs)
        {
            bom.ReturnToPool();
        }
        _spawnedBombs.Clear();

        if (_currentMarker != null)
        {
            _currentMarker.ReleaseMarker();
        }
    }

    public override void ExecuteAttack(BossAttackContext context)
    {
        int parentIndex = MechanicalSpiderUtils.ConvertToPortToIndex(context.SinglePort);
        if (parentIndex < 0 || parentIndex >= _bomParents.Length)
        {
            Debug.LogWarning("Invalid parent index, missing parent transform, or missing bom prefab. Cannot spawn bombs.");
            return;
        }

        Transform parentTransform = _bomParents[parentIndex];

        // �O�̂��߁A�V�����U���J�n���Ƀ��X�g���N���A
        _spawnedBombs.Clear();

        _currentMarker.Initialize(parentTransform.position, _bomMoveTarget, _settings.SeekSpeed, _bomMovement);

        for (int i = 0; i < _settings.NumberOfBoms; i++)
        {
            SpawnBom(parentTransform, context.BossManager.Position, context);
        }
    }

    private void SpawnBom(Transform parentTransform, Vector3 bossPosition, BossAttackContext context)
    {
        Vector2 randomPos = Random.insideUnitCircle * _settings.SpawnRadius;
        Vector3 spawnPosition = parentTransform.position + new Vector3(randomPos.x, randomPos.y, 0);

        MechanicalSpiderBom bomInstance = _poolManager.Get<MechanicalSpiderBom>(_settings.NumberOfBoms);
        bomInstance.transform.position = spawnPosition;

        _spawnedBombs.Add(bomInstance);

        // �V�����{���̃C���X�^���X�Ɉړ����W�b�N�ƃR�[���o�b�N��n��
        bomInstance.Activate(_settings, context.Pattern.CancelDamageThreshold, _bomMoveTarget.position, bossPosition,
            (state) => OnBombStateChanged(state, bomInstance, context), _bomMovement);
    }

    private void OnBombStateChanged(BombState state, MechanicalSpiderBom instance, BossAttackContext context)
    {
        // ���X�g����{�������S�ɍ폜
        if (_spawnedBombs.Contains(instance))
        {
            _spawnedBombs.Remove(instance);
        }

        if (state == BombState.DestroyedByHealth)
        {
            DamageBoss(context);
        }
        else if (state == BombState.ExplodedByTimer)
        {
            DamagePlayer(context);
        }

        if (_spawnedBombs.Count == 0)
        {
            _currentMarker.ReleaseMarker();
        }
    }
}
