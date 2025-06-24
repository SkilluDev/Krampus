using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using UnityEngine;

public class CanNunsCook : MonoBehaviour {
    [SerializeField] private Animator m_cylinder, m_normal;
    [SerializeField] private NunAnimator m_nunAnimator;

	[SerializeField] private Nun m_nun;



	private void CookCheck() {
		if (Game.SetMan.GetValue<bool>("Walter White")) {
			Cook();
		} else {
			Uncook();
		}
	}

	private void Ready() {
		CookCheck();

		Game.GlobalEvents.onSetManChange.AddListener(OnSetManChange);
	}

	private void Unready() {
		Game.GlobalEvents.onSetManChange.RemoveListener(OnSetManChange);
	}

	private void OnSetManChange(string key) {
		if (key == "Walter White") {
			CookCheck();
		}
	}

	private void Cook() {
		m_cylinder.gameObject.SetActive(true);
		m_cylinder.gameObject.transform.SetAsFirstSibling();
		m_nun.SetModel();
		m_normal.gameObject.SetActive(false);
		m_nunAnimator.GetType().GetField("m_animator", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(m_nunAnimator, m_cylinder);
		m_nunAnimator.GetType().GetField("m_modelTransform", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(m_nunAnimator, m_cylinder.transform);
		Debug.Log("🗣️ THOSE NUNS CAN FUCKING COOK 🔥🔥");
	}

	private void Uncook() {
		m_cylinder.gameObject.SetActive(false);
		m_normal.gameObject.SetActive(true);
		m_normal.gameObject.transform.SetAsFirstSibling();
		m_nun.SetModel();
		m_nunAnimator.GetType().GetField("m_animator", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(m_nunAnimator, m_normal);
		m_nunAnimator.GetType().GetField("m_modelTransform", BindingFlags.Instance | BindingFlags.NonPublic).SetValue(m_nunAnimator, m_normal.transform);
	}
}
