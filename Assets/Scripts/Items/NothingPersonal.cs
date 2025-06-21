using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Items/NothingPerosnal ", fileName = "NothingPerosnal ")]
public class NothingPerosnal : Item {

    [SerializeField] private float m_windupGained;

    public override void RegisterEvents(KrampusEvents events) {
        events.onChildEaten.AddListener(BuffKrampus);
    }

    public override void UnregisterEvents(KrampusEvents events) {
        events.onChildEaten.RemoveListener(BuffKrampus);
    }

    private void BuffKrampus(Krampus krampus, Child child) {
        if (child.StateBeforeDeath == Child.State.Stunned) {
            krampus.Kontroller.AddWindUpPoints(m_windupGained);
        }
    }

}
