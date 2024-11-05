using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Rift : MonoBehaviour 
{
    public Rift linkedRift;
    public bool isActive = true;
    
    private MeshRenderer meshRenderer;
    
    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        
        // Set up rift material
        Material riftMaterial = new Material(Shader.Find("Unlit/Texture"));
        meshRenderer.material = riftMaterial;
    }

    void OnWillRenderObject()
    {
        if (!isActive) return;
        
        // Set up culling to only show what's through the rift
        Camera.main.cullingMask = ~(1 << gameObject.layer);
    }
}
