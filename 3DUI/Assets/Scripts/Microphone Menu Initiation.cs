using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;


public class MicrophoneMenyInitiation : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    // Start is called before the first frame update
    void Start()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        dropdown.options.Add(new TMP_Dropdown.OptionData("Microphone not supported on WebGL"));
        #else
        foreach (var device in Microphone.devices)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(device));
        }
        #endif
    }
}