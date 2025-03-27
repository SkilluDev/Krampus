using UnityEngine;
using System;
using static QuadDirection;

#if UNITY_EDITOR
using UnityEditor;
#endif

[CreateAssetMenu(menuName = "Game/Room Type", fileName = "New Room Type")]
public class RoomType : ScriptableObject, ISerializationCallbackReceiver {
    public GridRoomConstraint[,] constraints = new GridRoomConstraint[1, 1];
    public int gradeOffset;
    public int Width => constraints.GetLength(0);
    public int Height => constraints.GetLength(1);
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

    public bool CanPlace(int x, int y, DoorFlags[,] grid) {
        for (int i = 0; i < Width; i++) {
            for (int j = 0; j < Height; j++) {
                if (constraints[i, j] == null) continue;
                if (!constraints[i, j].CanPlace(grid[i + x, j + y])) return false;
            }
        }
        return true;
    }

    public void Rotate90Clockwise() {
        var old = constraints;
        constraints = new GridRoomConstraint[Height, Width];
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
        constraints = new GridRoomConstraint[Width, Height];
        for (int i = 0; i < Width; i++) {
            for (int j = 0; j < Height; j++) {
                constraints[i, j] = old[Width - 1 - i, j];
                if (constraints[i, j] == null) continue;
                constraints[i, j].requiredDoors = constraints[i, j].requiredDoors.InvertHorizontal();
                constraints[i, j].optionalDoors = constraints[i, j].optionalDoors.InvertHorizontal();
            }
        }
    }

    #region Serializer bullshit
    [SerializeField] private NullableSerializationContainer<GridRoomConstraint>[] m_constraints;
    [SerializeField] private int m_width, m_height;

    public void OnAfterDeserialize() {
        constraints = new GridRoomConstraint[m_width, m_height];
        for (int i = 0; i < m_width; i++) {
            for (int j = 0; j < m_height; j++) {
                constraints[i, j] = m_constraints[i * m_height + j].hasValue ? m_constraints[i * m_height + j].value : null;
            }
        }
    }

    public void OnBeforeSerialize() {
        m_width = constraints.GetLength(0);
        m_height = constraints.GetLength(1);
        m_constraints = new NullableSerializationContainer<GridRoomConstraint>[m_width * m_height];
        for (int i = 0; i < m_width; i++) {
            for (int j = 0; j < m_height; j++) {
                m_constraints[i * m_height + j] = new NullableSerializationContainer<GridRoomConstraint>(constraints[i, j]);
            }
        }
    }
    #endregion
}

#if UNITY_EDITOR // custom editor

[CustomEditor(typeof(RoomType))]
public class RoomTypeEditor : Editor {
    private const int MARGIN = 10;

    private RoomType Target => (RoomType)target;

    private GUIStyle m_emptyCellStyle, m_filledCellStyle, m_doorButtonStyle, m_phantomCellStyle, m_errorLabelStyle;
    private Texture2D m_optionalDoorTex, m_requiredDoorTex, m_blockedDoorTex, m_emptyTex, m_deleteTex, m_phantomTex, m_stripes;
    private bool m_showConstraints = true;

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

    public override void OnInspectorGUI() {
        if (Target.constraints == null) Target.constraints = new GridRoomConstraint[1, 1];

        int gw = Target.Width;
        int gh = Target.Height;

        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextField("Name", Target.name);
        EditorGUI.EndDisabledGroup();

        m_showConstraints = EditorGUILayout.BeginFoldoutHeaderGroup(m_showConstraints, $"Constraints ({Target.Width} x {Target.Height})");
        if (m_showConstraints) {
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
                    Target.constraints = new GridRoomConstraint[Target.Width, Target.Height - 1];
                    for (int i = 0; i < old.GetLength(0) && i < Target.Width; i++) {
                        for (int j = 0; j < old.GetLength(1) && j < Target.Height; j++) Target.constraints[i, j] = old[i, j + 1];
                    }
                } else {
                    if (Target.Height >= 9) return;
                    Target.constraints = new GridRoomConstraint[Target.Width, Target.Height + 1];
                    for (int i = 0; i < old.GetLength(0); i++) {
                        for (int j = 0; j < old.GetLength(1) && j < Target.Height; j++) Target.constraints[i, j + 1] = old[i, j];
                    }
                }
            }



            if (GUI.Button(new Rect(workArea.x - 16, workArea.y, 16, 16), Event.current.shift ? "-" : "+")) { //new column on the left
                var old = Target.constraints;
                if (Event.current.shift) {
                    if (Target.Width <= 1) return;
                    Target.constraints = new GridRoomConstraint[Target.Width - 1, Target.Height];
                    for (int i = 0; i < old.GetLength(0) && i < Target.Width; i++) {
                        for (int j = 0; j < old.GetLength(1) && j < Target.Height; j++) Target.constraints[i, j] = old[i + 1, j];
                    }
                } else {
                    if (Target.Width >= 9) return;
                    Target.constraints = new GridRoomConstraint[Target.Width + 1, Target.Height];
                    for (int i = 0; i < old.GetLength(0); i++) {
                        for (int j = 0; j < old.GetLength(1) && j < Target.Height; j++) Target.constraints[i + 1, j] = old[i, j];
                    }
                }
            }

            if (GUI.Button(new Rect(workArea.x + workArea.width - 16, workArea.y + workArea.height, 16, 16), Event.current.shift ? "-" : "+")) { // new row at the bottom
                var old = Target.constraints;
                if (Event.current.shift) {
                    if (Target.Height <= 1) return;
                    Target.constraints = new GridRoomConstraint[Target.Width, Target.Height - 1];
                } else {
                    if (Target.Height >= 9) return;
                    Target.constraints = new GridRoomConstraint[Target.Width, Target.Height + 1];
                }
                for (int i = 0; i < old.GetLength(0) && i < Target.Width; i++) {
                    for (int j = 0; j < old.GetLength(1) && j < Target.Height; j++) Target.constraints[i, j] = old[i, j];
                }
            }

            if (GUI.Button(new Rect(workArea.x + workArea.width, workArea.y + workArea.height - 16, 16, 16), Event.current.shift ? "-" : "+")) { // new column on the right
                var old = Target.constraints;
                if (Event.current.shift) {
                    if (Target.Height <= 0) return;
                    Target.constraints = new GridRoomConstraint[Target.Width - 1, Target.Height];
                } else {
                    if (Target.Width >= 9) return;
                    Target.constraints = new GridRoomConstraint[Target.Width + 1, Target.Height];
                }
                for (int i = 0; i < old.GetLength(0) && i < Target.Width; i++) {
                    for (int j = 0; j < old.GetLength(1) && j < Target.Height; j++) Target.constraints[i, j] = old[i, j];
                }
            }


            if (GUI.Button(new Rect(workArea.x - 16, workArea.y + workArea.height, 16, 16), Event.current.shift ? "M" : "R")) { //rotate
                if (Event.current.shift) Target.Flip();
                else Target.Rotate90Clockwise();
            }


            #endregion

            EditorGUILayout.HelpBox("Click on the empty (+) cells to create Room cells.\nClicking the (ghost) makes the cell Phantom - its constraints need to be met but it is not a part of the room.\nTo switch a cell back to a Room cell, click on the (+) in a Phantom cell.\nTo delete a cell, hold shift and click the (trashcan).\n\nClick on the icons to toggle door types.\nGreen = required door; Red = no door; Yellow = optional door;\nShift-clicking prevents affecting neighbours.\nThe [+] buttons in the corners can be used to increase the constraint area.\nShift-clicking [+] changes it to a [-] and reduces the area.\nTo rotate, use the [R] in the corner. Shift-clicking it mirrors the room.", MessageType.Info);

            if (GUILayout.Button("Reset Constraints") && EditorUtility.DisplayDialog("Reset?", "Are you sure to delete all the constraints and start from scratch?", "Yes", "Nah")) {
                Target.constraints = new GridRoomConstraint[1, 1];
                EditorUtility.SetDirty(Target);
            }
            GUILayout.Space(20);
        }

        EditorGUILayout.EndFoldoutHeaderGroup();


        EditorGUILayout.HelpBox("The room's grade determines how complex it is to place. Rooms with higher grade get placed first, as they fit a smaller amount of cases.", MessageType.Info);
        GUILayout.BeginHorizontal();
        EditorGUILayout.LabelField("Grade");
        if (GUILayout.Button("-")) Target.gradeOffset--;
        EditorGUI.BeginDisabledGroup(true);
        EditorGUILayout.TextField($"{Target.BaseGrade} {Target.gradeOffset:+ #;- #;+ 0} = {Target.Grade}");
        EditorGUI.EndDisabledGroup();
        if (GUILayout.Button("+")) Target.gradeOffset++;
        GUILayout.EndHorizontal();
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