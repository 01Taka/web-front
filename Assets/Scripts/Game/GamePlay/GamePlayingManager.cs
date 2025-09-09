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
    [SerializeField] private DeviceStateManager _deviceStateManager;

    private TimerManager _timerManager;
    private GameManager _gameManager;
    private bool _isInitialized = false;

    public bool Initialize()
    {
        if (_isInitialized)
        {
            Debug.LogWarning("Already Initialized");
            return true;
        }
        Debug.Log("Initialize Game");

        // TimerManagerのインスタンスを生成
        _timerManager = new TimerManager(_gameSetting.TimeLimit);
        // イベントリスナーの登録（ゲームのライフサイクルを通して共通）
        _timerManager.OnTimerTick += () => _gameTimerCountUI.UpdateTimerUI(_timerManager.GetRemainingTime());
        _timerManager.OnTimerEnd += OnTimeUp;

        _hordeSpawner.OnHordeEnemyKilled.AddListener(OnHordeEnemyKilled);

        if (SceneComponentManager.Instance.AttackManager.IsActiveTestMode)
        {
            _isInitialized = true;
            return true;
        }

        // AttackPointManagerの初期化は一度だけ
        if (TryGetPlayerCount(out int playerCount))
        {
            if (SceneComponentManager.Instance != null && SceneComponentManager.Instance.AttackPointManager != null)
            {
                // 一度だけ初期化する
                if (!SceneComponentManager.Instance.AttackPointManager.Initialize(playerCount - 1))
                {
                    Debug.LogError("Failed to Initialize AttackPoint");
                    return false;
                }
            }
            else
            {
                Debug.LogError("SceneComponentManager or AttackPointManager not found.");
                return false;
            }
        }
        else
        {
            Debug.LogError("Failed to get player count.");
            return false;
        }

        _isInitialized = true;
        return true;
    }

    // ゲーム開始時に毎回実行される初期化
    public void StartGame(GameManager gameManager)
    {
        if (_deviceStateManager.CurrentDeviceState != DeviceState.Host)
        {
            Debug.LogError("Only the host can start the game.");
            return;
        }

        if (!_isInitialized && !Initialize())
        {
            Debug.LogError("Game initialization failed. Aborting StartGame.");
            this.enabled = false;
            return;
        }

        _gameManager = gameManager;

        // ゲームシステムのリセットと再開
        _scoreManager.Initialize();
        _timerManager.ResetTimer();
        _timerManager.StartTimer();

        // ボスを再生成
        if (!_bossSpawner.TrySpawn(_bossId))
        {
            Debug.LogError("Failed to spawn boss. Disabling game manager.");
            this.enabled = false;
            return;
        }

        var bossManager = _bossSpawner.CurrentBossBossManager;
        if (bossManager != null)
        {
            // ボス固有のイベントリスナーは毎回再登録する
            bossManager.AddOnDeathAction(OnDeathBoss);
            bossManager.AddOnTransitionPhaseAction(OnPhaseTransition);
        }
        else
        {
            Debug.LogError("BossManager not found after spawning boss.");
        }

        // 敵のスポーンを開始
        _hordeSpawner.StartSpawn();
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

        StopGameSystems();
    }

    private void OnTimeUp()
    {
        ScoreBreakdown finalScoreBreakdown = _scoreManager.CalculateFinalScore(
            0,
            _bossSpawner.CurrentBossBossManager.CurrentPhaseIndex,
            _bossSpawner.CurrentBossBossManager.RemainingHealthRatio,
            _playerManager.TakenDamage);
        SetFinalScoreBreakDown(finalScoreBreakdown);

        StopGameSystems();
    }

    private void SetFinalScoreBreakDown(ScoreBreakdown finalScoreBreakdown)
    {
        _gameManager.SetScoreBreakDown(finalScoreBreakdown);
        _gameManager.SetGameState(GameManager.GameState.Score);
    }

    private void StopGameSystems()
    {
        _hordeSpawner.StopSpawn();
        _bossSpawner.DestroyBoss();
        _timerManager.StopTimer();
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
            if (GlobalRegistry.Instance.TryGetNetworkPlayerManager(out NetworkPlayerManager networkPlayerManager))
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