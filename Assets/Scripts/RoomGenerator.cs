using System.Collections.Generic;
using System.Linq;
using System.Text;
using KrampUtils;
using Roomgen;
using UnityEditor;
using UnityEngine;

public class RoomGenerator : MonoBehaviour {
    private DoorFlags[,] m_doorGrid;
    private bool[,] m_generationGrid;
    private Vector2Int m_spawnPoint;

    [SerializeField] private int m_width, m_height;
    [SerializeField] private RoomSet m_roomSet;
    [SerializeField] private int m_loopRectangles;

    private void Start() {
        Generate();
    }


    [NaughtyAttributes.Button("Generate Grid")]
    public void Generate() {
        Init();
        SelectSpawnPoint();
        CreateGrid();
        RemoveDeadDoors();

        PlaceRoom(m_roomSet.spawn, m_spawnPoint);

        var types = new List<RoomType>();
        foreach (var type in m_roomSet.types) {
            for (int i = 0; i < 4; i++) {
                if (type.supportedRots[i]) types.Add(RoomVariantManager.CreateRotatedInstance(type, i));
            }
        }

        // Using LINQ is probably suboptimal here.
        var grouped = types.GroupBy(x => x.Grade).OrderByDescending((w) => w.Key);

        foreach (var group in grouped) {
            Debug.Log($"Found {group.Count()} Room Variants with Tier {group.Key}");

            while (true) {
                var hardestToPlace = group.OrderBy(r => FindPossiblePlacements(r).Count).FirstOrDefault(r => FindPossiblePlacements(r).Count > 0);
                if (hardestToPlace == null) {
                    Debug.Log("No room could be placed");
                    break;
                }
                var placements = FindPossiblePlacements(hardestToPlace);

                Debug.Log($"Placing {hardestToPlace} in one of the {placements.Count} possible spots.");
                PlaceRoom(hardestToPlace, placements[Random.Range(0, placements.Count)]);
            }
        }

        RoomVariantManager.Release(types);
    }

    private void SelectSpawnPoint() {
        m_spawnPoint = new Vector2Int(m_width / 2, m_height / 2);
        m_generationGrid[m_width / 2, m_height / 2] = true;
    }

    private void Init() {
        m_doorGrid = new DoorFlags[m_width, m_height];
        m_generationGrid = new bool[m_width, m_height];
        for (int i = 0; i < m_width; i++) for (int j = 0; j < m_height; j++) m_doorGrid[i, j] = new DoorFlags();
    }


    private void CreateGrid() {
        void CreateRectangle(int sx, int sy, int ex, int ey) {
            if (sx > ex) (ex, sx) = (sx, ex);
            if (sy > ey) (ey, sy) = (sy, ey);

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

        // Create a rect that goes through the spawn
        { // yes this scope is useful, keeps the naming conventions and stuff
            int ex = Random.Range(0, m_width - 1);
            int ey = Random.Range(0, m_height - 1);
            if (ex >= m_spawnPoint.x) ex++;
            if (ey >= m_spawnPoint.y) ey++;
            CreateRectangle(m_spawnPoint.x, m_spawnPoint.y, ex, ey);

        }

        for (int i = 0; i < m_loopRectangles - 1; i++) {
            int sx = Random.Range(0, m_width - 1);
            int ex = Random.Range(sx + 1, m_width);
            int sy = Random.Range(0, m_height - 1);
            int ey = Random.Range(sy + 1, m_height);
            CreateRectangle(sx, sy, ex, ey);
        }
    }

    private void RemoveDeadDoors() {
        bool[,] floodFill = new bool[m_width, m_height];

        void FillCell(int x, int y) {
            if (floodFill[x, y]) return;
            floodFill[x, y] = true;
            if (m_doorGrid[x, y].North && y > 0) FillCell(x, y - 1);
            if (m_doorGrid[x, y].South && y < m_height - 1) FillCell(x, y + 1);
            if (m_doorGrid[x, y].East && x < m_width - 1) FillCell(x + 1, y);
            if (m_doorGrid[x, y].West && x > 0) FillCell(x - 1, y);
        }

        FillCell(m_spawnPoint.x, m_spawnPoint.y);

        for (int i = 0; i < m_width; i++) {
            for (int j = 0; j < m_height; j++) {
                if (!floodFill[i, j]) {
                    m_doorGrid[i, j].Reset();
                }
            }
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
                if (room.constraints[i - placement.x, j - placement.y] == null || room.constraints[i - placement.x, j - placement.y].phantom) continue;
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
