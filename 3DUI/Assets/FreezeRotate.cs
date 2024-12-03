using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FreezeRotate : MonoBehaviour
{
    GameObject parent;

    void Start()
    {
        parent = GameObject.Find("Wires");
    }

    // Update is called once per frame
    void Update()
    {
        if(transform.eulerAngles.y != parent.transform.eulerAngles.y || transform.eulerAngles.x != parent.transform.eulerAngles.x)
        {
            transform.rotation = Quaternion.Euler(parent.transform.eulerAngles.x, parent.transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }
}
