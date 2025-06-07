using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Rendering.Universal.Internal;


[CreateAssetMenu(menuName = "Game/Items/Stunlock", fileName = "Stunlock")]
public class Stunlock : Item {
    [SerializeField] private float m_speedValue;
    [SerializeField] private float m_duration;

    const string m_effectId = "ST_S";

    public override void RegisterEvents(KrampusEvents events) {
        events.onNunStunned.AddListener(MovementBuff);
    }

    public override void UnregisterEvents(KrampusEvents events) {
        events.onNunStunned.RemoveListener(MovementBuff);
    }

    private void MovementBuff(Krampus krampus, Nun nun) {

        krampus.Stats.RegisterEffect(new Effect(KrampusStats.Stat.Speed, m_speedValue, m_duration,m_effectId));
        Game.MainGameInfo.UI.DisplayEffect(m_duration, ItemIcon, ItemName,m_effectId);
    }
}
