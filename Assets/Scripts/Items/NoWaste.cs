using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Items/NoWaste", fileName = "NoWaste")]
public class NoWaste : Item {


    [SerializeField] private float m_comboGained;
   public override void RegisterEvents(KrampusEvents events) {
        events.onNiceChildEaten.AddListener(GainCombo);
    }

    public override void UnregisterEvents(KrampusEvents events) {
         events.onNiceChildEaten.RemoveListener(GainCombo);
    }

    private void GainCombo(Krampus krampus, Child child) {
        krampus.Kontroller.AddComboPoints(m_comboGained);
    }
}
