using System.Linq;
using LitMotion;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class ShaderManager : MonoBehaviour {
	[SerializeField] private Material m_material;
	[SerializeField] private float m_maxIntensity = 0.99f;

	[SerializeField] private AnimationCurve m_shaderCurveIn;
	[SerializeField] private AnimationCurve m_shaderCurveOut;
	[SerializeField] private float m_shaderDurationIn = 0.1f;
	[SerializeField] private float m_shaderDurationOut = 1f;
	[SerializeField] private float m_testValue = 1f;

	[SerializeField] private Volume m_uIPPVolume;
	private SplitToning m_splitToning;
	private float m_minSplitToning = 0f;
	private float m_maxSplitToning = 100f;

	[SerializeField] private UniversalRendererData m_universalRendererData;
	private bool m_shaderOn = true;



	[NaughtyAttributes.Button("Toggle Shader")]
	public void ToggleShader() {
		m_shaderOn = !m_shaderOn;
		foreach (var feature in m_universalRendererData.rendererFeatures) {
			feature.SetActive(m_shaderOn);
		}
		m_uIPPVolume.profile.TryGet(out m_splitToning);
		m_splitToning.active = m_shaderOn;
	}
	private void Start() {
		m_uIPPVolume.profile.TryGet(out m_splitToning);
		m_splitToning.balance.value = m_minSplitToning;
	}
	// Start is called before the first frame update
	[NaughtyAttributes.Button("Test Set Intensity")]
	public void SetIntensity() {
		SetIntensity(m_testValue);
	}
	private void SetIntensity(float intensity) {
		Debug.Log(intensity+"intensity");
		LMotion.Create(0,intensity, m_shaderDurationIn).WithEase(m_shaderCurveIn).WithOnComplete(
			()=>LMotion.Create(intensity, 0, m_shaderDurationOut).WithEase(m_shaderCurveOut).Bind(f=>m_material.SetFloat("_Intensity", f))
		).Bind(f=>m_material.SetFloat("_Intensity", f));
	}

	//[NaughtyAttributes.Button("Set Split Toning")]
	public void SetSplitToning() {
		SetSplitToning(m_testValue);
	}
	private void SetSplitToning(float ratio) {
		m_splitToning.balance.value = Mathf.Lerp(m_minSplitToning, m_maxSplitToning, ratio);
	}

	[Button("Hit")]
	private void ProcessKillTest() {
		float ratio = 0.6f;
		SetIntensity(m_maxIntensity);
		SetSplitToning(ratio);
	}

	public void ProcessKill(Krampus krampus, Child child) {
		if (!child.IsNaughty) return;
		float ratio = (Game.MainGameInfo.NaughtyChildrenCountOnStart - Game.MainGameInfo.NaughtyChildren.Count() + 1f) /
					  Game.MainGameInfo.NaughtyChildrenCountOnStart;
		SetIntensity(m_maxIntensity * ratio);
		SetSplitToning(ratio);
	}

	private void Unready() {
		m_minSplitToning = 0f;
		m_maxSplitToning = 100f;
		m_shaderOn = true;
		m_material.SetFloat("_Intensity", 0);
	}

	private void Ready() {
		m_minSplitToning = 0f;
		m_maxSplitToning = 100f;
		m_shaderOn = true;
		m_material.SetFloat("_Intensity", 0);
		Game.GlobalEvents.onChildEaten.AddListener(ProcessKill);
	}

	private void OnApplicationQuit() {
		m_minSplitToning = 0f;
		m_maxSplitToning = 100f;
		m_shaderOn = true;
		m_material.SetFloat("_Intensity", 0);
	}
}
