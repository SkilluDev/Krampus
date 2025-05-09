using NaughtyAttributes;
using UnityEngine;

public class NunAnimator : MonoBehaviour {
	[SerializeField] private Nun m_nun;
	[SerializeField] private Animator m_animator;
	[SerializeField] private Transform m_modelTransform;

	[SerializeField][AnimatorParam(nameof(m_animator))] private int m_speedProperty, m_listeningProperty, m_attackProperty, m_stunnedProperty;

	private float m_minimalVelocity;

	private void Start() {
		m_nun.onStateChanged += MovementStateChanged;
		m_nun.onAttack += OnNunAttack;
	}

	private void Update() {
		if (m_nun.VelocitySqr > 0.2f) {
			m_modelTransform.rotation = Quaternion.LookRotation(m_nun.VelocityVector, Vector3.up);
		}

		m_animator.SetFloat(m_speedProperty, Mathf.Max(m_minimalVelocity, m_nun.Velocity / m_nun.RunSpeed), 0.2f, Time.deltaTime);
		m_animator.SetBool(m_listeningProperty, m_nun.CurrentState == Nun.State.Listening);
		m_animator.SetBool(m_stunnedProperty, m_nun.CurrentState == Nun.State.Stunned);
	}

	private void MovementStateChanged(Nun.State previous, Nun.State current) {
		switch (previous, current) {
			case (Nun.State.Idle, Nun.State.ChasingKrampus):
				m_minimalVelocity = 1f;
				break;
		}
	}

	private void OnNunAttack(Nun.State state) {
		m_animator.SetTrigger(m_attackProperty);
	}

}


