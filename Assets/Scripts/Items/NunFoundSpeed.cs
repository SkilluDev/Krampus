using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Items/NunFoundSpeed", fileName = "NunFoundSpeed")]
public class NunFoundSpeed : Item {
    public override void RegisterEvents(KrampusEvents events) {
        events.onKrampusFoundByNun.AddListener(MovementBuff);
    }

    public override void UnregisterEvents(KrampusEvents events) {
        events.onKrampusFoundByNun.RemoveListener(MovementBuff);

    }

    private void MovementBuff(Krampus krampus, Nun nun) {
		RegisterAllEffects(krampus);
        //Game.MainGameInfo.UI.DisplayEffect(m_duration, ItemIcon, ItemName,m_effectId);
    }


}
