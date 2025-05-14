using NaughtyAttributes;
using UnityEngine;

namespace Sound {
    [CreateAssetMenu(menuName = "Game/Sex/Clip", fileName = "Sound")]
    public class ClipSex : Sex {
        public AudioClip clip;
        [MinMaxSlider(0.75f, 1.25f)] public Vector2 pitchRange = new Vector2(1f, 1f);
        [MinMaxSlider(0.0f, 2f)] public Vector2 volumeRange = new Vector2(1f, 1f);
        public float spatialBlend = 1f;
        public float maxDistance = 20f;

        internal override void PlayInternal(Vector3 location, float volume = 1) {
            var source = MakeSource(location, clip.length);
            source.clip = clip;
            source.volume = Random.Range(volumeRange.x, volumeRange.y) * volume;
            source.pitch = Random.Range(pitchRange.x, pitchRange.y);
            source.spatialBlend = spatialBlend;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.maxDistance = maxDistance;
            source.Play();
        }
    }
}