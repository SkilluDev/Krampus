using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class ItemChoiceMenuUI : MonoBehaviour {
    [SerializeField] private ItemCard[] m_itemCards;

    private Item[] m_itemsDisplayed;


    public void OnEnable() {
        SetItems();
    }

    public void SetItems() {
        var items = Game.MainGameInfo.ItemPool.RandomItemForKrampus(m_itemCards.Length, Game.MainGameInfo.Krampus.Stats);
		if (items.Count() == 0) {
			gameObject.SetActive(false);
			Game.MainGameInfo.SetState(MainGameInfo.State.Game);
			return;
		}
        m_itemsDisplayed = new Item[m_itemCards.Length];
        for (int i = 0; i < m_itemCards.Length; i++) {
			if (i < items.Count()) {
				var a = items[i];
				m_itemsDisplayed[i] = a;
				m_itemCards[i].SetInfo(a.ItemIcon, a.ItemName, a.Description);
			} else {
				m_itemCards[i].TurnOff();

			}

        }
    }

    public void ChooseItem(int pos) {
        Game.MainGameInfo.Krampus.Stats.AddItem(m_itemsDisplayed[pos]);
        Game.MainGameInfo.UI.UpdateInventory();
        Game.MainGameInfo.SetState(MainGameInfo.State.Game);

        gameObject.SetActive(false);



    }


}

