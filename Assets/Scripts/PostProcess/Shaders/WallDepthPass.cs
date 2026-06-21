using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.RenderGraphModule;
using UnityEngine.Rendering.Universal;
using UnityEngine.Experimental.Rendering;

namespace Shaders {
    public class WallDepthPass : ScriptableRenderPass {
        private readonly Material m_overrideMaterial;
        private readonly LayerMask m_layerMask;
        private const string PASS_NAME = "WallDepthPass";
        private static readonly int WALL_DEPTH_ID = Shader.PropertyToID("_WallDepthTexture");

        public WallDepthPass(Material mat, LayerMask mask) {
            m_overrideMaterial = mat;
            m_layerMask = 1 << 6; // this is hardcoded, because using the actual mask does not work for some bizzare reason.
            // Doesn't need to run at any particular point relative to opaques/
            // transparents/post-processing since it writes to its own target.
            renderPassEvent = RenderPassEvent.AfterRenderingOpaques;
        }

        private class PassData {
            public RendererListHandle rendererList;
        }

        public override void RecordRenderGraph(RenderGraph renderGraph, ContextContainer frameData) {
            if (m_overrideMaterial == null) {
                Debug.LogWarning("WallDepthPass: overrideMaterial is null, skipping.");
                return;
            }

            var cameraData = frameData.Get<UniversalCameraData>();
            var renderingData = frameData.Get<UniversalRenderingData>();
            var lightData = frameData.Get<UniversalLightData>();

            int width = cameraData.cameraTargetDescriptor.width;
            int height = cameraData.cameraTargetDescriptor.height;

            var depthDesc = new TextureDesc(width, height) {
                colorFormat = GraphicsFormat.None,   // no color attachment at all
                depthBufferBits = DepthBits.Depth32,
                name = "_WallDepthTexture",
                clearBuffer = true,
                clearColor = Color.clear,
                filterMode = FilterMode.Point         // depth should not be filtered
            };
            var wallDepthTexture = renderGraph.CreateTexture(depthDesc);
            var filterSettings = new FilteringSettings(RenderQueueRange.all, m_layerMask);

            var drawSettings = RenderingUtils.CreateDrawingSettings(
                new ShaderTagId("UniversalForward"),
                renderingData,
                cameraData,
                lightData,
                SortingCriteria.CommonOpaque);

            drawSettings.overrideMaterial = m_overrideMaterial;
            drawSettings.overrideMaterialPassIndex = 0;

            using (var builder = renderGraph.AddRasterRenderPass<PassData>(PASS_NAME, out var passData)) {
                var param = new RendererListParams(renderingData.cullResults, drawSettings, filterSettings);
                passData.rendererList = renderGraph.CreateRendererList(param);

                builder.UseRendererList(passData.rendererList);
                builder.SetRenderAttachmentDepth(wallDepthTexture, AccessFlags.Write);
                builder.AllowPassCulling(false);
                builder.AllowGlobalStateModification(true);

                builder.SetRenderFunc((PassData data, RasterGraphContext context) => {
                    context.cmd.DrawRendererList(data.rendererList);
                });

                // Makes the texture readable by name from ANY shader/pass that
                // runs afterwards in the frame, with no manual SetTexture calls.
                builder.SetGlobalTextureAfterPass(wallDepthTexture, WALL_DEPTH_ID);
            }
        }
    }

}