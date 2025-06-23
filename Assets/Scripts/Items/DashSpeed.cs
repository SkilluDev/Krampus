using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Items/DashSpeed", fileName = "DashSpeed")]
public class DashSpeed : Item {
    public override void RegisterEvents(KrampusEvents events) {
        events.onDashUsed.AddListener(BuffKrampus);
    }

    public override void UnregisterEvents(KrampusEvents events) {
        events.onDashUsed.RemoveListener(BuffKrampus);
    }

    private void BuffKrampus(Krampus krampus) {
        RegisterAllEffects(krampus);
    }

	

}
