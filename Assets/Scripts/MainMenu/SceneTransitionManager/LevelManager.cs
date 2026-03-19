using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class LevelManager : MonoBehaviour
{
    public static LevelManager instance;

    public Slider progressBar;
    public GameObject transitionsContainer;

    private SceneTransition[] transitions;

    // Add this one line
    private bool skipTransition = false;

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
        transitions = transitionsContainer.GetComponentsInChildren<SceneTransition>();
    }

    public void LoadScene(string sceneName, string transitionName)
    {
        // Add these 3 lines
        string currentScene = SceneManager.GetActiveScene().name;
        skipTransition = (currentScene != "Menu");

        StartCoroutine(LoadSceneAsync(sceneName, transitionName));
    }

    IEnumerator LoadSceneAsync(string sceneName, string transitionName)
    {
        SceneTransition transition = transitions.First(t => t.name == transitionName);

        AsyncOperation scene = SceneManager.LoadSceneAsync(sceneName);
        scene.allowSceneActivation = false;

        // Add this if statement
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

        scene.allowSceneActivation = true;
        progressBar.gameObject.SetActive(false);

        // Add this if statement
        if (!skipTransition)
        {
            yield return transition.AnimateTransitionOut();
        }
    }
}