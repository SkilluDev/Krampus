#if UNITY_EDITOR
using KrampUtils;
using UnityEditor;
#endif

using UnityEngine;

public abstract class KrampusBehaviour : MonoBehaviour {
    public Krampus Kramp { get; private set; }
}


#if UNITY_EDITOR

[CustomEditor(typeof(KrampusBehaviour), true)]
public class KrampusBehaviourEditor : Editor {
    private KrampusBehaviour Target => (KrampusBehaviour)target;
    public override void OnInspectorGUI() {
        if (Target.Kramp == null) {
            if (EditorGUIHelper.HelpBoxWithButton("Krampus not found!", "Try again", MessageType.Error)) {
                typeof(KrampusBehaviour).GetProperty(nameof(Target.Kramp)).SetValue(Target, Target.gameObject.GetComponentInParent<Krampus>());
            }
        } else {
            EditorGUI.BeginDisabledGroup(true);
            EditorGUILayout.ObjectField("Krampus", Target.Kramp, typeof(Krampus), false);
            EditorGUI.EndDisabledGroup();
        }
        base.OnInspectorGUI();
    }
}

#endif