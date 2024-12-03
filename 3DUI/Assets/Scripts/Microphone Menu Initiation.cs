using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using TMPro;


public class MicrophoneMenuInitiation : MonoBehaviour
{
    [SerializeField] private TMP_Dropdown dropdown;
    // Initalizes the microphone menu
    void Start()
    {
        #if UNITY_WEBGL && !UNITY_EDITOR
        dropdown.options.Add(new TMP_Dropdown.OptionData("Microphone not supported on WebGL"));
        #else
        //Checks for all the microphones available and adds them to the dropdown
        foreach (var device in Microphone.devices)
        {
            dropdown.options.Add(new TMP_Dropdown.OptionData(device));
        }
        #endif
    }
}