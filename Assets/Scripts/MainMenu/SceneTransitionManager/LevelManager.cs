using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance; // for easy access from other scripts

    public Slider progressBar; // reference to the UI slider that will show loading progress
    public GameObject transitionsContainer; // parent object that contains all transition objects

    private SceneTransition[] transitions; // array to hold references to all transition scripts
    private bool skipTransition = false; // flag to determine if we should skip transition

    // Awake is called when the script instance is being loaded
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

    // Public method to load a scene with a specified transition
    public void LoadScene(string sceneName, string transitionName)
    {
        // Check the current scene name to determine if we should skip the transition
        string currentScene = SceneManager.GetActiveScene().name;
        skipTransition = (currentScene != "Menu");

        // Start the asynchronous loading of the scene with the specified transition
        StartCoroutine(LoadSceneAsync(sceneName, transitionName));
    }

    // Coroutine to load a scene asynchronously with a transition
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