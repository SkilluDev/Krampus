using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Items/NoWaste", fileName = "NoWaste")]
public class NoWaste : Item {


    [SerializeField] private float m_windUpGained;
   public override void RegisterEvents(KrampusEvents events) {
        events.onNiceChildEaten.AddListener(GainWindUp);
    }

    public override void UnregisterEvents(KrampusEvents events) {
         events.onNiceChildEaten.RemoveListener(GainWindUp);
    }

    private void GainWindUp(Krampus krampus, Child child) {
          RegisterAllEffects(krampus);
        krampus.Kontroller.AddWindUpPoints(m_windUpGained);
    }
}
