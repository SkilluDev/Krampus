
using System;
using System.Collections.Generic;
using UnityEngine;
public static class GUIStyleHelper {
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

    internal static void Drop() => m_bgColors.Clear();
}