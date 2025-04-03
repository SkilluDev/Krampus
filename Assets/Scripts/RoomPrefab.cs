using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class RoomPrefab {
    private Room m_targetRoom;
    private GameObject m_targetObject;
    private RoomType m_type;
    private string m_prefabPath;


    public RoomPrefab(RoomType type, string savePath) {
        m_type = type;
        LoadObjectToMemory(CreatePrefabObject(type.name, savePath));
    }

    private GameObject CreatePrefabObject(string name, string savePath) {
        var sceneObj = new GameObject(name);
        var prefabObj = PrefabUtility.SaveAsPrefabAsset(sceneObj, savePath);
        Object.DestroyImmediate(sceneObj);
        return prefabObj;
    }

    public RoomPrefab(RoomType type, GameObject prefab) {
        m_type = type;
        LoadObjectToMemory(prefab);
    }

    private void LoadObjectToMemory(GameObject obj) {
        if (PrefabUtility.GetPrefabAssetType(obj) == PrefabAssetType.NotAPrefab)
            throw new System.Exception("This object is not a prefab; How did this happen");

        m_prefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
        obj = PrefabUtility.LoadPrefabContents(m_prefabPath);
        if (obj.TryGetComponent<Room>(out var room)) {
            m_targetRoom = room;
        } else {
            m_targetRoom = obj.AddComponent<Room>();
            m_targetRoom.m_type = m_type;
        }
        m_targetObject = obj;
    }

    public void ApplyAndSave() {
        PrefabUtility.SaveAsPrefabAsset(m_targetObject, m_prefabPath);
    }

    public static bool CheckSizeObsolete(Room room, RoomType type) {
        if (room == null) return true;
        return room.m_doorGrid == null || type.Width != room.Width || type.Height != room.Height;
    }

    public static bool CheckFloorObsolete(Room room, RoomType type) {
        if (CheckSizeObsolete(room, type)) return true;
        for (int i = 0; i < room.Width; i++) {
            for (int j = 0; j < room.Height; j++) {
                if (room.m_doorGrid[i, j] != null && (type.constraints[i, j] == null || type.constraints[i, j].phantom))
                    return true;
                if (room.m_doorGrid[i, j] == null && (type.constraints[i, j] != null && !type.constraints[i, j].phantom))
                    return true;
            }
        }
        return false;
    }

    public static bool CheckDoorsObsolete(Room room, RoomType type) {
        if (CheckSizeObsolete(room, type)) return true;
        for (int i = 0; i < room.Width; i++) {
            for (int j = 0; j < room.Height; j++) {
                var targetDoors = type.constraints[i, j]?.optionalDoors;

                if ((targetDoors == null && room.m_doorGrid[i, j] != null) || (room.m_doorGrid[i, j] == null && targetDoors != null))
                    return true;

                foreach (var dir in DirectionMethods.CARDINALS) {
                    if ((targetDoors[dir] && room.m_doorGrid[i, j][dir] == null) || (!targetDoors[dir] && room.m_doorGrid[i, j][dir] != null))
                        return true;
                }
            }
        }
        return false;
    }

    public MeshFilter GetFloorMeshFilter() {
        var mf = m_targetRoom.m_floorMeshFilter;

        if (mf == null) {
            var ngo = new GameObject("fag");
            mf = ngo.AddComponent<MeshFilter>();
            ngo.transform.SetParent(m_targetRoom.transform);
        }

        return mf;
    }

    public GameObject CreateFloor() {
        var verts = new List<Vector3>();
        var tris = new List<int>();
        var uvs = new List<Vector2>();

        for (int i = 0; i < m_type.Width; i++) {
            for (int j = 0; j < m_type.Height; j++) {
                if (m_type.constraints[i, j] == null || m_type.constraints[i, j].phantom)
                    continue;

                int offset = verts.Count;

                verts.Add(new Vector3(Room.CELL_SIZE * i, 0, -Room.CELL_SIZE * j));
                verts.Add(new Vector3(Room.CELL_SIZE * (i + 1), 0, -Room.CELL_SIZE * j));
                verts.Add(new Vector3(Room.CELL_SIZE * i, 0, -Room.CELL_SIZE * (j + 1)));
                verts.Add(new Vector3(Room.CELL_SIZE * (i + 1), 0, -Room.CELL_SIZE * (j + 1)));
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
        // floorObject.transform.SetParent(transform);
        var meshFilter = floorObject.AddComponent<MeshFilter>();
        meshFilter.sharedMesh = generation; ;
        return floorObject;
    }

    private void DestroyDoorGroups() {
        for (int i = 0; i < m_targetRoom.Width; i++) {
            for (int j = 0; j < m_targetRoom.Height; j++) {
                if (m_targetRoom.m_doorGrid[i, j] != null) {
                    m_targetRoom.m_doorGrid[i, j].Destroy();
                    m_targetRoom.m_doorGrid[i, j] = null;
                }
            }
        }
    }


    private void UpdateDoorGroups() {
        for (int i = 0; i < m_targetRoom.Width; i++) {
            for (int j = 0; j < m_targetRoom.Height; j++) {
                var targetDoors = m_type.constraints[i, j]?.optionalDoors;

                if (targetDoors == null) {
                    if (m_targetRoom.m_doorGrid[i, j] != null) {
                        m_targetRoom.m_doorGrid[i, j].Destroy();
                        m_targetRoom.m_doorGrid[i, j] = null;
                    }
                    continue;
                }

                if (m_targetRoom.m_doorGrid[i, j] == null) {
                    m_targetRoom.m_doorGrid[i, j] = new DoorPropGroups();
                }

                foreach (var dir in DirectionMethods.CARDINALS) {
                    if (targetDoors[dir] && m_targetRoom.m_doorGrid[i, j][dir] == null) {
                        m_targetRoom.m_doorGrid[i, j][dir] = PropGroup.Create(new Vector2Int(i, j), dir, m_targetRoom);
                    }
                    if (!targetDoors[dir] && m_targetRoom.m_doorGrid[i, j][dir] != null) {
                        m_targetRoom.m_doorGrid[i, j].Destroy(dir);
                    }
                }
            }
        }
    }

}