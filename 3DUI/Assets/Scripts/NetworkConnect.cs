using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode;

public class NetworkConnect : MonoBehaviour
{

    public void Host()
    {
        NetworkManager.Singleton.StartHost();
    }

    public void Join()
    {
        NetworkManager.Singleton.StartClient();
    }
}
