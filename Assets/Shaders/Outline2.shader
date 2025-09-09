Shader "Hidden/Outline2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        // No culling or depth
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM
            #pragma fragment frag
            #pragma vertex vert

            sampler2D _MainTex;

            struct appdata {
                float2 uv : TEXCOORD0;
            };

            appdata vert(appdata v) {
                return v;
            }

            float4 frag(appdata i) : SV_Target {
                return float4(1,1,1,1); 
            }
            ENDHLSL
        }
    }
}
