using System.Collections.Generic;
using System.Text;
using KrampUtils;
using Roomgen;
using UnityEngine;

public class RoomGenerator : MonoBehaviour {
    private DoorFlags[,] m_doorGrid;
    private bool[,] m_generationGrid;

    [SerializeField] private int m_width, m_height;
    [SerializeField] private RoomSet m_roomSet;
    [SerializeField] private int m_loopRectangles;
    [SerializeField] private RoomType m_testType;

    private void Start() {
        Generate();
    }

    public void Generate() {
        CreateGrid();
        DebugLogDoorset();
    }

    private void CreateGrid() {
        m_doorGrid = new DoorFlags[m_width, m_height];
        m_generationGrid = new bool[m_width, m_height];
        for (int i = 0; i < m_width; i++) for (int j = 0; j < m_height; j++) m_doorGrid[i, j] = new DoorFlags();

        void CreateRectangle(int sx, int sy, int ex, int ey) {
            if (sx > ex) (ex, sx) = (sx, ex);
            if (sy > ey) (ey, sy) = (sy, ey);
            if (sx == ex) return;
            if (sy == ey) return; // do not create zero width rects

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

        // create the spawn
        m_generationGrid[m_width / 2, m_height / 2] = true;

        for (int i = 0; i < m_loopRectangles; i++) {
            int sx = Random.Range(0, m_width - 1);
            int ex = Random.Range(sx + 1, m_width);
            int sy = Random.Range(0, m_height - 1);
            int ey = Random.Range(sy + 1, m_height);
            CreateRectangle(sx, sy, ex, ey);
        }
    }

    private List<Vector2Int> FindPossiblePlacements(RoomType room) {
        var list = new List<Vector2Int>();
        for (int i = 0; i < m_width - room.Width + 1; i++) {
            for (int j = 0; j < m_height - room.Height + 1; j++) {
                if (room.CanPlace(i, j, m_doorGrid, m_generationGrid)) list.Add(new Vector2Int(i, j));
            }
        }
        return list;
    }

    private Room PlaceRoom(RoomType room, Vector2Int placement) {
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


    #region Gizmos
    private void OnDrawGizmosSelected() {
        if (m_doorGrid == null) return;
        Gizmos.color = Color.gray;
        for (int i = 0; i < m_width; i++) {
            for (int j = 0; j < m_height; j++) {
                if (m_doorGrid[i, j] == null) continue;
                var rd = m_doorGrid[i, j];

                if (rd.North)
                    Gizmos.DrawLine(Room.GetCellCenter(i, j), Room.GetCellCenter(i, j - 1));

                if (rd.South)
                    Gizmos.DrawLine(Room.GetCellCenter(i, j), Room.GetCellCenter(i, j + 1));

                if (rd.East)
                    Gizmos.DrawLine(Room.GetCellCenter(i, j), Room.GetCellCenter(i + 1, j));

                if (rd.West)
                    Gizmos.DrawLine(Room.GetCellCenter(i, j), Room.GetCellCenter(i - 1, j));
            }
        }
    }

    #endregion
}
