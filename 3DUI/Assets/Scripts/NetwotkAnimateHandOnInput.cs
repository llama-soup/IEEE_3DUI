/// <summary>
/// Manages animating pinch and grab hand animations in multiplayer
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Unity.Netcode;

public class NetwotkAnimateHandOnInput : NetworkBehaviour
{
    public InputActionProperty pinchAnimationAction;
    public InputActionProperty gripAnimationAction;
    public Animator handAnimator;

    void Update()
    {
        if(IsOwner)
        {
            float triggerValue = pinchAnimationAction.action.ReadValue<float>();
            handAnimator.SetFloat("Trigger", triggerValue);

            float gribValue = gripAnimationAction.action.ReadValue<float>();
            handAnimator.SetFloat("Grip", gribValue);
        }
    }
}
