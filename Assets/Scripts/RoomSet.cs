using System.Collections.Generic;
using Roomgen;
using UnityEngine;


[CreateAssetMenu(fileName = "New Set", menuName = "Game/Room Set")]
public class RoomSet : ScriptableObject {
    public RoomType[] types;
    public GameObject[] doorPrefabs;
    public RoomType spawn;
}
