using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;
using KrampUtils;
using UnityEditor;

namespace Roomgen {
    public class RoomPrefabEditor {
        private const string ASSET_WALL_PATH = "Assets/Prefabs/Roomgen/Wall.prefab";
        private const string ASSET_WALLDOOR_PATH = "Assets/Prefabs/Roomgen/WallDoor.prefab";
        private const string ASSET_WALLSTUB_PATH = "Assets/Prefabs/Roomgen/WallStub.prefab";
        private Room m_targetRoom;
        private GameObject m_targetObject;
        private RoomType m_type;
        public string PrefabPath { get; private set; }

        public RoomPrefabEditor(RoomType type, string savePath) {
            m_type = type;
            LoadObjectToMemory(CreatePrefabObject(type.name, savePath));
        }

        public RoomPrefabEditor(RoomType type, GameObject prefab) {
            m_type = type;
            LoadObjectToMemory(prefab);
        }

        // Literally just creates an empty
        private GameObject CreatePrefabObject(string name, string savePath) {
            var sceneObj = new GameObject(name);
            var prefabObj = PrefabUtility.SaveAsPrefabAsset(sceneObj, savePath);
            Object.DestroyImmediate(sceneObj);
            return prefabObj;
        }

        // Unity needs to load the prefab to a temporary scene to actually make changes to it.
        private void LoadObjectToMemory(GameObject obj) {
            if (PrefabUtility.GetPrefabAssetType(obj) == PrefabAssetType.NotAPrefab)
                throw new System.Exception("This object is not a prefab; How did this happen");

            PrefabPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(obj);
            obj = PrefabUtility.LoadPrefabContents(PrefabPath);
            if (obj.TryGetComponent<Room>(out var room)) {
                m_targetRoom = room;
            } else {
                m_targetRoom = obj.AddComponent<Room>();
            }
            m_targetRoom.m_type = m_type;
            m_targetObject = obj;
        }

        /// <summary>
        /// End editing the prefab and save to file (needs to be called to actually change a thing)
        /// </summary>
        public GameObject ApplyAndSave() {
            return PrefabUtility.SaveAsPrefabAsset(m_targetObject, PrefabPath);
        }

        public static bool CheckSizeObsolete(Room room, RoomType type) {
            if (room == null) return true;
            return room.m_doorGrid == null || type.Width != room.Width || type.Height != room.Height;
        }

        public static bool CheckFloorObsolete(Room room, RoomType type) {
            if (CheckSizeObsolete(room, type)) return true;
            for (int i = 0; i < room.Width; i++) {
                for (int j = 0; j < room.Height; j++) {
                    if (room.m_doorGrid[i, j] != null && type.constraints[i, j] == null)
                        return true;
                    if (room.m_doorGrid[i, j] == null && type.constraints[i, j] != null)
                        return true;

                    if (room.m_doorGrid[i, j] != null && room.m_doorGrid[i, j].phantom != type.constraints[i, j].phantom)
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

                    if (targetDoors == null && room.m_doorGrid[i, j] == null) continue;

                    foreach (var dir in DirectionMethods.CARDINALS) {
                        if ((targetDoors[dir] && room.m_doorGrid[i, j][dir] == null) || (!targetDoors[dir] && room.m_doorGrid[i, j][dir] != null))
                            return true;
                    }
                }
            }
            return false;
        }

        private static PropGroup CreateDoorGroup(Vector2Int cellPosition, QuadDirection dir, Room room) =>
            PropGroup.CreatePropGroup($"Door [{cellPosition} {dir}]",
                Room.GetPleasantDoorPosition(cellPosition.x, cellPosition.y, dir),
                room
            );

        /// <summary>
        /// Create a MeshFilter for the floor as a child of the prefab or get one if it exists
        /// </summary>
        /// <returns>The MeshFilter</returns>
        public MeshFilter GetOrAddFloorObject() {
            var meshFilter = m_targetRoom.m_floorMeshFilter;
            if (meshFilter == null) {
                var go = new GameObject("Floor");
                go.transform.SetParent(m_targetRoom.transform);
                meshFilter = go.AddComponent<MeshFilter>();
                m_targetRoom.m_floorMeshFilter = meshFilter;
                go.AddComponent<MeshRenderer>().sharedMaterial = GraphicsSettings.defaultRenderPipeline.defaultMaterial;
            }
            return meshFilter;
        }

        public List<GameObject> GetOrAddWallList() {
            var list = m_targetRoom.m_autoWalls;
            if (list == null) {
                m_targetRoom.m_autoWalls = new List<GameObject>();
                list = m_targetRoom.m_autoWalls;
            }
            return list;
        }

        /// <summary>
        /// Makes sure no objects that were attached to the walls (such a rookie mistake!) get deleted - they get unparented instead.
        /// </summary>
        public void CleanupWallList() {
            var list = GetOrAddWallList();
            foreach (var go in list) {
                if (go == null) continue;
                if (!PrefabUtility.IsPartOfPrefabInstance(go)) continue;
                var added = PrefabUtility.GetAddedGameObjects(go);
                foreach (var w in added) w.instanceGameObject.transform.SetParent(m_targetRoom.transform);
                Object.DestroyImmediate(go);
            }
            list.Clear();
        }

        /// <summary>
        /// Updates the door grid size
        /// </summary>
        public void RecreateDoorGrid() {
            if (m_targetRoom.m_doorGrid == null) {
                m_targetRoom.m_doorGrid = new Array2D<DoorPropGroups>(m_type.Width, m_type.Height);
            } else {
                DestroyDoorGroups();
                m_targetRoom.m_doorGrid = new Array2D<DoorPropGroups>(m_type.Width, m_type.Height);
            }
        }

        /// <summary>
        /// Creates floor and updates the doorGrid to match
        /// </summary>
        /// <returns>The mesh created. Should be checked whether it can be reused or does it need saving first</returns>
        public Mesh CreateFloorMesh() {
            if (m_targetRoom.m_doorGrid == null) throw new System.Exception("Uninitiated door grid!");

            var verts = new List<Vector3>();
            var tris = new List<int>();
            var uvs = new List<Vector2>();

            for (int i = 0; i < m_type.Width; i++) {
                for (int j = 0; j < m_type.Height; j++) {
                    if (m_type.constraints[i, j] == null) {
                        if (m_targetRoom.m_doorGrid[i, j] != null)
                            m_targetRoom.m_doorGrid[i, j].Destroy();
                        continue;
                    }

                    if (m_targetRoom.m_doorGrid[i, j] == null)
                        m_targetRoom.m_doorGrid[i, j] = new DoorPropGroups();

                    if (m_type.constraints[i, j].phantom) {
                        m_targetRoom.m_doorGrid[i, j].phantom = true;
                        continue;
                    }

                    m_targetRoom.m_doorGrid[i, j].phantom = false;

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

            var mf = GetOrAddFloorObject();
            if (mf != null && mf.sharedMesh != null && !mf.sharedMesh.isReadable) {
                Object.Destroy(mf.gameObject);
                Debug.LogWarning("The default floor's MeshFilter has an unreadable Mesh assigned; Please don't do this!");
                mf = GetOrAddFloorObject();
            }
            var generation = mf.sharedMesh;
            if (generation == null) generation = new Mesh();
            generation.Clear();
            generation.SetVertices(verts);
            generation.SetUVs(0, uvs);
            generation.SetTriangles(tris, 0);
            generation.RecalculateNormals();
            mf.sharedMesh = generation;

            return generation;
        }

        /// <summary>
        /// Destroys all the door groups
        /// </summary>
        public void DestroyDoorGroups() {
            if (m_targetRoom.m_doorGrid == null) throw new System.Exception("Uninitiated door grid!");

            for (int i = 0; i < m_targetRoom.Width; i++) {
                for (int j = 0; j < m_targetRoom.Height; j++) {
                    if (m_targetRoom.m_doorGrid[i, j] != null) {
                        m_targetRoom.m_doorGrid[i, j].Destroy();
                        m_targetRoom.m_doorGrid[i, j] = null;
                    }
                }
            }
        }

        public void CreateWalls() {
            if (m_targetRoom.m_doorGrid == null) throw new System.Exception("Uninitiated door grid!");

            var list = GetOrAddWallList();

            var oldMats = list.Count == 0 || list.FirstOrDefault(w => w != null) == null ?
                null : list.FirstOrDefault(w => w != null).GetComponentInChildren<MeshRenderer>().sharedMaterials.ToList();

            CleanupWallList();

            var wallWall = AssetDatabase.LoadAssetAtPath<GameObject>(ASSET_WALL_PATH);
            var wallDoor = AssetDatabase.LoadAssetAtPath<GameObject>(ASSET_WALLDOOR_PATH);
            var wallStub = AssetDatabase.LoadAssetAtPath<GameObject>(ASSET_WALLSTUB_PATH);

            GameObject MakeWall(GameObject which, int i, int j, Quaternion rot) {
                var instance = (GameObject)PrefabUtility.InstantiatePrefab(which, m_targetObject.transform);
                instance.transform.position = Room.GetCellCenter(i, j);
                instance.transform.rotation = rot;

                var renderers = instance.GetComponentsInChildren<MeshRenderer>();
                if (oldMats != null) {
                    foreach (var ren in renderers.Where(w => w.sharedMaterials.Length == 3))
                        ren.SetSharedMaterials(oldMats);
                }

                list.Add(instance);
                return instance;
            }

            for (int i = 0; i < m_targetRoom.Width; i++) {
                for (int j = 0; j < m_targetRoom.Height; j++) {
                    if (m_type.constraints[i, j] == null) continue;
                    foreach (var dir in DirectionMethods.CARDINALS) {
                        if (m_type.constraints[i, j].optionalDoors[dir]) {
                            if (
                                m_type.constraints.InBounds(new Vector2Int(i, j) + dir.IJ()) &&
                                m_type.constraints[new Vector2Int(i, j) + dir.IJ()] != null &&
                                !m_type.constraints[new Vector2Int(i, j) + dir.IJ()].phantom
                            ) {
                                MakeWall(wallStub, i, j, dir.Rotate90Clockwise(3).YRotation());
                                continue;
                            }

                            var wo = MakeWall(wallDoor, i, j, dir.YRotation());
                            if (
                                wo.transform.childCount <= 0 ||
                                !m_targetRoom.m_doorGrid.InBounds(i, j) ||
                                m_targetRoom.m_doorGrid[i, j] == null ||
                                m_targetRoom.m_doorGrid[i, j][dir] == null
                            ) continue;
                            m_targetRoom.m_doorGrid[i, j][dir].AddToDisableList(wo.transform.GetChild(0).gameObject);
                            m_targetRoom.m_doorGrid[i, j][dir].Cleanup();
                        } else if (m_type.constraints[i, j].requiredDoors[dir]) {
                            if (
                                m_type.constraints.InBounds(new Vector2Int(i, j) + dir.IJ()) &&
                                m_type.constraints[new Vector2Int(i, j) + dir.IJ()] != null &&
                                !m_type.constraints[new Vector2Int(i, j) + dir.IJ()].phantom
                            ) {
                                MakeWall(wallStub, i, j, dir.Rotate90Clockwise(3).YRotation());
                                continue;
                            }

                            var wo = MakeWall(wallDoor, i, j, dir.YRotation());
                            if (wo.transform.childCount <= 0) continue;
                            wo.transform.GetChild(0).gameObject.SetActive(false);
                        } else {
                            MakeWall(wallWall, i, j, dir.YRotation());
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Updates the door groups to match RoomType
        /// </summary>
        public void UpdateDoorGroups() {
            if (m_targetRoom.m_doorGrid == null) throw new System.Exception("Uninitiated door grid!");

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
                            m_targetRoom.m_doorGrid[i, j][dir] = RoomPrefabEditor.CreateDoorGroup(new Vector2Int(i, j), dir, m_targetRoom);
                        }
                        if (!targetDoors[dir] && m_targetRoom.m_doorGrid[i, j][dir] != null) {
                            m_targetRoom.m_doorGrid[i, j].Destroy(dir);
                        }
                    }
                }
            }
        }

    }
}