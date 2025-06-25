using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;

namespace Sound {
    [CreateAssetMenu(menuName = "Game/Sex/Varied Volume", fileName = "Sound")]
    public class VariedVolumeSex : Sex {
        [System.Serializable]
        public class Clip {
            [MinMaxSlider(0, 1)] public Vector2 volume;
            public Sex clip;
        }

        public Clip[] clips;
        public bool remapVolume = false;
        public bool forceFullVolume = false;

        internal override void PlayInternal(Vector3 location, AudioMixerGroup group, float volume = 1) {
            if (clips.Length == 0) {
                Debug.LogError("Effect {name} contains no clips!");
                return;
            }

            var selected = clips.FirstOrDefault(clip => clip.volume.x <= volume && clip.volume.y >= volume);
            float vol = forceFullVolume ? 1 :
                remapVolume ? Mathf.InverseLerp(selected.volume.x, selected.volume.y, volume) : volume;
            selected.clip.PlayInternal(location, group, vol);
        }
    }
}