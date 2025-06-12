using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Items/Jojo", fileName = "JOJO")]
public class QuickEscape : Item {
	public override void RegisterEvents(KrampusEvents events) {
		events.onLockOut.AddListener(BuffKrampus);

	}

	public override void UnregisterEvents(KrampusEvents events) {
		events.onLockOut.RemoveListener(BuffKrampus);
	}

	private void BuffKrampus(Krampus krampus) {
		RegisterAllEffects(krampus);
	}

}
