using UnityEngine;

[System.Serializable]
public class DoorGroups : QuadDirectional<DoorGroups, RoomDoorGroup> {
    // [SerializeField] private RoomDoorGroup m_north, m_east, m_south, m_west; // need that for serializing??
    public override RoomDoorGroup North { get; set; }
    public override RoomDoorGroup East { get; set; }
    public override RoomDoorGroup South { get; set; }
    public override RoomDoorGroup West { get; set; }
}