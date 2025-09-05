using UnityEngine;
using System.Collections.Generic;
using UnityEngine.InputSystem;

public class BarrierScript : MonoBehaviour
{
    [Header("Color Settings")]
    [SerializeField, Tooltip("ScriptableObject holding color settings.")]
    private BarrierColorConfig _colorConfig;

    [Header("Sound Settings")]
    [SerializeField, Tooltip("List of sounds for each step.")]
    private List<AudioClip> _stepSounds = new List<AudioClip>();

    [SerializeField, Tooltip("Sound for barrier destruction.")]
    private AudioClip _destroySound;

    [SerializeField, Tooltip("Sound for barrier deployment/reset.")]
    private AudioClip _deploySound;

    [Header("Sprite Renderer Settings")]
    [SerializeField, Tooltip("Sprite renderer for the main barrier.")]
    private SpriteRenderer _barrierSpriteRenderer;

    [SerializeField, Tooltip("Sprite renderer for the barrier pattern.")]
    private SpriteRenderer _patternSpriteRenderer;

    [Header("Initial State")]
    [SerializeField, Tooltip("If true, the barrier starts in a destroyed state.")]
    private bool _startDestroyed = false;

    private int _currentStep = 0;
    public int CurrentStep => _currentStep;
    public int ColorStepsCount => _colorConfig.ColorSteps.Count;

    void Start()
    {
        if (_colorConfig == null)
        {
            Debug.LogError("Color Config is not set on BarrierScript.");
            return;
        }

        if (_startDestroyed)
        {
            DestroyBarrier(false);
        }
        else
        {
            ResetBarrier(false);
        }
    }

    void Update()
    {
        // For debug: press space key to advance to the next step
        if (Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            NextStep();
        }

        // For debug: press R key to reset with sound
        if (Keyboard.current.rKey.wasPressedThisFrame)
        {
            ResetBarrier(true);
        }
    }

    /// <summary>
    /// Applies the nearest step's color and sound based on the given ratio. Destroys the barrier if the ratio is 0.
    /// </summary>
    /// <param name="ratio">The color ratio to apply (from 0.0 to 1.0)</param>
    public void SetColorAndSoundByRatio(float ratio)
    {
        if (_colorConfig == null)
        {
            Debug.LogError("Color Config is not set.");
            return;
        }

        ratio = Mathf.Clamp01(ratio);

        if (ratio <= 0)
        {
            DestroyBarrier();
        }
        else
        {
            int stepIndex = Mathf.RoundToInt(ratio * (_colorConfig.ColorSteps.Count - 1));
            SetBarrierColorByStep(stepIndex);
        }
    }

    /// <summary>
    /// Advances to the next step. The barrier is destroyed when the next step exceeds the maximum.
    /// </summary>
    public void NextStep()
    {
        int nextStep = _currentStep + 1;
        if (nextStep >= _colorConfig.ColorSteps.Count)
        {
            DestroyBarrier();
        }
        else
        {
            SetBarrierColorByStep(nextStep);
        }
    }

    /// <summary>
    /// Applies the color and sound for the specified step.
    /// </summary>
    /// <param name="step">The color step to apply (0-based index)</param>
    public void SetBarrierColorByStep(int step)
    {
        if (step >= 0 && step < _colorConfig.ColorSteps.Count)
        {
            _currentStep = step;
            SetBarrierVisuals(true, _colorConfig.ColorSteps[_currentStep].barrierColor, _colorConfig.ColorSteps[_currentStep].patternColor);
            TryPlaySound(_stepSounds, _currentStep);
        }
        else
        {
            Debug.LogWarning("The specified step is out of the color array bounds.");
        }
    }

    /// <summary>
    /// Resets the barrier to its initial state, with an option to play a sound.
    /// </summary>
    /// <param name="withSound">Whether to play the deployment sound.</param>
    public void ResetBarrier(bool withSound = true)
    {
        _currentStep = 0;
        SetBarrierColorByStep(_currentStep);
        if (withSound)
        {
            TryPlaySound(_deploySound);
        }
    }

    /// <summary>
    /// Hides the barrier and plays the destruction sound, with an option to skip the sound.
    /// </summary>
    /// <param name="withSound">Whether to play the destruction sound.</param>
    public void DestroyBarrier(bool withSound = true)
    {
        SetBarrierVisuals(false);
        if (withSound)
        {
            TryPlaySound(_destroySound);
        }
    }

    /// <summary>
    /// Sets the visuals for the barrier, including colors and visibility.
    /// </summary>
    private void SetBarrierVisuals(bool isVisible, Color? barrierColor = null, Color? patternColor = null)
    {
        if (_barrierSpriteRenderer != null)
        {
            _barrierSpriteRenderer.enabled = isVisible;
            if (barrierColor.HasValue) _barrierSpriteRenderer.color = barrierColor.Value;
        }
        if (_patternSpriteRenderer != null)
        {
            _patternSpriteRenderer.enabled = isVisible;
            if (patternColor.HasValue) _patternSpriteRenderer.color = patternColor.Value;
        }
    }

    /// <summary>
    /// Plays an audio clip if the SoundManager instance is available.
    /// </summary>
    private void TryPlaySound(AudioClip clip)
    {
        if (SoundManager.Instance != null && clip != null)
        {
            SoundManager.Instance.PlayEffect(clip);
        }
    }

    /// <summary>
    /// Plays the step sound based on the step index. Reuses the last sound if the index is out of bounds.
    /// </summary>
    private void TryPlaySound(List<AudioClip> clips, int step)
    {
        if (SoundManager.Instance == null || clips == null || clips.Count == 0)
        {
            return;
        }

        AudioClip clipToPlay = (step < clips.Count) ? clips[step] : clips[clips.Count - 1];
        TryPlaySound(clipToPlay);
    }
}