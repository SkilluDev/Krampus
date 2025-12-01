using NUnit.Framework;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ShopCard : MonoBehaviour
{
    [SerializeField] private Image m_iconImage;
    [SerializeField] private TextMeshProUGUI m_priceText;
    private bool sold;
    private ShopItem m_shopItem;


    public void SetInfo(ShopItem shopItem, bool purchesed) {
        m_iconImage.sprite = shopItem.Item.ItemIcon;
        m_priceText.text = purchesed?"SOLD":shopItem.price.ToString();
        if(purchesed) {
            GetComponent<Image>().color = new Color(0.5f,0.5f,0.5f,1);
           
        }
        m_shopItem = shopItem;  
        sold =purchesed;
    }

    public void ShowDetails() {
        Game.Lobbyinfo.UI.ShopPanel.ShowDetails(m_shopItem,sold);
    }
}
