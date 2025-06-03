using System;
using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using TMPro;
using UnityEngine;
using UnityEngine.Playables;

public class OutroScript : MonoBehaviour {
	[SerializeField] Animator m_animator;
	[SerializeField] PlayableDirector m_director;
	[SerializeField] LineRenderer m_tongue;
	[SerializeField] private Transform jawOrigin;
	[SerializeField] private Transform jawTarget;
	bool isPlaying = false;


	private void Update() {
		if (!isPlaying) return;
		m_tongue.SetPosition(0, jawOrigin.position);
		m_tongue.SetPosition(1, jawTarget.position);
	}

	[Button("WIN")]
	public void PlayOutro() {
		m_director.Play();
		transform.position = Game.MainGameInfo.Krampus.transform.position;
		Game.MainGameInfo.Krampus.Animator.SetEnableModel(false);
		//m_animator.SetTrigger("Outro");
		isPlaying = true;
	}
}
