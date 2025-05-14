using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class Parallaxer : MonoBehaviour
{
    [SerializeField] private float m_mouseFactor = 1;
    [SerializeField] private float m_smoothingSpeed = 5;
    [SerializeField] private float m_rotationInfluence = 0.5f;
    private Transform[] m_layers;
    private void Awake() {
        m_layers = new Transform[gameObject.transform.childCount];
        for (int i = 0;i<m_layers.Length;i++){
            m_layers[m_layers.Length-1-i] = gameObject.transform.GetChild(i);
        }
	}
    // Update is called once per frame
    private void Update()
    {
        for (int i=0;i<m_layers.Length; i++){
            m_layers[i].SetPositionAndRotation(
                Vector3.Lerp(
                    transform.position,
                    transform.position + GetTilt() * m_mouseFactor * i, m_smoothingSpeed * Time.deltaTime
                ), transform.rotation);
        }
        
    }

    private Vector3 GetTilt() {
        switch (InputSubscribe.InputMethod) {
            case InputSubscribe.Method.PC:
                return new Vector3(InputSubscribe.UITilt.x / Screen.width, InputSubscribe.UITilt.y / Screen.height) - new Vector3(0.5f, 0.5f);
            case InputSubscribe.Method.Console:
                var currentObjPosition = EventSystem.current.currentSelectedGameObject.GetComponent<RectTransform>().position;
                return new Vector3(currentObjPosition.x / Screen.width, currentObjPosition.y / Screen.height) * 1.5f - new Vector3(0.5f, 0.5f);
            case InputSubscribe.Method.Mobile:
                return new Vector3(InputSubscribe.UITilt.x, InputSubscribe.UITilt.y);
            default:
                return default;
        }
    }
}
