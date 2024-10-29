using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Open : MonoBehaviour
{
    private Renderer _renderer;
    bool open = false;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void OnMouseDown()
    {
        if(open)
        {
            _renderer.transform.Rotate(Vector3.up, 90);
            _renderer.transform.position = _renderer.transform.position + new Vector3(0.5f, 0, -0.5f);
        }
        else
        {
            _renderer.transform.Rotate(Vector3.up, -90);
            _renderer.transform.position = _renderer.transform.position + new Vector3(-0.5f, 0, 0.5f);
        }

        open = !open;
    }
}
