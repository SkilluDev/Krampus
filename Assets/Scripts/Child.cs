using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Child : MonoBehaviour
{
    public bool isBad;
    public Material mat;

    private void Start()
    {
        transform.GetChild(8).GetComponent<Renderer>().material = mat;
    }

    public void Die() { }
}
