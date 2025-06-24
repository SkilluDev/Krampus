using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class Shattering : MonoBehaviour {
    [SerializeField] private Rigidbody[] m_shardRbs;
    [SerializeField] private float m_shatterForce = 5;
    [SerializeField] private Vector3 m_shatterOffset;
    [SerializeField] private AnimationCurve m_curve = AnimationCurve.Linear(0, 1, 1, 0);
    [SerializeField] private float m_lifetime = 5;
    private float m_remainingLifetime;

    private List<Vector3> m_scales;

    [Button("Assign Components")]
    private void Assign() {
        m_shardRbs = GetComponentsInChildren<Rigidbody>();
    }

    private void Start() {
        m_scales = new List<Vector3>();

        foreach (var rb in m_shardRbs) {
            rb.AddExplosionForce(m_shatterForce, transform.TransformPoint(m_shatterOffset), 4);
            m_scales.Add(rb.transform.localScale);
        }

        m_remainingLifetime = m_lifetime;
        Destroy(gameObject, m_lifetime);
    }

    private void Update() {
        m_remainingLifetime -= Time.deltaTime;

        for (int i = 0; i < m_shardRbs.Length; i++) {
            var rb = m_shardRbs[i];
            rb.transform.localScale = m_scales[i] * m_curve.Evaluate(m_remainingLifetime / m_lifetime);
            if (m_curve.Evaluate(m_remainingLifetime / m_lifetime) < 0.1f) rb.isKinematic = true;
        }
    }
}

