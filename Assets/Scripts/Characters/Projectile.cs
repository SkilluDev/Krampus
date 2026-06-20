using KrampUtils;
using SaintsField;
using SaintsField.Playa;
using Sound;
using UnityEngine;

[RequireComponent(typeof(Rigidbody))]

public class Projectile : MonoBehaviour {
    [SerializeField] protected float m_speed = 14;
    [SerializeField] protected float m_turningSpeed = 15;
    [SerializeField] protected bool m_activateToungables = true;
    [SerializeField] protected LayerMask m_destroyedBy;
    [SerializeField] protected Tag[] m_targetGroups;

    [Layout("Components", ELayout.FoldoutBox)][SerializeField] protected Rigidbody m_rigidbody;
    [Layout("Components", ELayout.FoldoutBox)][SerializeField] protected AudioSource m_audioSource;
    [Layout("Components", ELayout.FoldoutBox)][SerializeField] protected Animator m_animator;
    [Layout("Sounds", ELayout.FoldoutBox)][SerializeField] protected Sex m_sexShoot;
    [Layout("Sounds", ELayout.FoldoutBox)][SerializeField] protected Sex m_sexHit;
    [Layout("Sounds", ELayout.FoldoutBox)][SerializeField] protected Sex m_sexMiss;
    [Layout("Effects", ELayout.FoldoutBox)][SerializeField] protected GameObject m_fxShoot;
    [Layout("Effects", ELayout.FoldoutBox)][SerializeField] protected GameObject m_fxHit;
    [Layout("Effects", ELayout.FoldoutBox)][SerializeField] protected GameObject m_fxMiss;
    [Layout("Auto Destroy", ELayout.FoldoutBox)][SerializeField] protected bool m_dieOnMiss;
    [Layout("Auto Destroy", ELayout.FoldoutBox)][EnableIf("m_dieOnMiss")][SerializeField] protected float m_timeMiss;
    [Layout("Auto Destroy", ELayout.FoldoutBox)][SerializeField] protected bool m_dieOnHit;
    [Layout("Auto Destroy", ELayout.FoldoutBox)][EnableIf("m_dieOnHit")][SerializeField] protected float m_timeHit;


    [LayoutShowIf(nameof(HasAnimator))][Layout("Animations", ELayout.FoldoutBox)][SerializeField][AnimatorParam("m_animator")] protected int m_animShoot;
    [ShowIf(nameof(HasAnimator))][Layout("Animations", ELayout.FoldoutBox)][SerializeField][AnimatorParam("m_animator")] protected int m_animHit;
    [ShowIf(nameof(HasAnimator))][Layout("Animations", ELayout.FoldoutBox)][SerializeField][AnimatorParam("m_animator")] protected int m_animMiss;

    protected bool HasAnimator => m_animator != null;
    protected bool HasAudioSource => m_audioSource != null;

    protected bool m_isActive;
    protected Transform m_target;

    public static T Shoot<T>(T projectile, Vector3 position, Vector3 direction, Transform target = null, Collider sender = null) where T : Projectile {
        var obj = Instantiate(projectile.gameObject);
        obj.transform.SetPositionAndRotation(position, Quaternion.LookRotation(direction));
        var obpr = obj.GetComponent<T>();
        obpr.Shoot(target);

        if (sender != null) {
            Physics.IgnoreCollision(obpr.GetComponent<Collider>(), sender);
        }
        return obpr;
    }

    protected virtual void Shoot(Transform target) {
        Debug.Break();
        m_target = target;
        m_sexShoot.Play(transform.position);
        if (m_fxShoot != null) Instantiate(m_fxShoot, transform.position, transform.rotation);
        if (m_animator != null) m_animator.SetTrigger(m_animShoot);
        m_isActive = true;
    }

    protected virtual void Miss(Collider other) {
        m_sexMiss.Play(transform.position);
        if (m_fxMiss != null) Instantiate(m_fxMiss, transform.position, transform.rotation);
        if (m_animator != null) m_animator.SetTrigger(m_animMiss);
        m_isActive = false;
        if (m_dieOnMiss) Destroy(gameObject, m_timeMiss);
    }

    protected virtual void Hit(Collider other) {
        m_sexHit.Play(transform.position);
        if (m_fxHit != null) Instantiate(m_fxHit, transform.position, transform.rotation);
        if (m_animator != null) m_animator.SetTrigger(m_animHit);
        m_isActive = false;
        if (m_dieOnHit) Destroy(gameObject, m_timeHit);
    }

    protected virtual void FixedUpdate() {
        if (!m_isActive) {
            m_rigidbody.linearVelocity = Vector3.zero;
            return;
        }

        m_rigidbody.linearVelocity = transform.forward * m_speed;

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
        if (!m_isActive) return;
        if (m_destroyedBy.IsLayerMasked(other.gameObject.layer)) {
            Miss(other);
        }
        // && other.gameObject.TryGetComponent<ITongueable>(out var t)
        if (m_activateToungables) {
            print("projectile passby " + other.gameObject.name);
            // t.TonguePassBy(null, transform.position, 0); //bad!
        }

        if (other.gameObject.HasAnyTag(m_targetGroups)) {
            Hit(other);
        }
    }
}
