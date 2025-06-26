using System;
using System.Collections;
using System.Collections.Generic;
using LitMotion;
using LitMotion.Extensions;
using Sound;
using UnityEngine;

public class WorldSpaceUI : MonoBehaviour {
	// Start is called before the first frame update
	[SerializeField] private GameObject m_quickActionUI;
	public GameObject QuickActionUI => m_quickActionUI;
	// todo: sorry im in a rush
	[SerializeField] private Sex m_sexCue;

	public void SetDashIconPosition(IInteractable dashTarget) {
		Vector3 direction = dashTarget.InteractionPoint - Camera.main.transform.position;

		transform.position = dashTarget.InteractionPoint - direction.normalized * 1f;
	}

	public void SetDashIcon(bool canDash) {
		m_quickActionUI.SetActive(canDash);
		if (canDash) {
			var oldScale = m_quickActionUI.transform.localScale;
			LMotion.Create(oldScale, oldScale * 1.35f, 0.25f).WithEase(Ease.InOutBack).WithOnComplete(
			   () => LMotion.Create(oldScale * 1.35f, oldScale, 0.25f).WithEase(Ease.OutBounce).BindToLocalScale(m_quickActionUI.transform)
		   ).BindToLocalScale(m_quickActionUI.transform);
			m_sexCue.Play(transform.position, 1);
		}
	}
}
