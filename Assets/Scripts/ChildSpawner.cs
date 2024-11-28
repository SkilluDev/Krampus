using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;
using Random = UnityEngine.Random;

public class ChildSpawner : MonoBehaviour
{

    private Vector3[] randomSpawnPoints;

    public GameObject childTemplate;

    public Material[] materials;
    
    public Transform spawnPoint;

    public static int goodChildrenCount;
    public static int badChildrenCount;
    
    [SerializeField] int spawnPointCount = 30;

    string goodChildrenText;

    private void Start()
    {
        
        int goodColor = Random.Range(0, materials.Length);
        if (goodColor == 0)
        {
            goodChildrenText = "green shirts";
        }
        else if (goodColor == 1)
        {
            goodChildrenText = "blue shirts";
        }
        else if (goodColor == 2)
        {
            goodChildrenText = "purple shirts";
        }
        else if (goodColor == 3)
        {
            goodChildrenText = "red shirts";
        }
        else if (goodColor == 4)
        {
            goodChildrenText = "yellow shirts";
        }
        randomSpawnPoints = new Vector3[spawnPointCount];
        for (int i = 0; i < spawnPointCount; i++)
        {
            randomSpawnPoints[i] = RandomPoint(transform.position, 100);
        }
        foreach (Vector3 point in randomSpawnPoints)
        {
            int mat = Random.Range(0, materials.Length);
            bool isBad = true;
            if (mat == goodColor)
            {
                goodChildrenCount++;
                isBad = false;
            }
            else
            {
                badChildrenCount++;
            }
            CreateChild(point, materials[mat], isBad);
        }

        Child newChild = CreateChild(spawnPoint.position, materials[Math.Abs(goodColor-1)%materials.Length], true);
        Destroy(newChild.GetComponent<Rigidbody>());
        newChild.GetComponent<ChildContoller>().isDummy = true;
        Debug.Log(newChild.name+": Empty child");
        //Destroy(newChild.GetComponent<NavMeshAgent>());
        //Destroy(newChild.GetComponent<ChildContoller>());
        badChildrenCount++;
        GameObject.Find("shirttext").GetComponent<Text>().text = goodChildrenText;
    }

    Child CreateChild(Vector3 spawn, Material material, bool isBad)
    {
        Child newChild = Instantiate(childTemplate, spawn, Quaternion.identity).GetComponent<Child>();
        newChild.mat = material;
        newChild.isBad = isBad;
        return newChild;
    }
    
    Vector3 RandomPoint(Vector3 center, float range) {
        Vector3 result;
        while(true) {
            Vector2 point = Random.insideUnitCircle;
            Vector3 randomPoint = center + new Vector3(point.x,0,point.y) * range;
            NavMeshHit hit;
            if (NavMesh.SamplePosition(randomPoint, out hit, 1.0f, NavMesh.AllAreas)) {
                result = hit.position;
                return result;
            }
        }
    }
}