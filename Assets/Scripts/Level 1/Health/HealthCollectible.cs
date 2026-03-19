using UnityEngine;

public class HealthCollectible : MonoBehaviour
{
    [SerializeField] private float healthAmount;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            collision.GetComponent<Health>().Heal(healthAmount);
            SoundManager.Instance.PlaySound2D("Health Pickup");
            gameObject.SetActive(false);
        }
    }

}
