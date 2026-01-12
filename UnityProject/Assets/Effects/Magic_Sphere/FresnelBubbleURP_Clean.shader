Shader "Custom/FresnelBubbleURP_Clean"
{
    Properties
    {
        _Tint ("Tint", Color) = (0.35, 0.8, 1, 1)
        _Opacity ("Opacity", Range(0,1)) = 0.18

        _RimColor ("Rim Color", Color) = (0.6, 0.95, 1, 1)
        _RimPower ("Rim Power", Range(0.1, 10)) = 3.0
        _RimIntensity ("Rim Intensity", Range(0, 10)) = 3.0

        _SparkleTex ("Sparkle (R)", 2D) = "white" {}
        _SparkleStrength ("Sparkle Strength", Range(0, 5)) = 1.2
        _SparkleTiling ("Sparkle Tiling", Range(0.1, 30)) = 8.0
        _SparkleSpeedX ("Sparkle Speed X", Range(-2, 2)) = 0.10
        _SparkleSpeedY ("Sparkle Speed Y", Range(-2, 2)) = 0.20
        _SparklePower ("Sparkle Contrast", Range(0.5, 10)) = 3.5

        _InnerGlow ("Inner Glow", Range(0, 5)) = 0.6
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" "Queue"="Transparent" "RenderType"="Transparent" }

        Pass
        {
            Name "Forward"
            Tags { "LightMode"="UniversalForward" }

            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            Cull Back

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_SparkleTex);
            SAMPLER(sampler_SparkleTex);

            CBUFFER_START(UnityPerMaterial)
                float4 _Tint;
                float _Opacity;

                float4 _RimColor;
                float _RimPower;
                float _RimIntensity;

                float _SparkleStrength;
                float _SparkleTiling;
                float _SparkleSpeedX;
                float _SparkleSpeedY;
                float _SparklePower;

                float _InnerGlow;
            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS   : NORMAL;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float3 normalWS    : TEXCOORD0;
                float3 viewDirWS   : TEXCOORD1;
                float2 uv          : TEXCOORD2;
            };

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                float3 posWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.positionHCS = TransformWorldToHClip(posWS);
                OUT.normalWS = TransformObjectToWorldNormal(IN.normalOS);
                OUT.viewDirWS = GetWorldSpaceViewDir(posWS);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                float3 N = normalize(IN.normalWS);
                float3 V = normalize(IN.viewDirWS);

                float fresnel = pow(1.0 - saturate(dot(N, V)), _RimPower);
                float rim = fresnel * _RimIntensity;

                float2 uv1 = IN.uv * _SparkleTiling + _Time.y * float2(_SparkleSpeedX, _SparkleSpeedY);
                float2 uv2 = IN.uv.yx * (_SparkleTiling * 1.23) + _Time.y * float2(-_SparkleSpeedY, _SparkleSpeedX);

                float s1 = SAMPLE_TEXTURE2D(_SparkleTex, sampler_SparkleTex, uv1).r;
                float s2 = SAMPLE_TEXTURE2D(_SparkleTex, sampler_SparkleTex, uv2).r;
                float sparkleRaw = saturate(s1 * 0.65 + s2 * 0.35);
                float sparkle = pow(sparkleRaw, _SparklePower) * _SparkleStrength;

                float3 col = _Tint.rgb;
                col += _InnerGlow * _Tint.rgb * (0.5 + 0.5 * fresnel);
                col += _RimColor.rgb * rim;
                col += sparkle * _RimColor.rgb;

                float alpha = _Opacity + fresnel * (_Opacity * 1.2) + sparkle * 0.08;
                alpha = saturate(alpha);

                return half4(col, alpha);
            }
            ENDHLSL
        }
    }
}
