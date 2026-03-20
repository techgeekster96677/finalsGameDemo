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

    public void Restart()
    {
        Time.timeScale = 1f;
        Debug.Log("Restart button clicked!");

        if (LevelManager.instance == null)
        {
            Debug.LogError("LevelManager.instance is NULL! Make sure LevelManager is in the scene.");
            return;
        }

        string currentSceneName = SceneManager.GetActiveScene().name;
        Debug.Log($"Restarting current level: {currentSceneName}");

        LevelManager.instance.LoadScene(currentSceneName, "CrossFade");
    }

    //public void ReturnToMenu()
    //{
    //    Time.timeScale = 1f;
    //    LevelManager.instance.LoadScene("Menu", "CrossFade");
    //}

    public void QuitGame()
    {
        Application.Quit();
        UnityEditor.EditorApplication.isPlaying = false;
    }

    public void NextLevel()
    {
        Time.timeScale = 1f;

        string currentSceneName = SceneManager.GetActiveScene().name;

        if (currentSceneName.StartsWith("Level "))
        {
            string levelNumberStr = currentSceneName.Replace("Level ", "");
            if (int.TryParse(levelNumberStr, out int currentLevel))
            {
                int nextLevel = currentLevel + 1;

                if (nextLevel <= totalLevels)
                {
                    string nextSceneName = $"Level {nextLevel}";
                    Debug.Log($"Loading next level: {nextSceneName}");

                    if (LevelManager.instance != null)
                    {
                        LevelManager.instance.LoadScene(nextSceneName, "CrossFade");
                    }
                    else
                    {
                        SceneManager.LoadScene(nextSceneName);
                    }
                }
                //else
                //{
                //    Debug.Log("All levels completed! Returning to menu...");
                //    ReturnToMenu();
                //}
            }
            else
            {
                Debug.LogError($"Could not parse level number from: {currentSceneName}");
                LoadFallbackLevel();
            }
        }
        else
        {
            Debug.LogError($"Scene name format incorrect: {currentSceneName}");
            LoadFallbackLevel();
        }
    }

    private void LoadFallbackLevel()
    {
        if (LevelManager.instance != null)
        {
            LevelManager.instance.LoadScene("Level 2", "CrossFade");
        }
        else
        {
            SceneManager.LoadScene("Level 2");
        }
    }
}