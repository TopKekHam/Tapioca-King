using System;
using System.Collections.Generic;
using UnityEngine;



public class HighlightableMesh : MonoBehaviour, Highlightable
{
    public Material highlightMaterial;
    public MeshRenderer meshRenderer;

    Material[] materials;
    Material[] materialsWithHighLight;

    void Start()
    {
        
        materials = new Material[meshRenderer.materials.Length];
        materialsWithHighLight = new Material[materials.Length + 1];
        Array.Copy(meshRenderer.materials, materials, materials.Length);
        Array.Copy(meshRenderer.materials, materialsWithHighLight, materials.Length);
        materialsWithHighLight[^1] = highlightMaterial;
    }

    public void DeHighlight()
    {
        meshRenderer.materials = materials;
    }

    public void Highlight()
    {
        meshRenderer.materials = materialsWithHighLight;
    }
}
