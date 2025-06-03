using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Items/NoWaste", fileName = "NoWaste")]
public class NoWaste : Item {
   

    [SerializeField] private float m_comboGaind;
   public override void RegisterEvents(KrampusEvents events) {
        events.onNiceChildEaten.AddListener(GainCombo);
    }

    public override void UnregisterEvents(KrampusEvents events) {
         events.onNiceChildEaten.RemoveListener(GainCombo);
    }

    private void GainCombo(Krampus krampus, Child child) {
        Debug.Log("kILL KILL KILL");
        krampus.Kontroller.AddComboPoints(m_comboGaind);
       
    }
}
