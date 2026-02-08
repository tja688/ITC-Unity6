Shader "Tests/LihuiSpriteMotion"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _BreathAmp ("Breath Amp", Float) = 0
        _BreathSpeed ("Breath Speed", Float) = 1
        _WaveAmp ("Wave Amp", Float) = 0
        _WaveFreq ("Wave Frequency", Float) = 3
        _WaveSpeed ("Wave Speed", Float) = 1.6
        _RuntimePulse ("Runtime Pulse", Float) = 0
        _AlphaMultiplier ("Alpha Multiplier", Range(0,1)) = 1
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

        Cull Off
        Lighting Off
        ZWrite Off
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            HLSLPROGRAM
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
                float2 texcoord : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            float _BreathAmp;
            float _BreathSpeed;
            float _WaveAmp;
            float _WaveFreq;
            float _WaveSpeed;
            float _RuntimePulse;
            float _AlphaMultiplier;

            v2f vert(appdata_t v)
            {
                v2f o;

                float t = _Time.y;
                float breath = sin(t * _BreathSpeed) * _BreathAmp;
                float wave = sin((v.vertex.y * _WaveFreq) + (t * _WaveSpeed)) * _WaveAmp;
                float pulse = _RuntimePulse;
                float scale = 1.0 + breath + pulse;

                float2 xy = v.vertex.xy * scale;
                xy.x += wave;

                o.vertex = UnityObjectToClipPos(float4(xy, v.vertex.z, 1.0));
                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 c = tex2D(_MainTex, i.texcoord) * i.color;
                c.a *= _AlphaMultiplier;
                return c;
            }
            ENDHLSL
        }
    }
}

