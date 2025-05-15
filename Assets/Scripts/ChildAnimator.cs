using NaughtyAttributes;
using Sound;
using UnityEngine;

public class ChildAnimator : MonoBehaviour {
    [SerializeField] private Child m_child;
    [SerializeField] private Animator m_animator;
    [SerializeField] private Transform m_model;
    [SerializeField] private float m_turningSpeed = 5f;
    [SerializeField][AnimatorParam(nameof(m_animator))] private int m_speedProperty, m_stunProperty, m_panicProperty, m_reportingProperty, m_deathTriggr;

    [BoxGroup("State Sprites")][SerializeField] private StatusSprite m_spriteRenderer;
    [BoxGroup("State Sprites")][SerializeField] private Sprite m_panicSprite;
    [BoxGroup("State Sprites")][SerializeField] private Sprite m_alertedSprite;
    [BoxGroup("Sounds")][SerializeField] private Sex m_soundShock;
    [BoxGroup("Sounds")][SerializeField] private float m_screamVolume = 0.6f;
    [BoxGroup("Sounds")][SerializeField] private AudioSource m_screamSource;

    private Quaternion m_rotationTarget;

    private void Start() {
        m_child.onStateChanged += ChildStateChanged;
        m_screamSource.time = Random.Range(0, m_screamSource.time);
    }



    private void ChildStateChanged(Child.State previous, Child.State current) {
        switch ((previous, current)) {
            case (_, Child.State.Stunned):
                m_rotationTarget = Quaternion.LookRotation(Game.MainGameInfo.Krampus.transform.position - m_child.transform.position, Vector3.up);
                m_animator.SetTrigger(m_stunProperty);
                m_spriteRenderer.SetSprite(m_panicSprite, 2);
                m_soundShock.Play(transform.position);
                m_screamSource.Play();
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
        m_screamSource.volume = Mathf.Lerp(m_screamSource.volume, (m_child.CurrentState is Child.State.Panic or Child.State.InitialPanic) ? m_screamVolume : 0f, Time.deltaTime);
        m_model.rotation = Quaternion.Slerp(m_model.rotation, Quaternion.Euler(0, m_child.FacingAngle, 0), Time.deltaTime * m_turningSpeed);

        m_animator.SetBool(m_panicProperty, m_child.CurrentState is Child.State.Panic or Child.State.InitialPanic);
        m_animator.SetBool(m_reportingProperty, m_child.CurrentState == Child.State.Reporting);
        m_animator.SetFloat(m_speedProperty, m_child.Velocity / (m_child.CurrentState == Child.State.Panic ? m_child.RunSpeed : m_child.BaseMovementSpeed));
    }
}
