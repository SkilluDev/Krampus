using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Set", menuName = "Game/Room Set")]
public class RoomSet : ScriptableObject {
    public RoomType[] types;


    public List<RoomType> GetTierSortedList() {
        var list = new List<RoomType>();

        foreach (var r in types) {
            list.Add(Instantiate(r));
        }

        list.Sort((a, b) => b.Grade - a.Grade);

        return list;
    }
}
