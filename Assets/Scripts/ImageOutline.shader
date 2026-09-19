Shader "UI/WhiteOutlineSafeAtlas"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)

        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineWidth ("Outline Width", Range(0, 10)) = 1

        // xy = atlas UV min
        // zw = atlas UV max
        [HideInInspector] _SpriteUVRect ("Sprite UV Rect", Vector) = (0,0,1,1)

        [HideInInspector] _StencilComp ("Stencil Comparison", Float) = 8
        [HideInInspector] _Stencil ("Stencil ID", Float) = 0
        [HideInInspector] _StencilOp ("Stencil Operation", Float) = 0
        [HideInInspector] _StencilWriteMask ("Stencil Write Mask", Float) = 255
        [HideInInspector] _StencilReadMask ("Stencil Read Mask", Float) = 255

        [HideInInspector] _ColorMask ("Color Mask", Float) = 15
        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
            "CanUseSpriteAtlas" = "True"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]

        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
            Name "Default"

            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex   : POSITION;
                float4 color    : COLOR;
                float2 texcoord : TEXCOORD0;

                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex        : SV_POSITION;
                fixed4 color         : COLOR;
                float2 texcoord      : TEXCOORD0;
                float4 worldPosition : TEXCOORD1;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;

            fixed4 _Color;
            fixed4 _OutlineColor;

            float _OutlineWidth;

            float4 _MainTex_TexelSize;
            float4 _SpriteUVRect;
            float4 _ClipRect;

            v2f vert(appdata_t v)
            {
                v2f OUT;

                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(OUT);

                OUT.worldPosition = v.vertex;
                OUT.vertex = UnityObjectToClipPos(v.vertex);

                OUT.texcoord = v.texcoord;
                OUT.color = v.color * _Color;

                return OUT;
            }

            // 判断 UV 是否还处于当前 Sprite 的 Atlas 区域
            float IsInsideSprite(float2 uv)
            {
                float insideX =
                    step(_SpriteUVRect.x, uv.x) *
                    step(uv.x, _SpriteUVRect.z);

                float insideY =
                    step(_SpriteUVRect.y, uv.y) *
                    step(uv.y, _SpriteUVRect.w);

                return insideX * insideY;
            }

            float SampleSpriteAlpha(float2 uv)
            {
                float inside = IsInsideSprite(uv);

                // 非常重要：
                // 超出 Sprite UV 范围后直接返回透明，
                // 不让 Shader 去采样 Atlas 里的相邻 Sprite。
                if (inside < 0.5)
                    return 0.0;

                return tex2D(_MainTex, uv).a;
            }

            fixed4 frag(v2f IN) : SV_Target
            {
                float2 uv = IN.texcoord;

                fixed4 mainCol = tex2D(_MainTex, uv) * IN.color;

                float mainAlpha = mainCol.a;

                // Atlas 的 texel 大小
                float2 texel = _MainTex_TexelSize.xy;

                float2 offset = texel * _OutlineWidth;

                float outlineAlpha = 0.0;

                // 上下左右
                outlineAlpha = max(
                    outlineAlpha,
                    SampleSpriteAlpha(uv + float2( offset.x, 0))
                );

                outlineAlpha = max(
                    outlineAlpha,
                    SampleSpriteAlpha(uv + float2(-offset.x, 0))
                );

                outlineAlpha = max(
                    outlineAlpha,
                    SampleSpriteAlpha(uv + float2(0,  offset.y))
                );

                outlineAlpha = max(
                    outlineAlpha,
                    SampleSpriteAlpha(uv + float2(0, -offset.y))
                );

                // 四个斜方向
                outlineAlpha = max(
                    outlineAlpha,
                    SampleSpriteAlpha(
                        uv + float2( offset.x, offset.y)
                    )
                );

                outlineAlpha = max(
                    outlineAlpha,
                    SampleSpriteAlpha(
                        uv + float2(-offset.x, offset.y)
                    )
                );

                outlineAlpha = max(
                    outlineAlpha,
                    SampleSpriteAlpha(
                        uv + float2(offset.x, -offset.y)
                    )
                );

                outlineAlpha = max(
                    outlineAlpha,
                    SampleSpriteAlpha(
                        uv + float2(-offset.x, -offset.y)
                    )
                );

                // 原图区域不要覆盖成描边色
                float borderAlpha =
                    saturate(outlineAlpha - mainAlpha);

                fixed4 outlineCol = _OutlineColor;
                outlineCol.a *= borderAlpha * IN.color.a;

                fixed4 color;

                // 原图优先显示
                color.rgb = lerp(
                    outlineCol.rgb,
                    mainCol.rgb,
                    mainAlpha
                );

                color.a = max(
                    mainAlpha,
                    outlineCol.a
                );

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(
                    IN.worldPosition.xy,
                    _ClipRect
                );
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip(color.a - 0.001);
                #endif

                return color;
            }

            ENDCG
        }
    }
}