using UnityEngine;
using UnityEngine.UI;

public class Healthbar : MonoBehaviour
{
    [SerializeField] private Health playerHealth;
    [SerializeField] private Image totalHealthBar;
    [SerializeField] private Image currentHealthBar;

    public void Start()
    {
        totalHealthBar.fillAmount = playerHealth.currentHealth / 10;
        
    }

    public void Update()
    {
       // currentHealthBar.fillAmount = playerHealth.currentHealth / playerHealth.startingHealth;
        currentHealthBar.fillAmount = playerHealth.currentHealth / 10;

    }
}
