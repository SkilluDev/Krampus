using System.Collections;
using System.Collections.Generic;
using KrampUtils;
using UnityEngine;

public class NunMissle : MonoBehaviour {
    [SerializeField] private Transform m_target;
    [SerializeField] private float m_speed = 20;
    [SerializeField] private float m_rotationSpeed = 50;

    [SerializeField] private Rigidbody m_rigidbody;

    private Vector3 m_direction;


    private void Update() {
        if (m_target == null) return;




        transform.Translate(m_direction * m_speed * Time.deltaTime, Space.Self);

    }

    public void SetTarget(Transform target, Vector3 direction) {
        m_target = target;
        m_direction = direction;
    }

    void OnTriggerEnter(Collider other) {
        if (other.gameObject.layer == LayerMask.NameToLayer("Wall")) {
            Destroy(gameObject);
        }
        if (other.gameObject.tag == "Player") {
            KrampusStats krampus = other.GetComponent<KrampusStats>();
            krampus.RegisterEffect(new Effect(KrampusStats.Stat.Speed, -0.3f, 3, "Nun_Slow", null));
            Destroy(gameObject);
         }
	}
}
