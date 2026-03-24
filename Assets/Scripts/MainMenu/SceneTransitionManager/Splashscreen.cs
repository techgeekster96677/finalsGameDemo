using UnityEngine;
using System.Collections;
using DG.Tweening;

/// <summary>
/// Manages splash screen display with fade-out animation using DOTween.
/// Automatically fades out the splash screen after a specified delay.
/// </summary>
public class Splashscreen : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 2f;
    [SerializeField] private float delayBeforeFade = 3f;

    /// <summary>
    /// Initializes the splash screen and begins the fade-out sequence.
    /// 
    /// BEHAVIOR:
    /// - Ensures CanvasGroup component is assigned (attempts to get if missing)
    /// - Sets initial alpha to 1 (fully visible)
    /// - Delays then fades out using DOTween
    /// </summary>
    void Start()
    {
        // Ensure canvasGroup is assigned
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                Debug.LogError("CanvasGroup component not found!");
                return;
            }
        }

        // Start with the splash screen fully visible
        canvasGroup.alpha = 1f;

        // Fade out after delay using DOTween
        canvasGroup.DOFade(0f, fadeDuration)
            .SetDelay(delayBeforeFade)
            .SetEase(Ease.InOutQuad); // Optional: smoother fade curve
    }
}