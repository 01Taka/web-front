using System;
using UnityEngine;
using UnityEngine.Events;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private Transform _parent;
    [SerializeField] private Vector3 _spawnOffset;
    [SerializeField] private BossSettings[] _bossSettings;
    [SerializeField] private HpBarManager _hpBarManager;
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private ScreenShake _screenShake;

    private ConcreteBossManager _currentBossBossManager;
    public ConcreteBossManager CurrentBossBossManager => _currentBossBossManager;
    [SerializeField] private UnityEvent _onBossDestroyed = new UnityEvent();

    /// <summary>
    /// �w�肳�ꂽID�̃{�X�𐶐�����
    /// </summary>
    /// <param name="id">��������{�X��ID</param>
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
            Debug.LogError("Error: BossSettings with id " + id.ToString() + " not found.");
            return false;
        }
    }

    private void Spawn(BossSettings bossSetting)
    {
        // �e�I�u�W�F�N�g�̈ʒu�ɃI�t�Z�b�g���������ʒu�Ń{�X���C���X�^���X��
        Vector3 spawnPosition = _parent.position + _spawnOffset;
        var boss = Instantiate(bossSetting.bossInstance, spawnPosition, Quaternion.identity, _parent);

        // �������ꂽ�I�u�W�F�N�g�� ConcreteBossManager ���t���Ă��邩�m�F
        if (boss.TryGetComponent(out ConcreteBossManager bossManager))
        {
            _hpBarManager.SetTarget(bossManager.HealthManager);
            _currentBossBossManager = bossManager;
            bossManager.Initialize(_playerManager);

            // �{�X�� BossEntrance �R���|�[�l���g������Ώ�����
            if (boss.TryGetComponent(out BossEntrance bossEntrance))
            {
                // BossEntrance�ɉ��o�ݒ��n���ď�����
                if (bossSetting.entranceSettings != null)
                {
                    bossEntrance.Initialize(bossSetting.entranceSettings, spawnPosition.y, _screenShake);
                }
                else
                {
                    Debug.LogWarning("BossEntranceSettings���ݒ肳��Ă��܂���B", boss);
                }
            }
        }
        else
        {
            Debug.LogError($"Spawned boss prefab '{boss.name}' does not have a ConcreteBossManager component.");
        }
    }

    public void DestroyBoss()
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