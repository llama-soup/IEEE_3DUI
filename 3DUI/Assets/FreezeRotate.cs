using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/* Holds the x and y rotations of the object this script is attached to constant
 * to the x and y rotations of the Wire Puzzle.
 *
 * Author: Alexander Li
 */
public class FreezeRotate : MonoBehaviour
{
    GameObject parent;

    void Start()
    {
        parent = GameObject.Find("Wires");
    }

    // Update is called once per frame. Fixes the rotatable wires' x and y rotations so that
    // they will both always be constant to that of their parent - the wire puzzle itself, meaning when players grab the wires, they
    // can only be rotated around the z-axis.
    void Update()
    {
        if(transform.eulerAngles.y != parent.transform.eulerAngles.y || transform.eulerAngles.x != parent.transform.eulerAngles.x)
        {
            transform.rotation = Quaternion.Euler(parent.transform.eulerAngles.x, parent.transform.eulerAngles.y, transform.eulerAngles.z);
        }
    }
}
