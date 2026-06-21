Shader "Custom/WallDepthOnly"
{
    // Minimal depth-only shader, intended to be used as an OVERRIDE material
    // in a custom ScriptableRenderPass (via DrawingSettings.overrideMaterial).
    // It is never assigned directly to the wall objects in the scene —
    // the walls keep their own ShaderGraph material for normal rendering.
    //
    // This shader writes ONLY to depth (ColorMask 0) so it can be drawn into
    // a dedicated depth-only render target (e.g. "_WallDepthTexture").

    Properties
    {
        // No exposed properties needed — this shader doesn't sample anything
        // or use any per-material data. Geometry/transform is all it needs.
    }

    SubShader
    {
        Tags
        {
            "RenderType" = "Opaque"
            "RenderPipeline" = "UniversalPipeline"
        }

        Pass
        {
            Name "DepthOnly"

            // IMPORTANT: this tag is required so URP's ShaderTagId filtering
            // (e.g. new ShaderTagId("UniversalForward")) actually matches this pass.
            // Without it, RenderingUtils.CreateDrawingSettings(...) will silently
            // skip this shader entirely — draw call count will be 0, with no error.
            Tags { "LightMode" = "UniversalForward" }

            ColorMask 0     // don't write any color channels — depth-only
            ZWrite On
            ZTest LEqual
            Cull Back

            HLSLPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag

            // Support GPU instancing so batched wall meshes still work correctly
            #pragma multi_compile_instancing

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            Varyings Vert(Attributes input)
            {
                Varyings output = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);

                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                return output;
            }

            half4 Frag(Varyings input) : SV_Target
            {
                // Unused due to ColorMask 0, but a fragment shader must return
                // something for the shader to compile.
                return 0;
            }
            ENDHLSL
        }
    }

    FallBack Off
}
