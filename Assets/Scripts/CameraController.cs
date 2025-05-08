using System.Collections;
using UnityEngine;
using NaughtyAttributes;

// Based on @WalkanFL's camera
public class CameraController : MonoBehaviour {
	public Matrix4x4 Matrix { get; private set; }
	public Camera Raw => m_camera;
	[SerializeField] private Krampus m_krampus;
	[SerializeField] private Camera m_camera;
	[SerializeField] private AnimationCurve m_zoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

	[SerializeField] private Vector3 m_baseOffset = Vector3.one * 10;
	[BoxGroup("Movement")][SerializeField] private float m_lookAheadStrength;

	[BoxGroup("Zoom")][SerializeField] private float m_zoomedOrtoSize = 5;
	[BoxGroup("Zoom")][SerializeField] private float m_unzoomedOrtoSize = 8;
	[BoxGroup("Camera Smoothing")][SerializeField] private float m_cameraSpeed = 0.125f;
	[BoxGroup("Zoom")][SerializeField] private float m_zoomSpeed = 0.2f;
	[BoxGroup("Zoom")][SerializeField] private float m_zoomBuffer = 0.75f;
	[BoxGroup("Zoom")][SerializeField] private float m_unzoomSpeed = 1f;



	private float m_zoomFactor = 1;

	[BoxGroup("Shake")][SerializeField] private float shakeForce = 0.7f;
	[BoxGroup("Shake")][SerializeField] private float shakeMagnitude = 0.1f;
	// The speed at which the shake decays (higher value = faster decay)
	[BoxGroup("Shake")][SerializeField] float shakeDampingSpeed = 1.0f;

	private void Awake() {
		Matrix = Matrix4x4.Rotate(Quaternion.Euler(0, transform.eulerAngles.y, 0));
	}

	private void LateUpdate() {
		if (m_krampus == null || m_camera == null) return;

		transform.position = Vector3.Lerp(transform.position, m_krampus.transform.position + ComputeOffset(), m_cameraSpeed * Time.deltaTime);
		m_camera.orthographicSize = ComputeOrtoSize();
	}

	private Vector3 ComputeOffset() {
		var movementOffset = Vector3.ClampMagnitude(m_krampus.Kramp.Kontroller.VelocityVector, 1) * m_lookAheadStrength;
		return m_baseOffset + movementOffset;
	}

	private float ComputeOrtoSize() {
		if (m_krampus.Kontroller.CurrentState == KrampusController.State.Run) {
			m_zoomFactor -= m_unzoomSpeed * Time.deltaTime;
		} else {
			m_zoomFactor += m_zoomSpeed * Time.deltaTime;
		}
		m_zoomFactor = Mathf.Clamp(m_zoomFactor, -m_zoomBuffer, 1);

		return Mathf.Lerp(m_unzoomedOrtoSize, m_zoomedOrtoSize, m_zoomCurve.Evaluate(Mathf.Clamp01(m_zoomFactor)));
	}


	[Button("Set Current As Base")]
	private void SetBaseCurrent() {
		if (m_krampus == null || m_camera == null)
			throw new System.Exception("Objects are not assigned to the CameraController");

		transform.SetPositionAndRotation(m_camera.transform.position, m_camera.transform.rotation);
		m_camera.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		m_baseOffset = transform.position - m_krampus.transform.position;
		m_zoomedOrtoSize = m_camera.orthographicSize;
	}

	[Button("Show Base")]
	private void ShowBase() {
		if (m_krampus == null || m_camera == null)
			throw new System.Exception("Objects are not assigned to the CameraController");

		transform.position = m_krampus.transform.position + ComputeOffset();
		m_camera.orthographicSize = ComputeOrtoSize();
	}


	[Button("Shake")]
	public void Shake() {

		StartCoroutine(Shake(0.2f));
	}

	private IEnumerator Shake(float duration) {
		Vector3 originalPosition = transform.localPosition;
		float elapsedTime = 0f;

		shakeMagnitude = shakeForce;
		while (elapsedTime < duration) {
			var shakeOffset = Random.insideUnitSphere * shakeMagnitude;
			transform.localPosition = originalPosition + shakeOffset;
			elapsedTime += Time.deltaTime;
			shakeMagnitude = Mathf.Lerp(shakeMagnitude, 0f, elapsedTime / duration);
			yield return null;
		}
		transform.localPosition = originalPosition;
	}
}
