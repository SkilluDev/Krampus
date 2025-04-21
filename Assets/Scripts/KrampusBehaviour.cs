#if UNITY_EDITOR
using NaughtyAttributes.Editor;
using UnityEditor;
#endif

using UnityEngine;

public class KrampusBehaviour : MonoBehaviour {
    public Krampus Kramp { get; private set; }
}


#if UNITY_EDITOR

[CustomEditor(typeof(KrampusBehaviour), true)]
public class KrampusBehaviourEditor : NaughtyInspector {
    private KrampusBehaviour Target => (KrampusBehaviour)target;
    public override void OnInspectorGUI() {
        if (Target.Kramp == null) {
            EditorGUILayout.HelpBox("Krampus not found!", MessageType.Error);
        } else {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Krampus", Target.Kramp, typeof(Krampus), false);
            EditorGUI.EndDisabledGroup();
        }
        base.OnInspectorGUI();
    }
}

#endif