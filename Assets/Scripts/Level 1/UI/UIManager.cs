using UnityEngine;

public class UIManager : MonoBehaviour
{
    [SerializeField] private GameObject gameoverUI;
    [SerializeField] private GameObject nextLevelUI;
    public static UIManager instance;

    public void Awake()
    {
        gameoverUI.SetActive(false);
        nextLevelUI.SetActive(false);
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
        Time.timeScale = 1f; // Ensure time scale is reset in case it was modified
        Debug.Log("Restart button clicked!");

        if (LevelManager.instance == null)
        {
            Debug.LogError("LevelManager.instance is NULL! Make sure LevelManager is in the scene.");
            return;
        }

        Debug.Log($"LevelManager.instance found. Loading 'Level 1' with 'CrossFade' transition");
        LevelManager.instance.LoadScene("Level 1", "CrossFade");
    }

    public void ReturnToMenu()
    {
        Time.timeScale = 1f; // Ensure time scale is reset in case it was modified
        LevelManager.instance.LoadScene("Menu", "CrossFade");
    }

    public void QuitGame()
    {
        Application.Quit(); // for build
        UnityEditor.EditorApplication.isPlaying = false;    //Exit play mode
    }

    public void NextLevel()
    {
        Time.timeScale = 1f; // Ensure time scale is reset in case it was modified
        //LevelManager.instance.LoadScene("Level 2", "CrossFade");
        UnityEngine.SceneManagement.SceneManager.LoadScene("Level 2");
    }
}
