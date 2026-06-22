using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

namespace Shaders {
    internal class KrampusEffectsRendererFeature : ScriptableRendererFeature {
        [SerializeField] private Material m_postProcessMaterial;

        private KrampusPostPass m_pass;

        public override void Create() {
            m_pass = new KrampusPostPass(m_postProcessMaterial);
        }

        public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData) {
            renderer.EnqueuePass(m_pass);
        }
    }
}