using System;
using System.Collections.Generic;
using UnityEngine;

[Flags]
[Serializable]
public enum ItemTag {
	LockIn = 1,
	NextTag = 2,
}
[Flags]
[Serializable]
public enum ItemEffectiveType {
	Temporary, //last for set duration
	Switch, //can be switch on and off
	Stackable, // can be stacked

	Paff,
	
}


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
	[SerializeField] private ItemTag m_itemTags;

	[SerializeField] private ItemEffectiveType m_itemEffectiveType;
	public ItemEffectiveType EffectiveType => m_itemEffectiveType;

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
			m_effects.Add(e.ToEffect(ItemName, ItemIcon));
		}
		Game.MainGameInfo.UI.EffectBar.RegisterIcon(this);
	}
	/// <summary>
	/// Called when this item gets removed from a Krampus
	/// </summary>
	public  void ItemRemoved(Krampus krampus) {
		UnregisterEvents(krampus.KrampusEvents);
		m_effects.Clear();
	}




	public virtual void RegisterAllEffects(Krampus krampus) {
		//ResetItem();
		foreach (var effect in m_effects) {
			krampus.Stats.RegisterEffect(effect);
			
		}
		krampus.KrampusEvents.onItemActivated.Invoke(krampus, this);

	}
	public virtual void RegisterEffect(Krampus krampus, int i) {
		krampus.Stats.RegisterEffect(m_effects[i]);
		
		krampus.KrampusEvents.onItemActivated.Invoke(krampus, this);
	}

	public virtual void UnregisterAllEffects(Krampus krampus) {
		foreach (var effect in m_effects) {
			krampus.Stats.UnregisterEffect(effect);
		}
		krampus.KrampusEvents.onItemDesactivated.Invoke(krampus, this);

		
	}

	public void ResetItem() {
		ItemRemoved(Game.MainGameInfo.Krampus);
		ItemAdded(Game.MainGameInfo.Krampus);
	}

	public bool HasTag(ItemTag tag) {
		return m_itemTags.HasFlag(tag);
	}

	public virtual float GetDuration() {
		return m_effects.Count > 0 ? m_effects[0].Duration:0;
	}

	public virtual float GetStackAmount() {
		return 0;
	 }
}
