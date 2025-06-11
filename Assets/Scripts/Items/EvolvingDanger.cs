using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Items/EvolvingDanger", fileName = "EvolvingDanger")]
public class EvolvingDanger : Item {

    private int m_level;
	public override void RegisterEvents(KrampusEvents events) {
        events.onWindUpChanged.AddListener(BuffKrampus);
        m_level = 0;

    }
    public override void UnregisterEvents(KrampusEvents events) {
        events.onWindUpChanged.RemoveListener(BuffKrampus);
        m_level = 0;

    }


    private void BuffKrampus(Krampus krampus, float windup) {
        if (windup >= 100) {

            m_level++;
            switch (m_level) {
                case 1:
                    RegisterEffect(krampus, 0);
                    break;

                case 2:
                    RegisterEffect(krampus, 1);
                    break;
                case 3:
                    RegisterEffect(krampus, 2);
                    break;
                default:
                    return;
            }
            krampus.Kontroller.SpendWindUpPoints(100);

        }
         

    }

	private void DebuffKrampus(Krampus krampus) {
		UnregisterAllEffects(krampus);
	}
}
