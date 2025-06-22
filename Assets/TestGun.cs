using UnityEngine;

public class TestGun : MonoBehaviour {
    [SerializeField] private Projectile m_projectile;
    [SerializeField] private float m_interval = 4;
    [SerializeField] private Transform m_target;

    private float m_countdown;

    private void Start() {
        m_countdown = m_interval;
    }

    private void Update() {
        if (m_countdown <= 0) {
            Projectile.Shoot(m_projectile, transform.position, transform.forward, m_target);
            m_countdown = m_interval;
        }
        m_countdown -= Time.deltaTime;
    }
}

