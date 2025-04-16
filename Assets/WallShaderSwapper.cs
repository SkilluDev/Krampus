using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class WallShaderSwapper : MonoBehaviour
{
	private Material m_material;
    // Start is called before the first frame update
    void Start()
    {
        m_material = GetComponent<MeshRenderer>().materials[1];
    }

    // Update is called once per frame
    void Update()
    {
	    if (m_material.color.a < 0.99f) {
		    m_material.shader = Shader.Find("Custom/Lit2");
	    } else {
		    m_material.shader = Shader.Find("Custom/Lit");
	    }
    }
}
