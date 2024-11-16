using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildList : MonoBehaviour
{

    public Material[] materials;

    public GameObject[] objectsToChange;

    void ApplyMaterialsToObjects()
    {
        foreach (GameObject obj in objectsToChange)
        {
            ChangeMaterialRandomly(obj);
        }
    }

    void ChangeMaterialRandomly(GameObject obj)
    {
        Renderer renderer = obj.GetComponent<Renderer>();

        if (renderer != null)
        {
            int randomIndex = Random.Range(0, materials.Length);
            renderer.material = materials[randomIndex];
        }
    }


    void Start()
    {
        ApplyMaterialsToObjects();

    }

    void Update()
    {
        
    }

}