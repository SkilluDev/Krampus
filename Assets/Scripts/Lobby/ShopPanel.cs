using System;
using UnityEngine.UI;   
using TMPro;
using UnityEngine;
using System.Collections.Generic;
using NUnit.Framework;




public class ShopPanel : MonoBehaviour
{
    [SerializeField] private RectTransform m_shopContent;

    [SerializeField] private ShopCard m_shopCardPref;

    private ShopItem m_highlightedItem;


    [Header("Description")]
    [SerializeField] private Image m_descIcon;  
    [SerializeField] private TextMeshProUGUI m_titleText;
    [SerializeField] private TextMeshProUGUI m_descText;
    [SerializeField] private TextMeshProUGUI m_priceText;

    [SerializeField] private TextMeshProUGUI m_buttonText;

    private void Start() {
        UpdateShop();
    }


	public void UpdateShop() {

        int numChildren = m_shopContent.childCount;
        for( int i=numChildren-1 ; i>=0 ; i-- )
        {
            GameObject.Destroy( m_shopContent.GetChild(i).gameObject );
        }

		List<ShopItem> items =  Game.Lobbyinfo.ShopScript.shopItems;
        items.Sort(delegate(ShopItem s1,ShopItem s2) {
            return Game.PogMan.m_allKrampusItems.Contains(s1.Item)?1:0 - (Game.PogMan.m_allKrampusItems.Contains(s2.Item)?1:0);
        });

        foreach(ShopItem i in items) {
            ShopCard shopCard = Instantiate(m_shopCardPref);
            shopCard.SetInfo(i, Game.PogMan.m_allKrampusItems.Contains(i.Item));  
            shopCard.transform.SetParent(m_shopContent,false);
            
            
        }
	}

    
    public void ShowDetails(ShopItem item,bool sold) {
        m_descIcon.sprite = item.Item.ItemIcon;
        m_titleText.text = item.Item.ItemName;
        m_descText.text = item.Item.Description;
        m_buttonText.text = sold?"SOLD":"BUY";
        m_priceText.text = item.price.ToString();
        m_highlightedItem = item;

    }

    public void  BuyItem() {
        if(m_highlightedItem == null) return;
        
        if(Game.PogMan.HasGold(m_highlightedItem.price))
        {
            Game.PogMan.PayGold(m_highlightedItem.price);
            Game.PogMan.m_allKrampusItems.Add(m_highlightedItem.Item);
            UpdateShop();
            Game.Lobbyinfo.UI.UpdateEqu();
            ShowDetails(m_highlightedItem, true);
        }
        else {
            //No feed
        }
    }
}
