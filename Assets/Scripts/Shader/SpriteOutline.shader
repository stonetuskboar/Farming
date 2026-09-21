Shader "Custom/SpriteOutline"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}

        _Color ("Tint", Color) = (1,1,1,1)

        _OutlineColor ("Outline Color", Color) = (1,1,1,1)
        _OutlineSize ("Outline Size", Range(0, 10)) = 1

        [MaterialToggle] PixelSnap ("Pixel Snap", Float) = 0
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
            HLSLPROGRAM

            #pragma vertex SpriteVert
            #pragma fragment SpriteFrag
            #pragma target 2.0

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            float4 _MainTex_TexelSize;

            float4 _Color;
            float4 _OutlineColor;
            float _OutlineSize;

            Varyings SpriteVert(Attributes input)
            {
                Varyings output;

                output.positionCS =
                    TransformObjectToHClip(input.positionOS.xyz);

                output.uv = input.uv;
                output.color = input.color * _Color;

                return output;
            }

            float GetAlpha(float2 uv)
            {
                return SAMPLE_TEXTURE2D(
                    _MainTex,
                    sampler_MainTex,
                    uv
                ).a;
            }

            half4 SpriteFrag(Varyings input) : SV_Target
            {
                float4 spriteColor =
                    SAMPLE_TEXTURE2D(
                        _MainTex,
                        sampler_MainTex,
                        input.uv
                    );

                spriteColor *= input.color;

                float2 texel =
                    _MainTex_TexelSize.xy * _OutlineSize;

                // 8方向采样
                float alpha =
                    GetAlpha(input.uv + float2( texel.x,  0)) +
                    GetAlpha(input.uv + float2(-texel.x,  0)) +
                    GetAlpha(input.uv + float2( 0,  texel.y)) +
                    GetAlpha(input.uv + float2( 0, -texel.y)) +
                    GetAlpha(input.uv + float2( texel.x,  texel.y)) +
                    GetAlpha(input.uv + float2(-texel.x,  texel.y)) +
                    GetAlpha(input.uv + float2( texel.x, -texel.y)) +
                    GetAlpha(input.uv + float2(-texel.x, -texel.y));

                alpha = saturate(alpha);

                // 当前像素没有Sprite，
                // 但附近有Sprite => 描边
                float outlineMask =
                    saturate(alpha * 2.0) *
                    (1.0 - spriteColor.a);

                float4 finalColor =
                    lerp(
                        spriteColor,
                        _OutlineColor,
                        outlineMask
                    );

                // 保证描边也有透明度
                finalColor.a =
                    max(
                        spriteColor.a,
                        outlineMask * _OutlineColor.a
                    );

                return finalColor;
            }

            ENDHLSL
        }
    }
}