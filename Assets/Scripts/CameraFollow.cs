using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class CameraFollow : MonoBehaviour {
	// Reference to the target (e.g., player or object to follow)
	public Transform target;
	//all the neccesary krampus components so we only call for them once
	private GameObject krampus;
	private Rigidbody m_kRigidBody;
	private KrampusController m_kController;
	//Camera values
	private Camera m_camera;
	private float m_originalCameraSize;
	private bool m_zoomOut;
	[SerializeField] private float m_cameraSizeAddition;
	[SerializeField] private float m_velocityLookAheadDamp;
	//[SerializeField] private float cameraZoomDivider; //part of velocity based camera

	// Offset distance between the camera and the target
	public Vector3 offset;

	// Smooth follow speed
	public float smoothSpeed = 0.05f;

	[SerializeField] private float shakeForce = 0.7f;
	public float shakeMagnitude = 0.1f;
	// The speed at which the shake decays (higher value = faster decay)
	public float shakeDampingSpeed = 1.0f;


	private void Awake() {
		krampus = GameObject.FindGameObjectWithTag("Player");
		target = krampus.transform;
		m_kRigidBody = krampus.GetComponent<Rigidbody>();
		m_kController = krampus.GetComponent<KrampusController>();

		m_camera = gameObject.GetComponent<Camera>();
		m_originalCameraSize = m_camera.orthographicSize;

	}

	private void LateUpdate() {
		// Ensure the target exists
		if (target != null) {
			// Calculate the desired position (target position + offset)
			Vector3 desiredVelOffset = Vector3.Lerp(new Vector3(0, 0, 0), (m_kRigidBody.velocity / m_velocityLookAheadDamp), smoothSpeed);
			Vector3 desiredPosition = target.position + offset + (desiredVelOffset);

			// Smoothly interpolate the camera's position towards the desired position
			Vector3 smoothedPosition = Vector3.Lerp(transform.position, desiredPosition, smoothSpeed);

			// Update the camera's position
			transform.position = smoothedPosition;

			// Optionally, you can make the camera always look at the target

		}

		if (m_camera.orthographic) {
			//velocity based camera increase
			//cameraSizeAddition = Mathf.Abs((cameraZoomDivider * (kRigidBody.velocity.magnitude))); //17 is more or less max velocity
			//desiredCameraSize = (originalCameraSize + cameraSizeAddition);

			//separate camera increase, the one that we want
			//if moving and running make the camera size wider, otherwise we want to use the original size
			if (m_kRigidBody.velocity.magnitude > 0 && m_kController.isRunning) m_zoomOut = true;
			else m_zoomOut = false;

			//smoothly Lerp between current and desired
			float smoothedCameraSize;
			if (m_zoomOut) {
				smoothedCameraSize = Mathf.Lerp(m_camera.orthographicSize, m_originalCameraSize + m_cameraSizeAddition, smoothSpeed);
			} else {
				smoothedCameraSize = Mathf.Lerp(m_camera.orthographicSize, m_originalCameraSize, smoothSpeed / 2);
			}
			//actually change the camera size
			m_camera.orthographicSize = smoothedCameraSize;

		}

	}

	public void Shake() {

		StartCoroutine(Shake(0.2f));
	}

	private IEnumerator Shake(float duration) {

		Vector3 originalPosition = transform.localPosition;
		float elapsedTime = 0f;

		shakeMagnitude = shakeForce;

		while (elapsedTime < duration) {
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

		Debug.Log("Shake");
	}


}
