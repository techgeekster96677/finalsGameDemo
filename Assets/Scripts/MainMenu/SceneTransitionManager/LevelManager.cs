using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

/// <summary>
/// Manages asynchronous scene loading with configurable transition effects.
/// Implements singleton pattern to persist across scene loads.
/// </summary>
public class LevelManager : MonoBehaviour
{
    public static LevelManager instance; // for easy access from other scripts

    public Slider progressBar; // reference to the UI slider that will show loading progress
    public GameObject transitionsContainer; // parent object that contains all transition objects

    private SceneTransition[] transitions; // array to hold references to all transition scripts
    private bool skipTransition = false; // flag to determine if we should skip transition

    /// <summary>
    /// Singleton initialization. First instance persists; duplicates self-destruct.
    /// </summary>
    public void Awake()
    {
        if (instance == null)
        {
            instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
            Destroy(gameObject);
    }

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    private void Start()
    {
        // Get all SceneTransition components from the children of transitionsContainer
        transitions = transitionsContainer.GetComponentsInChildren<SceneTransition>();
    }

    /// <summary>
    /// Loads a scene with the specified transition effect.
    /// 
    /// BEHAVIOR NOTES:
    /// - Transitions are only played when loading from a scene named "Menu"
    /// - If loading from any other scene, skipTransition is set to true
    /// - This prevents transition animations when returning to menu from gameplay
    /// 
    /// TROUBLESHOOTING: If transitions are not playing:
    /// - Verify the current scene name exactly matches "Menu" (case-sensitive)
    /// - Ensure transitionsContainer has child GameObjects with SceneTransition components
    /// </summary>
    public void LoadScene(string sceneName, string transitionName)
    {
        // Check the current scene name to determine if we should skip the transition
        string currentScene = SceneManager.GetActiveScene().name;
        skipTransition = (currentScene != "Menu");

        // Start the asynchronous loading of the scene with the specified transition
        StartCoroutine(LoadSceneAsync(sceneName, transitionName));
    }

    /// <summary>
    /// Coroutine that handles asynchronous scene loading with progress bar and transitions.
    /// 
    /// LOADING BEHAVIOR:
    /// - Scene loads asynchronously with allowSceneActivation = false until progress reaches 0.9f
    /// - Progress bar shows loading progress (0 to 0.9)
    /// - Scene activates once progress reaches 0.9f and loading is complete
    /// </summary>
    IEnumerator LoadSceneAsync(string sceneName, string transitionName)
    {
        // Find the transition script that matches the specified transition name
        SceneTransition transition = transitions.First(t => t.name == transitionName);

        // load the scene asynchronously and prevent it from activating until ready
        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        // if skipTransition is false,
        // play the transition animation before showing the progress bar
        if (!skipTransition)
        {
            yield return transition.AnimateTransitionIn();
        }

        progressBar.gameObject.SetActive(true);

        do
        {
            progressBar.value = scene.progress;
            yield return null;
        } while (scene.progress < 0.9f);

        scene.allowSceneActivation = true; // allow the scene to activate once loading is complete
        progressBar.gameObject.SetActive(false); // hide the progress bar after loading is complete

        // if skipTransition is false,
        // play the transition animation after the scene has loaded
        if (!skipTransition)
        {
            yield return transition.AnimateTransitionOut();
            // play the transition animation after the scene has loaded
        }
    }
}