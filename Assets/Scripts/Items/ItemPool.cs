using System.Collections.Generic;
using KrampUtils;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Items/ItemPool", fileName = "ItemPool")]
public class ItemPool : ScriptableObject {
    public Item[] items;

    public Item[] RandomItemFor(int size, KrampusStats krampusStats) {

        var indexes = new List<int>();
        for (int i = 0; i < items.Length; i++) {
            Item item = items[i];
            //Debug.Log("Checking " + item.ItemName);
            if (!krampusStats.HasItem(item)) {
                indexes.Add(i);
                //Debug.Log("Added " + i);
            }
        }

        int[] pos = indexes.UnityShuffle();
        var results = new Item[size];

        for (int j = 0; j < size; j++) {
            results[j] = items[pos[j]];
        }

        return results;
    }

}


