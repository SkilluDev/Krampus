using System;
using NaughtyAttributes;
using UnityEngine;

public class ChildAnimator : MonoBehaviour {
    [SerializeField] private Child m_child;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Transform m_model;
    [SerializeField][AnimatorParam(nameof(m_animator))] private int m_speedProperty, m_stunProperty, m_panicProperty, m_reportingProperty, m_deathTriggr;

	[BoxGroup("StateSprites")] [SerializeField] private SpriteRenderer m_spriteRenderer;
    [BoxGroup("StateSprites")] [SerializeField] private Sprite m_idleSprite;
    [BoxGroup("StateSprites")] [SerializeField] private Sprite m_stunnedSprite;
    [BoxGroup("StateSprites")] [SerializeField] private Sprite m_initalPanicSprite;
    [BoxGroup("StateSprites")] [SerializeField] private Sprite m_panicSprite;
    [BoxGroup("StateSprites")] [SerializeField] private Sprite m_reportingSprite;
    [BoxGroup("StateSprites")] [SerializeField] private Sprite m_alertedSprite;
    [BoxGroup("StateSprites")] [SerializeField] private Sprite m_deadSprite;

    private Quaternion m_rotationTarget;

    private void Start() {
        m_child.onStateChanged += ChildStateChanged;
    }



    private void ChildStateChanged(Child.State previous, Child.State current) {
        switch ((previous, current)) {
            case (_, Child.State.Stunned):
                m_rotationTarget = Quaternion.LookRotation(Game.MainGameInfo.Krampus.transform.position - m_child.transform.position, Vector3.up);
                m_animator.SetTrigger(m_stunProperty);
                m_spriteRenderer.sprite = m_stunnedSprite;

                break;
            case (_, Child.State.Dead):
                m_animator.SetTrigger(m_deathTriggr);
                m_spriteRenderer.sprite = m_deadSprite;
                break;
            case (_, Child.State.Idle):
	            m_spriteRenderer.sprite = m_idleSprite;
	            break;
            case (_, Child.State.InitialPanic):
	            m_spriteRenderer.sprite = m_initalPanicSprite;
	            break;
            case (_, Child.State.Panic):
	            m_spriteRenderer.sprite = m_panicSprite;
	            break;
            case (_, Child.State.Reporting):
	            m_spriteRenderer.sprite = m_reportingSprite;
	            break;
            case(_, Child.State.Alerted):
	            m_spriteRenderer.sprite = m_alertedSprite;
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
