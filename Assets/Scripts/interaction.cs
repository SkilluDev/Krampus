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

        RaycastHit hit;

        if (Input.GetButtonDown("Fire1"))
        {
            if (Physics.Raycast(transform.position, dir, out hit , tongueLength, tonguable) && !Physics.Raycast(transform.position, dir,(hit.rigidbody.position - transform.position).magnitude, 1<<6))
            {
                Vector3.Lerp(hit.rigidbody.position, transform.position, 0.5f);

            }

        }

    }
}
