using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

internal class ColorBlitRendererFeature : ScriptableRendererFeature
{
	//public Shader m_Shader;
	//public float m_Intensity;
	//public float m_Levels;

	[SerializeField] private Material m_Material;
	[SerializeField] private LayerMask transparentLayerMask = -1;
	[SerializeField] private RenderQueueRange transparentRenderQueue = RenderQueueRange.transparent;

	ColorBlitPass m_RenderPass = null;

	private TransparentDepthPass m_TransparentDepthPass;
	private RTHandle m_TransparentDepthRTHandle;
	private const string m_TransparentDepthRTName = "_TransparentDepthTexture";

	public override void AddRenderPasses(ScriptableRenderer renderer,
		ref RenderingData renderingData)
	{

		if (renderingData.cameraData.cameraType == CameraType.Game) {
			renderer.EnqueuePass(m_TransparentDepthPass);
			renderer.EnqueuePass(m_RenderPass);
		}
	}

	public override void SetupRenderPasses(ScriptableRenderer renderer,
		in RenderingData renderingData)
	{
		if (renderingData.cameraData.cameraType == CameraType.Game)
		{
			// Calling ConfigureInput with the ScriptableRenderPassInput.Color argument
			// ensures that the opaque texture is available to the Render Pass.

			var depthDescriptor = renderingData.cameraData.cameraTargetDescriptor;
			if (depthDescriptor.width <= 0 || depthDescriptor.height <= 0)
			{
				// Use camera pixel dimensions as a fallback
				depthDescriptor.width = renderingData.cameraData.camera.pixelWidth;
				depthDescriptor.height = renderingData.cameraData.camera.pixelHeight;
				if (depthDescriptor.width <= 0 || depthDescriptor.height <= 0) {
					Debug.LogError("[ColorBlitRendererFeature] Setup: Invalid camera dimensions for depth descriptor.");
					return; // Cannot proceed with invalid dimensions
				}
			}
			depthDescriptor.graphicsFormat = GraphicsFormat.None; // We want depth, GraphicsFormat handles this better
			// *** REVISED CHECK: Use SystemInfo with FormatUsage ***
			UnityEngine.Experimental.Rendering.FormatUsage usage = UnityEngine.Experimental.Rendering.FormatUsage.Depth;

			// Check for supported depth formats in preferred order
			if (SystemInfo.IsFormatSupported(GraphicsFormat.D32_SFloat, usage)) {
				preferredDepthFormat = GraphicsFormat.D32_SFloat;
			} else if (SystemInfo.IsFormatSupported(GraphicsFormat.D24_UNorm_S8_UInt, usage)) {
				preferredDepthFormat = GraphicsFormat.D24_UNorm_S8_UInt;
			} else if (SystemInfo.IsFormatSupported(GraphicsFormat.D16_UNorm, usage)) {
				preferredDepthFormat = GraphicsFormat.D16_UNorm;
			}

			// Check if we found a supported format
			if (preferredDepthFormat == GraphicsFormat.None) {
				Debug.LogError("[ColorBlitRendererFeature] Setup: No supported depth format found for the depth texture! Disabling pass setup.");
				// Handle error case - perhaps return here to prevent further setup?
				return;
			} else {
				// Debug.Log($"[ColorBlitRendererFeature] Setup: Selected depth format: {preferredDepthFormat}"); // Optional log
			}

			depthDescriptor.depthStencilFormat = preferredDepthFormat;
			depthDescriptor.msaaSamples = 1;
			depthDescriptor.bindMS = false;
			depthDescriptor.colorFormat = RenderTextureFormat.Depth; // Explicitly set for clarity if needed, but depthStencilFormat is preferred

			// Pass the descriptor and the RTHandle to the pass for setup
			m_TransparentDepthPass.Setup(depthDescriptor, m_TransparentDepthRTHandle);

			m_RenderPass.ConfigureInput(ScriptableRenderPassInput.Color);
			m_RenderPass.SetTarget(renderer.cameraColorTargetHandle); //, m_Intensity, m_Levels);
		}
	}

	public override void Create()
	{
		//m_Material = CoreUtils.CreateEngineMaterial(m_Shader);
		m_RenderPass = new ColorBlitPass(m_Material);
		m_TransparentDepthRTHandle = RTHandles.Alloc(m_TransparentDepthRTName, name: m_TransparentDepthRTName);

		m_TransparentDepthPass = new TransparentDepthPass(transparentRenderQueue, transparentLayerMask, "TransparentDepthPrepass");

	}

	protected override void Dispose(bool disposing)
	{
		RTHandles.Release(m_TransparentDepthRTHandle);
		m_TransparentDepthRTHandle = null; // Good practice

		m_RenderPass = null; // Release references to passes
		m_TransparentDepthPass = null;

		base.Dispose(disposing);
	}
}
