using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Items/FastKill", fileName = "FastKill")]
public class FastKill : Item {




    public override void RegisterItem(KrampEvents events) {

        events.onChildEaten.AddListener(movementBuff);
    }


    void movementBuff(Krampus krampus) {


        krampus.Stats.RegisterStatModifier(new StatModifier(KrampusStats.Stat.Speed, 0.5f, 3));
     }
}
