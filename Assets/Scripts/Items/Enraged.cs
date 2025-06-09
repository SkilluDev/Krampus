using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Items/Enraged", fileName = "Enraged")]
public class Enraged : Item {
    [SerializeField] private float m_comboTreshold;

	private bool m_isActive = false;

    public override void RegisterEvents(KrampusEvents events) {
        events.onComboChanged.AddListener(BuffKrampus);
        m_isActive = false;
    }

    public override void UnregisterEvents(KrampusEvents events) {
        events.onComboChanged.RemoveListener(BuffKrampus);
    }

    private void BuffKrampus(Krampus krampus, float combo) {
        if (combo >= m_comboTreshold && !m_isActive) {
			RegisterAllEffects(krampus);
			m_isActive = true;
        } else if(m_isActive) {
            UnregisterAllEffects(krampus);
		}
    }
}
