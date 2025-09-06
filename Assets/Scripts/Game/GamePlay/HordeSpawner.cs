using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Events; // Add this using directive for UnityEvent

public class HordeSpawner : MonoBehaviour
{
    // === Fields ===
    [Header("Spawn Settings")]
    [Tooltip("The enemy prefab to be spawned.")]
    [SerializeField] private GameObject _enemyPrefab;

    [SerializeField] private Transform _parent;

    [Tooltip("The start position of the enemy spawn line.")]
    [SerializeField] private Transform _startPoint;

    [Tooltip("The end position of the enemy spawn line.")]
    [SerializeField] private Transform _endPoint;

    [Tooltip("The target for enemies to pursue.")]
    [SerializeField] private Transform _target;

    [Tooltip("The time in seconds until the next wave (wave interval).")]
    [SerializeField] private float _spawnInterval = 3f;

    [Tooltip("The number of enemies to spawn in a single wave.")]
    [SerializeField] private int _enemiesPerWave = 5;

    // --- UnityEvent for external communication ---
    [Header("Events")]
    [Tooltip("Invoked when a horde enemy is defeated.")]
    public UnityEvent OnHordeEnemyKilled;

    // === Private Variables ===
    private List<GameObject> _spawnedEnemies = new List<GameObject>();
    private Coroutine _spawnCoroutine;

    // === Methods ===
    private void Start()
    {
        if (_target == null)
        {
            Debug.LogWarning("Target is not set. Disabling the spawner.");
            this.enabled = false;
            return;
        }
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
            CleanUpEnemies();
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
            GameObject enemyObj = Instantiate(_enemyPrefab, spawnPos, Quaternion.identity, _parent);

            if (enemyObj.TryGetComponent<HordeEnemy>(out HordeEnemy hordeEnemy))
            {
                hordeEnemy.SetTarget(_target);
                // Subscribe to the enemy's death action to trigger our own event
                hordeEnemy.AddDeathAction((DeathReason deathReason) => OnDeathHordeEnemy(enemyObj, deathReason));
            }
            else
            {
                Debug.LogWarning("The spawned enemy prefab does not have a HordeEnemy component.");
            }

            _spawnedEnemies.Add(enemyObj);
        }
    }

    private void OnDeathHordeEnemy(GameObject enemy, DeathReason deathReason)
    {
        // Remove the enemy from the list
        _spawnedEnemies.Remove(enemy);

        if (deathReason == DeathReason.PlayerDefeated)
        {
            // Invoke the UnityEvent to notify other systems
            OnHordeEnemyKilled.Invoke();
        }

        // Destroy the enemy object
        Destroy(enemy);
    }

    private Vector3 GetRandomPointOnLine(Vector3 start, Vector3 end)
    {
        float t = Random.Range(0f, 1f);
        return Vector3.Lerp(start, end, t);
    }

    public void CleanUpEnemies()
    {
        foreach (var enemy in _spawnedEnemies)
        {
            if (enemy != null)
            {
                Destroy(enemy);
            }
        }
        _spawnedEnemies.Clear();
    }
    private void OnDisable()
    {
        OnHordeEnemyKilled.RemoveAllListeners();
    }
}

