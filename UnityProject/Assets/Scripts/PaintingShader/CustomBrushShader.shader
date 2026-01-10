Shader "Custom/BrushShader"
{
    Properties {
        _MainTex ("Source", 2D) = "white" {}
        _PaintUV ("Paint UV", Vector) = (0,0,0,0)
        _BrushSize ("Brush Size", Float) = 0.05
    }
    SubShader {
        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Varyings { float4 pos : SV_POSITION; float2 uv : TEXCOORD0; };
            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            float4 _PaintUV; float _BrushSize;

            Varyings vert (float4 pos : POSITION, float2 uv : TEXCOORD0) {
                Varyings outVar;
                outVar.pos = TransformObjectToHClip(pos.xyz);
                outVar.uv = uv;
                return outVar;
            }

            half4 frag (Varyings i) : SV_Target {
                half4 col = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.uv);
                float dist = distance(i.uv, _PaintUV.xy);
                if (dist < _BrushSize) return half4(1, 1, 1, 1); // Male Weiß
                return col;
            }
            ENDHLSL
        }
    }
}