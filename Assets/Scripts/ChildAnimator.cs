using System;
using NaughtyAttributes;
using UnityEngine;

public class ChildAnimator : MonoBehaviour {
    [SerializeField] private Child m_child;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Transform m_model;
    [SerializeField][AnimatorParam(nameof(m_animator))] private int m_speedProperty, m_stunProperty, m_panicProperty, m_reportingProperty, m_deathTriggr;

    [BoxGroup("State Sprites")][SerializeField] private StatusSprite m_spriteRenderer;
    [BoxGroup("State Sprites")][SerializeField] private Sprite m_panicSprite;
    [BoxGroup("State Sprites")][SerializeField] private Sprite m_alertedSprite;

    private Quaternion m_rotationTarget;

    private void Start() {
        m_child.onStateChanged += ChildStateChanged;
    }



    private void ChildStateChanged(Child.State previous, Child.State current) {
        switch ((previous, current)) {
            case (_, Child.State.Stunned):
                m_rotationTarget = Quaternion.LookRotation(Game.MainGameInfo.Krampus.transform.position - m_child.transform.position, Vector3.up);
                m_animator.SetTrigger(m_stunProperty);
                m_spriteRenderer.SetSprite(m_panicSprite, 2);
                break;
            case (_, Child.State.Dead):
                m_animator.SetTrigger(m_deathTriggr);
                break;
            case (_, Child.State.Idle):
                m_spriteRenderer.ClearSprite();
                break;
            case (_, Child.State.InitialPanic):
                break;
            case (_, Child.State.Panic):
                m_spriteRenderer.SetSprite(m_panicSprite);
                break;
            case (_, Child.State.Reporting):
                break;
            case (_, Child.State.Alerted):
                m_spriteRenderer.SetSprite(m_alertedSprite);
                break;

        }
    }

    private void Update() {
        if (m_child.VelocitySqr > 0.1f) {
            m_rotationTarget = Quaternion.LookRotation(m_child.VelocityVector, Vector3.up);
        }
        m_model.rotation = Quaternion.Slerp(m_model.rotation, m_rotationTarget, Time.deltaTime * 5);

        m_animator.SetBool(m_panicProperty, m_child.CurrentState is Child.State.Panic or Child.State.InitialPanic);
        m_animator.SetBool(m_reportingProperty, m_child.CurrentState == Child.State.Reporting);
        m_animator.SetFloat(m_speedProperty, m_child.Velocity / (m_child.CurrentState == Child.State.Panic ? m_child.RunSpeed : m_child.BaseMovementSpeed));
    }
}
