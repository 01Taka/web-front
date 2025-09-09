using System;
using System.Collections.Generic;
using UnityEngine;

public class MechanicalSpiderLegPart : BossPart, IDamageable
{
    [SerializeField] private Transform _targetMarkPosition;
    public Transform TargetMarkPosition => _targetMarkPosition;

    private DamageTakenManager _damageManager;

    // �����̃��R�[�hID�ƃR�[���o�b�N���Ǘ����邽�߂̎���
    private readonly Dictionary<int, Action<float>> _recordedCallbacks = new Dictionary<int, Action<float>>();

    protected override void Awake()
    {
        _damageManager = new DamageTakenManager();
        base.Awake();
    }

    private void OnDestroy()
    {
        // �j�����ɂ��ׂẴ��R�[�h�ƃR�[���o�b�N�����S�ɏI��������
        if (_damageManager != null)
        {
            List<int> recordIdsToCleanup = new List<int>(_recordedCallbacks.Keys);
            foreach (int recordId in recordIdsToCleanup)
            {
                EndRecordAndUnregisterCallback(recordId);
            }
        }
    }

    public override void TakeDamage(float amount)
    {
        _damageManager.TakeDamage(amount);
        base.TakeDamage(amount);
    }

    /// <summary>
    /// �w�肳�ꂽID�Ń_���[�W�L�^���J�n���A�_���[�W���󂯂��Ƃ��̃R�[���o�b�N��o�^���܂��B
    /// ���ɓ���ID���o�^����Ă���ꍇ�́A�������܂���B
    /// </summary>
    /// <param name="recordId">�J�n����L�^ID</param>
    /// <param name="callback">�o�^����R�[���o�b�N�֐�</param>
    public void StartRecordAndRegisterCallback(int recordId, Action<float> callback)
    {
        // ���ɓ���ID�̃��R�[�h���o�^����Ă���ꍇ�́A�������Ȃ�
        if (_recordedCallbacks.ContainsKey(recordId))
        {
            return;
        }

        _damageManager.StartRecord(recordId);
        _damageManager.RegisterOnDamageTakenCallback(callback);
        _recordedCallbacks.Add(recordId, callback);
    }

    /// <summary>
    /// �w�肳�ꂽID�̃_���[�W�L�^���I�����A�Ή�����R�[���o�b�N���������āA���Ԓ��Ɏ󂯂��_���[�W��Ԃ��܂��B
    /// </summary>
    /// <param name="recordId">�I������L�^ID</param>
    /// <returns>�L�^���Ԓ��Ɏ󂯂����_���[�W�ʁB�w��ID�����݂��Ȃ��ꍇ��0�B</returns>
    public float EndRecordAndUnregisterCallback(int recordId)
    {
        // �w�肳�ꂽID�����݂��邩�m�F
        if (!_recordedCallbacks.TryGetValue(recordId, out Action<float> callback))
        {
            return 0f;
        }

        // �_���[�W���v�Z���A���R�[�h���I������
        float recordedDamage = _damageManager.GetRecordedDamage(recordId);
        _damageManager.EndRecord(recordId);

        // �R�[���o�b�N����������
        if (callback != null)
        {
            _damageManager.UnregisterOnDamageTakenCallback(callback);
        }

        // ��������G���g�����폜����
        _recordedCallbacks.Remove(recordId);

        return recordedDamage;
    }

    public float GetRecordedDamage(int recordId)
    {
        // ���̃��\�b�h�́A���R�[�h���I���������Ƀ_���[�W���擾���邽�߂Ɏg�p�����
        return _damageManager.GetRecordedDamage(recordId);
    }
}