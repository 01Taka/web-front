using System;
using System.Collections.Generic;

public class DamageTakenManager
{
    private float _damage = 0f;
    public float TakenDamage { get { return _damage; } }

    private Dictionary<int, float> _recordedDamageStarts = new Dictionary<int, float>();

    // �C�x���g�� private �ɂ��ĊO������̒��ڃA�N�Z�X�𐧌�����
    private event Action<float> OnDamageTakenEvent;

    /// <summary>
    /// �_���[�W���󂯂��Ƃ��̃R�[���o�b�N�֐���o�^���܂��B
    /// </summary>
    /// <param name="callback">�o�^����R�[���o�b�N�֐�</param>
    public void RegisterOnDamageTakenCallback(Action<float> callback)
    {
        OnDamageTakenEvent += callback;
    }

    /// <summary>
    /// �_���[�W���󂯂��Ƃ��̃R�[���o�b�N�֐����폜���܂��B
    /// </summary>
    /// <param name="callback">�폜����R�[���o�b�N�֐�</param>
    public void UnregisterOnDamageTakenCallback(Action<float> callback)
    {
        OnDamageTakenEvent -= callback;
    }

    /// <summary>
    /// �o�^����Ă��邷�ׂẴR�[���o�b�N�֐����폜���܂��B
    /// </summary>
    public void ClearAllCallbacks()
    {
        // �C�x���g�ɓo�^���ꂽ���ׂẴf���Q�[�g���������
        OnDamageTakenEvent = null;
    }

    public void TakeDamage(float damage)
    {
        if (damage < 0f) return;
        _damage += damage;

        // �C�x���g���Ăяo��
        OnDamageTakenEvent?.Invoke(damage);
    }

    public void StartRecord(int recordId)
    {
        if (_recordedDamageStarts.ContainsKey(recordId))
        {
            return;
        }
        _recordedDamageStarts[recordId] = _damage;
    }

    public float GetRecordedDamage(int recordId)
    {
        if (!_recordedDamageStarts.ContainsKey(recordId))
        {
            return 0f;
        }
        float startDamage = _recordedDamageStarts[recordId];
        return _damage - startDamage;
    }

    public void EndRecord(int recordId)
    {
        _recordedDamageStarts.Remove(recordId);
    }
}