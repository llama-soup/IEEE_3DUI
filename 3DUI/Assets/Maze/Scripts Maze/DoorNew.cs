using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorNew : MonoBehaviour
{
    private Renderer _renderer;
    bool open = false;
    // Start is called before the first frame update
    void Start()
    {
        _renderer = GetComponent<Renderer>();
    }

    private void OnMouseDown()
    { //test
        //close
        if(open)
        {
            _renderer.transform.Rotate(Vector3.up, 90);
            _renderer.transform.position = _renderer.transform.position + new Vector3(0.5f*GetComponent<Renderer>().bounds.size.z, 0, 5*GetComponent<Renderer>().bounds.size.x);
            //
        }
        //opening
        else
        {
            _renderer.transform.Rotate(Vector3.up, -90);
            _renderer.transform.position = _renderer.transform.position + new Vector3(-0.5f*GetComponent<Renderer>().bounds.size.x, 0, -5*GetComponent<Renderer>().bounds.size.z);
        }
        //
        open = !open;
    }

}