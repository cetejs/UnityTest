Shader "Custom/2D/Shadow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1, 1, 1, 1)
        _ShadowX ("Shadow X", Range(-0.5, 0.5)) = 0
        _ShadowY ("Shadow Y", Range(-0.5, 0.5)) = 0
        _ShadowAlpha("Shadow Alpha", Range(0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float4 _Color;
            fixed _ShadowX;
            fixed _ShadowY;
            fixed _ShadowAlpha;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * _Color;
                fixed4 shadow = tex2D(_MainTex, i.uv + fixed2(_ShadowX, _ShadowY));
                col.a = max(col.a, shadow.a * _ShadowAlpha * _Color);
                return col;
            }
            ENDCG
        }
    }
}
