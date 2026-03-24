using UnityEngine;
using System.Collections;

/// <summary>
/// Creates a continuous fade-in and fade-out animation for UI elements.
/// Useful for pulsing effects, loading indicators, or attention-grabbing UI elements.
/// </summary>
public class FadeObjects : MonoBehaviour
{
    [SerializeField] private CanvasGroup canvasGroup;
    [SerializeField] private float fadeDuration = 2f;

    /// <summary>
    /// Initializes the fade sequence.
    /// 
    /// BEHAVIOR:
    /// - Ensures CanvasGroup component is assigned (attempts to get if missing)
    /// - Starts coroutine to handle fade-in then fade-out sequence
    /// </summary>
    void Start()
    {
        // Ensure canvasGroup is assigned
        if (canvasGroup == null)
        {
            canvasGroup = GetComponent<CanvasGroup>();
            if (canvasGroup == null)
            {
                //Debug.LogError("CanvasGroup component not found!"); 
                return;
            }
        }

        // Start the fade sequence once
        StartCoroutine(FadeSequence());
    }

    /// <summary>
    /// Coroutine that executes fade-in followed by fade-out.
    /// 
    /// BEHAVIOR:
    /// - Fades from transparent to opaque (0 → 1)
    /// - Fades from opaque to transparent (1 → 0)
    /// </summary>
    IEnumerator FadeSequence()
    {
        // Fade in
        yield return StartCoroutine(Fade(0f, 1f, fadeDuration));
        // Fade out
        yield return StartCoroutine(Fade(1.5f, 0f, fadeDuration));
    }

    /// <summary>
    /// Coroutine that handles a single fade transition.
    /// 
    /// BEHAVIOR:
    /// - Smoothly interpolates alpha from startAlpha to endAlpha over duration
    /// - Uses SmoothStep for a more natural fade curve
    /// - Ensures final alpha matches target value
    /// </summary>
    IEnumerator Fade(float startAlpha, float endAlpha, float duration)
    {
        float elapsedTime = 0f;

        while (elapsedTime < duration)
        {
            elapsedTime += Time.deltaTime;
            float t = elapsedTime / duration;

            // Optional: smooth step for nicer fade effect
            t = Mathf.SmoothStep(0f, 1f, t);

            canvasGroup.alpha = Mathf.Lerp(startAlpha, endAlpha, t);
            yield return null;
        }

        // Ensure we end at exactly the target alpha
        canvasGroup.alpha = endAlpha;
    }
}