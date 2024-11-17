using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class interaction : MonoBehaviour
{
    public Camera cam;
    public float tongueLength;
    public LayerMask tonguable;
    
    void Start()
    {
        cam = GameObject.FindWithTag("MainCamera").GetComponent<Camera>();
    }



    void Update()
    {
        Vector3 worldPosition = cam.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, 5));
        Vector3 upray = new Vector3(worldPosition.x, transform.position.y, worldPosition.z);
        Vector3 dir = ( upray - transform.position ).normalized;
        
        //Debug.DrawLine(transform.position, worldPosition);
        //Debug.DrawLine(worldPosition, upray);
        Debug.DrawLine(transform.position, upray);
        Debug.DrawRay(transform.position, dir);   

        if (Input.GetButtonDown("Fire1"))
        {
            if (Physics.Raycast(transform.position, dir, tongueLength, tonguable))
            {
                Debug.Log("yeah");
            }
           
        }

    }
}
