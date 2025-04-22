using NaughtyAttributes;
using UnityEngine;

public class KrampusAnimator : KrampusBehaviour {
    [SerializeField] private Animator m_animator;
    [SerializeField] private Transform m_modelTransform;
    [SerializeField] private float m_fastVelocity = 4;
    [SerializeField] private float m_rotationSmoothing = 15;

    [SerializeField][AnimatorParam(nameof(m_animator))] private int m_speedProperty, m_stopProperty;

    private float m_minimalVelocity;

    private void Update() {
        if (Kramp.Kontroller.CurrentState != KrampusController.State.Idle) {
            m_modelTransform.rotation = Quaternion.Slerp(m_modelTransform.rotation, Quaternion.LookRotation(Kramp.Kontroller.VelocityVector, Vector3.up), Time.deltaTime * m_rotationSmoothing);
        }

        m_animator.SetFloat(m_speedProperty, Mathf.Max(m_minimalVelocity, Kramp.Kontroller.Velocity / Kramp.Kontroller.RunSpeed),0.1f, Time.deltaTime);
    }

    public void MovementStateChanged(KrampusController.State previous, KrampusController.State current, bool isSudden) {
        switch ((previous, current)) {
            case (KrampusController.State.Run, KrampusController.State.Idle):
	            if (isSudden) {
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
        }
    }
}
