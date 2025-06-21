using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Items/Enraged", fileName = "Enraged")]
public class Enraged : Item {
    [SerializeField] private float m_windUpTreshold;

	private bool m_isActive = false;

    public override void RegisterEvents(KrampusEvents events) {
        events.onWindUpChanged.AddListener(BuffKrampus);
        m_isActive = false;
    }

    public override void UnregisterEvents(KrampusEvents events) {
        events.onWindUpChanged.RemoveListener(BuffKrampus);
    }

	private void BuffKrampus(Krampus krampus, float windUp) {
		bool previouslyActive = m_isActive;
		if (windUp >= m_windUpTreshold) {
			m_isActive = true;
		} else {
			m_isActive = false;
		}

		if (!previouslyActive && m_isActive) {
			RegisterAllEffects(krampus);
		} else if(previouslyActive && !m_isActive){
			UnregisterAllEffects(krampus);
		}
    }
}
