using System.Linq;
using KrampUtils;
using UnityEngine;

[CreateAssetMenu(menuName = "Game/Items/ItemPool", fileName = "ItemPool")]
public class ItemPool : ScriptableObject {
    public Item[] items;

    public Item[] RandomItemForKrampus(int howMany, KrampusStats krampusStats) =>
        items.Where(w => !krampusStats.HasItem(w)).UnityShuffle().Take(howMany).ToArray();

}
