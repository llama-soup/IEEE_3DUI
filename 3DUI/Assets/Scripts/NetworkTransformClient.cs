/// <summary>
/// Overrides server authoritative logic for object transforms
/// </summary>

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Netcode.Components;

public class NetworkTransformClient : NetworkTransform
{
    protected override bool OnIsServerAuthoritative()
    {
        return false;
    }
}
