Shader "Test/ColorBlit"
{
    Properties
    {
        _Intensity ("Intensity", Range(0.0, 5.0)) = 1.0 
        _MainTex ("Texture", 2D) = "white" {}
        _Levels ("Levels", Float) = 20.0
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1) // Black outline
        _OutlineThickness ("Outline Thickness", Range(0.5, 5.0)) = 1.0
        _DepthSensitivity ("Depth Sensitivity", Range(0.0, 100.0)) = 10.0 // Controls how sensitive depth difference is
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off
        Cull Off
        Blend Off
        Pass
        {
            Name "ColorBlitPass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            
            // The Blit.hlsl file provides the vertex shader (Vert),
            // the input structure (Attributes) and the output structure (Varyings)
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

            // Set the color texture from the camera as the input texture

            SAMPLER(sampler_BlitTexture);
            

            // Set up an intensity parameter
            float _Intensity;
            float _Levels;

            float4 _OutlineColor;
            float _OutlineThickness;
            float _DepthSensitivity;

            float4 _CameraDepthTexture_TexelSize;

            float GetLinearEyeDepth(float2 uv)
            {
                // SampleSceneDepth samples the platform-specific depth texture 
                // and returns linear depth in eye space (0 at near plane, far plane distance at far plane).
                return SampleSceneDepth(uv); 
            }

            half4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord;
                float depth = SampleSceneDepth(uv);

                // --- VISUALIZE the raw depth value ---

                // Option A: Grayscale depth (0=near, 1=far)
                // Requires knowing the far plane (_ProjectionParams.z)
                //return frac(depth*100); 

                float2 texelSize = _CameraDepthTexture_TexelSize.xy; 
                float centerDepth = GetLinearEyeDepth(uv);
                float depthUp    = GetLinearEyeDepth(uv + float2(0, texelSize.y * _OutlineThickness));
                float depthDown  = GetLinearEyeDepth(uv - float2(0, texelSize.y * _OutlineThickness));
                float depthLeft  = GetLinearEyeDepth(uv - float2(texelSize.x * _OutlineThickness, 0));
                float depthRight = GetLinearEyeDepth(uv + float2(texelSize.x * _OutlineThickness, 0));
                
                float depthDiffH = abs(depthRight - depthLeft);
                float depthDiffV = abs(depthUp - depthDown);
                float depthEdge = sqrt(depthDiffH * depthDiffH + depthDiffV * depthDiffV) * _DepthSensitivity; 
                
                float edgeFactor = saturate(depthEdge);
                float minNeighborDepth = min(min(depthUp, depthDown), min(depthLeft, depthRight));
                float depthEpsilon = minNeighborDepth * 0.005;
                edgeFactor *= step(centerDepth, minNeighborDepth + depthEpsilon);
                
                // Sample the color from the input texture
                float4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.texcoord);
                color.rgb = floor(color.rgb * _Levels) / _Levels;
                // Output the color from the texture, with the green value set to the chosen intensity
                half4 finalColor = lerp(color, _OutlineColor, edgeFactor);
                finalColor.a = color.a; 
                return finalColor;
            }
            ENDHLSL
        }
    }
}
