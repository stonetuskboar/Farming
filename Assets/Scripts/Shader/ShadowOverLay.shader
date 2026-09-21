Shader "Custom/2D/TilemapCloudShadow"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _Color ("Tilemap Color", Color) = (1,1,1,1)

        [Header(Cloud Shadow)]
        _ShadowColor ("Shadow Color", Color) = (0.35,0.38,0.42,1)
        _ShadowStrength ("Shadow Strength", Range(0,1)) = 0.35

        [Header(Noise)]
        _NoiseScale ("Cloud Scale", Range(0.1,20)) = 2
        _NoiseContrast ("Cloud Contrast", Range(0.1,5)) = 1.5
        _NoiseThreshold ("Cloud Threshold", Range(0,1)) = 0.5

        [Header(Movement)]
        _MoveX ("Cloud Speed X", Float) = 0.01
        _MoveY ("Cloud Speed Y", Float) = 0.005

        [Header(Softness)]
        _CloudSoftness ("Cloud Softness", Range(0.01,1)) = 0.25
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "RenderPipeline"="UniversalPipeline"
            "CanUseSpriteAtlas"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Tags
            {
                "LightMode"="Universal2D"
            }

            HLSLPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            CBUFFER_START(UnityPerMaterial)

            float4 _Color;

            float4 _ShadowColor;
            float _ShadowStrength;

            float _NoiseScale;
            float _NoiseContrast;
            float _NoiseThreshold;

            float _MoveX;
            float _MoveY;

            float _CloudSoftness;

            CBUFFER_END

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
                float3 worldPos : TEXCOORD1;
            };

            // --------------------------------------------------
            // Hash
            // --------------------------------------------------

            float hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);

                return frac(p.x * p.y);
            }

            // --------------------------------------------------
            // Value Noise
            // --------------------------------------------------

            float noise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);

                // Smooth interpolation
                f = f * f * (3.0 - 2.0 * f);

                float a = hash21(i);
                float b = hash21(i + float2(1,0));
                float c = hash21(i + float2(0,1));
                float d = hash21(i + float2(1,1));

                return lerp(
                    lerp(a, b, f.x),
                    lerp(c, d, f.x),
                    f.y
                );
            }

            // --------------------------------------------------
            // Fractal Brownian Motion
            // 多层噪声，让云更加自然
            // --------------------------------------------------

            float fbm(float2 p)
            {
                float value = 0.0;
                float amplitude = 0.5;

                value += noise(p) * amplitude;

                p *= 2.0;
                amplitude *= 0.5;

                value += noise(p) * amplitude;

                p *= 2.0;
                amplitude *= 0.5;

                value += noise(p) * amplitude;

                p *= 2.0;
                amplitude *= 0.5;

                value += noise(p) * amplitude;

                return value;
            }

            Varyings vert(Attributes input)
            {
                Varyings output;

                VertexPositionInputs vertexInput =
                    GetVertexPositionInputs(input.positionOS.xyz);

                output.positionHCS = vertexInput.positionCS;

                output.uv = input.uv;

                output.color = input.color * _Color;

                output.worldPos = vertexInput.positionWS;

                return output;
            }

            half4 frag(Varyings input) : SV_Target
            {
                // --------------------------------------------------
                // 读取 Tilemap 原始颜色
                // --------------------------------------------------

                half4 baseColor =
                    SAMPLE_TEXTURE2D(
                        _MainTex,
                        sampler_MainTex,
                        input.uv
                    );

                baseColor *= input.color;

                // --------------------------------------------------
                // 世界坐标作为云的坐标
                // --------------------------------------------------

                float2 cloudUV = input.worldPos.xy;

                cloudUV *= _NoiseScale;

                // --------------------------------------------------
                // 云移动
                // --------------------------------------------------

                float2 movement =
                    _Time.y *
                    float2(_MoveX, _MoveY);

                cloudUV += movement;

                // --------------------------------------------------
                // 生成云噪声
                // --------------------------------------------------

                float cloud = fbm(cloudUV);

                cloud = saturate(
                    (cloud - 0.5) *
                    _NoiseContrast +
                    0.5
                );

                // --------------------------------------------------
                // 云边缘软化
                // --------------------------------------------------

                float minValue =
                    _NoiseThreshold -
                    _CloudSoftness;

                float maxValue =
                    _NoiseThreshold +
                    _CloudSoftness;

                cloud = smoothstep(
                    minValue,
                    maxValue,
                    cloud
                );

                // --------------------------------------------------
                // 云影 Alpha
                // --------------------------------------------------

                float shadowAlpha =
                    cloud *
                    _ShadowStrength;

                // --------------------------------------------------
                // 关键：
                // 不再让透明区域直接消失。
                //
                // 将原来的 Tilemap 和云影进行 Alpha 合成。
                // --------------------------------------------------

                float baseAlpha = baseColor.a;

                float finalAlpha =
                    baseAlpha +
                    shadowAlpha * (1.0 - baseAlpha);

                // --------------------------------------------------
                // 云影颜色
                // --------------------------------------------------

                float3 shadowColor =
                    baseColor.rgb * _ShadowColor.rgb;

                // --------------------------------------------------
                // Alpha 合成后的 RGB
                //
                // base
                //     +
                // shadow
                // --------------------------------------------------

                float3 finalRGB =
                    baseColor.rgb * baseAlpha +
                    shadowColor * shadowAlpha * (1.0 - baseAlpha);

                // 防止除以 0
                finalRGB =
                    finalAlpha > 0.0001
                    ? finalRGB / finalAlpha
                    : 0;

                return half4(
                    finalRGB,
                    finalAlpha
                );
            }

            ENDHLSL
        }
    }
}