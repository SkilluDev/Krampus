using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.InputSystem;

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

    private Vector3 GetTilt() {
        switch (InputSubscribe.InputMethod) {
            case InputSubscribe.Method.PC:
                return new Vector3(InputSubscribe.Aim.x / Screen.width, InputSubscribe.Aim.y / Screen.height) - new Vector3(0.5f, 0.5f);
            case InputSubscribe.Method.Console:
                var currentObjPosition = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().position;
                return new Vector3(currentObjPosition.x / Screen.width, currentObjPosition.y / Screen.height) * 1.5f - new Vector3(0.5f, 0.5f);
            default:
                return default;
        }
    }

    private void Update() {
        transform.SetPositionAndRotation(
            Vector3.Lerp(
                transform.position,
                m_center.position + m_initialOffset + GetTilt() * m_mouseFactor, m_smoothingSpeed * Time.deltaTime
            ),
            Quaternion.Lerp(
                m_initialRotation, Quaternion.LookRotation(m_center.transform.position - transform.position, Vector3.up), m_rotationInfluence
            ));
    }
}
