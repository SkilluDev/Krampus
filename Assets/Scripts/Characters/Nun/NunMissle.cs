using System.Collections;
using System.Collections.Generic;
using KrampUtils;
using UnityEngine;

public class NunMissle : MonoBehaviour {
    [SerializeField] private Transform m_target;
    [SerializeField] private float m_speed = 20;
    [SerializeField] private float m_rotationSpeed = 50;

    [SerializeField] private Rigidbody m_rigidbody;
    [SerializeField] private Transform m_visuals;
    [SerializeField] private GameObject m_missleMesh;
    [SerializeField] public Sprite m_slowEffectIcon;

    private Vector3 m_direction;
    private bool m_isActive = true;


    private void Update() {
        if (m_target == null) return;
        if (m_isActive == false) {
            return;
        }




        transform.Translate(m_direction * m_speed * Time.deltaTime, Space.Self);

    }

    public void SetTarget(Transform target, Vector3 direction) {
        m_isActive = true;
        m_target = target;
        m_direction = direction;
        m_visuals.rotation = Quaternion.LookRotation(direction);
    }

	private void OnTriggerEnter(Collider other) {
        if (m_isActive == false) return;
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall")) {
            DeactivateMissle();

        }
        if (other.gameObject.tag == "Player") {
            KrampusStats krampus = other.GetComponent<KrampusStats>();
            krampus.RegisterEffect(new Effect(KrampusStats.Stat.Speed, -0.3f, 2, "Nun_Slow", m_slowEffectIcon));
            DeactivateMissle();
        }
    }

    private void DeactivateMissle() {
        m_isActive = false;
        m_missleMesh.SetActive(false);
         Destroy(gameObject, 2);
     }
}
