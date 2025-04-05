using System.Collections.Generic;
using System.Text;
using Roomgen;
using UnityEngine;

public class RoomGenerator : MonoBehaviour {
    private DoorFlags[,] m_doorGrid;
    private bool[,] m_generationGrid;

    [SerializeField] private int m_width, m_height;
    [SerializeField] private RoomSet m_roomSet;

    private void Start() {
        Generate();
    }

    public void Generate() {
        m_doorGrid = new DoorFlags[m_width, m_height];
        m_generationGrid = new bool[m_width, m_height];
        for (int i = 0; i < m_width; i++) for (int j = 0; j < m_height; j++) m_doorGrid[i, j] = new DoorFlags();

        void CreateRectangle(int sx, int sy, int ex, int ey) {
            for (int i = sx, j = sy; i <= ex; i++) {
                if (i != ex)
                    m_doorGrid[i, j].East = true;
                if (i != sx)
                    m_doorGrid[i, j].West = true;
            }

            for (int i = sx, j = ey; i <= ex; i++) {
                if (i != ex)
                    m_doorGrid[i, j].East = true;
                if (i != sx)
                    m_doorGrid[i, j].West = true;
            }

            for (int i = sx, j = sy; j <= ey; j++) {
                if (j != sy)
                    m_doorGrid[i, j].North = true;
                if (j != ey)
                    m_doorGrid[i, j].South = true;
            }

            for (int i = ex, j = sy; j <= ey; j++) {
                if (j != sy)
                    m_doorGrid[i, j].North = true;
                if (j != ey)
                    m_doorGrid[i, j].South = true;
            }
        }
        CreateRectangle(1, 1, 3, 3);
        CreateRectangle(0, 0, 1, 1);

        List<Vector2Int> FindPossiblePlacements(RoomType room) {
            var list = new List<Vector2Int>();
            for (int i = 0; i < m_width - room.Width + 1; i++) {
                for (int j = 0; j < m_height - room.Height + 1; j++) {
                    if (room.CanPlace(i, j, m_doorGrid, m_generationGrid)) list.Add(new Vector2Int(i, j));
                }
            }
            return list;
        }

        Room PlaceRoom(RoomType room, Vector2Int placement) {
            var origin = Room.GetCellTopLeft(placement.x, placement.y);
            for (int i = placement.x; i < placement.x + room.Width; i++) {
                for (int j = placement.y; j < placement.y + room.Height; j++) {
                    m_generationGrid[i, j] = true;
                }
            }
            var prefab = Instantiate(room.prefab, origin, Quaternion.identity, transform).GetComponent<Room>();
            prefab.ConfigureDoors(placement.x, placement.y, m_doorGrid);

            return prefab;
        }

        foreach (var rt in m_roomSet.GetTierSortedList()) {
            var possiblePlacements = new List<Vector2Int>();
            while ((possiblePlacements = FindPossiblePlacements(rt)).Count > 0) {
                PlaceRoom(rt, possiblePlacements[0]);
            }

        }

        DebugLogDoorset();
    }

    private void DebugLogDoorset() {
        var sb = new StringBuilder();
        for (int j = 0; j < m_height; j++) {
            for (int i = 0; i < m_width; i++) {
                var directions = m_doorGrid[i, j];
                char c = (directions.North, directions.East, directions.South, directions.West) switch {
                    (true, false, false, false) => '╀',
                    (true, false, false, true) => '╃',
                    (true, true, false, false) => '╄',
                    (true, true, false, true) => '╇',
                    (true, false, true, false) => '╂',
                    (true, false, true, true) => '╉',
                    (true, true, true, false) => '╊',
                    (true, true, true, true) => '╋',
                    (false, false, true, false) => '╁',
                    (false, false, true, true) => '╅',
                    (false, true, false, false) => '┽',
                    (false, true, false, true) => '┿',
                    (false, true, true, false) => '╆',
                    (false, true, true, true) => '╈',
                    (false, false, false, true) => '┽',
                    (false, false, false, false) => '┼', // Default empty space
                };
                sb.Append(c);
            }
            sb.Append('\n');
        }
        Debug.Log(sb.ToString());
    }
}
