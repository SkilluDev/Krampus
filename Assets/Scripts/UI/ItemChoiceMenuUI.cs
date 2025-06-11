using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemChoiceMenuUI : MonoBehaviour {
    [SerializeField] private ItemCard[] m_itemCards;

    private Item[] m_itemsDisplayed;


    public void OnEnable() {
        SetItems();
    }

    public void SetItems() {
        var items = Game.MainGameInfo.ItemPool.RandomItemFor(m_itemCards.Length, Game.MainGameInfo.Krampus.Stats);
        m_itemsDisplayed = new Item[m_itemCards.Length];
        for (int i = 0; i < m_itemCards.Length; i++) {
            var a = items[i];
            m_itemsDisplayed[i] = a;
            m_itemCards[i].SetInfo(a.ItemIcon, a.ItemName, a.Description);
        }
    }

    public void ChooseItem(int pos) {
        Game.MainGameInfo.Krampus.Stats.AddItem(m_itemsDisplayed[pos]);

        Game.MainGameInfo.SetState(MainGameInfo.State.Game);

        gameObject.SetActive(false);

        Debug.Log("Got item: " + m_itemsDisplayed[pos].ItemName);
    }


}

