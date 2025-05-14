using NaughtyAttributes;
using UnityEngine;

namespace Sound {
    [CreateAssetMenu(menuName = "Game/Sex/Random", fileName = "Sound")]
    public class RandomSex : Sex {
        public Sex[] clips;
        private int m_lastRandom = -1;
        public bool preventDoubleRandom = false;

        internal override void PlayInternal(Vector3 location, float volume = 1) {
            if (clips.Length == 0) {
                Debug.LogError("Effect {name} contains no clips!");
                return;
            }

            int selection;
            while ((selection = Random.Range(0, clips.Length)) == m_lastRandom && preventDoubleRandom) { }

            clips[selection].PlayInternal(location, volume);
        }
    }
}