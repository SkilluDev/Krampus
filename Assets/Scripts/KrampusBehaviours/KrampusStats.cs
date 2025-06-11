using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class KrampusStats : KrampusBehaviour {

	public enum Stat {
		Speed,
		TongueRange,
		WindUpGain,
    }

    public enum StatMode {
        MultiplyPercent,
        AddPercent,
        AddRaw
    }
    [Serializable]
    public class RawStat : ValueConnectedToEnum<Stat>{
        [SerializeField] private float m_value;
        public float Value => m_value;

        [SerializeField] private StatMode m_statMode;
        public StatMode StatMode => m_statMode;

    }

	[SerializeField] private SerializedEnumDictionary<Stat, RawStat> m_rawStatDict;

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
    private void Start() {
        LoadItems();
		foreach (var rs in m_rawStatDict.Values) {
            m_effects.Add(rs.Key, new List<Effect>());
            m_calculatedMultipliers.Add(rs.Key, 1f);
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
		m_rawStatDict.Validate();
        RecalculateStats();
    }

    private void RecalculateStats() {
        if (!Game.Balling) return;
        foreach (var rs in m_rawStatDict.Values) {
            float totalMultiplier = 0f;
            switch (rs.StatMode) {
                case StatMode.MultiplyPercent:
                    totalMultiplier = m_effects[rs.Key].Aggregate(1.0f, (accumulator, e) => accumulator * e.StatModifier.Modifier);
                    break;
                case StatMode.AddPercent:
                    totalMultiplier = 1.0f + m_effects[rs.Key].Sum(e => e.StatModifier.Modifier);
                    break;
                case StatMode.AddRaw:
                    totalMultiplier = m_effects[rs.Key].Sum(e => e.StatModifier.Modifier);
                    break;
            }
            m_calculatedMultipliers[rs.Key] = totalMultiplier;
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

    public bool HasItemWithTag(ItemTag tag) {
        foreach (var i in m_items) {
            if (i.hasTag(tag)) { return true; }
        }
        return false;
     }
}
