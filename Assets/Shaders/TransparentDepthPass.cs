// TransparentDepthPass.cs
using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;
using System.Collections.Generic;

public class TransparentDepthPass : ScriptableRenderPass
{
    // Removed temporary RT name/ID, we'll use the RTHandle directly.

    private RTHandle m_DepthAttachmentHandle { get; set; } // To configure the target
    private RenderTextureDescriptor m_Descriptor;
    private FilteringSettings m_FilteringSettings;
    private string m_ProfilerTag = "TransparentDepthPrepass";
    private List<ShaderTagId> m_ShaderTagIdList = new List<ShaderTagId>();

    // Name used for allocating the RTHandle and binding the texture in the shader
    private static readonly int k_TransparentDepthTextureID = Shader.PropertyToID("_TransparentDepthTexture"); // More efficient ID lookup

    public TransparentDepthPass(RenderQueueRange renderQueueRange, LayerMask layerMask, string profilerTag)
    {
        renderPassEvent = RenderPassEvent.BeforeRenderingOpaques; // Run before opaques to capture depth early
        m_FilteringSettings = new FilteringSettings(renderQueueRange, layerMask);
        m_ShaderTagIdList.Add(new ShaderTagId("DepthOnly")); // Standard URP DepthOnly pass tag
        // Add other tags if needed, e.g., new ShaderTagId("UniversalForwardOnly") if your transparent shaders have that for depth
        m_ProfilerTag = profilerTag;
    }

    // Call this from the Feature's SetupRenderPasses to set up
    public void Setup(RenderTextureDescriptor baseDescriptor, RTHandle depthAttachmentHandle)
    {
        // Store the handle provided by the Feature (used for ConfigureTarget)
        m_DepthAttachmentHandle = depthAttachmentHandle;

        // Configure the descriptor for our depth texture based on camera's descriptor
        m_Descriptor = baseDescriptor;
        m_Descriptor.colorFormat = RenderTextureFormat.Depth; // Depth format
        m_Descriptor.depthBufferBits = 32; // Standard depth bits (32 or 24)
        m_Descriptor.msaaSamples = 1; // No MSAA needed for depth texture
        // Ensure bind flags are appropriate if needed (usually handled by RTHandle allocation)
        // m_Descriptor.bindMS = false; // Not needed for Depth format typically
    }

    public override void Configure(CommandBuffer cmd, RenderTextureDescriptor cameraTextureDescriptor)
    {
        // Configure the pass to write to the RTHandle provided by the feature
        // This tells the pipeline context where this pass writes.
        ConfigureTarget(m_DepthAttachmentHandle);
        // Clear the depth buffer at the start of the pass to the far plane value (1.0)
        ConfigureClear(ClearFlag.Depth, Color.clear); // Using Color.clear is fine for depth-only clear

        // We DON'T use GetTemporaryRT anymore, the Feature manages the RTHandle
    }

    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        // Check if the handle is valid before proceeding
        if (m_DepthAttachmentHandle == null)
        {
            Debug.LogError($"[{m_ProfilerTag}] RTHandle is not set up. Skipping pass.");
            return;
        }

        CommandBuffer cmd = CommandBufferPool.Get(m_ProfilerTag);
        using (new ProfilingScope(cmd, new ProfilingSampler(m_ProfilerTag)))
        {
            context.ExecuteCommandBuffer(cmd); // Execute any configuration commands first (like clear)
            cmd.Clear();

            // Log BEFORE drawing
            if (m_DepthAttachmentHandle.rt == null) {
	            Debug.LogWarning($"[{m_ProfilerTag}] RTHandle.rt is NULL **before** DrawRenderers."); // Use Warning
            } else {
	            Debug.Log($"[{m_ProfilerTag}] RTHandle.rt is VALID **before** DrawRenderers: {m_DepthAttachmentHandle.rt.name}");
            }

            var sortingCriteria = SortingCriteria.CommonTransparent; // Use transparent sorting
            var drawingSettings = CreateDrawingSettings(m_ShaderTagIdList, ref renderingData, sortingCriteria);
            drawingSettings.enableDynamicBatching = renderingData.supportsDynamicBatching; // Use camera setting

            // Draw the renderers matching the filtering settings into our depth target
            context.DrawRenderers(renderingData.cullResults, ref drawingSettings, ref m_FilteringSettings);
            if (m_DepthAttachmentHandle.rt == null) {
	            Debug.LogError($"[{m_ProfilerTag}] RTHandle.rt is STILL NULL **after** DrawRenderers!"); // Use Error if still null
            } else {
	            Debug.Log($"[{m_ProfilerTag}] RTHandle.rt became VALID **after** DrawRenderers: {m_DepthAttachmentHandle.rt.name}, Format: {m_DepthAttachmentHandle.rt.graphicsFormat}");
            }
            // Crucially, bind the resulting texture so the shader in the later pass can sample it.
            // Use the ID derived from the consistent name.
            cmd.SetGlobalTexture(k_TransparentDepthTextureID, m_DepthAttachmentHandle);
        }
        context.ExecuteCommandBuffer(cmd); // Execute draw calls and the SetGlobalTexture command
        CommandBufferPool.Release(cmd); // Release the command buffer
    }

    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        // We don't own the RTHandle itself, the Feature does.
        // No need to release temporary RTs as we didn't get any.
        // If we needed to unbind the global texture, we could do it here, but it's often not necessary.
    }
}
