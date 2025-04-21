using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteAlways]
public class UpdateWallShader : MonoBehaviour {
    [SerializeField] private Vector2 m_wallFade = new Vector2(-1, 1);
    private void Update() {
        Shader.SetGlobalVector("_KrampusPosition", transform.position);
        Shader.SetGlobalVector("_WallFadeSetting", m_wallFade);
    }
}
