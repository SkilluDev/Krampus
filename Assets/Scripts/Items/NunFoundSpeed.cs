using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Items/NunFoundSpeed", fileName = "NunFoundSpeed")]
public class NunFoundSpeed : Item {
    public override void RegisterEvents(KrampusEvents events) {
        events.onKrampusFoundByNun.AddListener(BuffKrampus);
    }

    public override void UnregisterEvents(KrampusEvents events) {
        events.onKrampusFoundByNun.RemoveListener(BuffKrampus);

    }

    private void BuffKrampus(Krampus krampus, Nun nun) {
		RegisterAllEffects(krampus);
    }


}
