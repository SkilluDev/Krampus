using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.AI;

public class NunAnimator : MonoBehaviour
{
	[SerializeField] private Animator m_animator;
	[SerializeField] private Transform m_modelTransform;

	[SerializeField][AnimatorParam(nameof(m_animator))] private int m_speedProperty, m_listeningProperty, m_attackTrigger, m_tongueReadyProperty;

	private float m_minimalVelocity;
	private Nun nunController;

	private void Start() {
		nunController = GetComponent<Nun>();
		nunController.onStateChanged += MovementStateChanged;
		nunController.onAttack += OnNunAttack;
	}

	void Update() {
		m_animator.SetFloat(m_speedProperty, Mathf.Max(m_minimalVelocity, nunController.Velocity / nunController.RunSpeed), 0.2f, Time.deltaTime);
		if (nunController.VelocitySqr > 0.4f) {
			m_modelTransform.rotation = Quaternion.LookRotation(nunController.VelocityVector, Vector3.up);
		}
	}

	void MovementStateChanged(Nun.State previous, Nun.State current)
	{
		switch (previous, current) {
			case (Nun.State.Idle,Nun.State.ChasingKrampus):
				m_minimalVelocity = 1f;
				break;
			case(Nun.State.Idle,Nun.State.Listening):
				m_animator.SetBool(m_listeningProperty, true);
				break;
			case (Nun.State.Listening,Nun.State.Idle):
				m_animator.SetBool(m_listeningProperty, false);
				break;
			case (Nun.State.Listening,Nun.State.ChasingKrampus):
				m_animator.SetBool(m_listeningProperty, false);
				break;
		}
	}

	void OnNunAttack(Nun.State state) {
		m_animator.SetTrigger(m_attackTrigger);
	}

}


