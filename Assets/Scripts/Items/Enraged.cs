using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Items/Enraged", fileName = "Enraged")]
public class Enraged : Item {
    
    [SerializeField] private float m_speedValue;
    [SerializeField] private float m_comboTreshold;

    bool isActive = false;

    const string m_effectId = "E_S";

    public override void RegisterEvents(KrampusEvents events) {
        events.onComboChanged.AddListener(MovementBuff);
        isActive = false;
    }

    public override void UnregisterEvents(KrampusEvents events) {
        events.onComboChanged.RemoveListener(MovementBuff);
    }

    private void MovementBuff(Krampus krampus, float combo) {

        if (combo >= m_comboTreshold) {
            if (isActive == false) {
                krampus.Stats.RegisterEffect(new Effect(KrampusStats.Stat.Speed, m_speedValue, m_effectId));
                Game.MainGameInfo.UI.DisplayEffect(ItemIcon, ItemName, m_effectId);
                isActive = true;
            }
        } else if(isActive) {
            
                Effect e = krampus.Stats.GetEffect(m_effectId);
                if (e != null) {
                    krampus.Stats.UnregisterEffect(e);
                    Game.MainGameInfo.UI.RemovEffectIcon(m_effectId);
                }

            }
       
    }
}
