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
    /// 指定されたIDのボスを生成する
    /// </summary>
    /// <param name="id">生成するボスのID</param>
    public bool TrySpawn(BossId id)
    {
        // 指定された id の設定を BossSettings 配列から探す
        BossSettings bossSetting = Array.Find(_bossSettings, boss => boss.id == id);

        // 一致する設定が見つかったか確認
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
        // 親オブジェクトの位置にオフセットを加えた位置でボスをインスタンス化
        Vector3 spawnPosition = _parent.position + _spawnOffset;
        var boss = Instantiate(bossSetting.bossInstance, spawnPosition, Quaternion.identity, _parent);

        // 生成されたオブジェクトに ConcreteBossManager が付いているか確認
        if (boss.TryGetComponent(out ConcreteBossManager bossManager))
        {
            _hpBarManager.SetTarget(bossManager.HealthManager);
            _currentBossBossManager = bossManager;
            bossManager.Initialize(_playerManager);

            // ボスに BossEntrance コンポーネントがあれば初期化
            if (boss.TryGetComponent(out BossEntrance bossEntrance))
            {
                // BossEntranceに演出設定を渡して初期化
                if (bossSetting.entranceSettings != null)
                {
                    bossEntrance.Initialize(bossSetting.entranceSettings, spawnPosition.y, _screenShake);
                }
                else
                {
                    Debug.LogWarning("BossEntranceSettingsが設定されていません。", boss);
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