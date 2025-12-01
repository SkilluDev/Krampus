using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class ShopItem {
    public int price;
    public Item Item;
    
}

public class ShopScript : MonoBehaviour
{
    public List<ShopItem> shopItems = new List<ShopItem>();

    

}
