using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Shaders {
	internal class ColorBlitRendererFeature : ScriptableRendererFeature {
		//public Shader m_Shader;
		//public float m_Intensity;
		//public float m_Levels;

		[SerializeField] private Material m_Material;

		ColorBlitPass m_RenderPass = null;


		public override void AddRenderPasses(ScriptableRenderer renderer,
			ref RenderingData renderingData) {

			if (renderingData.cameraData.cameraType == CameraType.Game) {
				renderer.EnqueuePass(m_RenderPass);
			}
		}

		public override void SetupRenderPasses(ScriptableRenderer renderer,
			in RenderingData renderingData) {
			if (renderingData.cameraData.cameraType == CameraType.Game) {
				// Calling ConfigureInput with the ScriptableRenderPassInput.Color argument
				// ensures that the opaque texture is available to the Render Pass.

				var depthDescriptor = renderingData.cameraData.cameraTargetDescriptor;
				if (depthDescriptor.width <= 0 || depthDescriptor.height <= 0) {
					// Use camera pixel dimensions as a fallback
					depthDescriptor.width = renderingData.cameraData.camera.pixelWidth;
					depthDescriptor.height = renderingData.cameraData.camera.pixelHeight;
					if (depthDescriptor.width <= 0 || depthDescriptor.height <= 0) {
						Debug.LogError(
							"[ColorBlitRendererFeature] Setup: Invalid camera dimensions for depth descriptor.");
						return; // Cannot proceed with invalid dimensions
					}
				}

				depthDescriptor.msaaSamples = 1;
				depthDescriptor.bindMS = false;
				depthDescriptor.colorFormat =
					RenderTextureFormat
						.Depth; // Explicitly set for clarity if needed, but depthStencilFormat is preferred


				m_RenderPass.ConfigureInput(ScriptableRenderPassInput.Color);
				m_RenderPass.SetTarget(renderer.cameraColorTargetHandle); //, m_Intensity, m_Levels);
			}
		}

		public override void Create() {
			//m_Material = CoreUtils.CreateEngineMaterial(m_Shader);
			m_RenderPass = new ColorBlitPass(m_Material);


		}

		protected override void Dispose(bool disposing) {

			m_RenderPass = null; // Release references to passes

			base.Dispose(disposing);
		}
	}
}
