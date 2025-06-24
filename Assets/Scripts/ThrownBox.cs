using UnityEngine;

public class ThrownBox : Projectile {
    [SerializeField] private Tag m_doorTag;
    [SerializeField] private float m_stunDuration = 1.2f;
    [SerializeField] private float m_doorSlowdown = 0.5f;
    protected override void Miss(Collider other) {
        if (other.gameObject.HasTag(m_doorTag)) {
            other.gameObject.GetComponent<Door>().Open(true, transform.position);
            m_speed *= m_doorSlowdown; // probably should be attribute based

            if (m_speed < 1) base.Miss(other);
        } else
            base.Miss(other);
    }

    protected override void Hit(Collider other) {
        base.Hit(other);

        if (other.TryGetComponent<Nun>(out var nun)) nun.Stun(m_stunDuration);
        if (other.TryGetComponent<Child>(out var child)) child.Stun(m_stunDuration);
    }
}
