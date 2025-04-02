
public class RoomPrefab {
    private Room m_target;
    private RoomType m_type;

    public RoomPrefab(RoomType type) {
        m_type = type;
    }

    public bool CheckSizeObsolete() {
        return m_groups == null || type.Width != Width || type.Height != Height;
    }

    public bool CheckFloorObsolete() {
        if (CheckSizeObsolete()) return true;
        for (int i = 0; i < Width; i++) {
            for (int j = 0; j < Height; j++) {
                if (m_groups[i, j] != null && (type.constraints[i, j] == null || type.constraints[i, j].phantom))
                    return true;
                if (m_groups[i, j] == null && (type.constraints[i, j] != null && !type.constraints[i, j].phantom))
                    return true;
            }
        }
        return false;
    }

    public bool CheckDoorsObsolete() {
        if (CheckSizeObsolete()) return true;
        for (int i = 0; i < Width; i++) {
            for (int j = 0; j < Height; j++) {
                var targetDoors = type.constraints[i, j]?.optionalDoors;

                if ((targetDoors == null && m_groups[i, j] != null) || (m_groups[i, j] == null && targetDoors != null))
                    return true;

                foreach (var dir in DirectionMethods.CARDINALS) {
                    if ((targetDoors[dir] && m_groups[i, j][dir] == null) || (!targetDoors[dir] && m_groups[i, j][dir] != null))
                        return true;
                }
            }
        }
        return false;
    }

    public void RegenerateLayout() {
        if (CheckFloorObsolete()) {
            if (m_floorObject != null) DestroyImmediate(m_floorObject);
            m_floorObject = CreateFloor();
        }

        if (CheckSizeObsolete()) {
            if (m_groups != null) DestroyDoorGroups();
            m_groups = new Array2D<DoorPropGroups>(type.Width, type.Height);
        }

        // no need to check whether the doors are obsolete, this will not do anything if it has nothing to do.
        UpdateDoorGroups();
    }

    private GameObject CreateFloor() {
        var verts = new List<Vector3>();
        var tris = new List<int>();
        var uvs = new List<Vector2>();

        for (int i = 0; i < type.Width; i++) {
            for (int j = 0; j < type.Height; j++) {
                if (type.constraints[i, j] == null || type.constraints[i, j].phantom)
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
        meshFilter.sharedMesh = generation; ;
        return floorObject;
    }

    private void DestroyDoorGroups() {
        for (int i = 0; i < m_groups.Width; i++) {
            for (int j = 0; j < m_groups.Height; j++) {
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
                var targetDoors = type.constraints[i, j]?.optionalDoors;

                if (targetDoors == null) {
                    if (m_groups[i, j] != null) {
                        m_groups[i, j].Destroy();
                        m_groups[i, j] = null;
                    }
                    continue;
                }

                if (m_groups[i, j] == null) {
                    m_groups[i, j] = new DoorPropGroups();
                }

                foreach (var dir in DirectionMethods.CARDINALS) {
                    if (targetDoors[dir] && m_groups[i, j][dir] == null) {
                        m_groups[i, j][dir] = PropGroup.Create(new Vector2Int(i, j), dir, this);
                    }
                    if (!targetDoors[dir] && m_groups[i, j][dir] != null) {
                        m_groups[i, j].Destroy(dir);
                    }
                }
            }
        }
    }
}