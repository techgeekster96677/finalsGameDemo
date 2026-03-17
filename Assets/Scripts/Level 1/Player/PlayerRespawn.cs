using UnityEngine;

public class PlayerRespawn : MonoBehaviour
{
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
            SoundManager.Instance.PlaySound2D("Checkpoint Appear");
            transform.position = currentCheckpoint.position;
            playerHealth.Respawn();
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
            SoundManager.Instance.PlaySound2D("Checkpoint Appear");
            collision.GetComponent<Collider2D>().enabled = false; // Disable collider to prevent multiple triggers
            collision.GetComponent<Animator>()?.SetTrigger("appear");
        }
    }
}
