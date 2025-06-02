using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Items/Item", fileName = "Item")]
public class Item : ScriptableObject {

    [SerializeField] protected string m_itemName = "Item Name";
    public string ItemName => m_itemName;


    [SerializeField] private Sprite m_itemIcon;
    public Sprite ItemIcon => m_itemIcon;


    [TextArea(4, 25)]
    [SerializeField] private string m_description;
    public string Description => m_description;

    protected Krampus m_krampus;


    virtual public void RegisterItem(Krampus krampus) {

        m_krampus = krampus;
    }


    public bool IsItem(Item item) {
        return this.m_itemName.Equals(item.m_itemName);
     }



}
