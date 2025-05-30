using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(menuName = "Game/Items/Item", fileName = "Item")]
public class Item : ScriptableObject {

    [SerializeField] private string m_ItemName = "Item Name";
    [SerializeField] private Sprite m_ItemIcon;


    virtual protected void OnPassiveActivated() { }

    



}
