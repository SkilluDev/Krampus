using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;

namespace Settings {
    public class SetMan : MonoBehaviour, ISerializationCallbackReceiver {
        private Dictionary<string, Setting> m_settingDictionary;

#if !UNITY_EDITOR // cursed? maybe. we'll see
        public IReadOnlyCollection<Setting> Settings => m_settings;
#else 
        public List<Setting> Settings => m_settings;
#endif

        private List<Setting> m_settings = new List<Setting>();

        [SerializeField] private string m_json;

        [SerializeField] private List<GameObject> m_fieldPrefabs;

        private void Compile() {
            m_settingDictionary = new Dictionary<string, Setting>();
            foreach (var w in m_settings) {
                m_settingDictionary.Add(w.name, w);
            }
        }

        public T GetSetting<T>(string name) where T : Setting {
            if (m_settingDictionary == null) Compile();

            return (T)m_settingDictionary[name];
        }

        public string ToJson() {
            return JsonConvert.SerializeObject(m_settings, new JsonSerializerSettings() {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
                Formatting = Formatting.Indented
            });
        }

        public void FromJson(string json) {
            m_settings = JsonConvert.DeserializeObject<List<Setting>>(json, new JsonSerializerSettings() {
                TypeNameHandling = TypeNameHandling.Objects,
                TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
            });
        }

        public void OnBeforeSerialize() {
            m_json = ToJson();
        }
        public void OnAfterDeserialize() {
            FromJson(m_json);
        }
    }


#if UNITY_EDITOR

    [CustomEditor(typeof(SetMan))]
    public class SetManEditor : Editor {
        public SetMan Target => (SetMan)target;

        public override void OnInspectorGUI() {
            EditorGUILayout.TextArea(Target.ToJson());

            foreach (var set in Target.Settings) {
                GUILayout.Label(set.name);
                set.OnInspectorGUI();
                GUILayout.Space(10);
            }


            if (GUILayout.Button("New Integer")) {
                Target.Settings.Add(new IntegerSetting());
                EditorUtility.SetDirty(Target);
            }

            base.OnInspectorGUI();
        }
    }

#endif

}
