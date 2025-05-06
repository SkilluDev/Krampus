using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;


[CreateAssetMenu(menuName = "Game/Sound Bite", fileName = "Sound")]
public class SoundBite : ScriptableObject {
	public AudioClip[] clips;
	public AudioMixerGroup group;
	private int m_nextIndex = -1;
	[MinMaxSlider(0.5f, 2f)] public Vector2 pitch = new Vector2(0.9f, 1.1f);
	[MinMaxSlider(0.5f, 2f)] public Vector2 volume = new Vector2(0.9f, 1.1f);
	public float GetPitch() => Random.Range(pitch.x, pitch.y);
	public float GetVolume() => Random.Range(volume.x, volume.y);
	public AudioClip GetClip(bool sequential = false) => clips[sequential?Random.Range(0, clips.Length):m_nextIndex];

	public void Play(Vector3 location, float threed, bool sequential = false) {
		m_nextIndex = (m_nextIndex + 1) % clips.Length;
		var src = new GameObject(name).AddComponent<AudioSource>();
		src.spatialBlend = threed;
		src.volume = GetVolume();
		src.pitch = GetPitch();
		var clip = GetClip(sequential);
		src.clip = clip;
		src.outputAudioMixerGroup = group;
		src.playOnAwake = false;

		src.transform.position = location;

		src.Play();
		Destroy(src.gameObject, clip.length);
	}
}
