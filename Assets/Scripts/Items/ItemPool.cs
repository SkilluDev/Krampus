using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Items/ItemPool", fileName = "ItemPool")]
public class ItemPool : ScriptableObject {
    public Item[] items;


    public Item[] RandomItemFor(int size, KrampusStats krampusStats) {

        List<int> indexes = new List<int>();
        for (int i = 0; i < items.Length; i++) {
            Item item = items[i];
            //Debug.Log("Checking " + item.ItemName);
            if (!krampusStats.HasItem(item)) {
                indexes.Add(i);
                //Debug.Log("Added " + i);
            }
        }

        int[] pos =  Shuffle(indexes.ToArray());
        Item[] results = new Item[size];

        for (int j = 0; j < size; j++) {
            results[j] = items[pos[j]];
        }

        return results;
    }


    private int[] Shuffle(int[] el) {
        // Knuth shuffle algorithm :: courtesy of Wikipedia :)
        for (int t = 0; t < el.Length; t++) {
            int tmp = el[t];
            int r = UnityEngine.Random.Range(t, el.Length);
            el[t] = el[r];
            el[r] = tmp;
        }

        return el;
    }
}


