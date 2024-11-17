using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ChildSpawner : MonoBehaviour
{
    public Transform[] spawnPoint;

    public GameObject childTemplate;

    public Material[] materials;

    public int goodChildrenCountMax;

    string wrongChildrenText;

    private void Start()
    {
        
        int badColor = Random.Range(0, materials.Length);
        if (badColor == 0)
        {
            wrongChildrenText = "green shirts";
        }
        else if (badColor == 1)
        {
            wrongChildrenText = "blue shirts";
        }
        else if (badColor == 2)
        {
            wrongChildrenText = "purple shirts";
        }
        else if (badColor == 3)
        {
            wrongChildrenText = "red shirts";
        }
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
        GameObject.Find("shirttext").GetComponent<Text>().text = wrongChildrenText;
    }

    void CreateChild(Transform spawn, Material material, bool isBad)
    {
        Child newChild = Instantiate(childTemplate, spawn.position, Quaternion.identity).GetComponent<Child>();
        newChild.mat = material;
        newChild.isBad = isBad;
    }

}