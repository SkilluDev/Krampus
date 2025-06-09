using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Items/Item", fileName = "Item")]
public class Item : ScriptableObject {
	[SerializeField] protected string m_itemName = "Item Name";
	public string ItemName => m_itemName;

	[SerializeField] private Sprite m_itemIcon;
	public Sprite ItemIcon => m_itemIcon;

	[TextArea(4, 25)]
	[SerializeField] private string m_description;
	public string Description => m_description;

	[SerializeField] private List<EffectInEditor> m_effectsInEditor = new List<EffectInEditor>();

	protected List<Effect> m_effects = new List<Effect>();



	/// <summary>
	/// Caled when this Item's events should be registered
	/// </summary>
	public virtual void RegisterEvents(KrampusEvents events) {

	}
	/// <summary>
	/// Called when this Item's events should be unregistered
	/// </summary>
	public virtual void UnregisterEvents(KrampusEvents events) {

	}
	/// <summary>
	/// Called while evaluating modifiers
	/// </summary>
	/// <returns>The list of modifiers which should be currently applied to Krampus</returns>
	public virtual IEnumerable<StatModifier> GetModifiers(Krampus krampus) {
		return Array.Empty<StatModifier>();
	}
	/// <summary>
	/// Called when this item gets added to a Krampus
	/// </summary>
	public virtual void ItemAdded(Krampus krampus) {
		foreach (var e in m_effectsInEditor) {
			m_effects.Add(e.ToEffect(ItemName));
		}

	}
	/// <summary>
	/// Called when this item gets removed from a Krampus
	/// </summary>
	public virtual void ItemRemoved(Krampus krampus) {
		foreach (var e in m_effectsInEditor) {
			m_effects.Remove(e.ToEffect(ItemName));
		}
	}

	public virtual void RegisterAllEffects(Krampus krampus) {
		foreach (var effect in m_effects) {
			krampus.Stats.RegisterEffect(effect);
		}
	}

	public virtual void UnregisterAllEffects(Krampus krampus) {
		foreach (var effect in m_effects) {
			krampus.Stats.UnregisterEffect(effect);
		}
	}
}
