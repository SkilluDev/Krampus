using System;
using NaughtyAttributes;
using UnityEngine;

public class ChildAnimator : MonoBehaviour {
    [SerializeField] private Child m_child;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Transform m_model;
    [SerializeField][AnimatorParam(nameof(m_animator))] private int m_speedProperty, m_stunProperty, m_panicProperty, m_reportingProperty, m_deathTriggr;

    private void Start() {
        m_child.onStateChanged += ChildStateChanged;
    }



    private void ChildStateChanged(Child.State previous, Child.State current) {
        switch ((previous, current)) {
            case (_, Child.State.Stunned):
                m_animator.SetTrigger(m_stunProperty);
                break;
			case (_, Child.State.Dead):
				m_animator.SetTrigger(m_deathTriggr);
				break;
        }
    }

    private void Update() {
        if (m_child.VelocitySqr > 0.1f) {
            m_model.rotation = Quaternion.LookRotation(m_child.VelocityVector, Vector3.up);
        }

        m_animator.SetBool(m_panicProperty, m_child.CurrentState == Child.State.Panic);
        m_animator.SetBool(m_reportingProperty, m_child.CurrentState == Child.State.Reporting);
        m_animator.SetFloat(m_speedProperty, m_child.Velocity / (m_child.CurrentState == Child.State.Panic ? m_child.RunSpeed : m_child.BaseMovementSpeed));
    }
}
