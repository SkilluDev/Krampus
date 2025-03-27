using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class RoomDoorGroup : MonoBehaviour {
    public UnityAction onGenerateWith;
    public UnityAction onGenerateWithout;
    private bool m_generated = false;

    [SerializeField] private List<GameObject> m_disableList;

    private Color m_gizmoColor;


    // Start and update are edit-mode only!
    private void Start() {
        m_gizmoColor = Color.HSVToRGB(Random.Range(0, 36) * 10, Random.Range(5, 10) / 10.0f, 0.8f);
        m_gizmoColor.a = 0.2f;
    }

    private void Update() {
        for (int i = 0; i < transform.childCount; i++) {
            var childObject = transform.GetChild(i).gameObject;
            Debug.LogWarning($"A RoomDoorGroup should not contain {childObject.name}. Object was added to the disable list, however it was not parented");
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

    public void Generate(bool with) {
        if (m_generated) throw new System.Exception("Attempting to regenerate a generated door");

        foreach (var go in m_disableList) {
            go.SetActive(!with);
        }

        if (with) {
            onGenerateWith.Invoke();
        } else {
            onGenerateWithout.Invoke();
        }

        gameObject.SetActive(with);
        m_generated = true;
    }

    private void OnDrawGizmos() {
        Gizmos.color = m_gizmoColor;
        foreach (var go in m_disableList) {
            var renderer = go.GetComponent<Renderer>();
            var collider = go.GetComponent<Collider>();
            if (renderer != null) {
                Gizmos.DrawCube(renderer.bounds.center, renderer.bounds.extents);
            } else if (collider != null) {
                Gizmos.DrawCube(collider.bounds.center, collider.bounds.extents);
            } else {
                Gizmos.DrawSphere(go.transform.position, 0.2f);
            }
        }
    }
}