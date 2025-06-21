using System;
using KrampUtils;
using NaughtyAttributes;
using Sound;
using UnityEngine;

public abstract class Projectile : MonoBehaviour {
    [SerializeField] protected float m_speed = 14;
    [SerializeField] protected LayerMask m_destroyedBy;
    protected Tag m_targetGroup;
    protected Transform m_target;

    public static T Shoot<T>(T projectile, Vector3 position, Vector3 direction, Transform target = null, Tag targetGroup = null) where T : Projectile {
        var obj = Instantiate(projectile.gameObject);
        obj.transform.SetPositionAndRotation(position, Quaternion.LookRotation(direction));
        var obpr = obj.GetComponent<T>();
        obpr.Shoot(target, targetGroup);
        return obpr;
    }

    protected virtual void Shoot(Transform target, Tag targetGroup) {
        m_target = target;
        m_targetGroup = targetGroup;
    }

    private void OnTriggerEnter(Collider other) {
        if (m_destroyedBy.IsLayerMasked(other.gameObject.layer)) {
            Miss(other);
        }

        if (other.gameObject.HasTag(m_targetGroup)) {
            Hit(other);
        }
    }

    protected abstract void Miss(Collider other);
    protected abstract void Hit(Collider other);

    protected virtual void Update() {

    }
}
