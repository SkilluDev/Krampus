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
        private Dictionary<string, object> m_values;

#if !UNITY_EDITOR // cursed? maybe. we'll see
		public IReadOnlyCollection<ParametrizedSetting> Settings => m_settings;
#else
        public List<ParametrizedSetting> Settings => m_settings;
#endif
        public bool Ready => m_settingDictionary != null;

        [SerializeField] private List<ParametrizedSetting> m_settings = new List<ParametrizedSetting>();

        [SerializeField] private string m_json;


        private void Awake() {
            Compile();
        }

        private void Compile() {
            m_settingDictionary = new Dictionary<string, ParametrizedSetting>();
            m_values = new Dictionary<string, object>();
            foreach (var w in m_settings) {
                m_settingDictionary.Add(w.Name, w);
            }
        }

        public void SetValue<T>(string key, T value) {
            if (m_settingDictionary[key].setting is not ValueSetting<T>) throw new Exception();
            m_values[key] = value;
        }

        public T GetValue<T>(string key) {
            if (!m_values.ContainsKey(key))
                return m_settingDictionary[key].GetParam<T>("defaultValue");
            return (T)m_values[key];
        }



        public object GetValue(string key) {
            if (!m_values.ContainsKey(key))
                return m_settingDictionary[key].GetParam("defaultValue");
            return m_values[key];
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
                bool wantsDie = EditorGUILayout.BeginToggleGroup(Target.Settings[i].setting != null ? Target.Settings[i].Name : "[none]", true);

                if (!wantsDie) {
                    Target.Settings.RemoveAt(i);
                    return;
                }

                var setting = (Setting)EditorGUILayout.ObjectField("Setting", Target.Settings[i].setting, typeof(Setting), false);

                if (setting != Target.Settings[i].setting) {
                    if (setting == null || (Target.Settings[i].setting != null && setting.GetType() != Target.Settings[i].setting.GetType())) Target.Settings[i].parameters.Clear();
                    Target.Settings[i].setting = setting;
                    EditorUtility.SetDirty(Target);
                }

                if (setting) {
                    var properties = Target.Settings[i].GetAvailableProperties();

                    if (DrawPropertyNotify(Target.Settings[i], properties.First(w => w.Name == "name"), "Name")) EditorUtility.SetDirty(Target);

                    foreach (var p in properties) {
                        if (p.Name == "name") continue;
                        if (DrawPropertyNotify(Target.Settings[i], p)) EditorUtility.SetDirty(Target);
                    }

                    if (Target.Ready) {
                        object value = Target.GetValue(Target.Settings[i].Name);
                        GUILayout.Label($"Current Value = ({value.GetType().Name}) {value}");
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
                case Type _ when p.FieldType == typeof(Int64): {
                        long original = (Int64)parametrizedSetting.GetParamIntegral(p.Name);
                        long value = EditorGUILayout.LongField(displayName, original);
                        if (original == value) break;

                        parametrizedSetting.SetParam<Int64>(p.Name, value);
                        return true;
                    }
                case Type _ when p.FieldType == typeof(string[]): {
                        string[] original = parametrizedSetting.GetParam<string[]>(p.Name);
                        var value = new List<string>(original);

                        for (int i = 0; i < original.Length; i++) {
                            value[i] = EditorGUILayout.TextField($"{displayName}[{i}]", value[i]);
                        }
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("+")) value.Add("");
                        if (GUILayout.Button("-")) value.RemoveAt(value.Count - 1);
                        GUILayout.EndHorizontal();

                        if (Enumerable.SequenceEqual(original, value)) break;

                        parametrizedSetting.SetParam<string[]>(p.Name, value.ToArray());
                        return true;
                    }
                case Type _ when p.FieldType == typeof(string): {
                        string original = parametrizedSetting.GetParam<string>(p.Name);
                        string value = EditorGUILayout.TextField(displayName, original);
                        if (original == value) break;

                        parametrizedSetting.SetParam<string>(p.Name, value);
                        return true;
                    }
                case Type _ when p.FieldType == typeof(bool): {
                        bool original = parametrizedSetting.GetParam<bool>(p.Name);
                        bool value = EditorGUILayout.Toggle(displayName, original);
                        if (original == value) break;

                        parametrizedSetting.SetParam<bool>(p.Name, value);
                        return true;
                    }
            }

            return false;
        }
    }

#endif

}
