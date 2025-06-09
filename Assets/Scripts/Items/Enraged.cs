using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Items/Enraged", fileName = "Enraged")]
public class Enraged : Item {
    [SerializeField] private float m_comboTreshold;

	private bool m_isActive = false;

    public override void RegisterEvents(KrampusEvents events) {
        events.onComboChanged.AddListener(MovementBuff);
        m_isActive = false;
    }

    public override void UnregisterEvents(KrampusEvents events) {
        events.onComboChanged.RemoveListener(MovementBuff);
    }

    private void MovementBuff(Krampus krampus, float combo) {

        if (combo >= m_comboTreshold && !m_isActive) {
			//Debug.Log("COMBOACTIVE");
			RegisterAllEffects(krampus);
			m_isActive = true;
			//Game.MainGameInfo.UI.DisplayEffect(ItemIcon, ItemName, m_effectId);
        } else if(m_isActive) {
			//Debug.Log("COMBODISACTIVE");
            UnregisterAllEffects(krampus);
            //Game.MainGameInfo.UI.RemovEffectIcon(m_effectId);
		}
    }
}
