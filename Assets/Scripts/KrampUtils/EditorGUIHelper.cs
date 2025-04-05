#if UNITY_EDITOR

using System.Collections.Generic;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace KrampUtils {
    public static class EditorGUIHelper {
        private static Dictionary<Color, GUIStyleState> m_bgColors = new Dictionary<Color, GUIStyleState>();

        public static GUIStyleState GetColored(Color c) {
            if (m_bgColors.ContainsKey(c)) return m_bgColors[c];

            var tex = new Texture2D(1, 1);
            tex.SetPixel(0, 0, c);
            tex.Apply();
            m_bgColors.Add(c, new GUIStyleState() {
                background = tex
            });

            return GetColored(c);
        }

        // some magic i wrote
        public static bool HelpBoxWithButton(string msg, string btn, MessageType t) {
            var icon = new GUIContent(EditorGUIUtility.IconContent(t switch { MessageType.Info => "console.infoicon", MessageType.Warning => "console.warnicon", MessageType.Error => "console.erroricon", _ => "" })) { text = msg };
            EditorGUILayout.LabelField(GUIContent.none, icon, EditorStyles.helpBox);
            var rec = GUILayoutUtility.GetLastRect();
            var dims = EditorStyles.objectField.CalcSize(new GUIContent(btn));
            rec.xMin = rec.xMax - dims.x; rec.yMin += (rec.height - dims.y) / 2; rec.yMax -= (rec.height - dims.y) / 2; rec.x -= 8;
            return GUI.Button(rec, new GUIContent(btn));
        }

        public static string GetFolderAlike(string what) {
            string folder = System.IO.Directory.GetDirectories(Application.dataPath).FirstOrDefault(w => w.Contains(what, System.StringComparison.InvariantCultureIgnoreCase));
            if (string.IsNullOrWhiteSpace(folder)) folder = "Assets/";
            return folder;
        }

        internal static void Drop() => m_bgColors.Clear();
    }
}

#endif