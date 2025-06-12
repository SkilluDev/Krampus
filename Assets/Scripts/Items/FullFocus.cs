using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;


[CreateAssetMenu(menuName = "Game/Items/FullFocus", fileName = "FullFocus")]
public class FullFocus : Item {
	public override void RegisterEvents(KrampusEvents events) {
		events.onLockIn.AddListener(BuffKrampus);
		events.onLockOut.AddListener(DebuffKrampus);

	}

	public override void UnregisterEvents(KrampusEvents events) {
		events.onLockIn.RemoveListener(BuffKrampus);
		events.onLockOut.RemoveListener(DebuffKrampus);
	}

	private void BuffKrampus(Krampus krampus) {
		RegisterAllEffects(krampus);
	}

	private void DebuffKrampus(Krampus krampus) {
		UnregisterAllEffects(krampus);
	}
}
