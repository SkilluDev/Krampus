using UnityEngine;

[System.Serializable]
public class DoorGroups : QuadDirectional<DoorGroups, RoomDoorGroup> {
    [SerializeField] private RoomDoorGroup m_north, m_east, m_south, m_west; // need that for serializing??
    public override RoomDoorGroup North {
        get => m_north;
        set => m_north = value;
    }
    public override RoomDoorGroup East {
        get => m_east;
        set => m_east = value;
    }
    public override RoomDoorGroup South {
        get => m_south;
        set => m_south = value;
    }
    public override RoomDoorGroup West {
        get => m_west;
        set => m_west = value;
    }

    public void Configure(QuadDirection dir) {
        foreach (var d in DirectionMethods.CARDINALS) {
            if (this[d] != null) this[d].Generate(!dir.HasFlag(d));
        }
    }


    public void Destroy(QuadDirection dir = QuadDirection.ALL) {
        if (Application.isPlaying) {
            if (North != null && dir.HasFlag(QuadDirection.NORTH)) Object.Destroy(North.gameObject);
            if (East != null && dir.HasFlag(QuadDirection.EAST)) Object.Destroy(East.gameObject);
            if (South != null && dir.HasFlag(QuadDirection.SOUTH)) Object.Destroy(South.gameObject);
            if (West != null && dir.HasFlag(QuadDirection.WEST)) Object.Destroy(West.gameObject);
        } else {
            if (North != null && dir.HasFlag(QuadDirection.NORTH)) Object.DestroyImmediate(North.gameObject);
            if (East != null && dir.HasFlag(QuadDirection.EAST)) Object.DestroyImmediate(East.gameObject);
            if (South != null && dir.HasFlag(QuadDirection.SOUTH)) Object.DestroyImmediate(South.gameObject);
            if (West != null && dir.HasFlag(QuadDirection.WEST)) Object.DestroyImmediate(West.gameObject);
        }
    }
}