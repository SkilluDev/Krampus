using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChildDistance : MonoBehaviour {
    private GameObject[] children;
    public float dist = 100000;
    private Transform closestChild;

    private void Start() {
        children = GameObject.FindGameObjectsWithTag("Child");
        closestChild = children[0].transform;
    }

    private void Update() {
        children = GameObject.FindGameObjectsWithTag("Child");
        if (!closestChild) {
            closestChild = children[0].transform;
        }
        Vector3 closestOffset = closestChild.transform.position - transform.position;
        dist = closestOffset.sqrMagnitude;

        foreach (GameObject child in children) {
            if (child.GetComponent<Child>().isBad) {
                Vector3 offset = child.transform.position - transform.position;
                float sqrLen = offset.sqrMagnitude;

                if (sqrLen < dist) {
                    closestChild = child.transform;
                }
            }
        }

        //Debug.Log(Mathf.Sqrt(dist));
    }
}
