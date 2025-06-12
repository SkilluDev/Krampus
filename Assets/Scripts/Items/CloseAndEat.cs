using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Items/CloseAndEat", fileName = "CloseAndEat")]
public class CloseAndEat : Item {

    private bool m_isActive;
    [SerializeField] private float m_windupGained;

    public override void RegisterEvents(KrampusEvents events) {
        events.onNunStunned.AddListener(BuffKrampus);
        events.onChildEaten.AddListener(RemoveBuff);
        m_isActive = false;
    }

    public override void UnregisterEvents(KrampusEvents events) {
        events.onNunStunned.RemoveListener(BuffKrampus);
        events.onChildEaten.RemoveListener(RemoveBuff);
    }

    private void BuffKrampus(Krampus krampus, Nun nun) {
        if (m_isActive) return;


        RegisterAllEffects(krampus);
        m_isActive = true;
    }
    private void RemoveBuff(Krampus krampus, Child child) {
        if (!m_isActive) return;

        UnregisterAllEffects(krampus);
        krampus.Kontroller.AddWindUpPoints(m_windupGained); 

        m_isActive = false;
     }
}
