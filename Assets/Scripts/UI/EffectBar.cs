using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EffectBar : MonoBehaviour {


    private Dictionary<Item, EffectIcon> m_effectIcons = new Dictionary<Item, EffectIcon>();
    [SerializeField] private EffectIcon m_IconPrefabe;



    public void RegisterIcon(Item item) {
        EffectIcon effectIcon = Instantiate(m_IconPrefabe);
        effectIcon.transform.SetParent(this.transform, false);
        effectIcon.SetIcon("1", item.ItemIcon, item.ItemName);
        m_effectIcons.Add(item, effectIcon);
    }

    public void ActivateIcon(Item item, float duration) {
        m_effectIcons[item].Activate(duration);
    }
    public void ActivateIcon(Item item) {

    }
    public void DesactivateIcon(Item item) {
        
     }
}
