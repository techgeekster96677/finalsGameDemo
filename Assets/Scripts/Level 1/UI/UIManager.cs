using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages UI elements across levels including game over, next level prompts, and scene navigation.
/// Handles level completion transitions, restart functionality, and music playback per scene.
/// </summary>
public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameoverUI;
    [SerializeField] private GameObject nextLevelUI;
    [SerializeField] private int totalLevels = 3;
    public static UIManager instance;

    /// <summary>
    /// Initializes UI elements to hidden state on awake.
    /// </summary>
    public void Awake()
    {
        gameoverUI.SetActive(false);
        nextLevelUI.SetActive(false);
    }

    /// <summary>
    /// Plays the appropriate music for the current scene when the UIManager starts.
    /// </summary>
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

    /// <summary>
    /// Called when a new scene is loaded. Updates music to match the new scene.
    /// </summary>
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        PlayMusicForCurrentScene();
    }

    /// <summary>
    /// Plays music based on the current scene name.
    /// - Level scenes: Plays "Level X" music where X is the level number
    /// - Menu scene: Plays "Menu" music
    /// </summary>
    private void PlayMusicForCurrentScene()
    {
        string sceneName = SceneManager.GetActiveScene().name;

        if (sceneName.StartsWith("Level "))
        {
            // Extract level number and play corresponding music
            string levelNumberStr = sceneName.Replace("Level ", "");
            if (int.TryParse(levelNumberStr, out int levelNumber))
            {
                // Play music for the current level
                MusicManager.instance.PlayMusic($"Level {levelNumber}");
            }
        }
        else if (sceneName == "Menu")
        {
            // Play menu music if we're in the menu scene
            MusicManager.instance.PlayMusic("Menu");
        }
    }

    /// <summary>
    /// Shows the next level UI when the player completes a level.
    /// Plays level complete sound effect.
    /// </summary>
    public void ShowNextLevelUI()
    {
        nextLevelUI.SetActive(true);
        SoundManager.Instance.PlaySound2D("Level Complete");
    }

    /// <summary>
    /// Shows the game over UI when the player dies.
    /// Plays game over sound effect.
    /// </summary>
    public void ShowGameOverUI()
    {
        gameoverUI.SetActive(true);
        SoundManager.Instance.PlaySound2D("Game Over");
    }

    /// <summary>
    /// Restarts the current level using LevelManager for smooth transition.
    /// Resets time scale to normal before reloading.
    /// </summary>
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

    /// <summary>
    /// Quits the game application.
    /// In Editor, stops Play Mode instead.
    /// </summary>
    public void QuitGame()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    /// <summary>
    /// Returns to the main menu using LevelManager for smooth transition.
    /// Resets time scale before loading.
    /// </summary>
    public void MainMenu()
    {
        Debug.Log("Main Menu button clicked!");
        Time.timeScale = 1f;

        // Check if LevelManager instance exists
        if (LevelManager.instance == null)
        {
            Debug.LogError("LevelManager.instance is NULL! Make sure LevelManager is in the scene.");
            return;
        }

        // Load the main menu scene using LevelManager with transition
        Debug.Log("Loading Menu scene via LevelManager");
        LevelManager.instance.LoadScene("Menu", "CrossFade");
    }

    /// <summary>
    /// Loads the next level based on the current scene name.
    /// - Extracts level number from current scene name
    /// - Calculates next level number
    /// - Loads next level if within total levels
    /// - Falls back to Level 2 if parsing fails
    /// </summary>
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

    /// <summary>
    /// Loads a fallback level (Level 2) if an error occurs while determining the next level.
    /// Uses LevelManager if available, otherwise falls back to direct scene loading.
    /// </summary>
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