using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShaderManager : MonoBehaviour
{
	[SerializeField] private Material m_material;
	[SerializeField] private float m_intensity;

	[SerializeField] private float m_step;
    // Start is called before the first frame update
    public void Set_Intensity(float intensity) {
	    m_intensity = intensity;
    }

    // Update is called once per frame
    void Update() {
	    if (m_intensity < 0) {
		    m_intensity = 0;
	    }
	    if (m_intensity > 0) {
		    m_intensity = Mathf.Lerp(m_intensity, 0, m_step);
		    m_material.SetFloat("_Intensity", m_intensity);
	    }
	    if (m_intensity == 0) {
		    m_material.SetFloat("_Intensity", m_intensity);

	    }

    }
}
