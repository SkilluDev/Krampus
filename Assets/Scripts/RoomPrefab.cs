using System;
using System.Collections.Generic;
using UnityEngine;

public class RoomPrefab : MonoBehaviour {

    public const int CELL_SIZE = 10;
    // Gizmo constants
    public const float DOOR_SIZE = 1f;
    public static readonly Vector3 DOOR_NS_SIZE = new Vector3(DOOR_SIZE, DOOR_SIZE / 2, DOOR_SIZE * 2);
    public static readonly Vector3 DOOR_EW_SIZE = new Vector3(DOOR_SIZE * 2, DOOR_SIZE / 2, DOOR_SIZE);
    // 
    public int Width => m_type.Width;
    public int Height => m_type.Height;
    [SerializeField] private RoomType m_type;
    [SerializeField][HideInInspector] private GameObject m_floorObject;
    [SerializeField][HideInInspector] private Array2D<DoorGroups> m_groups;


    public static Vector3 GetCellCenter(int i, int j) {
        return new Vector3((CELL_SIZE * i) + CELL_SIZE / 2f, 0, (-CELL_SIZE * j) - CELL_SIZE / 2f);
    }

    public static Vector3 GetPleasantDoorPosition(int i, int j, QuadDirection dir) {
        return GetCellCenter(i, j) + (dir.XZ() * (CELL_SIZE / 2 - DOOR_SIZE)); // el funky mathematico to get a nice door position; 
    }

    public static Vector3 GetDoorPosition(int i, int j, QuadDirection dir) {
        return GetCellCenter(i, j) + dir.XZ() * CELL_SIZE;
    }

    public static Vector3 GetCellTopLeft(int i, int j) {
        return new Vector3(CELL_SIZE * i, 0, -CELL_SIZE * j);
    }


    public bool CheckNeedsUpdate(RoomType type) {
        if (m_type != type) return true;
        // not implemented!
        return false;
    }

    [ContextMenu("generate")]
    public void RegenerateLayout() {
        if (Application.isPlaying) throw new Exception("Cannot generate layouts at runtime; You are probably doing something wrong.");

        if (m_floorObject != null) DestroyImmediate(m_floorObject);
        m_floorObject = CreateFloor();

        if (m_groups == null) m_groups = new Array2D<DoorGroups>(Width, Height);
        if (m_type.Width != m_groups.Width || m_type.Height != m_groups.Height) {
            DestroyDoorGroups();
            m_groups = new Array2D<DoorGroups>(Width, Height);
        }
        UpdateDoorGroups();
    }

    private GameObject CreateFloor() {
        var verts = new List<Vector3>();
        var tris = new List<int>();
        var uvs = new List<Vector2>();

        for (int i = 0; i < Width; i++) {
            for (int j = 0; j < Height; j++) {
                if (m_type.constraints[i, j] == null || m_type.constraints[i, j].phantom)
                    continue;

                int offset = verts.Count;

                verts.Add(new Vector3(CELL_SIZE * i, 0, -CELL_SIZE * j));
                verts.Add(new Vector3(CELL_SIZE * (i + 1), 0, -CELL_SIZE * j));
                verts.Add(new Vector3(CELL_SIZE * i, 0, -CELL_SIZE * (j + 1)));
                verts.Add(new Vector3(CELL_SIZE * (i + 1), 0, -CELL_SIZE * (j + 1)));
                uvs.Add(new Vector2(0, 0));
                uvs.Add(new Vector2(1, 0));
                uvs.Add(new Vector2(0, 1));
                uvs.Add(new Vector2(1, 1));

                tris.AddRange(new int[] {
                    offset, offset + 1, offset + 2,
                    offset + 3, offset + 2, offset + 1
                });
            }
        }
        var generation = new Mesh();
        generation.SetVertices(verts);
        generation.SetUVs(0, uvs);
        generation.SetTriangles(tris, 0);
        generation.RecalculateNormals();

        var floorObject = new GameObject("Floor");
        floorObject.transform.SetParent(transform);
        var meshFilter = floorObject.AddComponent<MeshFilter>();
        var meshRenderer = floorObject.AddComponent<MeshRenderer>();

        meshFilter.sharedMesh = generation;
        meshRenderer.material = new Material(Shader.Find("Universal Render Pipeline/Lit"));
        return floorObject;
    }

    private void DestroyDoorGroups() {
        for (int i = 0; i < Width; i++) {
            for (int j = 0; j < Height; j++) {
                if (m_groups[i, j] != null) {
                    m_groups[i, j].Destroy();
                    m_groups[i, j] = null;
                }
            }
        }
    }


    private void UpdateDoorGroups() {
        for (int i = 0; i < Width; i++) {
            for (int j = 0; j < Height; j++) {
                var targetDoors = m_type.constraints[i, j]?.optionalDoors;

                if (targetDoors == null) {
                    if (m_groups[i, j] != null) {
                        m_groups[i, j].Destroy();
                        m_groups[i, j] = null;
                    }
                    continue;
                }

                if (m_groups[i, j] == null) {
                    m_groups[i, j] = new DoorGroups();
                }

                foreach (var dir in DirectionMethods.cardinals) {
                    if (targetDoors[dir] && m_groups[i, j][dir] == null) {
                        m_groups[i, j][dir] = RoomDoorGroup.Create(new Vector2Int(i, j), dir, this);
                    }
                    if (!targetDoors[dir] && m_groups[i, j][dir] != null) {
                        m_groups[i, j].Destroy(dir);
                    }
                }
            }
        }
    }


    #region Gizmos
    private void OnDrawGizmos() {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(Vector3.zero, DOOR_SIZE);

        var required = new Color(0.06f, 0.6f, 0.06f, 0.4f);
        var optional = new Color(0.6f, 0.6f, 0.06f, 0.4f);
        var none = new Color(0.6f, 0.06f, 0.06f, 0f);

        for (int i = 0; i < Width; i++) {
            for (int j = 0; j < Height; j++) {
                if (m_type.constraints[i, j] == null) continue;
                var rd = m_type.constraints[i, j].requiredDoors;
                var od = m_type.constraints[i, j].optionalDoors;

                Gizmos.color = rd.North ? required : (od.North ? optional : none);
                Gizmos.DrawWireCube(GetPleasantDoorPosition(i, j, QuadDirection.NORTH), DOOR_NS_SIZE);

                Gizmos.color = rd.South ? required : (od.South ? optional : none);
                Gizmos.DrawWireCube(GetPleasantDoorPosition(i, j, QuadDirection.SOUTH), DOOR_NS_SIZE);

                Gizmos.color = rd.East ? required : (od.East ? optional : none);
                Gizmos.DrawWireCube(GetPleasantDoorPosition(i, j, QuadDirection.EAST), DOOR_EW_SIZE);

                Gizmos.color = rd.West ? required : (od.West ? optional : none);
                Gizmos.DrawWireCube(GetPleasantDoorPosition(i, j, QuadDirection.WEST), DOOR_EW_SIZE);
            }
        }
    }
    #endregion
}
