using UnityEngine;

public class NunMissile : Projectile {
    [SerializeField] private EffectInEditor m_effectToAdd;
    [SerializeField] private Tag m_doorTag;
    [SerializeField] private Sprite m_slowEffectIcon;
    protected override void Hit(Collider other) {
        base.Hit(other);
        var krampus = other.GetComponent<Krampus>().Stats;
        krampus.RegisterEffect(m_effectToAdd.ToEffect("Nun_Slow", m_slowEffectIcon));
    }

    protected override void Miss(Collider other) {
        if (other.gameObject.HasTag(m_doorTag)) {
            if (!other.gameObject.GetComponent<Door>().IsOpen) {
                base.Miss(other);
            }
        } else
            base.Miss(other);
    	}


}
