using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    // Reference to the target (e.g., player or object to follow)
    public Transform target;

    // Offset distance between the camera and the target
    public Vector3 offset;

    // Smooth follow speed
    public float smoothSpeed = 0.125f;


    private void Awake()
    {
        target = GameObject.FindGameObjectWithTag("Player").transform;
    }

    void LateUpdate()
    {
        // Ensure the target exists
        if (target != null)
        {
            // Calculate the desired position (target position + offset)
            Vector3 desiredPosition = target.position + offset;

            // Smoothly interpolate the camera's position towards the desired position
            Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

            // Update the camera's position
            transform.position = smoothedPosition;

            // Optionally, you can make the camera always look at the target
            
        }
    }
}
