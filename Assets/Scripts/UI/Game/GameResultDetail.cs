using TMPro;
using UnityEngine;
using System.Collections;
using UnityEngine.UI;

/// <summary>
/// This class handles the initialization and animation of a game result UI element.
/// </summary>
public class GameResultDetail : MonoBehaviour
{
    // These fields are assigned in the Unity Inspector.
    [SerializeField] private TextMeshProUGUI _scoreTypeText;
    [SerializeField] private TextMeshProUGUI _scoreValueText;
    [SerializeField] private Image _headerChipImage;
    [SerializeField] private UIComplexAnimator _animator;

    /// <summary>
    /// Initializes the UI element with the provided score type and value.
    /// </summary>
    /// <param name="scoreType">The type of score to display.</param>
    /// <param name="value">The integer value of the score.</param>
    public void Initialize(ScoreType scoreType, int value)
    {
        _headerChipImage.color = scoreType == ScoreType.DamageTakenPenalty ? Color.red : Color.green;

        _scoreTypeText.text = scoreType.ToString();
        _scoreValueText.color = (value >= 0) ? Color.green : Color.red;
        _scoreValueText.text = FormatScoreValue(value);
    }

    public void PlayAnimation()
    {
        AnimateWithDelay(0);
    }

    /// <summary>
    /// Plays the animation on the UI element after a specified delay.
    /// </summary>
    /// <param name="delayTime">The delay time in seconds before the animation starts.</param>
    public void AnimateWithDelay(float delayTime)
    {
        StartCoroutine(AnimateAfterDelay(delayTime));
    }

    /// <summary>
    /// Formats the score value with a leading '+' for positive numbers.
    /// </summary>
    /// <param name="value">The score value.</param>
    /// <returns>A formatted string of the score value.</returns>
    private string FormatScoreValue(int value)
    {
        return value >= 0 ? $"+{value}" : value.ToString();
    }

    /// <summary>
    /// Coroutine to wait for a delay and then play the animation.
    /// </summary>
    /// <param name="delayTime">The delay time in seconds.</param>
    private IEnumerator AnimateAfterDelay(float delayTime)
    {
        yield return new WaitForSeconds(delayTime);
        _animator?.Show();
    }
}
