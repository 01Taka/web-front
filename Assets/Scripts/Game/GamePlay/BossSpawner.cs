using System;
using UnityEngine;
using UnityEngine.Events;

public class BossSpawner : MonoBehaviour
{
    [SerializeField] private Transform _parent;
    [SerializeField] private Vector3 _spawnOffset; // スポーン位置を調整するためのフィールドを追加
    [SerializeField] private BossSettings[] _bossSettings; // ボス設定の配列
    [SerializeField] private HpBarManager _hpBarManager;
    [SerializeField] private PlayerManager _playerManager;

    private ConcreteBossManager _currentBossBossManager;
    public ConcreteBossManager CurrentBossBossManager => _currentBossBossManager;
    [SerializeField] private UnityEvent _onBossDestroyed = new UnityEvent();

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
            // 一致する設定が見つからなかった場合、エラーログを出力
            Debug.LogError("Error: BossSettings with id " + id.ToString() + " not found.");
            return false;
        }
    }

    void Spawn(BossSettings bossSetting)
    {
        // 親オブジェクトの位置にオフセットを加えた位置でボスをインスタンス化
        var boss = Instantiate(bossSetting.bossInstance, _parent.position + _spawnOffset, Quaternion.identity, _parent);

        // 生成されたオブジェクトに BaseBossManager が付いているか確認
        if (boss.TryGetComponent(out ConcreteBossManager bossManager))
        {
            _hpBarManager.SetTarget(bossManager.HealthManager);
            _currentBossBossManager = bossManager;
            bossManager.Initialize(_playerManager);
        }
        else
        {
            // もしコンポーネントが見つからなかった場合に警告を出力
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