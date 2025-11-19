Shader "Custom/PainterlyLightingURP"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        [HDR]_SpecularColor("Specular Color", Color) = (1,1,1,1)

        _MainTex ("MainTex", 2D) = "white" {}
        [Normal]_Normal("Normal", 2D) = "bump" {}
        _NormalStrength("Normal Strength", Range(-2, 2)) = 1

        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

        _PainterlyGuide("Painterly Guide", 2D) = "white" {}
        _ShadingGradient("Shading Gradient", 2D) = "white" {}
        _PainterlySmoothness("Painterly Smoothness", Range(0,1)) = 0.1
    }

    SubShader
    {
        Tags 
        { 
            "RenderType"="Opaque"
            "Queue"="Geometry"
        }

        Pass
        {
            Name "ForwardLit"
            Tags { "LightMode" = "UniversalForward" }

            // 🔥 FIX: macht Shader wirklich opaque
            Blend Off
            ZWrite On
            Cull Back

            HLSLPROGRAM

            // URP Lighting & Keywords
            #pragma vertex vert
            #pragma fragment frag

            #pragma multi_compile_fog
            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _SHADOWS_SOFT

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            // Material variables
            CBUFFER_START(UnityPerMaterial)
                float4 _Color;
                float4 _SpecularColor;
                float _Metallic;
                float _Glossiness;
                float _PainterlySmoothness;
                float _NormalStrength;
            CBUFFER_END

            TEXTURE2D(_MainTex);        SAMPLER(sampler_MainTex);
            TEXTURE2D(_Normal);         SAMPLER(sampler_Normal);
            TEXTURE2D(_PainterlyGuide); SAMPLER(sampler_PainterlyGuide);
            TEXTURE2D(_ShadingGradient);SAMPLER(sampler_ShadingGradient);

            // -------- VERTEX DATA --------
            struct Attributes
            {
                float4 positionOS : POSITION;
                float3 normalOS : NORMAL;
                float4 tangentOS : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 posCS : SV_POSITION;
                float3 posWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;
                float3 tangentWS : TEXCOORD3;
                float2 uv : TEXCOORD0;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                OUT.posWS = TransformObjectToWorld(IN.positionOS.xyz);
                OUT.posCS = TransformWorldToHClip(OUT.posWS);

                VertexNormalInputs n = GetVertexNormalInputs(IN.normalOS, IN.tangentOS);
                OUT.normalWS = n.normalWS;
                OUT.tangentWS = n.tangentWS;

                OUT.uv = IN.uv;
                return OUT;
            }

            // -------- PAINTERLY LIGHTING --------
            float3 PainterlyLight(float3 posWS, float3 normalWS, float2 uv)
            {
                float3 viewDir = GetWorldSpaceNormalizeViewDir(posWS);

                float3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, uv).rgb * _Color.rgb;
                float painterGuide = SAMPLE_TEXTURE2D(_PainterlyGuide, sampler_PainterlyGuide, uv).r;

                float3 result = 0;

                // --- Main Light ---
                Light mainLight = GetMainLight();
                float3 L = normalize(mainLight.direction);

                float ndl = saturate(dot(normalWS, L) + 0.2);

                float diff = smoothstep(
                    painterGuide - _PainterlySmoothness,
                    painterGuide + _PainterlySmoothness,
                    ndl
                );

                float3 grad = SAMPLE_TEXTURE2D(_ShadingGradient, sampler_ShadingGradient, float2(diff, 0)).rgb;

                float3 refl = reflect(-L, normalWS);
                float vDotR = dot(viewDir, refl);

                float specThreshold = painterGuide + _Glossiness;

                float spec = smoothstep(
                    specThreshold - _PainterlySmoothness,
                    specThreshold + _PainterlySmoothness,
                    vDotR
                );

                float3 specCol = _SpecularColor.rgb * mainLight.color * spec * _Glossiness;

                result += (albedo * grad + specCol) * mainLight.shadowAttenuation;

                // --- Additional Lights ---
                uint count = GetAdditionalLightsCount();
                for (uint i = 0; i < count; i++)
                {
                    Light Lgt = GetAdditionalLight(i);
                    float3 L2 = normalize(Lgt.direction);

                    float ndl2 = saturate(dot(normalWS, L2) + 0.2);

                    float diff2 = smoothstep(
                        painterGuide - _PainterlySmoothness,
                        painterGuide + _PainterlySmoothness,
                        ndl2
                    );

                    float3 grad2 = SAMPLE_TEXTURE2D(_ShadingGradient, sampler_ShadingGradient, float2(diff2, 0)).rgb;

                    float3 refl2 = reflect(-L2, normalWS);
                    float vDotR2 = dot(viewDir, refl2);

                    float spec2 = smoothstep(
                        specThreshold - _PainterlySmoothness,
                        specThreshold + _PainterlySmoothness,
                        vDotR2
                    );

                    float3 specCol2 = _SpecularColor.rgb * Lgt.color * spec2 * _Glossiness;

                    result += (albedo * grad2 + specCol2) * Lgt.distanceAttenuation;
                }

                return result;
            }

            // -------- FRAGMENT SHADER --------
            float4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv;

                float3 nTS = UnpackNormalScale(
                    SAMPLE_TEXTURE2D(_Normal, sampler_Normal, uv),
                    _NormalStrength
                );

                float3 binormalWS = cross(IN.normalWS, IN.tangentWS);
                float3x3 TBN = float3x3(IN.tangentWS, binormalWS, IN.normalWS);
                float3 normalWS = normalize(mul(nTS, TBN));

                float3 col = PainterlyLight(IN.posWS, normalWS, uv);

                return float4(col, 1);
            }

            ENDHLSL
        }
    }
}