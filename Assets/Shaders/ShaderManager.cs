using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class ShaderManager : MonoBehaviour
{
	[SerializeField] private Material m_material;
	[SerializeField] private float m_intensity;
	private float m_currentIntensity;

	[SerializeField] private AnimationCurve shaderCurveIn;
	[SerializeField] private AnimationCurve shaderCurveOut;
	[SerializeField] private float shaderDurationIn = 0.1f;
	[SerializeField] private float shaderDurationOut = 1f;
	private float m_shaderTime = 0f;
	[SerializeField] private float testValue = 1f;

	private bool m_fade;
    // Start is called before the first frame update
    [NaughtyAttributes.Button("Set Intensity")]
    public void SetIntensity() {
	    Set_Intensity(testValue);
    }
    public void Set_Intensity(float intensity) {
	    m_intensity = intensity;
	    m_currentIntensity = intensity;
	    m_fade = true;
    }

    // Update is called once per frame
    void Update() {

	    if (!m_fade) return;
	    if (m_shaderTime <= shaderDurationIn) {
			    m_currentIntensity = shaderCurveIn.Evaluate(m_shaderTime/shaderDurationIn)*m_intensity;
	    } else if (m_shaderTime <= shaderDurationOut+shaderDurationIn) {
			    m_currentIntensity = shaderCurveOut.Evaluate((m_shaderTime-shaderDurationIn)/shaderDurationOut)*m_intensity;
	    } else {
		    m_fade = false;
		    m_shaderTime = 0f;
	    }
	    m_material.SetFloat("_Intensity", m_currentIntensity);
	    m_shaderTime += Time.deltaTime;
    }
}
