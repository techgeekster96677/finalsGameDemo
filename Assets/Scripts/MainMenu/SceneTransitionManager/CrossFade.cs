using UnityEngine;
using System.Collections;
using DG.Tweening;

/// <summary>
/// Crossfade transition effect that fades the screen to black and back.
/// Implements the SceneTransition abstract class using DOTween animations.
/// </summary>
public class CrossFade : SceneTransition
{
    public CanvasGroup crossFade;

    /// <summary>
    /// Plays the entrance transition by fading from transparent to opaque.
    /// 
    /// BEHAVIOR: Fades the CanvasGroup alpha from 0 to 1 over 1 second.
    /// Used when transitioning into a new scene.
    /// </summary>
    public override IEnumerator AnimateTransitionIn()
    {
        var tweener = crossFade.DOFade(1f, 1f);
        yield return tweener.WaitForCompletion();
    }

    /// <summary>
    /// Plays the exit transition by fading from opaque to transparent.
    /// 
    /// BEHAVIOR: Fades the CanvasGroup alpha from 1 to 0 over 1 second.
    /// Used when transitioning out of the current scene.
    /// </summary>
    public override IEnumerator AnimateTransitionOut()
    {
        var tweener = crossFade.DOFade(0f, 1f);
        yield return tweener.WaitForCompletion();
    }
}