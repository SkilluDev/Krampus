using System;
using System.Collections;
using System.Collections.Generic;
using KrampUtils;
using UnityEngine;
using Random = UnityEngine.Random;

public class ArrowController : MonoBehaviour {
	private Transform m_target;
	private Transform m_krampus;
	[SerializeField] private GameObject m_arrowModel;


	private void Start() {
		m_krampus = Game.MainGameInfo.Krampus.transform;
	}

	private void Update() {

		if (Game.MainGameInfo.Timer.GameTime <= 15) {


			if (m_target == null) {
				var o = Game.MainGameInfo.GoodChildren;

				m_target = o.UnityRandomElement().transform;
			}

		} else {
			m_target = null;
			m_arrowModel.SetActive(false);
		}





		if (m_target != null) {
			m_arrowModel.SetActive(true);
			transform.position = m_krampus.position;
			Vector3 direction = (m_target.position - transform.position).normalized;
			transform.rotation = Quaternion.LookRotation(direction);

		}
	}
}
