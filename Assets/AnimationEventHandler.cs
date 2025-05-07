using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class AnimationEventHandler : MonoBehaviour
{
	[BoxGroup("Sounds")][SerializeField] private SoundBite m_StepSoundBite;

	public void Step() {
		m_StepSoundBite.Play(transform.position, 1, true);
	}
}
