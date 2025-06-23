using UnityEngine;

[CreateAssetMenu(menuName = "Game/Items/EvolvingDanger", fileName = "EvolvingDanger")]
[ItemState(typeof(EvolvingDanger.State))]
public class EvolvingDanger : Item {
	public class State {
		public int level;
	}

	public override void RegisterEvents(KrampusEvents events) {
		events.onWindUpChanged.AddListener(BuffKrampus);
	}

	public override void UnregisterEvents(KrampusEvents events) {
		events.onWindUpChanged.RemoveListener(BuffKrampus);
	}

	private void BuffKrampus(Krampus krampus, float windup) {
		Debug.Log($"EvolvingDanger: windup = {windup}");
		if (windup < 20) {
			return;
		}

		var state = krampus.Stats.GetItemState<State>(this);
		state.level++;
		switch (state.level) {
			case 1:
				RegisterEffect(krampus, 0);
				break;
			case 2:
				RegisterEffect(krampus, 1);
				break;
			case 3:
				RegisterEffect(krampus, 2);
				break;
			default:
				return;
		}
		krampus.Kontroller.SpendWindUpPoints(10);
		
		
	}

	public override int GetStackAmount(Krampus krampus) { return krampus.Stats.GetItemState<State>(this).level; }
}
