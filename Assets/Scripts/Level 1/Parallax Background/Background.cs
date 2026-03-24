using UnityEngine;

/// <summary>
/// Creates a parallax scrolling effect for background elements.
/// Moves the background at a different speed relative to the camera to simulate depth.
/// </summary>
public class Background : MonoBehaviour
{
    private float startPos;
    public GameObject cam;
    public float parallaxEffect; //speed at which the camera moves with the background

    /// <summary>
    /// Stores the initial X position of the background on start.
    /// Used as the reference point for parallax movement.
    /// </summary>
    void Start()
    {
        startPos = transform.position.x;
    }

    /// <summary>
    /// Updates background position in FixedUpdate for smooth camera following.
    /// 
    /// PARALLAX VALUES:
    /// - 0 = moves with camera (no parallax)
    /// - 0.5 = moves slower than camera (mid-ground)
    /// - 1 = moves at same speed as camera (foreground)
    /// </summary>
    void FixedUpdate()
    {
        float distance = cam.transform.position.x * parallaxEffect;
        // 0 = move with cam | 1 = move with cam but slower | 0.5 = move slower

        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
    }
}