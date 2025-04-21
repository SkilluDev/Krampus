using UnityEngine;
using NaughtyAttributes;


public class CameraController : MonoBehaviour {
    [SerializeField] private KrampusController m_krampus;
    [SerializeField] private Camera m_camera;

    [SerializeField] private Vector3 m_baseOffset = Vector3.one * 10;
    [SerializeField] private float m_baseOrtoSize = 5;


    private void LateUpdate() {
        if (m_krampus == null || m_camera == null) return;

        transform.position = m_krampus.transform.position + ComputeOffset();
        m_camera.orthographicSize = ComputeOrtoSize();
    }

    private Vector3 ComputeOffset() {
        return m_baseOffset;
    }

    private float ComputeOrtoSize() {
        return m_baseOrtoSize;
    }


    [Button("Set Current As Base")]
    private void SetBaseCurrent() {
        if (m_krampus == null || m_camera == null)
            throw new System.Exception("Objects are not assigned to the CameraController");

        transform.SetPositionAndRotation(m_camera.transform.position, m_camera.transform.rotation);
        m_camera.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        m_baseOffset = transform.position - m_krampus.transform.position;
        m_baseOrtoSize = m_camera.orthographicSize;
    }

    [Button("Show Base")]
    private void ShowBase() {
        if (m_krampus == null || m_camera == null)
            throw new System.Exception("Objects are not assigned to the CameraController");

        transform.position = m_krampus.transform.position + ComputeOffset();
        m_camera.orthographicSize = ComputeOrtoSize();
    }
}
