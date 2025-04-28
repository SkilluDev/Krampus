using UnityEngine;

public class MainMenuCamera : MonoBehaviour {

    [SerializeField] private Transform m_center;
    [SerializeField] private float m_mouseFactor = 1;
    [SerializeField] private float m_smoothingSpeed = 5;
    [SerializeField] private float m_rotationInfluence = 0.5f;

    private Vector3 m_initialOffset;
    private Quaternion m_initialRotation;

    private void Awake() {
        m_initialOffset = transform.position - m_center.position;
        m_initialRotation = transform.rotation;
    }

    private void Update() {
        transform.SetPositionAndRotation(
            Vector3.Lerp(
                transform.position,
                m_center.position + m_initialOffset + (new Vector3(Input.mousePosition.x / Screen.width, Input.mousePosition.y / Screen.height) - new Vector3(0.5f, 0.5f)) * m_mouseFactor, m_smoothingSpeed * Time.deltaTime
            ),
            Quaternion.Lerp(
                m_initialRotation, Quaternion.LookRotation(m_center.transform.position - transform.position, Vector3.up), m_rotationInfluence
            ));
    }
}
