using System.Collections.Generic;
#if UNITY_EDITOR
using UnityEditor;
#endif
using UnityEngine;
using UnityEngine.Events;

namespace Roomgen {
    [ExecuteInEditMode]
    public class PropGroup : MonoBehaviour {
        public UnityEvent onEnabled;
        public UnityEvent onDisabled;

        [SerializeField] private List<GameObject> m_disableList = new List<GameObject>();
        [SerializeField][HideInInspector] private Room m_room;


        private bool m_generated = false;
        [SerializeField] private Color m_gizmoColor;


        // Start and update are edit-mode only!
        private void Start() {
            m_gizmoColor = Color.HSVToRGB(Random.Range(0, 36) * 10f / 360f, Random.Range(5, 10) / 10.0f, 0.6f);
            m_gizmoColor.a = 0.7f;
        }

        private void Update() {
            for (int i = 0; i < transform.childCount; i++) {
                var childObject = transform.GetChild(i).gameObject;
                childObject.transform.SetParent(m_room.transform);
                Debug.LogWarning($"A PropGroup should not contain {childObject.name}. Object was added to the disable list, however it was not parented");
                AddToDisableList(childObject);
            }
        }

        // Probably wont get used but good to have
        public void AddToDisableList(GameObject go) {
            if (!m_disableList.Contains(go)) m_disableList.Add(go);
            else Debug.LogWarning($"Cannot add {go.name} to the disable list as it is already there!");
        }

        public void RemoveFromDisableList(GameObject go) {
            if (m_disableList.Contains(go)) m_disableList.Remove(go);
            else Debug.LogWarning($"Cannot remove {go.name} from the disable list as it is not there!");
        }

        public static PropGroup CreatePropGroup(string name, Vector3 position, Room room, params GameObject[] disableList) {
            var obj = new GameObject(name);
            var c = obj.AddComponent<PropGroup>();
            c.m_room = room;
            obj.transform.position = position;
            obj.transform.SetParent(room.transform);
            foreach (var go in disableList) c.AddToDisableList(go);
            return c;
        }


        public void SetState(bool enabled) {
            if (m_generated) throw new System.Exception("Attempting to regenerate a generated door");

            if (!enabled) {
                foreach (var go in m_disableList) {
                    if (go == null) continue;
                    go.SetActive(false);
                }
            }

            if (enabled) {
                onEnabled.Invoke();
            } else {
                onDisabled.Invoke();
            }

            gameObject.SetActive(enabled);
            m_generated = true;
        }

        #region Gizmos

#if UNITY_EDITOR

        private static GUIStyle m_tinylbl;

        private void OnDrawGizmosSelected() {
            Gizmos.color = m_gizmoColor;
            foreach (var go in m_disableList) {
                if (go == null) continue;
                var renderer = go.GetComponent<Renderer>();
                var collider = go.GetComponent<Collider>();
                if (collider != null) {
                    Gizmos.DrawCube(collider.bounds.center, collider.bounds.extents * 2.001f);
                } else if (renderer != null) {
                    Gizmos.DrawCube(renderer.bounds.center, renderer.bounds.extents * 2.001f);
                } else {
                    Gizmos.DrawSphere(go.transform.position, 0.2f);
                }
            }
        }

        private void OnDrawGizmos() {
            // this cannot be set in a constructor
            m_tinylbl ??= new GUIStyle(EditorStyles.miniLabel) { fontSize = 8, normal = new GUIStyleState() { textColor = new Color(1, 1, 1, 0.5f) } };

            Gizmos.color = m_gizmoColor;
            if (SceneView.currentDrawingSceneView != null && Vector3.Distance(SceneView.currentDrawingSceneView.camera.transform.position, transform.position) < 30)
                Handles.Label(transform.position + Vector3.up, $"\t{gameObject.name}\n\t{m_disableList.Count} obj", m_tinylbl);
            Gizmos.DrawCube(transform.position, Vector3.one * Room.DOOR_SIZE);
            foreach (var go in m_disableList) {
                if (go == null) continue;
                var renderer = go.GetComponent<Renderer>();
                var collider = go.GetComponent<Collider>();
                if (collider != null) {
                    Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.extents * 2.001f);
                } else if (renderer != null) {
                    Gizmos.DrawWireCube(renderer.bounds.center, renderer.bounds.extents * 2.001f);
                } else {
                    Gizmos.DrawSphere(go.transform.position, 0.2f);
                }

                if (Selection.activeGameObject == go) {
                    Handles.Label(Vector3.Lerp(go.transform.position, transform.position, 0.3f), $"Linked to {gameObject.name}", m_tinylbl);
                    Gizmos.DrawLine(go.transform.position, transform.position);
                }
            }
        }

#endif
        #endregion
    }

}