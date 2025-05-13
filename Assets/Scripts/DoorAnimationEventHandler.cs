using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorAnimationEventHandler : MonoBehaviour
{
	[SerializeField] private Collider m_interactionLeft;
	[SerializeField] private Collider m_interactionRight;

	public void DoorOff() {
		m_interactionLeft.enabled = false;
		m_interactionRight.enabled = false;
	}

	public void DoorOn() {
		m_interactionLeft.enabled = true;
		m_interactionRight.enabled = true;
	}
}
