using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

[ExecuteInEditMode]
public class RoomDoorGroup : MonoBehaviour {
    public UnityAction onGenerateWith;
    public UnityAction onGenerateWithout;

    [SerializeField] private List<GameObject> m_disableList = new List<GameObject>();
    [SerializeField][HideInInspector] private RoomPrefab m_room;
    [SerializeField][HideInInspector] private Vector2Int m_cellPosition;
    [SerializeField][HideInInspector] private QuadDirection m_direction;


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

    public static RoomDoorGroup Create(Vector2Int cellPosition, QuadDirection dir, RoomPrefab room) {
        var obj = new GameObject($"Door [{cellPosition} {dir}]");
        var c = obj.AddComponent<RoomDoorGroup>();
        c.m_direction = dir;
        c.m_cellPosition = cellPosition;
        c.m_room = room;
        obj.transform.position = RoomPrefab.GetPleasantDoorPosition(cellPosition.x, cellPosition.y, dir);
        obj.transform.SetParent(room.transform);

        return c;
    }

    public void Generate(bool with) {
        if (m_generated) throw new System.Exception("Attempting to regenerate a generated door");

        if (!with) {
            foreach (var go in m_disableList) {
                if (go == null) continue;
                go.SetActive(false);
            }
        }

        if (with) {
            onGenerateWith.Invoke();
        } else {
            onGenerateWithout.Invoke();
        }

        gameObject.SetActive(with);
        m_generated = true;
    }

    private void OnDrawGizmosSelected() {
        Gizmos.color = m_gizmoColor;
        foreach (var go in m_disableList) {
            if (go == null) continue;
            var renderer = go.GetComponent<Renderer>();
            var collider = go.GetComponent<Collider>();
            if (collider != null) {
                Gizmos.DrawCube(collider.bounds.center, collider.bounds.extents * 2);
            } else if (renderer != null) {
                Gizmos.DrawCube(renderer.bounds.center, renderer.bounds.extents * 2);
            } else {
                Gizmos.DrawSphere(go.transform.position, 0.2f);
            }
        }
    }

    private void OnDrawGizmos() {
        Gizmos.color = m_gizmoColor;
        Gizmos.DrawCube(transform.position, (m_direction.HasFlag(QuadDirection.NORTH) || m_direction.HasFlag(QuadDirection.SOUTH)) ? RoomPrefab.DOOR_NS_SIZE : RoomPrefab.DOOR_EW_SIZE);
        foreach (var go in m_disableList) {
            if (go == null) continue;
            var renderer = go.GetComponent<Renderer>();
            var collider = go.GetComponent<Collider>();
            if (collider != null) {
                Gizmos.DrawWireCube(collider.bounds.center, collider.bounds.extents * 2);
            } else if (renderer != null) {
                Gizmos.DrawWireCube(renderer.bounds.center, renderer.bounds.extents * 2);
            } else {
                Gizmos.DrawSphere(go.transform.position, 0.2f);
            }
        }
    }
}