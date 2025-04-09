using System.Collections.Generic;
using Roomgen;
using UnityEngine;


[CreateAssetMenu(fileName = "New Set", menuName = "Game/Room Set")]
public class RoomSet : ScriptableObject {
    public RoomType[] types;
    public GameObject[] doorPrefabs;

    public List<RoomType> GetTierSortedList() {
        var list = new List<RoomType>();

        foreach (var r in types) {
            for (int i = 0; i < 4; i++) {
            }
        }

        list.Sort((a, b) => b.Grade - a.Grade);

        return list;
    }
}
