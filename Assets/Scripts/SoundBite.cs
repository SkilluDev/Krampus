using System.Collections;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;


[CreateAssetMenu(menuName = "Game/Sound Bite", fileName = "Sound")]
public class SoundBite : ScriptableObject {
	public AudioClip[] clips;
	public AudioMixerGroup group;
	private int m_nextIndex = -1;
	[SerializeField] private float m_pitch = 0.91f;
	[SerializeField] private float m_volume = 1f;
	private float m_deviation = 0.1f;
	public float GetPitch(bool random = false) => random?Random.Range(m_pitch-m_deviation, m_pitch+m_deviation):m_pitch;
	public float GetVolume(bool random = false) => random?Random.Range(m_volume-m_deviation, m_volume+m_deviation):m_volume;
	public AudioClip GetClip(bool sequential = false) => clips[sequential?Random.Range(0, clips.Length):m_nextIndex];

	public void Play(Vector3 location, float threed, bool sequential = false, bool random = false) {
		m_nextIndex = (m_nextIndex + 1) % clips.Length;
		var src = new GameObject(name).AddComponent<AudioSource>();
		src.spatialBlend = threed;
		src.volume = GetVolume(random);
		src.pitch = GetPitch(random);
		src.priority = 0;
		var clip = GetClip(sequential);
		src.clip = clip;
		src.outputAudioMixerGroup = group;
		src.playOnAwake = false;

		src.transform.position = location;

		src.Play();

		Destroy(src.gameObject, clip.length);
	}

}
