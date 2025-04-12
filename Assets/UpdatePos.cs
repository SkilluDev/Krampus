using UnityEngine;

[ExecuteAlways]
public class UpdatePos : MonoBehaviour {

    [SerializeField] private Material[] m_mat;
    private void Update() {
        foreach (var mat in m_mat) {
            if (mat == null) continue;
            mat.SetVector("_KrampusPosition", transform.position);
        }
    }
}
