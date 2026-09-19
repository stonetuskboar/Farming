Shader "UI/CircleTransition"
{
    Properties
    {
        _Radius ("Radius", Range(0, 2)) = 0
        _Center ("Center", Vector) = (0.5, 0.5, 0, 0)
        _Softness ("Softness", Range(0, 0.1)) = 0
        _Color ("Color", Color) = (0,0,0,1)
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

        Pass
        {
            CGPROGRAM

            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            float _Radius;
            float4 _Center;
            float _Softness;
            fixed4 _Color;

            v2f vert(appdata v)
            {
                v2f o;

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;

                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 pos = i.uv - _Center.xy;

                // 修正屏幕宽高比
                pos.x *= _ScreenParams.x / _ScreenParams.y;

                float distance = length(pos);

                float alpha = 1 - smoothstep(
                    _Radius,
                    _Radius + _Softness,
                    distance
                );

                return fixed4(
                    _Color.rgb,
                    alpha * _Color.a
                );
            }

            ENDCG
        }
    }
}