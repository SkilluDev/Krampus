using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.RenderGraphModule.Util;
using UnityEngine.Rendering.Universal;

namespace Shaders {
    internal class KrampusPostPass : ScriptableRenderPass {
        private Material m_material;
        private const string PASS_NAME = "KrampusPost";

        private static readonly int PROP_POSTERIZE_INTENSITY = Shader.PropertyToID("_PosterizeIntensity");
        private static readonly int PROP_POSTERIZE_GAMMA_BOOST = Shader.PropertyToID("_PosterizeGammaBoost");
        private static readonly int PROP_POSTERIZE_LEVELS = Shader.PropertyToID("_PosterizeLevels");
        private static readonly int PROP_OUTLINE_INTENSITY = Shader.PropertyToID("_OutlineIntensity");
        private static readonly int PROP_OUTLINE_NOISE = Shader.PropertyToID("_OutlineNoise");
        private static readonly int PROP_OUTLINE_SECONDARY = Shader.PropertyToID("_OutlineSecondary");
        private static readonly int PROP_OUTLINE_NOISE_SCALE = Shader.PropertyToID("_OutlineNoiseScale");
        private static readonly int PROP_OUTLINE_COLOR = Shader.PropertyToID("_OutlineColor");
        private static readonly int PROP_OUTLINE_THICKNESS = Shader.PropertyToID("_OutlineThickness");
        private static readonly int PROP_OUTLINE_DEPTH_SENSITIVITY = Shader.PropertyToID("_OutlineDepthSensitivity");

        public KrampusPostPass(Material material) {
            m_material = material;
            requiresIntermediateTexture = true;
            renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData) {
            if (m_material == null) {
                Debug.LogWarning("Post processing material is not assigned, so Krampus Post Processing will be disabled");
                return;
            }

            var stack = VolumeManager.instance.stack;

            var posterize = stack.GetComponent<PosterizeComponent>();
            m_material.SetFloat(PROP_POSTERIZE_INTENSITY, posterize.intensity.value);
            m_material.SetFloat(PROP_POSTERIZE_GAMMA_BOOST, posterize.gammaBoost.value);
            m_material.SetFloat(PROP_POSTERIZE_LEVELS, posterize.levels.value);

            var outline = stack.GetComponent<OutlineComponent>();
            m_material.SetFloat(PROP_OUTLINE_INTENSITY, outline.outlineIntensity.value);
            m_material.SetFloat(PROP_OUTLINE_NOISE, outline.outlineNoiseIntensity.value);
            m_material.SetFloat(PROP_OUTLINE_SECONDARY, outline.outlineSecondary.value);
            m_material.SetFloat(PROP_OUTLINE_NOISE_SCALE, outline.outlineNoiseScale.value);
            m_material.SetColor(PROP_OUTLINE_COLOR, outline.outlineColor.value);
            m_material.SetFloat(PROP_OUTLINE_THICKNESS, outline.outlineThickness.value);
            m_material.SetFloat(PROP_OUTLINE_DEPTH_SENSITIVITY, outline.outlineDepthSensitivity.value);

            var resourceData = frameData.Get<UniversalResourceData>();
            var source = resourceData.activeColorTexture;
            if (!source.IsValid()) return;

            var destinationDesc = renderGraph.GetTextureDesc(source);
            destinationDesc.name = $"CameraColor-{PASS_NAME}";
            destinationDesc.clearBuffer = false;

            var destination = renderGraph.CreateTexture(destinationDesc);

            renderGraph.AddBlitPass(
                new RenderGraphUtils.BlitMaterialParameters(source, destination, m_material, 0),
                PASS_NAME
            );

            resourceData.cameraColor = destination;
        }
    }
}




