using LitMotion;
using LitMotion.Extensions;
using NaughtyAttributes.Test;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicMan : MonoBehaviour {

	[SerializeField] private AudioMixer m_mixer;
	[SerializeField] private AudioSource m_gameMusicLayer1;
	[SerializeField] private AudioSource m_gameMusicLayer2;

	[SerializeField] private float m_fadeOutTime = 5f;

	public float GameMusicLayer2Volume { get => m_gameMusicLayer2.volume; set => m_gameMusicLayer2.volume = value; }

	private MotionHandle m_motionHandle1;
	private MotionHandle m_motionHandle2;
	public void Ready() {
		switch (Game.CurrentState) {
			case Game.State.MainGame:
				if (m_motionHandle1.IsActive()) m_motionHandle1.Cancel();
				if (m_motionHandle2.IsActive()) m_motionHandle2.Cancel();
				m_gameMusicLayer1.volume = 1;
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
		m_motionHandle1 = LMotion.Create(m_gameMusicLayer1.volume, 0, m_fadeOutTime).WithOnComplete(m_gameMusicLayer1.Stop).BindToVolume(m_gameMusicLayer1);
		m_motionHandle2 = LMotion.Create(m_gameMusicLayer2.volume, 0, m_fadeOutTime).WithOnComplete(m_gameMusicLayer2.Stop).BindToVolume(m_gameMusicLayer2);
	}

	private void Update() {
		UpdateMixer();
	}

	public void UpdateMixer() {
		m_mixer.SetFloat("MusicVolume", ProcessFloatVolume(Game.SetMan.GetValue<long>("Music volume"), -10f));
		m_mixer.SetFloat("SFXVolume", ProcessFloatVolume(Game.SetMan.GetValue<long>("SFX volume")));
	}

	private float ProcessFloatVolume(float volume, float max = 0f) {
		return math.remap(
					-80f, 0f, -80f, max, Mathf.Log10(
						math.remap(
							0, 100, 0.0001f, 1f, Mathf.Clamp(volume, 0, 100)
						)
					) * 20
				);
	}


}
