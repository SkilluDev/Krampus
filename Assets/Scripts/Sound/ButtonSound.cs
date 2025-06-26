using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using Sound;
using UnityEngine;

public class ButtonSound : MonoBehaviour {
	[BoxGroup("Sounds")][SerializeField] private Sex m_sexHover;
	[BoxGroup("Sounds")][SerializeField] private Sex m_sexClick;

	public void OnHoverSound() {
		m_sexHover.Play(transform.position);
	}

	public void OnClickSound() {
		m_sexClick.Play(transform.position);
	}
}
