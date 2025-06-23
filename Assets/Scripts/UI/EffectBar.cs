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


    public void ActivateItem(Krampus krampus, Item item) {
        switch (item.EffectiveType) {
            case ItemEffectiveType.Temporary:
                ActivateIcon(item, item.GetDuration());
                break;
            case ItemEffectiveType.Switch:
                ActivateIcon(item);
                break;
            case ItemEffectiveType.Stackable:
                ActivateIcon(item, item.GetStackAmount());
                break;
            case ItemEffectiveType.Paff:
                ActivateIconPuff(item);
                break;
        }
         
     }
    public void ActivateIcon(Item item, float duration) {
        m_effectIcons[item].Activate(duration);
    }
    public void ActivateIcon(Item item) {
        Debug.Log("Aktywowano item:" + item.ItemName);
        m_effectIcons[item].Activate();
    }
     public void ActivateIconPuff(Item item) {
        Debug.Log("Aktywowano item:" + item.ItemName);
        m_effectIcons[item].ActivatePuff();
    }
    public void DesactivateIcon(Krampus krampus, Item item) {
        m_effectIcons[item].Desactivate();
    }
}
