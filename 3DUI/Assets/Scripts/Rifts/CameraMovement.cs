using UnityEngine;

public class CameraMovement : MonoBehaviour
{
    public float speed = 50f;
    
    void Update()
    {
        float xAxisValue = Input.GetAxis("Horizontal");
        float zAxisValue = Input.GetAxis("Vertical");
        
        Vector3 movement = new Vector3(xAxisValue, 0f, zAxisValue);
        transform.position += movement * speed * Time.deltaTime;
        
        // Optional: Add up/down movement with Q/E
        if (Input.GetKey(KeyCode.Q))
            transform.position += Vector3.down * speed * Time.deltaTime;
        if (Input.GetKey(KeyCode.E))
            transform.position += Vector3.up * speed * Time.deltaTime;
    }
}