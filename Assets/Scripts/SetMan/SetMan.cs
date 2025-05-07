using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Linq;

namespace Settings {
    public class SetMan : MonoBehaviour {
        private Dictionary<string, ParametrizedSetting> m_settingDictionary;

#if !UNITY_EDITOR // cursed? maybe. we'll see
        public IReadOnlyCollection<Setting> Settings => m_settings;
#else 
        public List<ParametrizedSetting> Settings => m_settings;
#endif

        private List<ParametrizedSetting> m_settings = new List<ParametrizedSetting>();

        [SerializeField] private string m_json;

        [SerializeField] private List<GameObject> m_fieldPrefabs;

        private void Compile() {
            m_settingDictionary = new Dictionary<string, ParametrizedSetting>();
            foreach (var w in m_settings) {
                m_settingDictionary.Add(w.Name, w);
            }
        }

        public T GetSetting<T>(string name) where T : Setting {
            if (m_settingDictionary == null) Compile();

            return (T)m_settingDictionary[name].setting;
        }

        // public string ToJson() {
        //     return JsonConvert.SerializeObject(m_settings, new JsonSerializerSettings() {
        //         TypeNameHandling = TypeNameHandling.Objects,
        //         TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple,
        //         Formatting = Formatting.Indented
        //     });
        // }

        // public void FromJson(string json) {
        //     m_settings = JsonConvert.DeserializeObject<List<Setting>>(json, new JsonSerializerSettings() {
        //         TypeNameHandling = TypeNameHandling.Objects,
        //         TypeNameAssemblyFormatHandling = TypeNameAssemblyFormatHandling.Simple
        //     });
        // }

    }


#if UNITY_EDITOR

    [CustomEditor(typeof(SetMan))]
    public class SetManEditor : Editor {
        public SetMan Target => (SetMan)target;

        public override void OnInspectorGUI() {

            for (int i = 0; i < Target.Settings.Count; i++) {
                EditorGUILayout.BeginToggleGroup(Target.Settings[i].setting != null ? Target.Settings[i].Name : "[none]", true);
                Target.Settings[i].setting = (Setting)EditorGUILayout.ObjectField("Setting", Target.Settings[i].setting, typeof(Setting), false);
                var setting = Target.Settings[i].setting;
                if (setting) {
                    var properties = Target.Settings[i].GetAvailableProperties();

                    var serializedObject = new SerializedObject(setting);
                    foreach (var p in properties) {
                        var property = serializedObject.FindProperty(p.Name);
                        EditorGUILayout.PropertyField(property);
                    }
                }
                EditorGUILayout.EndToggleGroup();
                GUILayout.Space(10);
            }


            if (GUILayout.Button("Add Setting")) {
                Target.Settings.Add(new ParametrizedSetting());
                EditorUtility.SetDirty(Target);
            }

        }
    }

#endif

}
