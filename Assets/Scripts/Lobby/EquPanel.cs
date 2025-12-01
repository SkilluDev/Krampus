using System.Collections.Generic;
using UnityEngine;

public class EquPanel : MonoBehaviour
{

    [SerializeField] private RectTransform m_allItemContainer;
    public RectTransform AllItemContainer => m_allItemContainer;

    [SerializeField] private InventoryCard m_inventoryCardPref;

    [SerializeField] private  ItemSlot[] m_itemSlots;


    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

   public void UpdateEqu() {
        int numChildren = m_allItemContainer.childCount;
        for( int i=numChildren-1 ; i>=0 ; i-- )
        {
            GameObject.Destroy( m_allItemContainer.GetChild(i).gameObject );
        }
        int tmp = 0;

        foreach(Item i in Game.PogMan.m_allKrampusItems) {
           
                InventoryCard ic = Instantiate(m_inventoryCardPref);
                ic.SetInfo(i);
                if (!Game.Lobbyinfo.Krampus.Stats.HasItem(i)) {
                    ic.transform.SetParent(m_allItemContainer, false);
                }
                else if(tmp < m_itemSlots.Length) {
                    ic.transform.SetParent(m_itemSlots[tmp].GetComponent<RectTransform>(),false);
                    ic.GetComponent<RectTransform>().localPosition = Vector2.zero;
                    tmp++;
                }
                
            
        }
    }


    public void ExitPanel() {
    var itemsToAdd = new List<Item>();
        Game.PogMan.Unpack(ref itemsToAdd);
        Game.Lobbyinfo.UI.EffectBar.ClearAll();

        Game.Lobbyinfo.Krampus.Stats.RemoveAllItems();
        foreach(var a in m_itemSlots) {
            if(a.GetComponent<RectTransform>().childCount > 0&& a.GetComponent<RectTransform>().GetChild(0)){
                if( a.GetComponent<RectTransform>().GetChild(0).GetComponent<InventoryCard>()) {
                    InventoryCard ic = a.GetComponent<RectTransform>().GetChild(0).GetComponent<InventoryCard>();
                    itemsToAdd.Add(ic.GetItem());
                    Game.Lobbyinfo.Krampus.Stats.AddItem(ic.GetItem());
                }
                
            }
        }
        
        Game.Lobbyinfo.UI.ExitPanel();
        
    }



}
