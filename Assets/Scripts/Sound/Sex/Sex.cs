using UnityEngine;
using UnityEngine.Audio;
using System.Runtime.CompilerServices;

namespace Sound {
    public abstract class Sex : ScriptableObject {
        public AudioMixerGroup group;
        public int soundPriority = 0;

        protected AudioSource MakeSource(Vector3 location, AudioMixerGroup group, float lifespan) {
            var src = new GameObject(name).AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.outputAudioMixerGroup = group;
            src.priority = soundPriority;
            src.transform.position = location;
            Destroy(src.gameObject, lifespan);
            return src;
        }

        internal abstract void PlayInternal(Vector3 location, AudioMixerGroup group, float volume = 1);
    }

    public static class SexTention {
        public static void Play(this Sex sex, Vector3 location, float volume = 1, [CallerMemberName] string message = null) {
            if (sex != null) sex.PlayInternal(location, sex.group, volume);
            else Debug.LogWarning($"[Sex] No sound to play! ({message})");
        }
    }
}
