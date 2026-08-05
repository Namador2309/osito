using UnityEngine;

public class FondoMovimiento : MonoBehaviour
{
    private float startPos, length;
    public GameObject cam;
    public float parallaxEffect;

   private void Start()
    {
        startPos = transform.position.x;
        length = GetComponent<Renderer>().bounds.size.x;
    }
    private void FixedUpdate()
    {
    
        float dist = (cam.transform.position.x * parallaxEffect);
        float movement = cam.transform.position.x * (1 - parallaxEffect);
        transform.position = new Vector3(startPos + dist, transform.position.y, transform.position.z);

        if (movement > startPos + length)
        {
            startPos += length;
        }
        else if (movement < startPos - length)
        {
            startPos -= length;
        }
    }
}