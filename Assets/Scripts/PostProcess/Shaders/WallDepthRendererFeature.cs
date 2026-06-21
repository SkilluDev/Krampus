using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Shaders {
    public class WallDepthRendererFeature : ScriptableRendererFeature {
        [SerializeField] private Material m_overrideMaterial;
        [SerializeField] private LayerMask m_wallLayerMask;
        [SerializeField] private RenderPassEvent m_renderPassEvent = RenderPassEvent.AfterRenderingOpaques;

        private WallDepthPass m_pass;

        public override void Create() {
            m_pass = new WallDepthPass(m_overrideMaterial, m_wallLayerMask);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            if (m_overrideMaterial == null) {
                Debug.LogWarning("WallDepthFeature: override material not assigned, skipping pass.");
                return;
            }

            renderer.EnqueuePass(m_pass);
        }
    }
}