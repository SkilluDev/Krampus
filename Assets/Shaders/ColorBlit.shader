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
        _TransparentDepthTexture ("Transparent Depth Source", 2D) = "black" {}
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

            TEXTURE2D_X(_TransparentDepthTexture);
            SAMPLER(sampler_TransparentDepthTexture);
            
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
                //return SampleSceneDepth(uv);

                // 1. Get standard opaque(-or-closer) depth (already Linear Eye)
                float opaqueDepth = SampleSceneDepth(uv);

                // 2. Sample our custom transparent depth texture (Raw depth, likely [0,1])
                float rawTransparentDepth = SAMPLE_TEXTURE2D_X(_TransparentDepthTexture, sampler_TransparentDepthTexture, uv).r;

                // 3. Check if transparent depth is valid (usually 1.0 or platform far plane value if nothing rendered)
                //    Using < 0.9999 is a common way to check if *something* drew closer than the far plane.
                if (rawTransparentDepth < 0.9999)
                {
                    // 4. Convert raw transparent depth to Linear Eye depth for comparison
                    float transparentLinearEyeDepth = LinearEyeDepth(rawTransparentDepth, _ZBufferParams);

                    // 5. Return the MINIMUM (closer) of the two depths
                    return min(opaqueDepth, transparentLinearEyeDepth);
                }
                else
                {
                    // 6. No valid transparent depth here, only use opaque depth
                    return opaqueDepth;
                }
            }

            half4 frag (Varyings input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);

                float2 uv = input.texcoord;
                //float depth = SampleSceneDepth(uv);

                float rawTransparentDepth = SAMPLE_TEXTURE2D_X(_TransparentDepthTexture, sampler_TransparentDepthTexture, uv).r;
                return half4(rawTransparentDepth.xxx, 1.0); // Output raw depth as grayscale


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
