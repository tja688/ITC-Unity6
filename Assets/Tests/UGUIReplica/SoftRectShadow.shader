Shader "Tests/UI/SoftRectShadow"
{
    Properties
    {
        [PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
        _Color("Tint", Color) = (0, 0, 0, 0.25)
        _Softness("Softness", Range(0.001, 0.6)) = 0.16
        _Radius("Corner Radius", Range(0.0, 0.5)) = 0.16
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t
            {
                float4 vertex : POSITION;
                float4 color : COLOR;
                float2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 uv : TEXCOORD0;
            };

            fixed4 _Color;
            float _Softness;
            float _Radius;

            v2f vert(appdata_t v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.texcoord;
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 p = (i.uv * 2.0) - 1.0;
                float radius = saturate(_Radius);
                float2 ext = float2(1.0 - radius, 1.0 - radius);

                float2 d = abs(p) - ext;
                float outside = length(max(d, 0.0)) + min(max(d.x, d.y), 0.0) - radius;

                float softness = max(_Softness, 0.001);
                float alpha = saturate(1.0 - smoothstep(0.0, softness, outside));

                fixed4 col = i.color;
                col.a *= alpha;
                return col;
            }
            ENDCG
        }
    }
}
