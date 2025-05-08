using NaughtyAttributes.Test;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicMan : MonoBehaviour {
	[SerializeField] private AudioSource m_gameMusicLayer1;
	[SerializeField] private AudioSource m_gameMusicLayer2;

	public float GameMusicLayer2Volume { get=>m_gameMusicLayer2.volume; set=>m_gameMusicLayer2.volume=value;}
	public void Ready() {
		switch (Game.CurrentState) {
			case Game.State.MainGame:
				m_gameMusicLayer1.Play();
				m_gameMusicLayer2.volume = 0;
				m_gameMusicLayer2.Play();
				break;
			default:
				m_gameMusicLayer1.Stop();
				m_gameMusicLayer2.Stop();
				break;
		}
	}


}
