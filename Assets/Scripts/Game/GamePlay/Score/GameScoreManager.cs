using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using TMPro;

public class GameScoreManager : MonoBehaviour
{
    // --- Inspector-configurable Fields ---

    // UI Prefabs and Containers
    [Header("UI References")]
    [Tooltip("The prefab for displaying individual score details.")]
    [SerializeField] private GameResultDetail _gameResultDetailPrefab;
    [Tooltip("The container transform for the score detail prefabs.")]
    [SerializeField] private Transform _scoreDetailContainer;

    // Text and UI Animation
    [Header("Score Display Text & Animation")]
    [Tooltip("The text component for the result header (e.g., VICTORY! or GAME OVER).")]
    [SerializeField] private TextMeshProUGUI _resultHeaderText;
    [Tooltip("The total score text components to be updated.")]
    [SerializeField] private TextMeshProUGUI[] _totalScoreTexts;
    [Tooltip("The UI animator for the total score display.")]
    [SerializeField] private UIComplexAnimator _totalScoreAnimator;

    // Animation Settings
    [Header("Animation Settings")]
    [Tooltip("The duration of the total score count-up animation.")]
    [SerializeField] private float _countUpDuration = 2f;
    [Tooltip("The delay between each individual score detail animation.")]
    [SerializeField] private float _detailAnimationDelay = 0.8f;

    // Audio Clips
    [Header("Audio")]
    [Tooltip("The sound played when an individual score detail is displayed.")]
    [SerializeField] private AudioClip _displayDetailScoreClip;
    [Tooltip("The sound played when the total score is displayed.")]
    [SerializeField] private AudioClip _displayTotalScoreClip;

    // Configurable Strings
    [Header("Result Text")]
    [Tooltip("The text to display on a victory.")]
    [SerializeField] private string _victoryText = "VICTORY!";
    [Tooltip("The text to display on a defeat.")]
    [SerializeField] private string _gameOverText = "GAME OVER";

    // --- Private Variables for Object Pooling ---
    private List<GameResultDetail> _scoreDetailPool = new List<GameResultDetail>();
    private const int POOL_SIZE = 5; // The number of different score types to display

    private void Start()
    {
        InitializeScoreDetails();
    }

    /// <summary>
    /// Pre-instantiates the GameResultDetail UI elements and prepares them for reuse.
    /// </summary>
    private void InitializeScoreDetails()
    {
        for (int i = 0; i < POOL_SIZE; i++)
        {
            GameResultDetail detailInstance = Instantiate(_gameResultDetailPrefab, _scoreDetailContainer);
            _scoreDetailPool.Add(detailInstance);
        }
    }

    /// <summary>
    /// Shows the score breakdown on the UI.
    /// </summary>
    /// <param name="scoreBreakdown">The ScoreBreakdown object containing all score data.</param>
    public void ShowScore(ScoreBreakdown scoreBreakdown)
    {
        HideAllScores();
        StartCoroutine(ShowScoreBreakdownCoroutine(scoreBreakdown));
    }

    /// <summary>
    /// Hides all pre-instantiated score detail objects.
    /// </summary>
    private void HideAllScores()
    {
        foreach (var detail in _scoreDetailPool)
        {
            detail.gameObject.SetActive(true);
            detail.ResetState();
            detail.gameObject.SetActive(false);
        }
        _totalScoreAnimator.SetStateToHide();
    }

    private IEnumerator ShowScoreBreakdownCoroutine(ScoreBreakdown scoreBreakdown)
    {
        SetResultHeaderText(scoreBreakdown.scores.GetValueOrDefault(ScoreType.BossDefeatBonus, 0) > 0);

        foreach (var text in _totalScoreTexts)
        {
            text.text = "0";
        }

        // Use a more structured and robust approach to iterate scores
        var orderedScores = new Dictionary<ScoreType, int>
        {
            { ScoreType.BossDefeatBonus, scoreBreakdown.scores.GetValueOrDefault(ScoreType.BossDefeatBonus, 0) },
            { ScoreType.BossDamageBonus, scoreBreakdown.scores.GetValueOrDefault(ScoreType.BossDamageBonus, 0) },
            { ScoreType.HordeEnemyScore, scoreBreakdown.scores.GetValueOrDefault(ScoreType.HordeEnemyScore, 0) },
            { ScoreType.TimeRemainingBonus, scoreBreakdown.scores.GetValueOrDefault(ScoreType.TimeRemainingBonus, 0) },
            { ScoreType.DamageTakenPenalty, scoreBreakdown.scores.GetValueOrDefault(ScoreType.DamageTakenPenalty, 0) }
        };

        int poolIndex = 0;
        foreach (var score in orderedScores)
        {
            // Use an instance from the pool instead of creating a new one
            if (poolIndex >= _scoreDetailPool.Count)
            {
                Debug.LogWarning("Score detail pool is too small. Consider increasing POOL_SIZE.");
                continue;
            }

            GameResultDetail detailInstance = _scoreDetailPool[poolIndex];
            detailInstance.gameObject.SetActive(true);
            detailInstance.Initialize(score.Key, score.Value);
            detailInstance.PlayAnimation();

            SoundManager.Instance?.PlayEffect(_displayDetailScoreClip);

            yield return new WaitForSeconds(_detailAnimationDelay);
            poolIndex++;
        }

        SoundManager.Instance?.PlayEffect(_displayTotalScoreClip);
        _totalScoreAnimator.Show();
        StartCoroutine(AnimateTotalScoreCountUp(scoreBreakdown.TotalScore));
    }

    private void SetResultHeaderText(bool isVictory)
    {
        if (_resultHeaderText != null)
        {
            _resultHeaderText.text = isVictory ? _victoryText : _gameOverText;
        }
    }

    private IEnumerator AnimateTotalScoreCountUp(int totalScore)
    {
        float startTime = Time.time;
        int startValue = 0;

        while (Time.time < startTime + _countUpDuration)
        {
            float progress = (Time.time - startTime) / _countUpDuration;
            float easedProgress = Mathf.SmoothStep(0f, 1f, progress);
            int currentValue = Mathf.RoundToInt(Mathf.Lerp(startValue, totalScore, easedProgress));

            foreach (var text in _totalScoreTexts)
            {
                text.text = currentValue.ToString();
            }

            yield return null;
        }

        foreach (var text in _totalScoreTexts)
        {
            text.text = totalScore.ToString();
        }
    }
}