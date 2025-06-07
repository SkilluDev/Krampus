using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Items/NunFoundSpeed", fileName = "NunFoundSpeed")]
public class NunFoundSpeed : Item {

    [SerializeField] private float m_speedValue;
    [SerializeField] private float m_duration;

    const string m_effectId = "NFS_S";

    public override void RegisterEvents(KrampusEvents events) {
        events.onKrampusFoundByNun.AddListener(MovementBuff);
    }

    public override void UnregisterEvents(KrampusEvents events) {
        events.onKrampusFoundByNun.RemoveListener(MovementBuff);

    }

    private void MovementBuff(Krampus krampus, Nun nun) {
        krampus.Stats.RegisterEffect(new Effect(KrampusStats.Stat.Speed, m_speedValue, m_duration,m_effectId));
        Game.MainGameInfo.UI.DisplayEffect(m_duration, ItemIcon, ItemName,m_effectId);
    }

    
}
