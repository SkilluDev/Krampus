using UnityEngine;
using UnityEngine.Rendering;
using UnityEngine.Rendering.Universal;

public class CustomPostEffectFeature : ScriptableRendererFeature
{
    [System.Serializable]
    public class FeatureSettings // Optional: Define settings here later
    {
        public bool isEnabled = true;
        public RenderPassEvent renderPassEvent = RenderPassEvent.BeforeRenderingPostProcessing;
        // Add settings like posterize levels, edge intensity etc.
        // public float posterizeLevels = 10f;
        // public float edgeIntensity = 1f;
    }

    // Public field for the shader - assign your PostEffectShader.shader in the Inspector
    public Shader postEffectShader;
    public FeatureSettings settings = new FeatureSettings();

    private CustomPostEffectPass m_ScriptablePass;
    private Material m_Material;

    // Called once when the feature is created (or settings change)
    public override void Create()
    {
        if (postEffectShader == null)
        {
            Debug.LogWarning("CustomPostEffectFeature: Post Effect Shader not assigned.");
            return;
        }

        // Create the material from the shader
        m_Material = CoreUtils.CreateEngineMaterial(postEffectShader);
        if (m_Material == null)
        {
            Debug.LogError("CustomPostEffectFeature: Failed to create material.");
            return;
        }

        // Create the render pass, passing the material to it
        m_ScriptablePass = new CustomPostEffectPass(m_Material);
        m_ScriptablePass.renderPassEvent = settings.renderPassEvent; // Set pass event from settings
    }

    // Called every frame for each camera
    public override void AddRenderPasses(ScriptableRenderer renderer, ref RenderingData renderingData)
    {
        // Check if the feature should be added
        if (!settings.isEnabled || m_ScriptablePass == null || m_Material == null)
            return;

        // Setup the pass with the current camera's color target as source
        m_ScriptablePass.Setup(renderer.cameraColorTarget);

        // Add the pass to the renderer
        renderer.EnqueuePass(m_ScriptablePass);
    }

    // Called when the feature is disabled or destroyed
    protected override void Dispose(bool disposing)
    {
        // Clean up the material
        CoreUtils.Destroy(m_Material);
        m_Material = null; // Prevent memory leaks
    }
}
