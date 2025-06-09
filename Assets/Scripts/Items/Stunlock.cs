using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;


[CreateAssetMenu(menuName = "Game/Items/Stunlock", fileName = "Stunlock")]
public class Stunlock : Item {
    public override void RegisterEvents(KrampusEvents events) {
        events.onNunStunned.AddListener(BuffKrampus);
    }

    public override void UnregisterEvents(KrampusEvents events) {
        events.onNunStunned.RemoveListener(BuffKrampus);
    }

    private void BuffKrampus(Krampus krampus, Nun nun) {
		RegisterAllEffects(krampus);
    }
}
