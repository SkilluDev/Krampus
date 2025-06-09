using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;


[CreateAssetMenu(menuName = "Game/Items/Stunlock", fileName = "Stunlock")]
public class Stunlock : Item {
    public override void RegisterEvents(KrampusEvents events) {
        events.onNunStunned.AddListener(MovementBuff);
    }

    public override void UnregisterEvents(KrampusEvents events) {
        events.onNunStunned.RemoveListener(MovementBuff);
    }

    private void MovementBuff(Krampus krampus, Nun nun) {
		RegisterAllEffects(krampus);
        //Game.MainGameInfo.UI.DisplayEffect(m_duration, ItemIcon, ItemName,m_effectId);
    }
}
