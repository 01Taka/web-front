using UnityEngine;
using System.Collections;
using UnityEngine.Events;

public class HordeSpawner : MonoBehaviour
{
    [Header("Spawn Settings")]
    [SerializeField] private HordeEnemy _enemyPrefab;
    [SerializeField] private Transform _parent;
    [SerializeField] private Transform _startPoint;
    [SerializeField] private Transform _endPoint;
    [SerializeField] private Transform _target;
    [SerializeField] private float _spawnInterval = 3f;
    [SerializeField] private int _enemiesPerWave = 5;
    [SerializeField] private int _initialPoolSize = 20;

    [Header("Events")]
    public UnityEvent OnHordeEnemyKilled;

    private ObjectPool<HordeEnemy> _enemyPool;
    private Coroutine _spawnCoroutine;

    private void Start()
    {
        if (_target == null)
        {
            Debug.LogWarning("Target is not set. Disabling the spawner.");
            enabled = false;
            return;
        }

        _enemyPool = new ObjectPool<HordeEnemy>(_enemyPrefab, _initialPoolSize, _parent);
    }

    public void StartSpawn()
    {
        if (_spawnCoroutine == null)
        {
            _spawnCoroutine = StartCoroutine(SpawnLoop());
        }
    }

    public void StopSpawn()
    {
        if (_spawnCoroutine != null)
        {
            StopCoroutine(_spawnCoroutine);
            _spawnCoroutine = null;
        }
    }

    private IEnumerator SpawnLoop()
    {
        while (true)
        {
            yield return new WaitForSeconds(_spawnInterval);
            SpawnWave();
        }
    }

    private void SpawnWave()
    {
        for (int i = 0; i < _enemiesPerWave; i++)
        {
            Vector3 spawnPos = GetRandomPointOnLine(_startPoint.position, _endPoint.position);
            HordeEnemy enemy = _enemyPool.Get();

            enemy.transform.position = spawnPos;
            enemy.transform.rotation = Quaternion.identity;
            enemy.SetTarget(_target);

            // プールに返却するコールバック登録
            enemy.AddDeathAction((DeathReason reason) => OnDeathHordeEnemy(enemy, reason));
        }
    }

    private void OnDeathHordeEnemy(HordeEnemy enemy, DeathReason reason)
    {
        if (reason == DeathReason.PlayerDefeated)
        {
            OnHordeEnemyKilled.Invoke();
        }
        _enemyPool.ReturnToPool(enemy);
    }

    private Vector3 GetRandomPointOnLine(Vector3 start, Vector3 end)
    {
        float t = Random.Range(0f, 1f);
        return Vector3.Lerp(start, end, t);
    }

    private void OnDisable()
    {
        OnHordeEnemyKilled.RemoveAllListeners();
    }
}
