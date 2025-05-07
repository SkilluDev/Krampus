
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
            parameters = JsonConvert.DeserializeObject<Dictionary<string, object>>(m_parameterList);
        }
        public void OnBeforeSerialize() {
            m_parameterList = JsonConvert.SerializeObject(parameters);
        }

        public IEnumerable<FieldInfo> GetAvailableProperties() {
            return setting.GetType().GetFields(BindingFlags.Instance | BindingFlags.Public).Where(w => w.GetCustomAttribute<SettingPropertyAttribute>() != null);
        }

        public string Name {
            get => parameters.ContainsKey("name") ? (string)parameters["name"] : setting.name;
            set => parameters["name"] = value;
        }
    }
}
