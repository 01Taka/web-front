using UnityEngine;
using System;

public class GamePlayingManager : MonoBehaviour
{
    [SerializeField] private GameSetting _gameSetting;
    [SerializeField] private TimerCountUI _gameTimerCountUI;
    [SerializeField] private PlayerManager _playerManager;
    [SerializeField] private BossSpawner _bossSpawner;
    [SerializeField] private HordeSpawner _hordeSpawner;
    [SerializeField] private BossId _bossId;
    [SerializeField] private ScoreManager _scoreManager;

    private TimerManager _timerManager;
    private GameManager _gameManager;

    // Initialization method called from GameManager instead of Start
    public void InitializeGame(GameManager gameManager)
    {
        // Initialize timer
        _timerManager = new TimerManager(_gameSetting.TimeLimit);
        _timerManager.OnTimerTick += () => _gameTimerCountUI.UpdateTimerUI(_timerManager.GetRemainingTime());
        _timerManager.OnTimerEnd += OnTimeUp;
        _timerManager.StartTimer();
        _gameManager = gameManager;

        // Spawn boss and set up event listeners
        if (!_bossSpawner.TrySpawn(_bossId))
        {
            Debug.LogError("Failed to spawn boss. Disabling game manager.");
            this.enabled = false;
            return;
        }

        var bossManager = _bossSpawner.CurrentBossBossManager;
        if (bossManager != null)
        {
            bossManager.AddOnDeathAction(OnDeathBoss);
            bossManager.AddOnTransitionPhaseAction(OnPhaseTransition);
        }
        else
        {
            Debug.LogError("BossManager not found after spawning boss.");
        }

        // Start enemy wave and initialize other systems
        _hordeSpawner.StartSpawn();
        _scoreManager.Initialize(); // Ensure ScoreManager is initialized
        _hordeSpawner.OnHordeEnemyKilled.AddListener(OnHordeEnemyKilled);

        // Get player count and initialize AttackPointManager
        if (TryGetPlayerCount(out int playerCount))
        {
            if (SceneComponentManager.Instance != null && SceneComponentManager.Instance.AttackPointManager != null)
            {
                // Initialize after subtracting the administrator count
                SceneComponentManager.Instance.AttackPointManager.Initialize(playerCount - 1);
            }
            else
            {
                Debug.LogError("SceneComponentManager or AttackPointManager not found.");
            }
        }
        else
        {
            Debug.LogError("Failed to get player count.");
        }
    }

    private void Update()
    {
        if (_timerManager != null)
        {
            _timerManager.UpdateTimer();
        }
    }


    private void OnHordeEnemyKilled()
    {
        _scoreManager.AddHordeEnemyKill();
    }

    private void OnPhaseTransition(int nextPhase)
    {
        if (_timerManager != null)
        {
            _timerManager.AddTime(_gameSetting.ExtraTimeOnPhaseChange);
        }
    }

    private void OnDeathBoss()
    {
        _scoreManager.OnBossDefeat();

        ScoreBreakdown finalScoreBreakdown = _scoreManager.CalculateFinalScore(
            _timerManager.GetRemainingTime(),
            _bossSpawner.CurrentBossBossManager.CurrentPhaseIndex,
            0f,
            _playerManager.TakenDamage);
        SetFinalScoreBreakDown(finalScoreBreakdown);

        // Stop all game systems
        _hordeSpawner.StopSpawn();
        _bossSpawner.DestoryBoss();
        _timerManager.StopTimer();
    }

    private void OnTimeUp()
    {
        ScoreBreakdown finalScoreBreakdown = _scoreManager.CalculateFinalScore(
            0,
            _bossSpawner.CurrentBossBossManager.CurrentPhaseIndex,
            _bossSpawner.CurrentBossBossManager.RemainingHealthRatio,
            _playerManager.TakenDamage);
        SetFinalScoreBreakDown(finalScoreBreakdown);

        // Stop all game systems
        _hordeSpawner.StopSpawn();
        _bossSpawner.DestoryBoss();
    }

    private void SetFinalScoreBreakDown(ScoreBreakdown finalScoreBreakdown)
    {
        _gameManager.SetScoreBreakDown(finalScoreBreakdown);
        _gameManager.SetGameState(GameManager.GameState.Score);
    }

    /// <summary>
    /// Safely gets the number of network players.
    /// </summary>
    /// <param name="count">The retrieved number of players.</param>
    /// <returns>True if the number of players is 1 or more.</returns>
    private bool TryGetPlayerCount(out int count)
    {
        count = 0;
        try
        {
            var networkPlayerManager = GlobalRegistry.Instance?.GetNetworkPlayerManager();
            if (networkPlayerManager != null)
            {
                count = networkPlayerManager.PlayerCount;
                return count > 0;
            }
        }
        catch (Exception e)
        {
            Debug.LogError($"An error occurred while getting the player count: {e.Message}");
        }
        return false;
    }


}