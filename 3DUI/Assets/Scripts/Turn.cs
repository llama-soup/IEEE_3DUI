using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Turn : MonoBehaviour
{
    private Transform localTransform;
    public float xYRot = 0.0f;

    // Start is called before the first frame update
    void Start()
    {
        localTransform = GetComponent<Transform>();
    }

    private void OnMouseDown()
    {
        // _renderer.transform.Rotate(0.0f, 0.0f, -90.0f);
    }
}
