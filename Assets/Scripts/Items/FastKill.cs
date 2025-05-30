using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Items/FastKill", fileName = "FastKill")]
public class FastKill : Item {


    [SerializeField] private float m_speedValue;
    [SerializeField] private float m_duretion;


    public override void RegisterItem(Krampus krampus) {
        base.RegisterItem(krampus);
        krampus.KrampEvents.onChildEaten.AddListener(MovementBuff);
    }


    void MovementBuff(Child child) {


        m_krampus.Stats.RegisterStatModifier(new StatModifier(KrampusStats.Stat.Speed, m_speedValue, m_duretion));
     }
}
