using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class EdibleProp : MonoBehaviour, IEdible {
    [SerializeField] private ParticleSystem m_consumeParticles;
    [SerializeField] private Transform m_model;


    private bool ModelSet => m_model != null;
    private Vector3 m_initialScale;

    [SerializeField]
    [ShowIf("ModelSet")]
    private AnimationCurve m_modelScale = new AnimationCurve(
        new Keyframe(0f, 1f, 0f, 0f),
        new Keyframe(1f, 0f, -90f, 0f, 0.01f, 0f)
    );

    private void Awake() {
        if (ModelSet) {
            m_initialScale = m_model.localScale;
        }
    }

    public void Consume(Krampus krampus) {
        if (m_consumeParticles != null) {
            var particleObject = Instantiate(m_consumeParticles, transform.position, Quaternion.identity);
            particleObject.gameObject.SetActive(true);
        }
        Destroy(gameObject);
    }

    public void Hit(Krampus krampus) {
        foreach (var c in Physics.OverlapSphere(transform.position, 3).Select(w => w.GetComponent<Rigidbody>()).Where(w => w != null)) c.WakeUp();
        foreach (var c in GetComponentsInChildren<Rigidbody>()) c.isKinematic = true;
        foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = false;
    }
    public void Prepare(Krampus krampus) { }
    public void ReelIn(Krampus krampus, Vector3 position, float progress) {
        transform.position = position;
        if (ModelSet) {
            m_model.localScale = m_initialScale * m_modelScale.Evaluate(progress);
        }
    }
}
