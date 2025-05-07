using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Reflection;

namespace Settings {
    public class SetMan : MonoBehaviour {
        private Dictionary<string, ParametrizedSetting> m_settingDictionary;

#if !UNITY_EDITOR // cursed? maybe. we'll see
        public IReadOnlyCollection<Setting> Settings => m_settings;
#else 
        public List<ParametrizedSetting> Settings => m_settings;
#endif

        [SerializeField] private List<ParametrizedSetting> m_settings = new List<ParametrizedSetting>();

        [SerializeField] private string m_json;


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
                var setting = (Setting)EditorGUILayout.ObjectField("Setting", Target.Settings[i].setting, typeof(Setting), false);

                if (setting != Target.Settings[i].setting) {
                    Target.Settings[i].setting = setting;
                    if (setting == null) Target.Settings[i].parameters.Clear();
                    EditorUtility.SetDirty(Target);
                }

                if (setting) {
                    var properties = Target.Settings[i].GetAvailableProperties();

                    if (DrawPropertyNotify(Target.Settings[i], properties.First(w => w.Name == "name"), "Name")) EditorUtility.SetDirty(Target);

                    foreach (var p in properties) {
                        if (p.Name == "name") continue;
                        if (DrawPropertyNotify(Target.Settings[i], p)) EditorUtility.SetDirty(Target);
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

        private bool DrawPropertyNotify(ParametrizedSetting parametrizedSetting, FieldInfo p, string nameOverride = null) {
            string displayName = nameOverride ?? p.Name;
            switch (p.FieldType) {
                case Type _ when p.FieldType == typeof(int): {
                        int original = (int)parametrizedSetting.GetParamIntegral(p.Name);
                        int value = EditorGUILayout.IntField(displayName, original);
                        if (original == value) break;

                        parametrizedSetting.SetParam<Int32>(p.Name, value);
                        return true;
                    }
                case Type _ when p.FieldType == typeof(string): {
                        string original = parametrizedSetting.GetParam<string>(p.Name);
                        string value = EditorGUILayout.TextField(displayName, original);
                        if (original == value) break;

                        parametrizedSetting.SetParam<string>(p.Name, value);
                        return true;
                    }
            }

            return false;
        }
    }

#endif

}
