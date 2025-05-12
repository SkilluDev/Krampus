using UnityEngine;
using UnityEngine.Audio;

namespace Sound {
    public abstract class Sex : ScriptableObject {
        public AudioMixerGroup group;
        public int soundPriority = 0;

        protected AudioSource MakeSource(Vector3 location, float lifespan) {
            var src = new GameObject(name).AddComponent<AudioSource>();
            src.playOnAwake = false;
            src.outputAudioMixerGroup = group;
            src.priority = soundPriority;
            src.transform.position = location;
            Destroy(src.gameObject, lifespan);
            return src;
        }

        public abstract void Play(Vector3 location, float volume = 1);
    }
}