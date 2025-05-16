using LitMotion;
using LitMotion.Extensions;
using NaughtyAttributes.Test;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MusicMan : MonoBehaviour {
	[SerializeField] private AudioSource m_gameMusicLayer1;
	[SerializeField] private AudioSource m_gameMusicLayer2;

	[SerializeField] private float m_fadeOutTime = 5f;

	public float GameMusicLayer2Volume { get=>m_gameMusicLayer2.volume; set=>m_gameMusicLayer2.volume=value;}

	private MotionHandle m_motionHandle1;
	private MotionHandle m_motionHandle2;
	public void Ready() {
		if(m_motionHandle1.IsActive()) m_motionHandle1.Cancel();
		if(m_motionHandle2.IsActive()) m_motionHandle2.Cancel();
		switch (Game.CurrentState) {
			case Game.State.MainGame:
				m_gameMusicLayer2.volume = 1;
				m_gameMusicLayer1.Play();
				m_gameMusicLayer2.volume = 0;
				m_gameMusicLayer2.Play();
				break;
			default:
				StopMusic();
				break;
		}
	}

	public void StopMusic() {
		m_motionHandle1 = LMotion.Create(m_gameMusicLayer1.volume,0,m_fadeOutTime).WithOnComplete(m_gameMusicLayer1.Stop).BindToVolume(m_gameMusicLayer1);
		m_motionHandle2 = LMotion.Create(m_gameMusicLayer2.volume,0,m_fadeOutTime).WithOnComplete(m_gameMusicLayer2.Stop).BindToVolume(m_gameMusicLayer2);
	}


}
