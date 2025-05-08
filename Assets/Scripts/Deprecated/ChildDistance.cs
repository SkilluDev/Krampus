using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class ChildDistance : MonoBehaviour {
    public float Dist { get; private set; } = float.MaxValue;
    private GameObject[] m_children;
    private Transform m_closestChild;

    private void Start() {
        m_children = GameObject.FindGameObjectsWithTag("Child");
        if (m_children.Length == 0) return;
        m_closestChild = m_children[0].transform;
    }

    private void Update() {
        if (m_children.Length == 0) return;

        if (!m_closestChild) {
            m_closestChild = m_children.FirstOrDefault(w => w != null).transform;
            if (!m_closestChild) return;
        }

        var closestOffset = m_closestChild.transform.position - transform.position;
        Dist = closestOffset.sqrMagnitude;

        foreach (var child in m_children) {
            if (child == null || !child.GetComponent<LegacyChild>().isBad) continue;
            var offset = child.transform.position - transform.position;
            float sqrLen = offset.sqrMagnitude;

            if (sqrLen < Dist) {
                m_closestChild = child.transform;
            }
        }
    }
}
