using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Serialization;

public class KrampusAnimator : KrampusBehaviour {
    [SerializeField] private Animator m_animator;
    [SerializeField] private Transform m_modelTransform;
    [SerializeField] private float m_rotationSmoothing = 15;

    [BoxGroup("Sounds")][SerializeField][FormerlySerializedAs("m_CatchSoundBite")] private SoundBite m_catchSoundBite;
    [BoxGroup("Sounds")][SerializeField][FormerlySerializedAs("m_TongueSoundBite")] private SoundBite m_tongueSoundBite;

    [SerializeField][AnimatorParam(nameof(m_animator))] private int m_speedProperty, m_stopProperty, m_tongueOutProperty, m_tongueReadyProperty, m_tongueShouldEatProperty, m_deathTrigger;

    private float m_minimalVelocity;
    private Quaternion m_rotationTarget;

    private void Start() {
        Kramp.Tongue.onStateChanged += TongueStateChanged;
        Kramp.Kontroller.onStateChanged += MovementStateChanged;
    }

    private void Update() {
        if (Kramp.Kontroller.CurrentState != KrampusController.State.Idle && Kramp.Tongue.CurrentState == KrampusTongue.State.Idle) {
            SetTargetView(Kramp.Kontroller.VelocityVector);
        } else if (Kramp.Tongue.CurrentState == KrampusTongue.State.Windup) {
            SetTargetView(Kramp.Tongue.TongueDirection);
        }

        m_modelTransform.rotation = Quaternion.Slerp(m_modelTransform.rotation, m_rotationTarget, Time.deltaTime * m_rotationSmoothing);
        m_animator.SetFloat(m_speedProperty, Mathf.Max(m_minimalVelocity, Kramp.Kontroller.Velocity / Kramp.Kontroller.RunSpeed), 0.2f, Time.deltaTime);
    }

    public void TongueStateChanged(KrampusTongue.State previous, KrampusTongue.State current) {
        switch ((previous, current)) {
            case (KrampusTongue.State.Idle, KrampusTongue.State.Windup):
                m_animator.SetBool(m_tongueReadyProperty, true);
                break;
            case (KrampusTongue.State.Windup, KrampusTongue.State.Idle):
                m_animator.SetBool(m_tongueReadyProperty, false);
                break;
            case (KrampusTongue.State.Windup, KrampusTongue.State.TargetFetch):
                m_animator.SetBool(m_tongueOutProperty, true);
                m_tongueSoundBite.Play(transform.position, 1, true);
                break;
            case (_, KrampusTongue.State.PreRetreat):
                m_animator.SetBool(m_tongueOutProperty, false);
                m_animator.SetBool(m_tongueReadyProperty, false);
                m_animator.SetBool(m_tongueShouldEatProperty, false);

                break;
            case (KrampusTongue.State.TargetFetch, KrampusTongue.State.Extending):
                SetTargetView(Kramp.Tongue.TongueDirection);
                break;
            case (KrampusTongue.State.PreRetreat, KrampusTongue.State.Retreating):
                if (Kramp.Tongue.HitInteractable != null) {
                    m_catchSoundBite.Play(transform.position, 1, true);

                }
                break;


        }
    }

    public void MovementStateChanged(KrampusController.State previous, KrampusController.State current, KrampusController.StateChangeReason reason) {
        switch ((previous, current)) {
            case (KrampusController.State.Run, KrampusController.State.Idle):
                if (reason == KrampusController.StateChangeReason.Rapid) {
                    m_animator.SetTrigger(m_stopProperty);
                }
                m_minimalVelocity = 0f;
                break;
            case (KrampusController.State.Idle, KrampusController.State.Run):
                m_minimalVelocity = 1f;
                break;
            case (_, KrampusController.State.Walk):
                m_minimalVelocity = 0.05f;
                break;
            case (_, KrampusController.State.Idle):
                m_minimalVelocity = 0f;
                break;
            case (_, KrampusController.State.Dead):
                m_animator.SetTrigger(m_deathTrigger);
                break;
        }
    }

    private void SetTargetView(Vector3 direction) {
        m_rotationTarget = Quaternion.LookRotation(direction, Vector3.up);
    }
}
