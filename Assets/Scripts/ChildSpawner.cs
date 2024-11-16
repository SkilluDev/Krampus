using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildSpawner : MonoBehaviour
{
    public Transform[] spawnPoint;

    public GameObject childTemplate;

    public Material[] materials;

    public int goodChildrenCountMax;

    private void Start()
    {
        int badColor = Random.Range(0, materials.Length);
        foreach (Transform point in spawnPoint)
        {
            int mat = Random.Range(0, materials.Length);
            if (goodChildrenCountMax == 0)
            {
                mat = badColor;
            }
            bool bad = false;
            goodChildrenCountMax--;
            if (mat == badColor)
            {
                goodChildrenCountMax++;
                bad = true;
            }
            CreateChild(point, materials[mat], bad);
        }
    }

    void CreateChild(Transform spawn, Material material, bool isBad)
    {
        Child newChild = Instantiate(childTemplate, spawn.position, Quaternion.identity).GetComponent<Child>();
        newChild.mat = material;
        newChild.isBad = isBad;
    }

}