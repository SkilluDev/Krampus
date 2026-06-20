using LitMotion;
using LitMotion.Extensions;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.SceneManagement;

public class MusicMan : MonoBehaviour {

	[SerializeField] private AudioMixer m_mixer;
	[SerializeField] private AudioSource m_gameMusicLayer1;
	[SerializeField] private AudioSource m_gameMusicLayer2;

	[SerializeField] private float m_fadeOutTime = 5f;

	[SerializeField] private float m_maxMusicVol = 0f;
	[SerializeField] private float m_maxSFXVol = 0f;

	public float GameMusicLayer2Volume { get => m_gameMusicLayer2.volume; set => m_gameMusicLayer2.volume = value; }

	private MotionHandle m_motionHandle1;
	private MotionHandle m_motionHandle2;
	public void Ready() {
		Game.GlobalEvents.onSetManChange.AddListener(OnSetManChange);
		UpdateMixer();
		switch (Game.CurrentState) {
			case Game.State.MainGame:
				if (m_motionHandle1.IsActive()) m_motionHandle1.Cancel();
				if (m_motionHandle2.IsActive()) m_motionHandle2.Cancel();
				m_gameMusicLayer1.volume = 0.5f;
				m_gameMusicLayer1.Play();
				m_gameMusicLayer2.volume = 0;
				m_gameMusicLayer2.Play();
				break;
			default:
				StopMusic();
				break;
		}
	}

	private void Unready() {
		Game.GlobalEvents.onSetManChange.RemoveListener(OnSetManChange);
	}

	private void OnSetManChange(string key) {
		if (key == "Music Volume" || key == "SFX Volume") {
			UpdateMixer();
		}
	}

	public void StopMusic() {
		if (m_motionHandle1.IsActive()) m_motionHandle1.Cancel();
		if (m_motionHandle2.IsActive()) m_motionHandle2.Cancel();
		m_motionHandle1 = LMotion.Create(m_gameMusicLayer1.volume, 0, m_fadeOutTime).WithOnComplete(m_gameMusicLayer1.Stop).BindToVolume(m_gameMusicLayer1);
		m_motionHandle2 = LMotion.Create(m_gameMusicLayer2.volume, 0, m_fadeOutTime).WithOnComplete(m_gameMusicLayer2.Stop).BindToVolume(m_gameMusicLayer2);
	}

	public void UpdateMixer() {
		m_mixer.SetFloat("MusicVolume", ProcessFloatVolume(Game.SetMan.GetValue<long>("Music Volume"), m_maxMusicVol));
		m_mixer.SetFloat("SFXVolume", ProcessFloatVolume(Game.SetMan.GetValue<long>("SFX Volume"), m_maxSFXVol));
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
