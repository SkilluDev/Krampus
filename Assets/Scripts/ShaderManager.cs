using System.Collections;
using System.Collections.Generic;
using System.Linq;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ShaderManager : MonoBehaviour {
	[SerializeField] private Material m_material;
	[SerializeField] private float m_maxIntensity = 0.99f;
	private float m_intensity;
	private float m_currentIntensity;

	[SerializeField] private AnimationCurve shaderCurveIn;
	[SerializeField] private AnimationCurve shaderCurveOut;
	[SerializeField] private float shaderDurationIn = 0.1f;
	[SerializeField] private float shaderDurationOut = 1f;
	private float m_shaderTime = 0f;
	[SerializeField] private float testValue = 1f;

	[SerializeField] private Volume UIPPVolume;
	private SplitToning m_splitToning;
	private float m_minSplitToning = 0f;
	private float m_maxSplitToning = 100f;

	[SerializeField] private UniversalRendererData m_universalRendererData;
	private bool shaderOn = true;


	[NaughtyAttributes.Button("Toggle Shader")]
	public void ToggleShader() {
		shaderOn = !shaderOn;
		foreach (var feature in m_universalRendererData.rendererFeatures) {
			feature.SetActive(shaderOn);
		}
		UIPPVolume.profile.TryGet(out m_splitToning);
		m_splitToning.active = shaderOn;
	}
	private void Start() {
		UIPPVolume.profile.TryGet(out m_splitToning);
		m_splitToning.balance.value = m_minSplitToning;
	}
	private bool m_fade;
	// Start is called before the first frame update
	[NaughtyAttributes.Button("Test Set Intensity")]
	public void SetIntensity() {
		SetIntensity(testValue);
	}
	private void SetIntensity(float intensity) {
		m_intensity = intensity;
		m_currentIntensity = intensity;
		m_fade = true;
	}

	//[NaughtyAttributes.Button("Set Split Toning")]
	public void SetSplitToning() {
		SetSplitToning(testValue);
	}
	private void SetSplitToning(float ratio) {
		m_splitToning.balance.value = Mathf.Lerp(m_minSplitToning, m_maxSplitToning, ratio);
	}


	// Update is called once per frame
	private void Update() {

		if (!m_fade) return;
		if (m_shaderTime <= shaderDurationIn) {
			m_currentIntensity = shaderCurveIn.Evaluate(m_shaderTime / shaderDurationIn) * m_intensity;
		} else if (m_shaderTime <= shaderDurationOut + shaderDurationIn) {
			m_currentIntensity = shaderCurveOut.Evaluate((m_shaderTime - shaderDurationIn) / shaderDurationOut) * m_intensity;
		} else {
			m_fade = false;
			m_shaderTime = 0f;
			m_currentIntensity = 0f;
		}
		m_material.SetFloat("_Intensity", m_currentIntensity);
		m_shaderTime += Time.deltaTime;
	}

	//[Button("Hit")]
	private void ProcessKillTest() {
		float ratio = 0.6f;
		SetIntensity(m_maxIntensity * ratio);
		SetSplitToning(ratio);
	}

	public void ProcessKill() {
		float ratio = (Game.MainGameInfo.BadChildrenCountOnStart - Game.MainGameInfo.BadChildren.Count() + 1f) /
		              Game.MainGameInfo.BadChildrenCountOnStart;
		Debug.Log(ratio+"ratio");
		SetIntensity(m_maxIntensity * ratio);
		SetSplitToning(ratio);
	}

	private void Unready() {
		m_shaderTime = 0f;
		m_minSplitToning = 0f;
		m_maxSplitToning = 100f;
		shaderOn = true;
		m_intensity = 0f;
		m_currentIntensity = 0f;
		m_material.SetFloat("_Intensity", m_currentIntensity);
	}

	private void Ready() {
		m_shaderTime = 0f;
		m_minSplitToning = 0f;
		m_maxSplitToning = 100f;
		shaderOn = true;
		m_intensity = 0f;
		m_currentIntensity = 0f;
		m_material.SetFloat("_Intensity", m_currentIntensity);
	}


}
