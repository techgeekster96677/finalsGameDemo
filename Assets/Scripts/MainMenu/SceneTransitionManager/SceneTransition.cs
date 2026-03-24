using UnityEngine;
using System.Collections;

/// <summary>
/// Abstract base class for scene transition effects.
/// Defines the contract for transition animations that play when entering or exiting a scene.
/// </summary>
public abstract class SceneTransition : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created

    /// <summary>
    /// Coroutine that plays the entrance animation when loading into a scene.
    /// </summary>
    public abstract IEnumerator AnimateTransitionIn();

    /// <summary>
    /// Coroutine that plays the exit animation when leaving a scene.
    /// </summary>
    public abstract IEnumerator AnimateTransitionOut();
}