using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomPostEffectPass : ScriptableRenderPass
{
    private Material m_Material; // Material using our PostEffectShader.shader
    private RenderTargetIdentifier m_Source; // Source texture (from camera or previous pass)

    // String constant for the profiler tag
    private const string PROFILER_TAG = "Custom Post Effect Pass";

    // Constructor - receives the material to use
    public CustomPostEffectPass(Material material)
    {
        m_Material = material;
        // Set the render pass event - when should this pass execute?
        // BeforeRenderingPostProcessing is common for effects applied before URP's own stack
        // AfterRenderingPostProcessing is common for effects applied after URP's stack
        renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
    }

    // Called by the RendererFeature before executing the pass
    public void Setup(RenderTargetIdentifier source)
    {
        m_Source = source;
        // If your shader needs depth/normals, configure input here:
        ConfigureInput(ScriptableRenderPassInput.Depth | ScriptableRenderPassInput.Normal);
    }

    // Called every frame to execute the pass
    public override void Execute(ScriptableRenderContext context, ref RenderingData renderingData)
    {
        // Safety check
        if (m_Material == null)
        {
            Debug.LogError("CustomPostEffectPass: Material not assigned.");
            return;
        }

        // Get a command buffer from the pool
        CommandBuffer cmd = CommandBufferPool.Get(PROFILER_TAG);

        // --- Set shader properties here (if needed) ---
        // Example: m_Material.SetFloat("_PosterizeLevels", settings.posterizeLevels);
        // Example: m_Material.SetFloat("_EdgeIntensity", settings.edgeIntensity);

        // Execute the Blit command
        // This draws a fullscreen quad using our material/shader,
        // taking m_Source as input (_MainTex in shader) and rendering to the current camera target
        Blit(cmd, m_Source, renderingData.cameraData.renderer.cameraColorTarget, m_Material, 0); // 0 = first pass in shader

        // Execute and release the command buffer
        context.ExecuteCommandBuffer(cmd);
        CommandBufferPool.Release(cmd);
    }

    // Called when the camera stack finishes rendering
    public override void OnCameraCleanup(CommandBuffer cmd)
    {
        // Cleanup tasks, if any (usually not needed for simple Blit passes)
    }
}
