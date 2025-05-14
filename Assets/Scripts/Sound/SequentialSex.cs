using NaughtyAttributes;
using UnityEngine;

namespace Sound {
    [CreateAssetMenu(menuName = "Game/Sex/Sequential", fileName = "Sound")]
    public class SequentialSex : Sex {
        public Sex[] clips;
        [MinMaxSlider(1, 3)] public Vector2Int sequenceJump = new Vector2Int(1, 1);
        private int m_sequenceIndex;

        internal override void PlayInternal(Vector3 location, float volume = 1) {
            if (clips.Length == 0) {
                Debug.LogError("Effect {name} contains no clips!");
                return;
            }
            m_sequenceIndex += Random.Range(sequenceJump.x, sequenceJump.y + 1);
            m_sequenceIndex %= clips.Length;
            clips[m_sequenceIndex].PlayInternal(location, volume);
        }
    }
}