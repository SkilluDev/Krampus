using System;
using NaughtyAttributes;
using UnityEngine;

[Serializable]
public class EffectInEditor {
	[SerializeField] private KrampusStats.Stat m_stat;
	[SerializeField] private float m_duration;
	[SerializeField] private float m_modifier;

	public Effect ToEffect(string itemName, Sprite itemIcon) {
		if (m_duration > 0) {
			return new Effect(m_stat, m_modifier, m_duration, itemName+"_"+m_stat, itemIcon);
		}

		return new Effect(m_stat, m_modifier, itemName + "_" + m_stat, itemIcon);


	}
}

public class Effect {
	public enum Type {
		Permanent,
		Temporary
	}

	private string m_id;
	public string Id => m_id;

	private Type m_type;
	public Type EffectType => m_type;
	private Sprite m_itemIcon;

	private float m_duration = 0;
	public float Duration => m_duration;

	public Sprite ItemIcon => m_itemIcon;



	private bool m_isExpired = false;
	public bool IsExpired { get => m_isExpired; set => m_isExpired = value; }
	protected float m_timer;
	public float Timer => m_timer;

	private StatModifier m_statModifier;

	public StatModifier StatModifier => m_statModifier;



	public Effect(KrampusStats.Stat stat, float modifier, string id, Sprite itemIcon) {
		m_statModifier = new StatModifier(stat, modifier);
		m_type = Type.Permanent;
		m_id = id;
		m_itemIcon = itemIcon;
	}

	public Effect(KrampusStats.Stat stat, float modifier, float duration, string id, Sprite itemIcon) {
		m_statModifier = m_statModifier = new StatModifier(stat, modifier);
		m_timer = duration;
		m_type = Type.Temporary;
		m_id = id;
		m_itemIcon = itemIcon;
		m_duration = duration;
	}

	public void UpdateTimer(float deltaTime) {
		if (m_timer > 0) {
			m_timer -= deltaTime;
		} else {
			m_isExpired = true;
		}
	}
	public void ResetTimer() {
		m_timer = m_duration;
		m_isExpired = false;
	 }

	}
