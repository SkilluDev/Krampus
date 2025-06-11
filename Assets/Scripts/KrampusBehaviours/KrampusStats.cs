using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using UnityEditor;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.VFX;
using static KrampusStats;

public class KrampusStats : KrampusBehaviour {

    public enum Stat {
        Speed,
        TongueRange,
        WindUpGain
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

    public IReadOnlyDictionary<Stat, RawStat> RawStats => m_rawStatDict;
    private Dictionary<Stat, List<Effect>> m_effects = new Dictionary<Stat, List<Effect>>();

    private Dictionary<Stat, float> m_calculatedMultipliers = new Dictionary<Stat, float>();

    public bool hasMov;


    [SerializeField] private List<Item> m_items = new List<Item>();
    public IReadOnlyList<Item> Items => m_items;
    private List<Effect> m_effectsToClear = new List<Effect>();

    public void Update() {
        Debug.Log($"[Speed] StatsTest: {GetFinalStat(Stat.Speed)}");
        Debug.Log($"[TongueRange] StatsTest: {GetFinalStat(Stat.TongueRange)}");

        foreach (var stat in m_effects) {
            foreach (var effect in stat.Value) {
                if (effect.EffectType == Effect.Type.Temporary) {
                    effect.UpdateTimer(Time.deltaTime);
                }
                if (effect.IsExpired) {
                    m_effectsToClear.Add(effect);
                }
            }
        }
        Debug.Log("Ma speed buff:" + hasMov);

        ClearEffectsToClear();
    }

    private void ClearEffectsToClear() {
        foreach (var effect in m_effectsToClear) {
            UnregisterEffect(effect);
        }
        m_effectsToClear.Clear();
    }

    private void ClearAllTemporaryEffects() {
        foreach (var stat in m_effects) {
            foreach (var effect in stat.Value) {
                if (effect.EffectType != Effect.Type.Temporary) continue;
                effect.IsExpired = true;
                m_effectsToClear.Add(effect);
            }
        }

        ClearEffectsToClear();
    }



    private void OnEnable() {
        PopulateDictionary();
    }

    private void PopulateDictionary() {
        m_rawStatDict = new Dictionary<Stat, RawStat>();
        if (m_rawStatList == null) return;
        if (m_rawStatList.Count < Enum.GetValues(typeof(Stat)).Length) {
            Debug.LogError($"Not enough stats. Some are missing!", this);
        }
        foreach (var rawStat in m_rawStatList) {
            if (!m_rawStatDict.ContainsKey(rawStat.Stat)) {
                m_rawStatDict.Add(rawStat.Stat, rawStat);
            } else {
                Debug.LogError($"Duplicate Stat '{rawStat.Stat}' found in list. Using the first entry.", this);
            }
        }
    }

    private void Start() {
        LoadItems();
        foreach (var rs in m_rawStatList) {
            m_effects.Add(rs.Stat, new List<Effect>());
            m_calculatedMultipliers.Add(rs.Stat, 1f);
        }

    }

    private void OnDestroy() { // to be absolutely fair, i have no clue whether this will get correctly called; TODO:
        if (Game.IsLoading) {
            StoreItems();
        }
    }

    private void LoadItems() {
        var itemsToAdd = new List<Item>();
        Game.PogMan.Unpack(ref itemsToAdd);
        foreach (var item in itemsToAdd) {
            AddItem(item);
        }
    }
    private void StoreItems() {
        Game.PogMan.Store(ref m_items);
    }

    public void RegisterEffect(Effect effect) {
        Kramp.KrampusEvents.onEffectRegistered.Invoke(Kramp, effect);
        m_effects[effect.StatModifier.Stat].Add(effect);
        RecalculateStats();
    }


    public void UnregisterEffect(Effect effect) {
        Kramp.KrampusEvents.onEffectUnregistered.Invoke(Kramp, effect);
        m_effects[effect.StatModifier.Stat].Remove(effect);
        RecalculateStats();
    }

    private void OnValidate() {
        RecalculateStats();
    }

    private void RecalculateStats() {
        if (!Game.Balling) return;
        foreach (var rs in m_rawStatList) {
            float totalMultiplier = 0f;
            switch (rs.StatMode) {
                case StatMode.MultiplyPercent:
                    totalMultiplier = m_effects[rs.Stat].Aggregate(1.0f, (accumulator, e) => accumulator * e.StatModifier.Modifier);
                    break;
                case StatMode.AddPercent:
                    totalMultiplier = 1.0f + m_effects[rs.Stat].Sum(e => e.StatModifier.Modifier);
                    break;
                case StatMode.AddRaw:
                    totalMultiplier = m_effects[rs.Stat].Sum(e => e.StatModifier.Modifier);
                    break;
            }
            m_calculatedMultipliers[rs.Stat] = totalMultiplier;
        }


        if (m_calculatedMultipliers[Stat.Speed] > 1) {
            hasMov = true;
        } else {
            hasMov = false;
            Kramp.Animator.StopSmoke();
        }
    }

    public void AddItem(Item item) {
        item.ItemAdded(Kramp);
        if (!m_items.Contains(item)) {
            item.RegisterEvents(Kramp.KrampusEvents);
        }
        m_items.Add(item);
    }

    public void RemoveItem(Item item) {
        if (!m_items.Contains(item)) {
            Debug.LogError("Attempted to remove an item {item} which the player did not have.");
            return;
        }

        m_items.Remove(item);
        item.ItemRemoved(Kramp);
        if (!m_items.Contains(item)) item.UnregisterEvents(Kramp.KrampusEvents);
    }


    public bool HasItem(Item item) {
        return m_items.Contains(item);
    }

    public float GetFinalStat(Stat stat) {
        var rawStat = m_rawStatDict[stat];
        float finalStat = rawStat.Value;
        switch (rawStat.StatMode) {
            case StatMode.MultiplyPercent:
                finalStat *= m_calculatedMultipliers[stat];
                break;
            case StatMode.AddPercent:
                finalStat *= m_calculatedMultipliers[stat];
                break;
            case StatMode.AddRaw:
                finalStat += m_calculatedMultipliers[stat];
                break;
        }
        return finalStat;
    }

    public bool hasItemWithTag(ItemTag tag) {
        foreach (var i in m_items) {
            if (i.hasTag(tag)) { return true; }
        }
        return false;
     }
}





[CustomEditor(typeof(KrampusStats))]
public class StatHolderEditor : Editor {
    private SerializedProperty m_rawStatListProperty;

    private void OnEnable() {
        m_rawStatListProperty = serializedObject.FindProperty("m_rawStatList");
    }

    public override void OnInspectorGUI() {
        serializedObject.Update();
        DrawDefaultInspector();

        KrampusStats krampusStats = (KrampusStats)target;
        IReadOnlyCollection<RawStat> rawStatList = krampusStats.RawStatList;

        if (rawStatList != null) {
            var duplicateStats = rawStatList
                .GroupBy(rs => rs.Stat)
                .Where(g => g.Count() > 1)
                .Select(g => g.Key)
                .ToList();

            if (duplicateStats.Any()) {
                string warningMessage = "Warning: Duplicate Stat entries found in the list!\n";
                foreach (var stat in duplicateStats) {
                    warningMessage += $"- {stat}\n";
                }
                EditorGUILayout.HelpBox(warningMessage, MessageType.Warning);
            }

            int totalStatEnums = System.Enum.GetValues(typeof(Stat)).Length;

            if (rawStatList.Count < totalStatEnums) {
                var missingStats = new List<Stat>();
                foreach (Stat stat in System.Enum.GetValues(typeof(Stat))) {
                    if (!rawStatList.Select(rs => rs.Stat).Contains(stat)) {
                        missingStats.Add(stat);
                    }
                }
                string warningMessage = $"Warning: The list count ({rawStatList.Count}) does not match the total number of unique Stat enum values ({totalStatEnums}). Missing stats:\n";
                foreach (var stat in missingStats) {
                    warningMessage += $"- {stat}\n";
                }
                EditorGUILayout.HelpBox(warningMessage, MessageType.Warning);
            }
        }
        serializedObject.ApplyModifiedProperties();
    }
}
