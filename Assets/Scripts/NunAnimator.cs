using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class NunAnimator : MonoBehaviour
{
	[SerializeField] private Animator m_animator;
	[SerializeField][AnimatorParam(nameof(m_animator))] private int m_speedProperty, m_stopProperty, m_tongueOutProperty, m_tongueReadyProperty;

	private float m_minimalVelocity;
	private Nun nunController;

	private void Start() {
		nunController = GetComponent<Nun>();
		nunController.onStateChanged += MovementStateChanged;
	}

	void Update() {
		m_animator.SetFloat(m_speedProperty, Mathf.Max(m_minimalVelocity, nunController.Velocity / nunController.RunSpeed), 0.2f, Time.deltaTime);
	}

	void MovementStateChanged(Nun.State previous, Nun.State current)
	{
		switch (previous, current) {
			case (Nun.State.Idle,Nun.State.ChasingKrampus):
				m_minimalVelocity = 1f;
				break;
		}

	}

}


