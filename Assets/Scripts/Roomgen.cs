using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;


public class Roomgen : MonoBehaviour {
    private DoorFlags[,] m_doorGrid;
    [SerializeField] private int m_width, m_height;
    [SerializeField] private RoomType m_roomType;

    [ContextMenu("gen")]
    public void Generate() {
        m_doorGrid = new DoorFlags[m_width, m_height];
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
            for (int i = 0; i < m_width - room.Width; i++) {
                for (int j = 0; j < m_height - room.Height; j++) {
                    if (room.CanPlace(i, j, m_doorGrid)) list.Add(new Vector2Int(i, j));
                }
            }
            return list;
        }



        RoomPrefab PlaceRoom(RoomType room, Vector2Int placement) {
            var origin = RoomPrefab.GetCellTopLeft(placement.x, placement.y);
            return Instantiate(room.prefab, origin, Quaternion.identity, transform).GetComponent<RoomPrefab>();
        }

        // for now just place it
        var possiblePlacements = FindPossiblePlacements(m_roomType);

        if (possiblePlacements.Count > 0) PlaceRoom(m_roomType, possiblePlacements[0]);

        DebugLogDoorset();
        foreach (var c in possiblePlacements) Debug.Log(c);
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
