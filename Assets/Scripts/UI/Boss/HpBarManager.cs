using UnityEngine;
using UnityEngine.UI;

public class HpBarManager : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private Slider slider;
    [SerializeField] private Gradient gradient;
    [SerializeField] private Image fillImage;

    [Header("Settings")]
    [SerializeField][Range(0f, 1f)] private float alpha = 1f;

    private HealthManager healthManager;

    private void OnDestroy()
    {
        // Unsubscribe from the event when the object is destroyed to prevent memory leaks
        UnsubscribeFromEvents();
    }

    // Call this method to set the target to track
    public void SetTarget(HealthManager newHealthManager)
    {
        // Unsubscribe from any previous target's events
        UnsubscribeFromEvents();

        healthManager = newHealthManager;

        // Subscribe to the new target's events if it's not null
        if (healthManager != null)
        {
            slider.maxValue = healthManager.MaxHealth;
            UpdateHpBar(healthManager.CurrentHealth);
            healthManager.AddOnHealthChangeAction(UpdateHpBar); // Assuming BaseHealthManager has this event
        }
        else
        {
            Debug.LogError("The new BaseHealthManager target is null.");
        }
    }

    private void UpdateHpBar(float currentHealth)
    {
        if (slider == null)
        {
            Debug.LogError("Slider is not assigned to HpBarManager.");
            return;
        }

        slider.value = currentHealth;

        if (fillImage != null)
        {
            Color newFillColor = gradient.Evaluate(slider.normalizedValue);
            newFillColor.a = alpha;
            fillImage.color = newFillColor;
        }
    }

    private void UnsubscribeFromEvents()
    {
        if (healthManager != null)
        {
            healthManager.RemoveOnHealthChangeAction(UpdateHpBar);
        }
    }
}