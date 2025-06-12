using System.Collections;
using UnityEngine;
using NaughtyAttributes;
using Cinemachine;

// Based on @WalkanFL's camera
public class CameraController : MonoBehaviour {
	public Matrix4x4 Matrix { get; private set; }
	public CinemachineVirtualCamera Raw => m_camera;
	public Camera Rendering => m_renderingCamera;
	public CinemachineImpulseSource DefaultShake => m_defaultImpulse;
	private Krampus m_krampus;
	[SerializeField] private CinemachineVirtualCamera m_camera;
	[SerializeField] private Camera m_renderingCamera;
	[SerializeField] private CinemachineImpulseSource m_defaultImpulse;
	[SerializeField] private AnimationCurve m_zoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

	[SerializeField] private Vector3 m_baseOffset = Vector3.one * 10;
	[BoxGroup("Movement")][SerializeField] private float m_lookAheadStrength;

	[BoxGroup("Zoom")][SerializeField] private float m_zoomedOrtoSize = 20;
	[BoxGroup("Zoom")][SerializeField] private float m_unzoomedOrtoSize = 25;
	[BoxGroup("Camera Smoothing")][SerializeField] private float m_cameraSpeed = 0.125f;
	[BoxGroup("Zoom")][SerializeField] private float m_zoomSpeed = 0.2f;
	[BoxGroup("Zoom")][SerializeField] private float m_zoomBuffer = 0.75f;
	[BoxGroup("Zoom")][SerializeField] private float m_unzoomSpeed = 1f;
	[BoxGroup("Zoom")][SerializeField] private float m_aimZoomSpeed = 1f;
	private float m_zoomFactor = 1;
	[SerializeField] private float m_ortoChangeSpeed = 1f;

	private float m_ortoZoomBaseToAdd = 0f;
	private float m_ortoUnzoomBaseToAdd = 0f;

	private float m_ortoZoomBaseToSubstract = 0f;
	private float m_ortoUnzoomBaseToSubstract = 0f;

	private float m_unzoomToZoomStartRatio;

	private void Awake() {
		Matrix = Matrix4x4.Rotate(Quaternion.Euler(0, transform.eulerAngles.y, 0));
	}

	private void Ready() {
		m_krampus = Game.MainGameInfo.Krampus;
		Game.MainGameInfo.Krampus.KrampusEvents.onTongueLengthChanged.AddListener(ChangeOrto);
		m_unzoomToZoomStartRatio = m_unzoomedOrtoSize / m_zoomedOrtoSize;
		//Debug.Log("ratio" + m_unzoomToZoomStartRatio);
	}

	private void OnDestroy() {
		Game.MainGameInfo.Krampus.KrampusEvents.onTongueLengthChanged.RemoveListener(ChangeOrto);
	}

	private void FixedUpdate() {
		AddBaseOrto();
		SubstractBaseOrto();
	}

	private void LateUpdate() {
		if (m_krampus == null || m_camera == null) return;

		transform.position = Vector3.Lerp(transform.position, m_krampus.transform.position + ComputeOffset(), m_cameraSpeed * Time.deltaTime);
		m_camera.m_Lens.FieldOfView = ComputeOrtoSize();
	}

	private Vector3 ComputeOffset() {
		var movementOffset = Vector3.ClampMagnitude(m_krampus.Kramp.Kontroller.VelocityVector, 1) * m_lookAheadStrength;
		return m_baseOffset + movementOffset;
	}

	private float ComputeOrtoSize() {
		if (m_krampus.Tongue.CurrentState == KrampusTongue.State.Windup) {
			m_zoomFactor += m_aimZoomSpeed * Time.deltaTime;
		} else if (m_krampus.Kontroller.CurrentState == KrampusController.State.Run || m_krampus.Kontroller.CurrentState == KrampusController.State.Dead) {
			m_zoomFactor -= m_unzoomSpeed * Time.deltaTime;
		} else {
			m_zoomFactor += m_zoomSpeed * Time.deltaTime;
		}
		m_zoomFactor = Mathf.Clamp(m_zoomFactor, -m_zoomBuffer, 1);

		return Mathf.Lerp(m_unzoomedOrtoSize, m_zoomedOrtoSize, m_zoomCurve.Evaluate(Mathf.Clamp01(m_zoomFactor)));
	}

	private void ChangeOrto(Krampus krampus, float diff) {
		if (diff >= 0) {
			m_ortoZoomBaseToAdd += diff;
			m_ortoUnzoomBaseToAdd += diff*m_unzoomToZoomStartRatio;
		} else {
			m_ortoZoomBaseToSubstract -= diff;
			m_ortoUnzoomBaseToSubstract -= diff*m_unzoomToZoomStartRatio;
		}
	}

	private void AddBaseOrto() {
		if (m_ortoZoomBaseToAdd > 0) {
			float diff = Time.fixedDeltaTime * m_ortoChangeSpeed;
			m_ortoZoomBaseToAdd -= diff;
			m_zoomedOrtoSize += diff;
		}
		if (m_ortoUnzoomBaseToAdd > 0) {
			float diff = Time.fixedDeltaTime * m_ortoChangeSpeed;
			m_ortoUnzoomBaseToAdd -= diff;
			m_unzoomedOrtoSize += diff * m_unzoomToZoomStartRatio;
		}
	}

	private void SubstractBaseOrto() {
		if (m_ortoZoomBaseToSubstract > 0) {
			float diff = Time.fixedDeltaTime * m_ortoChangeSpeed;
			m_ortoZoomBaseToSubstract -= diff;
			m_zoomedOrtoSize -= diff;
		}
		if (m_ortoUnzoomBaseToSubstract > 0) {
			float diff = Time.fixedDeltaTime * m_ortoChangeSpeed;
			m_ortoUnzoomBaseToSubstract -= diff;
			m_unzoomedOrtoSize -= diff * m_unzoomToZoomStartRatio;
		}
	}




	[Button("Set Current As Base")]
	private void SetBaseCurrent() {
		if (m_krampus == null || m_camera == null)
			throw new System.Exception("Objects are not assigned to the CameraController");

		//transform.SetPositionAndRotation(m_camera.transform.position, m_camera.transform.rotation);
		//m_camera.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
		m_baseOffset = transform.position - m_krampus.transform.position;
		m_zoomedOrtoSize = m_camera.m_Lens.FieldOfView;
	}

	[Button("Show Base")]
	private void ShowBase() {
		if (m_krampus == null || m_camera == null)
			throw new System.Exception("Objects are not assigned to the CameraController");

		transform.position = m_krampus.transform.position + ComputeOffset();
		m_camera.m_Lens.FieldOfView = ComputeOrtoSize();
	}
}
