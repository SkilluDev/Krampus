using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallTransparency : MonoBehaviour {
    private bool m_z;
    private Material m_material;
    private Transform m_player;

    // Start is called before the first frame update
    private void Start() {
        m_material = GetComponent<Renderer>().material;
        m_player = GameObject.Find("Player").transform;
        if (transform.rotation.z == 90) {
            m_z = true;
        }
    }

    // Update is called once per frame
    private void Update() {
        if (m_z) {
            if (m_player.position.x > transform.position.x) {
                gameObject.SetActive(true);
            } else {
                GetComponent<Renderer>().material = m_material;
            }
        }
    }
}
