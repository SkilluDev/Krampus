using UnityEngine;
using KrampUtils;
using System;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Roomgen {
    public class Room : MonoBehaviour {
        public const int CELL_SIZE = 10;
        // Gizmo constants
        public const float DOOR_SIZE = 1f;
        public static readonly Vector3 DOOR_NS_SIZE = new Vector3(DOOR_SIZE, DOOR_SIZE / 2, DOOR_SIZE * 2);
        public static readonly Vector3 DOOR_EW_SIZE = new Vector3(DOOR_SIZE * 2, DOOR_SIZE / 2, DOOR_SIZE);
        // 
        public int Width => m_doorGrid.Width;
        public int Height => m_doorGrid.Height;

        [SerializeField][HideInInspector] internal RoomType m_type;
        [SerializeField][HideInInspector] internal Array2D<DoorPropGroups> m_doorGrid;
        [SerializeField][HideInInspector] internal MeshFilter m_floorMeshFilter;


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

        public void ConfigureDoors(int x, int y, DoorFlags[,] doors) {
            for (int i = 0; i < Width; i++) {
                for (int j = 0; j < Height; j++) {
                    m_doorGrid[i, j].SetState(doors[i + x, j + y]);
                }
            }
        }

        public void Rotate90Clockwise() {
            var old = m_doorGrid;
            var oldCenter = new Vector3(Width / 2f * CELL_SIZE, 0, -Height / 2f * CELL_SIZE);
            m_doorGrid = new Array2D<DoorPropGroups>(Height, Width);
            for (int i = 0; i < Width; i++) {
                for (int j = 0; j < Height; j++) {
                    m_doorGrid[i, j] = old[j, Width - 1 - i];
                    if (m_doorGrid[i, j] == null) continue;
                    m_doorGrid[i, j].Rotate90Clockwise();
                }
            }
            var newCenter = new Vector3(Width / 2f * CELL_SIZE, 0, -Height / 2f * CELL_SIZE);

            for (int i = 0; i < transform.childCount; i++) {
                var kid = transform.GetChild(i);
                kid.RotateAround(oldCenter, Vector3.up, 90);
                kid.position += newCenter - oldCenter;
            }
        }



        #region Gizmos
        private void OnDrawGizmos() {
            if (m_type == null) return;

            Gizmos.color = Color.blue;
            Gizmos.DrawSphere(transform.position, DOOR_SIZE);

            var required = new Color(0.06f, 0.6f, 0.06f, 0.4f);
            var optional = new Color(0.6f, 0.6f, 0.06f, 0.4f);
            var none = new Color(0.6f, 0.06f, 0.06f, 0f);

            for (int i = 0; i < m_type.Width; i++) {
                for (int j = 0; j < m_type.Height; j++) {
                    if (m_type.constraints[i, j] == null) continue;
                    var rd = m_type.constraints[i, j].requiredDoors;
                    var od = m_type.constraints[i, j].optionalDoors;

                    Gizmos.color = rd.North ? required : (od.North ? optional : none);
                    Gizmos.DrawWireCube(transform.position + GetPleasantDoorPosition(i, j, QuadDirection.NORTH), DOOR_NS_SIZE);

                    Gizmos.color = rd.South ? required : (od.South ? optional : none);
                    Gizmos.DrawWireCube(transform.position + GetPleasantDoorPosition(i, j, QuadDirection.SOUTH), DOOR_NS_SIZE);

                    Gizmos.color = rd.East ? required : (od.East ? optional : none);
                    Gizmos.DrawWireCube(transform.position + GetPleasantDoorPosition(i, j, QuadDirection.EAST), DOOR_EW_SIZE);

                    Gizmos.color = rd.West ? required : (od.West ? optional : none);
                    Gizmos.DrawWireCube(transform.position + GetPleasantDoorPosition(i, j, QuadDirection.WEST), DOOR_EW_SIZE);
                }
            }
        }

        #endregion
    }

#if UNITY_EDITOR

    [CustomEditor(typeof(Room))]
    public class RoomEditor : Editor {
        private RoomTypeEditor m_childEditor;
        private Room Target => (Room)target;

        public void OnEnable() {
            if (Target.m_type == null) {
                Debug.LogError($"Invalid RoomType on Room {Target.gameObject.name}!");
                return;
            }

            m_childEditor = (RoomTypeEditor)CreateEditor(Target.m_type);
            m_childEditor.m_showConstraints = false;
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();
            GUILayout.BeginHorizontal();
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Room Type", Target.m_type, typeof(RoomType), false);
            EditorGUI.EndDisabledGroup();
            if (Target.m_type != null && GUILayout.Button("Open")) Selection.activeObject = Target.m_type;
            EditorGUILayout.EndHorizontal();
            if (Target.m_type == null) {
                if (EditorGUIHelper.HelpBoxWithButton("The Room has no RoomType!", "Create", MessageType.Error)) {
                    CreateRoomType();
                }
                return;
            }
            if (m_childEditor == null) OnEnable();
            m_childEditor.OnPrefabInspectorGUI();
        }

        private void CreateRoomType() {
            var newrt = RoomType.CreateInstance<RoomType>();
            newrt.constraints = new Array2D<GridRoomConstraint>(Target.Width, Target.Height);
            for (int i = 0; i < Target.Width; i++) {
                for (int j = 0; j < Target.Height; j++) {
                    if (Target.m_doorGrid[i, j] == null) newrt.constraints[i, j] = null;
                    else {
                        newrt.constraints[i, j] = new GridRoomConstraint();
                        newrt.constraints[i, j].phantom = Target.m_doorGrid[i, j].phantom;
                        foreach (var w in DirectionMethods.CARDINALS) {
                            if (Target.m_doorGrid[i, j][w] != null) newrt.constraints[i, j].optionalDoors[w] = true;
                        }
                    }
                }
            }

            string prefPath = PrefabUtility.GetPrefabAssetPathOfNearestInstanceRoot(Target.gameObject);
            if (string.IsNullOrWhiteSpace(prefPath)) prefPath = UnityEditor.SceneManagement.PrefabStageUtility.GetCurrentPrefabStage().assetPath;
            if (string.IsNullOrWhiteSpace(prefPath)) throw new Exception("Failed to create the type");

            var tmpPrefab = AssetDatabase.LoadAssetAtPath<GameObject>(prefPath);

            newrt.prefab = tmpPrefab;
            string defaultPath = EditorGUIHelper.GetFolderAlike("room");
            string savePath = EditorUtility.SaveFilePanelInProject(
                "Save RoomType",
                Target.name.Replace("Type", "").Trim() + " Type",
                "asset", "Save the asset",
                defaultPath
            );
            if (string.IsNullOrWhiteSpace(savePath)) return;

            AssetDatabase.CreateAsset(newrt, savePath);
            AssetDatabase.SaveAssets();

            PrefabUtility.SaveAsPrefabAsset(tmpPrefab, prefPath); // what a fucking shitfuckery
            Target.GetComponent<Room>().m_type = newrt;
            EditorUtility.SetDirty(Target);
        }
    }
#endif
}