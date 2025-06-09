using UnityEngine;

[CreateAssetMenu(menuName = "Game/Items/FastKill", fileName = "FastKill")]
public class FastKill : Item {

	public override void RegisterEvents(KrampusEvents events) {
		events.onNaughtyChildEaten.AddListener(BuffKrampus);
		events.onNiceChildEaten.AddListener(BuffKrampus);
	}

	public override void UnregisterEvents(KrampusEvents events) {
		events.onNaughtyChildEaten.RemoveListener(BuffKrampus);
		events.onNiceChildEaten.RemoveListener(BuffKrampus);
	}

	private void BuffKrampus(Krampus krampus, Child child) {
		//Game.MainGameInfo.UI.DisplayEffect(m_duration, ItemIcon, ItemName,m_effectId);
		RegisterAllEffects(krampus);
	}
}
