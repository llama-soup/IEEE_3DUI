using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit;

public class RotateWire : MonoBehaviour
{
    public Transform visualTarget;

    private XRBaseInteractable interactable;
    private bool rotating = false;
    // Start is called before the first frame update
    void Start()
    {
        interactable = GetComponent<XRBaseInteractable>();
        interactable.hoverEntered.AddListener(Rotate);
        // interactable.selectEntered.AddListener(Rotate);
    }
    // BaseInteractionEventArgs select
    public void Rotate(BaseInteractionEventArgs hover)
    {
        if(hover.interactorObject is XRPokeInteractor){
            rotating = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if(rotating)
        {
            visualTarget.transform.Rotate(0.0f, 0.0f, -90.0f);
            rotating = false;
        }
    }
}
