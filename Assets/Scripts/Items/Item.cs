using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Items/Item", fileName = "Item")]
public class Item : ScriptableObject {

    [SerializeField] private string m_ItemName = "Item Name";
    [SerializeField] private Sprite m_ItemIcon;

    [TextArea(4,25)]
    [SerializeField] private string m_Description;


   

    virtual public void RegisterItem(KrampEvents events) { }

    



}
