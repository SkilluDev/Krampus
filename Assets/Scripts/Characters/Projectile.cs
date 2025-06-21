using KrampUtils;
using NaughtyAttributes;
using Sound;
using UnityEngine;

[RequireComponent(typeof(Rigidbody), typeof(AudioSource))]

public abstract class Projectile : MonoBehaviour {
    [BoxGroup("Components")][SerializeField] protected Rigidbody m_rigidbody;
    [BoxGroup("Components")][SerializeField] protected AudioSource m_audioSource;
    [SerializeField] protected float m_speed = 14;
    [SerializeField] protected float m_turningSpeed = 15;

    [BoxGroup("Sounds")][SerializeField] protected Sex m_sexShoot;
    [BoxGroup("Sounds")][SerializeField] protected Sex m_sexHit;
    [BoxGroup("Sounds")][SerializeField] protected Sex m_sexMiss;
    [BoxGroup("Effects")][SerializeField] protected GameObject m_efHit;
    [BoxGroup("Effects")][SerializeField] protected GameObject m_efMiss;


    [SerializeField] protected LayerMask m_destroyedBy;
    [SerializeField] protected Tag m_targetGroup;

    protected Transform m_target;

    public static T Shoot<T>(T projectile, Vector3 position, Vector3 direction, Transform target = null) where T : Projectile {
        var obj = Instantiate(projectile.gameObject);
        obj.transform.SetPositionAndRotation(position, Quaternion.LookRotation(direction));
        var obpr = obj.GetComponent<T>();
        obpr.Shoot(target);
        return obpr;
    }

    protected virtual void Shoot(Transform target) {
        m_target = target;
        m_sexShoot.Play(transform.position);
    }

    protected virtual void Miss(Collider other) {
        m_sexMiss.Play(transform.position);
        Instantiate(m_efMiss, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    protected virtual void Hit(Collider other) {
        m_sexHit.Play(transform.position);
        Instantiate(m_efHit, transform.position, transform.rotation);
        Destroy(gameObject);
    }

    protected virtual void FixedUpdate() {
        m_rigidbody.velocity = transform.forward * m_speed;

        if (m_target != null) {
            var directionToTarget = (m_target.position - transform.position).normalized;
            var targetRotation = Quaternion.LookRotation(directionToTarget);
            var newRotation = Quaternion.RotateTowards(
                m_rigidbody.rotation,
                targetRotation,
                m_turningSpeed * Time.fixedDeltaTime
            );
            m_rigidbody.MoveRotation(newRotation);
        }
    }

    private void OnTriggerEnter(Collider other) {
        if (m_destroyedBy.IsLayerMasked(other.gameObject.layer)) {
            Miss(other);
        }

        if (other.gameObject.HasTag(m_targetGroup)) {
            Hit(other);
        }
    }
}
