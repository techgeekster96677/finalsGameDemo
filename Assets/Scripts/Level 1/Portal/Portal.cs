using UnityEngine;

public class Portal : MonoBehaviour
{
    private UIManager uiManager;

    public void Awake()
    {
        uiManager = FindFirstObjectByType<UIManager>();
    }

    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.gameObject.CompareTag("Player"))
        {
            Time.timeScale = 0; // Ensure time scale is reset in case it was modified    
            uiManager.ShowNextLevelUI();
            SoundManager.Instance.PlaySound2D("Portal Enter");
        }
    }
}
