using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class SnapRotation : MonoBehaviour
{
    private XRBaseInteractable interactable;

    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<XRBaseInteractable>();
        interactable.selectExited.AddListener(Snap);
    }

    // Runs when player releases a wire they were grabbing and rotating. Checks
    // the rotation of the wire and automatically sets it to one of four rotation
    // positions - 0 degrees, 90 degrees, 180 degrees, or 270 degrees - depending on
    // which of the four positions they are closest to.
    public void Snap(BaseInteractionEventArgs select)
    {
        float anglesZ = transform.rotation.eulerAngles.z;

        if((anglesZ >= 315 && anglesZ <= 360) || (anglesZ >= 0 && anglesZ < 45))
        {
            transform.rotation = Quaternion.Euler(0, 0, 0);
        }
        else if((anglesZ >= 45 && anglesZ <= 90) || (anglesZ >= 90 && anglesZ < 135))
        {
            transform.rotation = Quaternion.Euler(0, 0, 90);
        }
        else if((anglesZ >= 135 && anglesZ <= 180) || (anglesZ >= 180 && anglesZ < 225))
        {
            transform.rotation = Quaternion.Euler(0, 0, 180);
        }
        else
        {
            transform.rotation = Quaternion.Euler(0, 0, 270);
        }
    }
}
