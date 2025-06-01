using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemChoiceMenuUI : MonoBehaviour {
    public ItemCard[] m_itemCards;


    public void ChooseItem(int pos) {

        //Coś tam

        Game.MainGameInfo.SetState(MainGameInfo.State.Game);
        Game.MainGameInfo.UI.UIElementsEntryAnimation();


        this.gameObject.SetActive(false);
 }


}

