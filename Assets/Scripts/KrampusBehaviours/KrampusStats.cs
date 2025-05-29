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
        Speed,
        Tongue_Range,
        ComboGain
    }

    public enum StatMode {
        MultiplyPercent,
        AddPercent,
        AddRaw
    }
    [Serializable]
    public class RawStat {
        [SerializeField] private Stat m_stat;
        public Stat Stat => m_stat;
        [SerializeField] private float m_value;
        public float Value => m_value;

        [SerializeField] private StatMode m_statMode;
        public StatMode StatMode => m_statMode;

    }

    [SerializeField] private List<RawStat> m_rawStatList;

    public IReadOnlyCollection<RawStat> RawStatList => m_rawStatList;

    private Dictionary<Stat, RawStat> m_rawStatDict;

    public IReadOnlyDictionary<Stat,RawStat> RawStats => m_rawStatDict;
    private Dictionary<Stat, List<StatModifier>> m_statsModifiers = new Dictionary<Stat, List<StatModifier>>();

    private Dictionary<Stat, float> m_calculatedStatModifiers = new Dictionary<Stat, float>();

    public void Update() {
        Debug.Log($"[Speed] StatsTest: {GetFinalStat(Stat.Speed)}");
        Debug.Log($"[NewExample] StatsTest: {GetFinalStat(Stat.Tongue_Range)}");
	}
	private void OnEnable() {
        PopulateDictionary();
    }

    private void PopulateDictionary()
    {
        m_rawStatDict = new Dictionary<Stat, RawStat>();
        if (m_rawStatList == null) return;
        if (m_rawStatList.Count < Enum.GetValues(typeof(Stat)).Length) {
            Debug.LogError($"Not enough stats. Some are missing!", this);
        }
        foreach (var rawStat in m_rawStatList)
        {
            if (!m_rawStatDict.ContainsKey(rawStat.Stat))
            {
                m_rawStatDict.Add(rawStat.Stat, rawStat);
            }
            else
            {
                Debug.LogError($"Duplicate Stat '{rawStat.Stat}' found in list. Using the first entry.", this);
            }
        }
    }

	private void Start() {
        foreach (RawStat rs in m_rawStatList) {
            m_statsModifiers.Add(rs.Stat, new List<StatModifier>());
            m_calculatedStatModifiers.Add(rs.Stat, 1f);
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

    private void OnValidate() {
        RecalculateStats();
	}

	private void RecalculateStats() {
        foreach (RawStat rs in m_rawStatList) {
            float totalModifier = 0f;
            switch (rs.StatMode) {
                case StatMode.MultiplyPercent:
                    totalModifier = m_statsModifiers[rs.Stat].Aggregate(1.0f, (accumulator, sm) => accumulator * sm.Modifier);
                    break;
                case StatMode.AddPercent:
                    totalModifier = 1.0f + m_statsModifiers[rs.Stat].Sum(sm => sm.Modifier);
                    break;
                case StatMode.AddRaw:
                    totalModifier = m_statsModifiers[rs.Stat].Sum(sm => sm.Modifier);
                    break;
            }
            m_calculatedStatModifiers[rs.Stat] = totalModifier;
        }
    }

    public float GetFinalStat(Stat stat) {
        RawStat rawStat = m_rawStatDict[stat];
        float finalStat = rawStat.Value;
        switch (rawStat.StatMode) {
            case StatMode.MultiplyPercent:
                finalStat *= m_calculatedStatModifiers[stat];
                break;
            case StatMode.AddPercent:
                finalStat *= m_calculatedStatModifiers[stat];
                break;
            case StatMode.AddRaw:
                finalStat += m_calculatedStatModifiers[stat];
                break;
        }
        return finalStat;
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
