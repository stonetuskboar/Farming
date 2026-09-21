Shader "Game/2D/CropWindLit"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [PerRendererData] _MaskTex ("Mask", 2D) = "white" {}

        _Color ("Tint", Color) = (1,1,1,1)

        [Header(Wind)]
        _WindStrength ("Wind Strength", Range(0, 2)) = 0.3
        _WindSpeed ("Wind Speed", Range(0, 5)) = 1.2
        _WindScale ("Wind Scale", Range(0.01, 10)) = 0.5

        _WindDirection ("Wind Direction", Vector) = (1, 0.15, 0, 0)
        _SwayDirection ("Sway Direction", Vector) = (1, 0.2, 0, 0)

        _BendPower ("Bend Power", Range(0.5, 8)) = 2.5
        _BottomLock ("Bottom Lock", Range(0, 1)) = 0.1

        _GustStrength ("Gust Strength", Range(0, 2)) = 1
        _GustScale ("Gust Scale", Range(0.01, 5)) = 0.18
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "RenderType" = "Transparent"
            "RenderPipeline" = "UniversalPipeline"
            "CanUseSpriteAtlas" = "True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            Tags
            {
                "LightMode" = "Universal2D"
            }

            HLSLPROGRAM

            #pragma vertex CombinedShapeLightVertex
            #pragma fragment CombinedShapeLightFragment

            // URP 2D Lights
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_0 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_1 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_2 __
            #pragma multi_compile USE_SHAPE_LIGHT_TYPE_3 __

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/LightingUtility.hlsl"

            struct Attributes
            {
                float3 positionOS : POSITION;
                float4 color      : COLOR;
                float2 uv         : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color      : COLOR;
                float2 uv         : TEXCOORD0;

                half2 lightingUV  : TEXCOORD1;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            TEXTURE2D(_MaskTex);
            SAMPLER(sampler_MaskTex);

            CBUFFER_START(UnityPerMaterial)

                float4 _Color;

                float _WindStrength;
                float _WindSpeed;
                float _WindScale;

                float4 _WindDirection;
                float4 _SwayDirection;

                float _BendPower;
                float _BottomLock;

                float _GustStrength;
                float _GustScale;

            CBUFFER_END


            // MaterialPropertyBlock 传进来
            //
            // xy = Atlas UV min
            // zw = Atlas UV size
            float4 _UVRect;


            // URP 2D Renderer shape-light textures.
            // CombinedShapeLightShared.hlsl samples these when the
            // corresponding USE_SHAPE_LIGHT_TYPE_n variant is enabled.
            #if USE_SHAPE_LIGHT_TYPE_0
            SHAPE_LIGHT(0)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_1
            SHAPE_LIGHT(1)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_2
            SHAPE_LIGHT(2)
            #endif

            #if USE_SHAPE_LIGHT_TYPE_3
            SHAPE_LIGHT(3)
            #endif


            // -------------------------------------------------
            // Noise
            // -------------------------------------------------

            float Hash21(float2 p)
            {
                p = frac(p * float2(123.34, 456.21));
                p += dot(p, p + 45.32);

                return frac(p.x * p.y);
            }


            float ValueNoise(float2 p)
            {
                float2 i = floor(p);
                float2 f = frac(p);

                f = f * f * (3.0 - 2.0 * f);

                float a = Hash21(i);
                float b = Hash21(i + float2(1, 0));
                float c = Hash21(i + float2(0, 1));
                float d = Hash21(i + float2(1, 1));

                return lerp(
                    lerp(a, b, f.x),
                    lerp(c, d, f.x),
                    f.y
                );
            }


            float FBM(float2 p)
            {
                float n = 0;

                n += ValueNoise(p) * 0.625;
                n += ValueNoise(p * 2.03 + 13.1) * 0.25;
                n += ValueNoise(p * 4.01 + 37.7) * 0.125;

                return n;
            }


            // -------------------------------------------------
            // Atlas UV -> Sprite local UV 0~1
            // -------------------------------------------------

            float2 GetNormalizedSpriteUV(float2 atlasUV)
            {
                float2 size =
                    max(_UVRect.zw, float2(0.00001, 0.00001));

                return saturate(
                    (atlasUV - _UVRect.xy) / size
                );
            }


            // -------------------------------------------------
            // Wind
            // -------------------------------------------------

            float2 GetCropWindOffset(
                float2 spriteUV,
                float3 positionWS
            )
            {
                float height = spriteUV.y;


                // 底部锁定
                float bend = saturate(
                    (height - _BottomLock)
                    /
                    max(1.0 - _BottomLock, 0.0001)
                );

                 bend = pow(bend, _BendPower);

                float2 windDir =
                    normalize(
                        _WindDirection.xy
                        + float2(0.0001, 0)
                    );


                // ------------------------
                // 中尺度风
                // ------------------------

                float2 windUV =
                    positionWS.xy * _WindScale
                    - windDir
                    * _Time.y
                    * _WindSpeed;

                float wind =
                    FBM(windUV) * 2.0 - 1.0;


                // ------------------------
                // 大尺度阵风
                // ------------------------

                float2 gustUV =
                    positionWS.xy * _GustScale
                    - windDir
                    * _Time.y
                    * _WindSpeed
                    * 0.6;

                float gustNoise = FBM(gustUV);

                float gust =
                    lerp(
                        1.0,
                        gustNoise,
                        saturate(_GustStrength)
                    );


                // ------------------------
                // 细小周期波
                // ------------------------

                float wave =
                    sin(
                        dot(positionWS.xy, windDir) * 1.6
                        - _Time.y * _WindSpeed * 3.0
                    );


                float finalWind =
                    wind * 0.75
                    + wave * 0.25;

                finalWind *= gust;


                float2 swayDir =
                    normalize(
                        _SwayDirection.xy
                        + float2(0.0001, 0)
                    );


                return
                    swayDir
                    * finalWind
                    * _WindStrength
                    * bend;
            }


            // =================================================
            // Vertex
            // =================================================

            Varyings CombinedShapeLightVertex(Attributes v)
            {
                Varyings o = (Varyings)0;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);


                // Sprite 本地 UV
                float2 spriteUV =
                    GetNormalizedSpriteUV(v.uv);


                // 原始 world pos
                float3 positionWS =
                    TransformObjectToWorld(v.positionOS);


                // 风偏移
                float2 windOffset =
                    GetCropWindOffset(
                        spriteUV,
                        positionWS
                    );


                // 直接改世界空间位置
                positionWS.xy += windOffset;


                // 注意：
                // 一定用“风吹之后”的 positionWS
                // 去重新算屏幕位置
                o.positionCS =
                    TransformWorldToHClip(positionWS);


                o.uv = v.uv;

                o.color =
                    v.color * _Color;


                // 2D Light 需要屏幕空间 Lighting UV
                //
                // 风吹之后重新计算，
                // 否则 Sprite 已经偏移，
                // 光照采样位置却还在原地。
                float4 screenPos =
                    ComputeScreenPos(o.positionCS);

                o.lightingUV =
                    screenPos.xy / screenPos.w;


                return o;
            }


            // =================================================
            // URP 2D Lighting
            // =================================================

            #include "Packages/com.unity.render-pipelines.universal/Shaders/2D/Include/CombinedShapeLightShared.hlsl"


            half4 CombinedShapeLightFragment(Varyings i)
                : SV_Target
            {
                half4 main =
                    SAMPLE_TEXTURE2D(
                        _MainTex,
                        sampler_MainTex,
                        i.uv
                    );

                main *= i.color;


                half4 mask =
                    SAMPLE_TEXTURE2D(
                        _MaskTex,
                        sampler_MaskTex,
                        i.uv
                    );


                SurfaceData2D surfaceData;
                InputData2D inputData;


                InitializeSurfaceData(
                    main.rgb,
                    main.a,
                    mask,
                    surfaceData
                );


                InitializeInputData(
                    i.uv,
                    i.lightingUV,
                    inputData
                );


                return CombinedShapeLightShared(
                    surfaceData,
                    inputData
                );
            }

            ENDHLSL
        }
    }
}