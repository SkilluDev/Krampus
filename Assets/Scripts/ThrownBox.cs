using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrownBox : Projectile {
    [SerializeField] private Tag m_doorTag;
    [SerializeField] private float m_stunDuration = 1.2f;
    protected override void Miss(Collider other) {
        if (other.gameObject.HasTag(m_doorTag))
            other.gameObject.GetComponent<Door>().Open(true, false);
        else
            base.Miss(other);
    }

    protected override void Hit(Collider other) {
        base.Hit(other);

        other.gameObject.GetComponent<Nun>().Stun(m_stunDuration);
    }
}
