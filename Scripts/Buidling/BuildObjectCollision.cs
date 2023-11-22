using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuildObjectCollision : MonoBehaviour
{
    public bool isColliding = false;
    public Material previewTrueMaterial;
    public Material previewFalseMaterial;
    private Renderer objRenderer;
    private Material[] tempRenderer;

    private void Awake()
    {
        objRenderer = GetComponent<Renderer>();
        tempRenderer = GetComponent<Renderer>().materials;
    }

    void OnTriggerStay(Collider other)
    {
        if (!isColliding)
        {
            isColliding = true;
            //objRenderer.material = previewFalseMaterial;
            for(int i = 0; i < objRenderer.materials.Length; i++)
            {
                tempRenderer[i] = previewFalseMaterial;
            }
            objRenderer.materials = tempRenderer;
        }
    }

    void OnTriggerExit(Collider other)
    {
        isColliding = false;
        //objRenderer.material = previewTrueMaterial;
        for (int i = 0; i < objRenderer.materials.Length; i++)
        {
            tempRenderer[i] = previewTrueMaterial;
        }
        objRenderer.materials = tempRenderer;
    }
}
