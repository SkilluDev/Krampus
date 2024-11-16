using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class interaction : MonoBehaviour
{
    public Camera cam;
    public float tongueLength;
    
    void Start()
    {
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }


    void Update()
    {
        Vector3 worldPosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5));
        Debug.DrawLine(transform.position,worldPosition);
        
        

        if (Input.GetButtonDown("Fire1"))
        {
            Debug.Log("yeah");
           
        }

    }
}
