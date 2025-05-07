using NaughtyAttributes;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour {
	[BoxGroup("Sounds")][SerializeField] private SoundBite m_stepSoundBite;

	public void Step() {
		m_stepSoundBite.Play(transform.position, 1, true);
	}
}
