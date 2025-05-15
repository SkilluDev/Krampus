using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Audio;
using KrampUtils;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Sound {
    [CreateAssetMenu(menuName = "Game/Sex/Clip", fileName = "Sound")]
    public class ClipSex : Sex {
        public AudioClip clip;
        [MinMaxSlider(0.75f, 1.25f)] public Vector2 pitchRange = new Vector2(1f, 1f);
        [MinMaxSlider(0.0f, 2f)] public Vector2 volumeRange = new Vector2(1f, 1f);
        public float spatialBlend = 1f;
        public float minDistance = 20f;
        public float maxDistance = 20f;
        public AudioRolloffMode rolloff;

        internal override void PlayInternal(Vector3 location, AudioMixerGroup group, float volume = 1) {
            var source = MakeSource(location, group, clip.length);
            source.clip = clip;
            source.volume = Random.Range(volumeRange.x, volumeRange.y) * volume;
            source.pitch = Random.Range(pitchRange.x, pitchRange.y);
            source.spatialBlend = spatialBlend;
            source.rolloffMode = AudioRolloffMode.Linear;
            source.maxDistance = maxDistance;
            source.minDistance = minDistance;
            source.rolloffMode = rolloff;
            source.Play();
        }

#if UNITY_EDITOR
        [MenuItem("Assets/Create/Sex From Audio", false, -100)]
        public static void CreateScriptableObject() {
            if (Selection.objects.Length > 1) {
                foreach (var selection in Selection.objects) {
                    if (selection is not AudioClip) continue;
                    var so = ScriptableObject.CreateInstance<ClipSex>();
                    string assetPath = System.IO.Path.GetDirectoryName(AssetDatabase.GetAssetPath(selection.GetInstanceID()));
                    so.clip = (AudioClip)selection;
                    AssetDatabase.CreateAsset(so, assetPath + "/" + selection.name.ToPrettyCase() + ".asset");
                }
            } else {
                var selection = Selection.activeObject;
                if (selection is not AudioClip) return;
                var so = ScriptableObject.CreateInstance<ClipSex>();
                so.clip = (AudioClip)selection;
                ProjectWindowUtil.CreateAsset(so, selection.name.ToPrettyCase() + ".asset");
            }
        }
        [MenuItem("Assets/Create/Sex From Audio", true)]
        public static bool CreateScriptableObjectValidate() {
            return Selection.activeObject is AudioClip clip;
        }
#endif
    }
}