using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Items/StingerSpeed", fileName = "StingerSpeed")]
public class StingerSpeed : Item {

    [SerializeField] private float m_speedValue;
    [SerializeField] private float m_duration;

    const string m_effectId = "SS_S";

    public override void RegisterEvents(KrampusEvents events) {
        events.onStingerUsed.AddListener(MovementBuff);

    }

    public override void UnregisterEvents(KrampusEvents events) {
        events.onStingerUsed.RemoveListener(MovementBuff);

    }

    private void MovementBuff(Krampus krampus) {

        krampus.Stats.RegisterEffect(new Effect(KrampusStats.Stat.Speed, m_speedValue, m_duration,m_effectId));
        Game.MainGameInfo.UI.DisplayEffect(m_duration, ItemIcon, ItemName,m_effectId);
    }

}
