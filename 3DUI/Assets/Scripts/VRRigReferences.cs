using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class VRRigReferences : MonoBehaviour
{
    public static VRRigReferences Singleton;
    

    public Transform root;
    public Transform head;
    public Transform leftHand;
    public Transform rightHand;
    
    private void Awake()
    {
        Singleton = this;
    }


    [SerializeField] private InputActionReference positionAction;
    [SerializeField] private InputActionReference rotationAction;

    private void OnEnable()
    {
        positionAction.action.Enable();
        rotationAction.action.Enable();
    }

    private void OnDisable()
    {
        positionAction.action.Disable();
        rotationAction.action.Disable();
    }

    private void Update()
    {
        Vector3 position = positionAction.action.ReadValue<Vector3>();
        Quaternion rotation = rotationAction.action.ReadValue<Quaternion>();

        Debug.Log($"Position: {position}, Rotation: {rotation}");
    }
}
