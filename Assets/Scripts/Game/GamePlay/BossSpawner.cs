using System;
using UnityEngine;
using UnityEngine.Events;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private Transform _parent;
    [SerializeField] private Vector3 _spawnOffset; // �X�|�[���ʒu�𒲐����邽�߂̃t�B�[���h��ǉ�
    [SerializeField] private BossSettings[] _bossSettings; // �{�X�ݒ�̔z��
    [SerializeField] private HpBarManager _hpBarManager;
    [SerializeField] private PlayerManager _playerManager;

    private ConcreteBossManager _currentBossBossManager;
    public ConcreteBossManager CurrentBossBossManager => _currentBossBossManager;
    [SerializeField] private UnityEvent _onBossDestroyed = new UnityEvent();

    public bool TrySpawn(BossId id)
    {
        // �w�肳�ꂽ id �̐ݒ�� BossSettings �z�񂩂�T��
        BossSettings bossSetting = Array.Find(_bossSettings, boss => boss.id == id);

        // ��v����ݒ肪�����������m�F
        if (bossSetting != null)
        {
            Spawn(bossSetting);
            return true;
        }
        else
        {
            // ��v����ݒ肪������Ȃ������ꍇ�A�G���[���O���o��
            Debug.LogError("Error: BossSettings with id " + id.ToString() + " not found.");
            return false;
        }
    }

    void Spawn(BossSettings bossSetting)
    {
        // �e�I�u�W�F�N�g�̈ʒu�ɃI�t�Z�b�g���������ʒu�Ń{�X���C���X�^���X��
        var boss = Instantiate(bossSetting.bossInstance, _parent.position + _spawnOffset, Quaternion.identity, _parent);

        // �������ꂽ�I�u�W�F�N�g�� BaseBossManager ���t���Ă��邩�m�F
        if (boss.TryGetComponent(out ConcreteBossManager bossManager))
        {
            _hpBarManager.SetTarget(bossManager.HealthManager);
            _currentBossBossManager = bossManager;
            bossManager.Initialize(_playerManager);
        }
        else
        {
            // �����R���|�[�l���g��������Ȃ������ꍇ�Ɍx�����o��
            Debug.LogError($"Spawned boss prefab '{boss.name}' does not have a BaseBossManager component.");
        }
    }

    public void DestoryBoss()
    {
        if (_currentBossBossManager != null)
        {
            _currentBossBossManager.DestroySelf();
            _currentBossBossManager = null;
            _onBossDestroyed?.Invoke();
        }
    }

    private void OnDisable()
    {
        _onBossDestroyed.RemoveAllListeners();
    }
}