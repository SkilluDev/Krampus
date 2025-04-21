using UnityEngine;
using NaughtyAttributes;

// Based on @WalkanFL's camera
public class CameraController : MonoBehaviour {
    public Matrix4x4 Matrix { get; private set; }
    [SerializeField] private Krampus m_krampus;
    [SerializeField] private Camera m_camera;
    [SerializeField] private AnimationCurve m_zoomCurve = AnimationCurve.EaseInOut(0, 0, 1, 1);

    [SerializeField] private Vector3 m_baseOffset = Vector3.one * 10;
    [SerializeField] private float m_zoomedOrtoSize = 5;
    [SerializeField] private float m_unzoomedOrtoSize = 8;
    [SerializeField] private float m_zoomSpeed = 0.2f;
    [SerializeField] private float m_zoomBuffer = 0.75f;
    [SerializeField] private float m_unzoomSpeed = 1f;

    private float m_zoomFactor = 1;

    private void Awake() {
        Matrix = Matrix4x4.Rotate(Quaternion.Euler(0, transform.eulerAngles.y, 0));
    }

    private void LateUpdate() {
        if (m_krampus == null || m_camera == null) return;

        transform.position = m_krampus.transform.position + ComputeOffset();
        m_camera.orthographicSize = ComputeOrtoSize();
    }

    private Vector3 ComputeOffset() {
        return m_baseOffset;
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
}
