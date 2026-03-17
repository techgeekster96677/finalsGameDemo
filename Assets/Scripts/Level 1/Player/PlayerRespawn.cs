using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
    [SerializeField] private AudioClip checkpointSound;
    private Transform currentCheckpoint;
    private Health playerHealth;
    private void Awake()
    {
        playerHealth = GetComponent<Health>();
    }

    public void RespawnToCheckpoint()
    {
        if (currentCheckpoint != null)
        {
            transform.position = currentCheckpoint.position;
            playerHealth.Respawn();
            AudioSource.PlayClipAtPoint(checkpointSound, transform.position);

        }
        else
        {
            Debug.LogWarning("No checkpoint set for respawn!");
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if(collision.CompareTag("Checkpoint"))
        {
            currentCheckpoint = collision.transform;
            AudioSource.PlayClipAtPoint(checkpointSound, transform.position);
            collision.GetComponent<Collider2D>().enabled = false; // Disable collider to prevent multiple triggers
            collision.GetComponent<Animator>()?.SetTrigger("appear");
        }
    }
}
