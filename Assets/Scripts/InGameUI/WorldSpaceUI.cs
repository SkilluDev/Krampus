using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WorldSpaceUI : MonoBehaviour {
	// Start is called before the first frame update
	[SerializeField] private GameObject m_quickActionUI;
	public GameObject QuickActionUI => m_quickActionUI;

	public void SetDashIconPosition(IInteractable dashTarget) {
		Vector3 direction = dashTarget.InteractionPoint - Camera.main.transform.position;

		transform.position = dashTarget.InteractionPoint - direction.normalized * 1f;
	}

	public void SetDashIcon(bool canDash) {
		QuickActionUI.SetActive(canDash);
	}
}
