using UnityEngine;

public class Background : MonoBehaviour
{
    private float startPos;
    public GameObject cam;
    public float parallaxEffect; //speed at which the camera moves with the background
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPos = transform.position.x;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        float distance = cam.transform.position.x * parallaxEffect; 
        // 0 = move with cam | 1 = move with cam but slower | 0.5 = move slower

        transform.position = new Vector3(startPos + distance, transform.position.y, transform.position.z);
    }
}
