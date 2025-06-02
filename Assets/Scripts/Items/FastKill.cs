using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Items/FastKill", fileName = "FastKill")]
public class FastKill : Item {


    [SerializeField] private float m_speedValue;
    [SerializeField] private float m_duration;


    public override void RegisterItem(Krampus krampus) {
        base.RegisterItem(krampus);
        krampus.KrampusEvents.onNaughtyChildEaten.AddListener(MovementBuff);
    }


    private void MovementBuff(Child child) {

        Debug.Log("kILL KILL KILL");
        m_krampus.Stats.RegisterEffect(new Effect(KrampusStats.Stat.Speed, m_speedValue, m_duration));

        Game.MainGameInfo.UI.DisplayEffect(m_duration, ItemIcon);
     }
}
