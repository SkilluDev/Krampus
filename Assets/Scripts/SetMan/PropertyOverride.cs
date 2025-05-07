
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using UnityEngine;

namespace Settings {
    [Serializable]
    public class ParametrizedSetting : ISerializationCallbackReceiver {
        public Setting setting = null;
        public Dictionary<string, object> parameters = new Dictionary<string, object>();
        [SerializeField] private string m_parameterList;

        public void OnAfterDeserialize() {
            parameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(m_parameterList, new JsonSerializerSettings() {
                TypeNameHandling = TypeNameHandling.All
            });
        }
        public void OnBeforeSerialize() {
            m_parameterList = JsonConvert.SerializeObject(parameters, new JsonSerializerSettings() {
                TypeNameHandling = TypeNameHandling.All
            });
        }

        public IEnumerable<FieldInfo> GetAvailableProperties() {
            return setting.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Where(w => w.GetCustomAttribute<SettingPropertyAttribute>() != null);
        }

        public T GetParam<T>(string name) {
            var field = setting.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public);
            if (field.GetCustomAttribute<SettingPropertyAttribute>() == null) throw new Exception();
            return parameters.ContainsKey(name) ? (T)parameters[name] : (T)field.GetValue(setting);
        }

        public Int64 GetParamIntegral(string name) {
            var field = setting.GetType().GetField(name, BindingFlags.Instance | BindingFlags.Public);
            if (field.GetCustomAttribute<SettingPropertyAttribute>() == null) throw new Exception();
            return (parameters.ContainsKey(name) ? parameters[name] : field.GetValue(setting)) switch {
                Int64 i64 => i64,
                Int32 i32 => i32,
                Int16 i16 => i16,
                Byte i8 => i8,
                _ => throw new Exception()
            };
        }

        public void SetParam<T>(string name, T value) {
            parameters[name] = value;
        }

        public string Name {
            get => parameters.ContainsKey("name") ? (string)parameters["name"] : setting.name;
            set => parameters["name"] = value;
        }
    }
}
