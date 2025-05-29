using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using static KrampusStats;

public class KrampusStats : KrampusBehaviour {
    
    public enum Stat {
        Example,
        NewExample
    }
    [Serializable]
    public class RawStat {
        [SerializeField] private Stat m_stat;
        public Stat Stat => m_stat;
        [SerializeField] private float m_value;
        public float Value => m_value;

    }

    [SerializeField] private List<RawStat> m_rawStatList;

    public IReadOnlyCollection<RawStat> RawStatList => m_rawStatList;

    private Dictionary<Stat, float> m_rawStatDict;

    public IReadOnlyDictionary<Stat,float> RawStats => m_rawStatDict;
    private Dictionary<Stat, List<StatModifier>> m_statsModifiers = new Dictionary<Stat, List<StatModifier>>();

    private Dictionary<Stat, float> m_calculatedStatModifiers = new Dictionary<Stat, float>();

    public void Update() {
        Debug.Log($"[Example] StatsTest: {GetFinalStat(Stat.Example)}");
        Debug.Log($"[NewExample] StatsTest: {GetFinalStat(Stat.NewExample)}");
	}
	private void OnEnable() {
        PopulateDictionary();
    }

    private void PopulateDictionary()
    {
        m_rawStatDict = new Dictionary<Stat, float>();
        if (m_rawStatList == null) return;
        if (m_rawStatList.Count < Enum.GetValues(typeof(Stat)).Length) {
            Debug.LogError($"Not enough stats. Some are missing!", this);
        }
        foreach (var rawStat in m_rawStatList)
        {
            if (!m_rawStatDict.ContainsKey(rawStat.Stat))
            {
                m_rawStatDict.Add(rawStat.Stat, rawStat.Value);
            }
            else
            {
                Debug.LogError($"Duplicate Stat '{rawStat.Stat}' found in list. Using the first entry.", this);
            }
        }
    }

	private void Start() {
        foreach (Stat stat in Enum.GetValues(typeof(Stat))) {
            m_statsModifiers.Add(stat, new List<StatModifier>());
            m_calculatedStatModifiers.Add(stat, 1f);
        }
    }
    public void RegisterStatModifier(StatModifier statModifier) {
        m_statsModifiers[statModifier.Stat].Add(statModifier);
        RecalculateStats();
    }

    public void UnRegisterStatModifier(StatModifier statModifier) {
        m_statsModifiers[statModifier.Stat].Remove(statModifier);
        RecalculateStats();
    }

    private void RecalculateStats() {
        foreach (Stat stat in Enum.GetValues(typeof(Stat))) {
            float totalModifier = m_statsModifiers[stat].Aggregate(1.0f, (accumulator, sm) => accumulator * sm.Modifier);
            m_calculatedStatModifiers[stat] = totalModifier;
        }
    }

    public float GetFinalStat(Stat stat) {
        return m_rawStatDict[stat] * m_calculatedStatModifiers[stat];
    }
}


[CustomEditor(typeof(KrampusStats))]
public class StatHolderEditor : Editor
{
    private SerializedProperty m_rawStatListProperty;

    private void OnEnable()
    {
        m_rawStatListProperty = serializedObject.FindProperty("m_rawStatList");
    }

    public override void OnInspectorGUI()
    {
        serializedObject.Update();
        DrawDefaultInspector();

        KrampusStats krampusStats = (KrampusStats)target;
        IReadOnlyCollection<RawStat> rawStatList = krampusStats.RawStatList;

        if (rawStatList != null)
        {
            var duplicateStats = rawStatList
                .GroupBy(rs => rs.Stat)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateStats.Any())
            {
                string warningMessage = "Warning: Duplicate Stat entries found in the list!\n";
                foreach (var stat in duplicateStats)
                {
                    warningMessage += $"- {stat}\n";
                }
                EditorGUILayout.HelpBox(warningMessage, MessageType.Warning);
            }

            int totalStatEnums = System.Enum.GetValues(typeof(Stat)).Length;

            if (rawStatList.Count < totalStatEnums)
            {
                var missingStats = new List<Stat>();
                foreach (Stat stat in System.Enum.GetValues(typeof(Stat))) {
                    if (!rawStatList.Select(rs=>rs.Stat).Contains(stat)) {
                        missingStats.Add(stat);
                    }
                }
                string warningMessage = $"Warning: The list count ({rawStatList.Count}) does not match the total number of unique Stat enum values ({totalStatEnums}). Missing stats:\n";
                foreach (var stat in missingStats)
                {
                    warningMessage += $"- {stat}\n";
                }
                EditorGUILayout.HelpBox(warningMessage, MessageType.Warning);
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
