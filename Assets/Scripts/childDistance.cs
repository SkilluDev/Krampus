using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class childDistance : MonoBehaviour
{
    GameObject[] children;
    public float dist = 100000;
    Transform closestChild;

    private void Start()
    {
        children = GameObject.FindGameObjectsWithTag("Child");
        closestChild = children[0].transform;
    }

    private void Update()
    {
        children = GameObject.FindGameObjectsWithTag("Child");
        Vector3 closestOffset = closestChild.transform.position - transform.position;
        dist = closestOffset.sqrMagnitude;

        foreach (GameObject child in children)
        {
            if (child.GetComponent<Child>().isBad == true)
            {
                Vector3 offset = child.transform.position - transform.position;
                float sqrLen = offset.sqrMagnitude;

                if (sqrLen < dist)
                {
                    closestChild = child.transform;
                }
            }
        }

        //Debug.Log(Mathf.Sqrt(dist));
    }
}
