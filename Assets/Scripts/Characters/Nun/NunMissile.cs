using UnityEngine;

public class NunMissile : Projectile {
    [SerializeField] private EffectInEditor m_effectToAdd;
    [SerializeField] private Sprite m_slowEffectIcon;
    protected override void Hit(Collider other) {
        base.Hit(other);
        var krampus = other.GetComponent<Krampus>().Stats;
        krampus.RegisterEffect(m_effectToAdd.ToEffect("Nun_Slow", m_slowEffectIcon));
    }

}