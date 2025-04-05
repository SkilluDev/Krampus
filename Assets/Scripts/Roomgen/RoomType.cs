using UnityEngine;
using System;
using static QuadDirection;
using Unity.VisualScripting;
using System.Linq;


#if UNITY_EDITOR
using UnityEditor;
#endif

namespace Roomgen {
    [CreateAssetMenu(menuName = "Game/Room Type", fileName = "New Room Type")]
    public class RoomType : ScriptableObject {
        public Array2D<GridRoomConstraint> constraints = new Array2D<GridRoomConstraint>(1, 1);
        public GameObject prefab;
        public string note = "";
        public RoomType basedOn;
        public int gradeOffset;
        public int Width => constraints.Width;
        public int Height => constraints.Height;
        public int BaseGrade {
            get {
                int grd = Width * Height * 4;
                for (int i = 0; i < Width; i++) {
                    for (int j = 0; j < Height; j++) {
                        if (constraints[i, j] == null) {
                            grd -= 4; // same as 4 optional doors
                            continue;
                        }
                        grd -= constraints[i, j].optionalDoors.Count;
                    }
                }
                return grd;
            }
        }

        public int Grade {
            get => BaseGrade + gradeOffset;
        }

        public bool CanPlace(int x, int y, DoorFlags[,] grid, bool[,] occupied) {
            for (int i = 0; i < Width; i++) {
                for (int j = 0; j < Height; j++) {
                    if (constraints[i, j] == null) continue;
                    if (occupied[i + x, j + y] && !constraints[i, j].phantom) return false;
                    if (!constraints[i, j].CanPlace(grid[i + x, j + y])) return false;
                }
            }
            return true;
        }

        public void Rotate90Clockwise() {
            var old = constraints;
            constraints = new Array2D<GridRoomConstraint>(Height, Width);
            for (int i = 0; i < Width; i++) {
                for (int j = 0; j < Height; j++) {
                    constraints[i, j] = old[j, Width - 1 - i];
                    if (constraints[i, j] == null) continue;
                    constraints[i, j].requiredDoors = constraints[i, j].requiredDoors.Rotate90Clockwise();
                    constraints[i, j].optionalDoors = constraints[i, j].optionalDoors.Rotate90Clockwise();
                }
            }
        }

        public void Flip() {
            var old = constraints;
            constraints = new Array2D<GridRoomConstraint>(Width, Height);
            for (int i = 0; i < Width; i++) {
                for (int j = 0; j < Height; j++) {
                    constraints[i, j] = old[Width - 1 - i, j];
                    if (constraints[i, j] == null) continue;
                    constraints[i, j].requiredDoors = constraints[i, j].requiredDoors.InvertHorizontal();
                    constraints[i, j].optionalDoors = constraints[i, j].optionalDoors.InvertHorizontal();
                }
            }
        }

        public RoomType CreateInstance(int rotation) {
            var ni = Instantiate(this);
            var np = ni.prefab.GetComponent<Room>();
            for (int i = 0; i < rotation; i++) {
                np.Rotate90Clockwise();
                ni.Rotate90Clockwise();
            }
            ni.basedOn = this;
            np.m_type = ni;
            return ni;
        }
    }

#if UNITY_EDITOR
    // DISCLAIMER - this is UI code. It is not meant to be aellegant or fast - it just has to be convinient to use and error-proof

    [CustomEditor(typeof(RoomType))]
    public class RoomTypeEditor : Editor {
        internal bool m_showConstraints = true, m_showNote = false;
        private const int MARGIN = 8;

        private RoomType Target => (RoomType)target;

        private GUIStyle m_emptyCellStyle, m_filledCellStyle, m_doorButtonStyle, m_phantomCellStyle, m_errorLabelStyle;
        private Texture2D m_optionalDoorTex, m_requiredDoorTex, m_blockedDoorTex, m_emptyTex, m_deleteTex, m_phantomTex, m_stripes;

        private void Awake() {
            PrepareAssets();
        }

        private void OnDisable() {
            PrepareAssets();
        }

        private void PrepareAssets() {
            GUIStyleHelper.Drop();
            m_optionalDoorTex = Resources.Load<Texture2D>("EditorIcons/OptionalDoor");
            m_blockedDoorTex = Resources.Load<Texture2D>("EditorIcons/BlockedDoor");
            m_requiredDoorTex = Resources.Load<Texture2D>("EditorIcons/RequiredDoor");
            m_emptyTex = Resources.Load<Texture2D>("EditorIcons/RoomEmpty");
            m_deleteTex = Resources.Load<Texture2D>("EditorIcons/RoomDelete");
            m_phantomTex = Resources.Load<Texture2D>("EditorIcons/RoomPhantom");
            m_stripes = Resources.Load<Texture2D>("EditorIcons/BgStripes");
            m_emptyCellStyle = new GUIStyle() {
                normal = GUIStyleHelper.GetColored(new Color(0.1f, 0.1f, 0.1f))
            };
            m_filledCellStyle = new GUIStyle() {
                normal = GUIStyleHelper.GetColored(new Color(0.25f, 0.25f, 0.25f))
            };
            m_doorButtonStyle = new GUIStyle() {
                normal = GUIStyleHelper.GetColored(new Color(0, 0, 0, 0)),
                hover = GUIStyleHelper.GetColored(new Color(1, 1, 1, 0.1f)),
                active = GUIStyleHelper.GetColored(new Color(1, 1, 1, 0.05f)),
                alignment = TextAnchor.MiddleCenter
            };
            m_phantomCellStyle = new GUIStyle() {
                normal = new GUIStyleState() {
                    background = m_stripes
                }
            };
            m_errorLabelStyle = new GUIStyle(m_emptyCellStyle);
            m_errorLabelStyle.normal.textColor = Color.red;
            m_errorLabelStyle.alignment = TextAnchor.MiddleCenter;
        }

        private void DrawConstraintGrid() {
            int gw = Target.Width;
            int gh = Target.Height;

            var workArea = GUILayoutUtility.GetAspectRect(gw / (float)gh);

            workArea.x += 16;
            workArea.width -= 32;
            workArea.height -= 32;
            workArea.y += 16;
            if (workArea.width > gw * 128) {
                GUI.Label(workArea, "", m_emptyCellStyle);

                for (int i = 0; i < gw; i++) {
                    for (int j = 0; j < gh; j++) {
                        var rect = new Rect(
                            workArea.x + (i * workArea.width / gw) + MARGIN,
                            workArea.y + (j * workArea.height / gh) + MARGIN,
                            (workArea.width / gw) - (MARGIN * 2),
                            (workArea.height / gh) - (MARGIN * 2)
                        );

                        if (Target.constraints[i, j] == null) {
                            if (GUI.Button(rect, m_emptyTex, m_doorButtonStyle)) {
                                Target.constraints[i, j] = new GridRoomConstraint();
                                EditorUtility.SetDirty(Target);
                            }
                        } else {
                            DrawGridRoom(i, j, rect);
                        }

                    }
                }

            } else {
                GUI.Label(workArea, "Expand the window!", m_errorLabelStyle);
            }

            #region Editing buttons

            if (GUI.Button(new Rect(workArea.x, workArea.y - 16, 16, 16), Event.current.shift ? "-" : "+")) { //new row at the top
                var old = Target.constraints;
                if (Event.current.shift) {
                    if (Target.Height <= 1) return;
                    Target.constraints = new Array2D<GridRoomConstraint>(Target.Width, Target.Height - 1);
                    for (int i = 0; i < old.Width && i < Target.Width; i++) {
                        for (int j = 0; j < old.Height && j < Target.Height; j++) Target.constraints[i, j] = old[i, j + 1];
                    }
                } else {
                    if (Target.Height >= 9) return;
                    Target.constraints = new Array2D<GridRoomConstraint>(Target.Width, Target.Height + 1);
                    for (int i = 0; i < old.Width; i++) {
                        for (int j = 0; j < old.Height && j < Target.Height; j++) Target.constraints[i, j + 1] = old[i, j];
                    }
                }
            }



            if (GUI.Button(new Rect(workArea.x - 16, workArea.y, 16, 16), Event.current.shift ? "-" : "+")) { //new column on the left
                var old = Target.constraints;
                if (Event.current.shift) {
                    if (Target.Width <= 1) return;
                    Target.constraints = new Array2D<GridRoomConstraint>(Target.Width - 1, Target.Height);
                    for (int i = 0; i < old.Width && i < Target.Width; i++) {
                        for (int j = 0; j < old.Height && j < Target.Height; j++) Target.constraints[i, j] = old[i + 1, j];
                    }
                } else {
                    if (Target.Width >= 9) return;
                    Target.constraints = new Array2D<GridRoomConstraint>(Target.Width + 1, Target.Height);
                    for (int i = 0; i < old.Width; i++) {
                        for (int j = 0; j < old.Height && j < Target.Height; j++) Target.constraints[i + 1, j] = old[i, j];
                    }
                }
            }

            if (GUI.Button(new Rect(workArea.x + workArea.width - 16, workArea.y + workArea.height, 16, 16), Event.current.shift ? "-" : "+")) { // new row at the bottom
                var old = Target.constraints;
                if (Event.current.shift) {
                    if (Target.Height <= 1) return;
                    Target.constraints = new Array2D<GridRoomConstraint>(Target.Width, Target.Height - 1);
                } else {
                    if (Target.Height >= 9) return;
                    Target.constraints = new Array2D<GridRoomConstraint>(Target.Width, Target.Height + 1);
                }
                for (int i = 0; i < old.Width && i < Target.Width; i++) {
                    for (int j = 0; j < old.Height && j < Target.Height; j++) Target.constraints[i, j] = old[i, j];
                }
            }

            if (GUI.Button(new Rect(workArea.x + workArea.width, workArea.y + workArea.height - 16, 16, 16), Event.current.shift ? "-" : "+")) { // new column on the right
                var old = Target.constraints;
                if (Event.current.shift) {
                    if (Target.Width <= 1) return;
                    Target.constraints = new Array2D<GridRoomConstraint>(Target.Width - 1, Target.Height);
                } else {
                    if (Target.Width >= 9) return;
                    Target.constraints = new Array2D<GridRoomConstraint>(Target.Width + 1, Target.Height);
                }
                for (int i = 0; i < old.Width && i < Target.Width; i++) {
                    for (int j = 0; j < old.Height && j < Target.Height; j++) Target.constraints[i, j] = old[i, j];
                }
            }


            if (GUI.Button(new Rect(workArea.x - 16, workArea.y + workArea.height, 16, 16), Event.current.shift ? "M" : "R")) { //rotate
                if (Event.current.shift) Target.Flip();
                else Target.Rotate90Clockwise();
            }


            #endregion

            EditorGUILayout.HelpBox("Click on the empty (+) cells to create Room cells.\nClicking the (ghost) makes the cell Phantom - its constraints need to be met but it is not a part of the room.\nTo switch a cell back to a Room cell, click on the (+) in a Phantom cell.\nTo delete a cell, hold shift and click the (trashcan).\n\nClick on the icons to toggle door types.\nGreen = required door; Red = no door; Yellow = optional door;\nShift-clicking prevents affecting neighbours.\nThe [+] buttons in the corners can be used to increase the constraint area.\nShift-clicking [+] changes it to a [-] and reduces the area.\nTo rotate, use the [R] in the corner. Shift-clicking it mirrors the room.", MessageType.Info);

            if (GUILayout.Button("Reset Constraints") && EditorUtility.DisplayDialog("Reset?", "Are you sure to delete all the constraints and start from scratch?", "Yes", "Nah")) {
                Target.constraints = new Array2D<GridRoomConstraint>(1, 1);
                EditorUtility.SetDirty(Target);
            }
            GUILayout.Space(20);
        }

        private void DrawGradeField() {
            GUILayout.BeginHorizontal();
            EditorGUILayout.LabelField("Grade");
            if (GUILayout.Button("-")) Target.gradeOffset--;
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField($"{Target.BaseGrade} {Target.gradeOffset:+ #;- #;+ 0} = {Target.Grade}");
            EditorGUI.EndDisabledGroup();
            if (GUILayout.Button("+")) Target.gradeOffset++;
            GUILayout.EndHorizontal();
            EditorGUILayout.HelpBox("The room's grade determines how complex it is to place. Rooms with higher grade get placed first, as they fit a smaller amount of cases.", MessageType.Info);
        }

        private void DrawNoteField() {
            Target.note = EditorGUILayout.TextArea(Target.note);
        }

        private void DrawPrefabField(bool inPrefab = false) {
            EditorGUI.BeginDisabledGroup(inPrefab);
            GUILayout.BeginHorizontal();
            Target.prefab = (GameObject)EditorGUILayout.ObjectField("Prefab", Target.prefab, typeof(GameObject), false);
            if (Target.prefab != null && !inPrefab && GUILayout.Button("Edit")) LoadPrefabEditing();
            GUILayout.EndHorizontal();
            EditorGUI.EndDisabledGroup();


            var roomScript = Target.prefab == null ? null : Target.prefab.GetComponent<Room>(); // not ideal but whatevs

            if (roomScript == null) {
                if (!inPrefab) {
                    if (HelpBoxWithButton("No prefab is assigned!", "Create", MessageType.Error))
                        CreateRoomPrefab();
                } else {
                    EditorGUILayout.HelpBox("No prefab is assigned!", MessageType.Error);
                }
            } else if (Event.current.shift) {
                GUILayout.Label("Warning - touch only if you know what you're doing!", EditorStyles.centeredGreyMiniLabel);
                GUILayout.BeginHorizontal();
                if (!inPrefab && GUILayout.Button("New")) CreateRoomPrefab();
                if (GUILayout.Button("Regenerate")) UpdateRoomPrefabComplete();
                if (GUILayout.Button("Update")) UpdateRoomPrefabDoors();
                GUILayout.EndHorizontal();
            } else if (RoomPrefabEditor.CheckFloorObsolete(roomScript, Target)) {
                if (HelpBoxWithButton("Prefab needs a regeneration!", "Regenerate", MessageType.Warning)) UpdateRoomPrefabComplete();
            } else if (RoomPrefabEditor.CheckDoorsObsolete(roomScript, Target)) {
                if (HelpBoxWithButton("Prefab needs an update!", "Update", MessageType.Warning)) UpdateRoomPrefabDoors();
            } else {
                if (!inPrefab) {
                    EditorGUILayout.HelpBox("What is actually placed when generating a room.", MessageType.Info);
                } else {
                    EditorGUILayout.HelpBox("Prefab is up to date with the layout.", MessageType.Info);
                }
            }
        }

        /// <summary>
        /// Draws default inspector
        /// </summary>
        public override void OnInspectorGUI() {
            if (Target.constraints == null) Target.constraints = new Array2D<GridRoomConstraint>(1, 1);

            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.TextField("Name", Target.name);
            EditorGUI.EndDisabledGroup();

            m_showConstraints = EditorGUILayout.BeginFoldoutHeaderGroup(m_showConstraints, $"Constraints ({Target.Width} x {Target.Height})");
            if (m_showConstraints) {
                DrawConstraintGrid();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            m_showNote = EditorGUILayout.BeginFoldoutHeaderGroup(
                m_showNote,
                "Note"
                    + (string.IsNullOrWhiteSpace(Target.note) ? "" : ": ")
                    + Target.note.Truncate(32).Replace("\n", " ")
            );
            if (m_showNote) {
                DrawNoteField();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            DrawGradeField();

            GUILayout.Space(10);
            DrawPrefabField();
        }

        /// <summary>
        /// Draws the inspector for the Prefab edior variant
        /// </summary>
        public void OnPrefabInspectorGUI() {
            if (Target.constraints == null) Target.constraints = new Array2D<GridRoomConstraint>(1, 1);
            DrawPrefabField(true);

            m_showConstraints = EditorGUILayout.BeginFoldoutHeaderGroup(m_showConstraints, $"Constraints ({Target.Width} x {Target.Height})");
            if (m_showConstraints) {
                DrawConstraintGrid();
            }
            EditorGUILayout.EndFoldoutHeaderGroup();

            if (!string.IsNullOrWhiteSpace(Target.note)) {
                EditorGUILayout.HelpBox(Target.note, MessageType.None);
            }
        }

        // some magic i wrote
        private static bool HelpBoxWithButton(string msg, string btn, MessageType t) {
            var icon = new GUIContent(EditorGUIUtility.IconContent(t switch { MessageType.Info => "console.infoicon", MessageType.Warning => "console.warnicon", MessageType.Error => "console.erroricon", _ => "" })) { text = msg };
            EditorGUILayout.LabelField(GUIContent.none, icon, EditorStyles.helpBox);
            var rec = GUILayoutUtility.GetLastRect();
            var dims = EditorStyles.objectField.CalcSize(new GUIContent(btn));
            rec.xMin = rec.xMax - dims.x; rec.yMin += (rec.height - dims.y) / 2; rec.yMax -= (rec.height - dims.y) / 2; rec.x -= 8;
            return GUI.Button(rec, new GUIContent(btn));
        }

        private void LoadPrefabEditing() {
            if (Target.prefab == null) return;
            AssetDatabase.OpenAsset(Target.prefab);
        }

        private void UpdateRoomPrefabComplete() {
            var editor = new RoomPrefabEditor(Target, Target.prefab);
            editor.RecreateDoorGrid();
            UpdateFloorDialog(editor);
            editor.UpdateDoorGroups();
            Target.prefab = editor.ApplyAndSave();
        }

        private void UpdateRoomPrefabDoors() {
            var editor = new RoomPrefabEditor(Target, Target.prefab);
            editor.UpdateDoorGroups();
            Target.prefab = editor.ApplyAndSave();
        }

        private void CreateRoomPrefab() {
            string defaultPath = System.IO.Directory.GetDirectories(Application.dataPath).FirstOrDefault(w => w.Contains("room", StringComparison.InvariantCultureIgnoreCase));
            if (string.IsNullOrWhiteSpace(defaultPath)) defaultPath = "Assets/";
            string savePath = EditorUtility.SaveFilePanelInProject(
                "Save prefab",
                Target.name.Replace("Type", "").Trim(),
                "prefab", "Save the prefab",
                defaultPath
            );
            if (string.IsNullOrWhiteSpace(savePath)) return;

            var editor = new RoomPrefabEditor(Target, savePath);
            editor.RecreateDoorGrid();
            UpdateFloorDialog(editor);
            Target.prefab = editor.ApplyAndSave();
        }

        private void UpdateFloorDialog(RoomPrefabEditor editor) {
            var mesh = editor.CreateFloorMesh();
            if (AssetDatabase.Contains(mesh)) {
                AssetDatabase.SaveAssets();
            } else {
                string nextToPath = editor.PrefabPath.Replace(Application.dataPath, "Assets/").Replace(".prefab", "");
                string savePath = EditorUtility.SaveFilePanelInProject(
                    "Save mesh",
                    Target.name.Replace("Type", "").Trim(),
                    "asset",
                    "Save the floor mesh",
                    nextToPath
                );

                if (string.IsNullOrWhiteSpace(savePath)) {
                    EditorUtility.DisplayDialog("Warning", "A new mesh was created but not saved. Shit >will< break. Try regenerating a new one if things go wrong", "Fine");
                    return;
                }

                AssetDatabase.CreateAsset(mesh, savePath);
                AssetDatabase.SaveAssets();
            }
        }


        private Texture2D TextureDoorForCellDirection(GridRoomConstraint c, QuadDirection dir) {
            if (c.requiredDoors[dir]) return m_requiredDoorTex;
            else if (c.optionalDoors[dir]) return m_optionalDoorTex;
            else return m_blockedDoorTex;
        }

        private void SetDoorForCellDirection(GridRoomConstraint c, QuadDirection dir, bool r, bool o) {
            if (r) o = false;
            if (o) r = false;
            c.optionalDoors[dir] = o;
            c.requiredDoors[dir] = r;
        }

        private void ToggleDoorDirection(int i, int j, QuadDirection dir) {
            var c = Target.constraints[i, j];
            var n = dir switch {
                NORTH => j > 0 ? Target.constraints[i, j - 1] : null, // up
                EAST => i < Target.Width - 1 ? Target.constraints[i + 1, j] : null, // right
                SOUTH => j < Target.Height - 1 ? Target.constraints[i, j + 1] : null, //down
                WEST => i > 0 ? Target.constraints[i - 1, j] : null, // left
                _ => throw new Exception("cannot set directions like this")
            };


            if (c.requiredDoors[dir]) {
                SetDoorForCellDirection(c, dir, false, true);
                if (!Event.current.shift && n != null) SetDoorForCellDirection(n, dir.Invert(), false, true);
            } else if (c.optionalDoors[dir]) {
                SetDoorForCellDirection(c, dir, false, false);
                if (!Event.current.shift && n != null) SetDoorForCellDirection(n, dir.Invert(), false, false);
            } else {
                SetDoorForCellDirection(c, dir, true, false);
                if (!Event.current.shift && n != null) SetDoorForCellDirection(n, dir.Invert(), true, false);
            }

            EditorUtility.SetDirty(Target);
        }

        private void DrawGridRoom(int i, int j, Rect at) {
            int sNormal = (int)(64f * Mathf.Min(at.width / 256, 1));
            int sSmall = sNormal / 2;

            if (Target.constraints[i, j].phantom) {
                GUI.Label(at, "", m_phantomCellStyle);
            } else {
                GUI.Label(at, "", m_filledCellStyle);

                #region Drawing rooms
                // draw the additional fills to make the rooms look more coherent
                if (i < Target.Width - 1 && Target.constraints[i + 1, j] != null && !Target.constraints[i + 1, j].phantom && Target.constraints[i + 1, j].requiredDoors.West && Target.constraints[i, j].requiredDoors.East)
                    GUI.Label(new Rect(at.x + at.width, at.y, MARGIN, at.height), "", m_filledCellStyle);
                if (j < Target.Height - 1 && Target.constraints[i, j + 1] != null && !Target.constraints[i, j + 1].phantom && Target.constraints[i, j + 1].requiredDoors.North && Target.constraints[i, j].requiredDoors.South)
                    GUI.Label(new Rect(at.x, at.y + at.height, at.width, MARGIN), "", m_filledCellStyle);
                if (i > 0 && Target.constraints[i - 1, j] != null && !Target.constraints[i - 1, j].phantom && Target.constraints[i - 1, j].requiredDoors.East && Target.constraints[i, j].requiredDoors.West)
                    GUI.Label(new Rect(at.x - MARGIN, at.y, MARGIN, at.height), "", m_filledCellStyle);
                if (j > 0 && Target.constraints[i, j - 1] != null && !Target.constraints[i, j - 1].phantom && Target.constraints[i, j - 1].requiredDoors.South && Target.constraints[i, j].requiredDoors.North)
                    GUI.Label(new Rect(at.x, at.y - MARGIN, at.width, MARGIN), "", m_filledCellStyle);

                // add additional fills for doors
                if (Target.constraints[i, j].requiredDoors.West || Target.constraints[i, j].optionalDoors.West)
                    GUI.Label(new Rect(at.x - MARGIN, at.y + at.height / 2 - sSmall, MARGIN, sNormal), "", m_filledCellStyle);

                if (Target.constraints[i, j].requiredDoors.North || Target.constraints[i, j].optionalDoors.North)
                    GUI.Label(new Rect(at.x + at.width / 2 - sSmall, at.y - MARGIN, sNormal, MARGIN), "", m_filledCellStyle);

                if (Target.constraints[i, j].requiredDoors.East || Target.constraints[i, j].optionalDoors.East)
                    GUI.Label(new Rect(at.x + at.width, at.y + at.height / 2 - sSmall, MARGIN, sNormal), "", m_filledCellStyle);

                if (Target.constraints[i, j].requiredDoors.South || Target.constraints[i, j].optionalDoors.South)
                    GUI.Label(new Rect(at.x + at.width / 2 - sSmall, at.y + at.height, sNormal, MARGIN), "", m_filledCellStyle);

                // holy mother of christ DO NOT ASK ME ABOUT THIS, it makes it so there are no black dots in the middle
                if ((j > 0 && Target.constraints[i, j - 1] != null && !Target.constraints[i, j - 1].phantom && Target.constraints[i, j - 1].requiredDoors.South && Target.constraints[i, j - 1].requiredDoors.West && Target.constraints[i, j].requiredDoors.North) && (i > 0 && Target.constraints[i - 1, j] != null && !Target.constraints[i - 1, j].phantom && Target.constraints[i - 1, j].requiredDoors.East && Target.constraints[i - 1, j].requiredDoors.North && Target.constraints[i, j].requiredDoors.West) && (Target.constraints[i - 1, j - 1] != null && !Target.constraints[i - 1, j - 1].phantom && Target.constraints[i - 1, j - 1].requiredDoors.South && Target.constraints[i - 1, j - 1].requiredDoors.East))
                    GUI.Label(new Rect(at.x - MARGIN * 2, at.y - MARGIN * 2, MARGIN * 2, MARGIN * 2), "", m_filledCellStyle);
                #endregion
            }

            #region Door buttons

            if (GUI.Button(
                new Rect(at.x + (at.width - sNormal) / 2, at.y, sNormal, sNormal),
                TextureDoorForCellDirection(Target.constraints[i, j], NORTH), m_doorButtonStyle)
            ) {
                ToggleDoorDirection(i, j, NORTH);
            }

            GUIUtility.RotateAroundPivot(180, new Vector2(at.x + (at.width - sNormal) / 2, at.y + at.height - sNormal) + Vector2.one * sSmall);

            if (GUI.Button(
                new Rect(at.x + (at.width - sNormal) / 2, at.y + at.height - sNormal, sNormal, sNormal),
                TextureDoorForCellDirection(Target.constraints[i, j], SOUTH), m_doorButtonStyle)
            ) {
                ToggleDoorDirection(i, j, SOUTH);
            }

            GUIUtility.RotateAroundPivot(180, new Vector2(at.x + (at.width - sNormal) / 2, at.y + at.height - sNormal) + Vector2.one * sSmall);


            GUIUtility.RotateAroundPivot(270, new Vector2(at.x, at.y + (at.height - sNormal) / 2) + Vector2.one * sSmall);

            if (GUI.Button(
                new Rect(at.x, at.y + (at.height - sNormal) / 2, sNormal, sNormal),
                TextureDoorForCellDirection(Target.constraints[i, j], WEST), m_doorButtonStyle)
            ) {
                ToggleDoorDirection(i, j, WEST);
            }
            GUIUtility.RotateAroundPivot(90, new Vector2(at.x, at.y + (at.height - sNormal) / 2) + Vector2.one * sSmall);


            GUIUtility.RotateAroundPivot(90, new Vector2(at.x + at.width - sNormal, at.y + (at.height - sNormal) / 2) + Vector2.one * sSmall);

            if (GUI.Button(
                new Rect(at.x + at.width - sNormal, at.y + (at.height - sNormal) / 2, sNormal, sNormal),
                TextureDoorForCellDirection(Target.constraints[i, j], EAST), m_doorButtonStyle)
            ) {
                ToggleDoorDirection(i, j, EAST);
            }

            GUIUtility.RotateAroundPivot(270, new Vector2(at.x + at.width - sNormal, at.y + (at.height - sNormal) / 2) + Vector2.one * sSmall);

            #endregion

            if (Target.constraints[i, j].phantom) {
                if (GUI.Button(new Rect(at.x + at.width / 2 - sSmall / 2, at.y + at.height / 2 - sSmall / 2, sSmall, sSmall), Event.current.shift ? m_deleteTex : m_emptyTex, m_doorButtonStyle)) {
                    if (Event.current.shift)
                        Target.constraints[i, j] = null;
                    else
                        Target.constraints[i, j].phantom = false;
                }
            } else {
                if (GUI.Button(new Rect(at.x + at.width / 2 - sSmall / 2, at.y + at.height / 2 - sSmall / 2, sSmall, sSmall), Event.current.shift ? m_deleteTex : m_phantomTex, m_doorButtonStyle)) {
                    if (Event.current.shift)
                        Target.constraints[i, j] = null;
                    else
                        Target.constraints[i, j].phantom = true;
                }
            }
        }

    }


#endif
}