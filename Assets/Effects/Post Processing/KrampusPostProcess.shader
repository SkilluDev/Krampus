Shader "Krampus/KrampusPostProcess"
{
    Properties
    {
        // _MainTex ("(in) Color Texture", 2D) = "white" {}

        _NoiseMap ("Noise Map", 2D) = "black" {}

        _PosterizeIntensity ("Posterize Intensity", Range(0.0, 1)) = 1.0 
        _PosterizeGammaBoost ("Posterize Gamma Boost", Range(0.0, 10)) = 1.9 
        _PosterizeLevels ("Posterize Levels", Float) = 20.0

        _OutlineIntensity ("Outline Intensity", Range(0.0, 5.0)) = 1.0 
        _OutlineNoise ("Outline Noise Intensity", Range(0.0, 0.1)) = 0.005 
        _OutlineSecondary ("Outline Secondary", Range(0.0, 1)) = 0.7 
        _OutlineNoiseScale ("Outline Noise Scale", Range(0.0, 0.1)) = 0.001
        _OutlineColor ("Outline Color", Color) = (0, 0, 0, 1) // Black outline
        _OutlineThickness ("Outline Thickness", Range(0, 5.0)) = 1.0
        _OutlineDepthSensitivity ("Outline Depth Sensitivity", Range(0.0, 100.0)) = 10.0 // Controls how sensitive depth difference is
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "RenderPipeline" = "UniversalPipeline"}
        LOD 100
        ZWrite Off
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha
        Pass
        {
            Name "ColorPass"

            HLSLPROGRAM
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"
            
            // The Blit.hlsl file provides the vertex shader (Vert),
            // the input structure (Attributes) and the output structure (Varyings)
            #include "Packages/com.unity.render-pipelines.core/Runtime/Utilities/Blit.hlsl"

            #pragma vertex Vert
            #pragma fragment frag

            TEXTURE2D(_WallDepthTexture);
            SAMPLER(sampler_WallDepthTexture);

            TEXTURE2D_X(_NoiseMap);
            SAMPLER(sampler_NoiseMap);
            
            SAMPLER(sampler_BlitTexture);
            

            float GetCombinedDepth(float2 uv)
            {
                float opaqueRawDepth = SampleSceneDepth(uv);
                float wallRawDepth = SAMPLE_TEXTURE2D_X(_WallDepthTexture, sampler_WallDepthTexture, uv).r;

                #if UNITY_REVERSED_Z
                    // Reversed-Z: larger raw value = closer to camera. Pick the closer surface.
                    return max(opaqueRawDepth, wallRawDepth);
                #else
                    // Standard Z: smaller raw value = closer to camera.
                    return min(opaqueRawDepth, wallRawDepth);
                #endif
            }

            float _OutlineIntensity;
            float _OutlineSecondary;
            float _OutlineNoise;
            float4 _OutlineColor;
            float _OutlineThickness;
            float _OutlineDepthSensitivity;
            float _OutlineNoiseScale;

            float edge_detect(float2 uv) {
                float2 texelSize = _CameraDepthTexture_TexelSize.xy; 
                float centerDepth = GetCombinedDepth(uv);
                float depthUp     = GetCombinedDepth(uv + float2(0, texelSize.y * _OutlineThickness));
                float depthDown   = GetCombinedDepth(uv - float2(0, texelSize.y * _OutlineThickness));
                float depthLeft   = GetCombinedDepth(uv - float2(texelSize.x * _OutlineThickness, 0));
                float depthRight  = GetCombinedDepth(uv + float2(texelSize.x * _OutlineThickness, 0));

                
                float laplacianH = depthLeft + depthRight - 2.0 * centerDepth;
                float laplacianV = depthUp + depthDown - 2.0 * centerDepth;
                float depthEdge = (abs(laplacianH) + abs(laplacianV)) / max(centerDepth, 0.0001) * _OutlineDepthSensitivity;
                
                float edgeFactor = saturate(depthEdge);
                float minNeighborDepth = min(min(depthUp, depthDown), min(depthLeft, depthRight));
                float depthEpsilon = minNeighborDepth * 0.005;
                edgeFactor *= step(centerDepth, minNeighborDepth + depthEpsilon);

                return edgeFactor;
            }

            half4 pp_outlines(half4 c, float2 uv)
            {
                float centerDepth = GetCombinedDepth(uv);
                float3 worldPos = ComputeWorldSpacePosition(uv, centerDepth, UNITY_MATRIX_I_VP);
                float times1 = (floor(_Time * 50) / 50 - 25) * 0.4711129;
                float times2 = (floor(_Time * 50) / 50 - 25) * 0.1711129;

                float edgeFactor = max(
                edge_detect(uv + (SAMPLE_TEXTURE2D_X(_NoiseMap, sampler_NoiseMap, ((worldPos.xz * _OutlineNoiseScale) + worldPos.y * _OutlineNoiseScale / 2) + float2(times1, times1 * 0.229)) - 0.5) * _OutlineNoise), 
                edge_detect(uv - (SAMPLE_TEXTURE2D_X(_NoiseMap, sampler_NoiseMap, ((worldPos.xz * _OutlineNoiseScale) + worldPos.y * _OutlineNoiseScale / 2) + float2(times2, times2 * 0.529)) - 0.5) * _OutlineNoise) * _OutlineSecondary
                );
                
                return lerp(c, _OutlineColor, saturate(edgeFactor * _OutlineIntensity));
            }

            float _PosterizeLevels;
            float _PosterizeIntensity;
            float _PosterizeGammaBoost;
            half4 pp_posterize(half4 c, float2 uv) 
            {
                float4 color = c;
                color.rgb = floor(color.rgb * _PosterizeLevels) / _PosterizeLevels;
                color.rgb *= _PosterizeGammaBoost;
                color.rgb = saturate(color);

                return lerp(c, color, _PosterizeIntensity);
            }

            half4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float4 color = SAMPLE_TEXTURE2D_X(_BlitTexture, sampler_BlitTexture, input.texcoord);
                color = pp_posterize(color, input.texcoord);
                color = pp_outlines(color, input.texcoord);
                return color;
            }
            ENDHLSL
        }
    }
}
