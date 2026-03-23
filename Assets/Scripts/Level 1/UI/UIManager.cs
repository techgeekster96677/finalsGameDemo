using UnityEngine;
using UnityEngine.SceneManagement;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameoverUI;
    [SerializeField] private GameObject nextLevelUI;
    [SerializeField] private int totalLevels = 3;
    public static UIManager instance;

    public void Awake()
    {
        gameoverUI.SetActive(false);
        nextLevelUI.SetActive(false);
    }

    private void Start()
    {
        // Play music for the current level when UIManager starts
        PlayMusicForCurrentScene();
    }

    private void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForCurrentScene();
    }

    private void PlayMusicForCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.StartsWith("Level "))
        {
            // Extract level number and play corresponding music
            string levelNumberStr = sceneName.Replace("Level ", "");
            if (int.TryParse(levelNumberStr, out int levelNumber))
            {
                MusicManager.instance.PlayMusic($"Level {levelNumber}");
            }
        }
        else if (sceneName == "Menu")
        {
            MusicManager.instance.PlayMusic("Menu");
        }
    }

    public void ShowNextLevelUI()
    {
        nextLevelUI.SetActive(true);
        SoundManager.Instance.PlaySound2D("Level Complete");
    }

    public void ShowGameOverUI()
    {
        gameoverUI.SetActive(true);
        SoundManager.Instance.PlaySound2D("Game Over");
    }

    // This method will restart the current level when the restart button is clicked
    public void Restart()
    {
        Time.timeScale = 1f;
        Debug.Log("Restart button clicked!");

        // Check if LevelManager instance exists before trying to use it
        if (LevelManager.instance == null)
        {
            Debug.LogError("LevelManager.instance is NULL! Make sure LevelManager is in the scene.");
            return;
        }

        // Get the current scene name and load it again using LevelManager for a smooth transition
        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Restarting current level: {currentSceneName}");

        LevelManager.instance.LoadScene(currentSceneName, "CrossFade");
    }

    // This method will quit the game when the quit button is clicked
    public void QuitGame()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    // This method will load the next level based on the current scene name
    public void NextLevel()
    {
        Time.timeScale = 1f; // Ensure time scale is reset when moving to the next level

        // Get the current scene name and determine the next level
        string currentSceneName = SceneManager.GetActiveScene().name;

        // Check if the current scene name follows the "Level X" format
        if (currentSceneName.StartsWith("Level "))
        {
            // Extract the level number from the scene name
            string levelNumberStr = currentSceneName.Replace("Level ", "");
            if (int.TryParse(levelNumberStr, out int currentLevel))
            {
                // Calculate the next level number
                int nextLevel = currentLevel + 1;

                // Check if the next level exists before trying to load it
                if (nextLevel <= totalLevels)
                {
                    // Construct the next scene name and load it
                    string nextSceneName = $"Level {nextLevel}";
                    Debug.Log($"Loading next level: {nextSceneName}");

                    // Use LevelManager to load the next scene with a transition if it exists,
                    // otherwise load directly
                    if (LevelManager.instance != null)
                    {
                        LevelManager.instance.LoadScene(nextSceneName, "CrossFade");
                    }
                    else
                    {
                        SceneManager.LoadScene(nextSceneName);
                    }
                }
            }
            else
            {
                // If we can't parse the level number, log an error and load a fallback level
                Debug.LogError($"Could not parse level number from: {currentSceneName}");
                LoadFallbackLevel();
            }
        }
        else
        {
            // If the scene name doesn't follow the expected format, log an error and load a fallback level
            Debug.LogError($"Scene name format incorrect: {currentSceneName}");
            LoadFallbackLevel();
        }
    }

    // This method will load a fallback level (e.g., Level 2)
    // if we encounter an error determining the next level
    private void LoadFallbackLevel()
    {
        // Load a default level (e.g., Level 2) if we can't determine the next level
        if (LevelManager.instance != null)
        {
            // Load "Level 2" with a transition as a fallback
            LevelManager.instance.LoadScene("Level 2", "CrossFade");
        }
        else
        {
            // If LevelManager is not available, load "Level 2" directly as a fallback
            SceneManager.LoadScene("Level 2");
        }
    }
}