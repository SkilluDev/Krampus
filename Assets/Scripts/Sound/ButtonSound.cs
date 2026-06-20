using System.Collections;
using System.Collections.Generic;
using SaintsField;
using SaintsField.Playa;
using Sound;
using UnityEngine;

public class ButtonSound : MonoBehaviour {
	[Layout("Sounds", ELayout.FoldoutBox)][SerializeField] private Sex m_sexHover;
	[Layout("Sounds", ELayout.FoldoutBox)][SerializeField] private Sex m_sexClick;

	public void OnHoverSound() {
		m_sexHover.Play(transform.position);
	}

	public void OnClickSound() {
		m_sexClick.Play(transform.position);
	}
}
