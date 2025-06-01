using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Items/Item", fileName = "Item")]
public class Item : ScriptableObject {

    [SerializeField] private string m_itemName = "Item Name";
    [SerializeField] private Sprite m_itemIcon;

    [TextArea(4,25)]
    [SerializeField] private string m_description;

    protected Krampus m_krampus;


    virtual public void RegisterItem(Krampus krampus) {

        m_krampus = krampus;
     }

    



}
