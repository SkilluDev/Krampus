using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Items/StingerSpeed", fileName = "StingerSpeed")]
public class StingerSpeed : Item {
    public override void RegisterEvents(KrampusEvents events) {
        events.onStingerUsed.AddListener(BuffKrampus);
    }

    public override void UnregisterEvents(KrampusEvents events) {
        events.onStingerUsed.RemoveListener(BuffKrampus);
    }

    private void BuffKrampus(Krampus krampus) {
		RegisterAllEffects(krampus);
        //Game.MainGameInfo.UI.DisplayEffect(m_duration, ItemIcon, ItemName,m_effectId);
    }

}
