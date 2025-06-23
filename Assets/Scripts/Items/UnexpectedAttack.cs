using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Items/UnexpectedAttack ", fileName = "UnexpectedAttack ")]
public class UnexpectedAttack : Item {

    [SerializeField] private float m_windupGained;

    public override void RegisterEvents(KrampusEvents events) {
        events.onChildEaten.AddListener(BuffKrampus);
    }

    public override void UnregisterEvents(KrampusEvents events) {
        events.onChildEaten.RemoveListener(BuffKrampus);
    }

    private void BuffKrampus(Krampus krampus, Child child) {
        if (child.StateBeforeDeath == Child.State.Idle || child.StateBeforeDeath == Child.State.Alerted) {
            RegisterAllEffects(krampus);
            krampus.Kontroller.AddWindUpPoints(m_windupGained);
        }
    }

}
