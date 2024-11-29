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

    [SerializeField] float shakeForce = 0.7f;
    public float shakeMagnitude = 0.1f;
    // The speed at which the shake decays (higher value = faster decay)
    public float shakeDampingSpeed = 1.0f;


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

    public void Shake() 
    {

        StartCoroutine(Shake(0.2f));
    }

    private IEnumerator Shake(float duration)
    {

       Vector3 originalPosition = transform.localPosition;
        float elapsedTime = 0f;

        shakeMagnitude = shakeForce;

        while (elapsedTime < duration)
        {
            // Generate a random shake offset (can customize for more control)
            Vector3 shakeOffset = Random.insideUnitSphere * shakeMagnitude;

            // Apply the shake offset to the camera position
            transform.localPosition = originalPosition + shakeOffset;

            // Increment the elapsed time
            elapsedTime += Time.deltaTime;

            // Optionally, you can add a damping factor to slow down the shake as it goes on
            shakeMagnitude = Mathf.Lerp(shakeMagnitude, 0f, elapsedTime / duration);

            // Wait for the next frame before continuing
            yield return null;
        }

        // After the shake ends, return the camera to its original position
        transform.localPosition = originalPosition;

        Debug.Log("Sahke");
    }


}
