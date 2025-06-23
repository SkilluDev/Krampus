using System.Linq;
using NaughtyAttributes;
using UnityEngine;

public class Box : MonoBehaviour, IEdible {
    [SerializeField] private ParticleSystem m_consumeParticles;
    [SerializeField] private Transform m_model;
    [SerializeField] private Projectile m_projectile;

    IEdible.EdibleType IEdible.Type => IEdible.EdibleType.DelayedAiming;

    private bool ModelSet => m_model != null;

    public int Priority => -1;

    private Vector3 m_initialScale;

    [SerializeField]
    [ShowIf("ModelSet")]
    private AnimationCurve m_modelScale = new AnimationCurve(
        new Keyframe(0f, 1f, 0f, 0f, 0f, 1f),
        new Keyframe(1f, 0f, -90f, 0f, 0.011f, 0f)
    );

    private void Awake() {
        if (ModelSet) {
            m_initialScale = m_model.localScale;
        }
    }

    public void Consume(Krampus krampus, Vector3 position, Quaternion rotation) {
        Projectile.Shoot(m_projectile, position, rotation * Vector3.forward, null, krampus.GetComponent<Collider>());
        Destroy(gameObject);
    }

    public void Hit(Krampus krampus) {
        foreach (var c in Physics.OverlapSphere(transform.position, 3).Select(w => w.GetComponent<Rigidbody>()).Where(w => w != null)) c.WakeUp();
        foreach (var c in GetComponentsInChildren<Rigidbody>()) c.isKinematic = true;
        foreach (var c in GetComponentsInChildren<Collider>()) c.enabled = false;
    }
    public void Prepare(Krampus krampus) { }
    public void AttachToTongue(Krampus krampus, Vector3 position, Quaternion rotation, float progress) {
        transform.position = position;
        transform.rotation = rotation;
        if (ModelSet) {
            m_model.localScale = m_initialScale * m_modelScale.Evaluate(progress);
        }
    }
}
