Shader "Custom/2D/InnerOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineThickness ("Outline Thickness", Range(0, 10)) = 1
        _OutlineAlpha ("Outline Alpha", Range(0, 1)) = 0.05
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
            float4 _MainTex_TexelSize;
            float4 _OutlineColor;
            float _OutlineThickness;
            float _OutlineAlpha;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            fixed3 Tex2DWithOffset(float2 offset, float2 uv, sampler2D tex)
            {
                return tex2D(tex, uv + offset * _MainTex_TexelSize.xy).rgb;
            }

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                // 计算内描边的强度，上下左右差值越大越强
                fixed3 innert = abs(Tex2DWithOffset(float2(_OutlineThickness, 0), i.uv, _MainTex) - Tex2DWithOffset(float2(-_OutlineThickness, 0), i.uv, _MainTex)) +
                                abs(Tex2DWithOffset(float2(0, _OutlineThickness), i.uv, _MainTex) - Tex2DWithOffset(float2(0, -_OutlineThickness), i.uv, _MainTex));
                innert *= col.a * _OutlineAlpha; // 剔除透明像素点
                col.rgb += length(innert) * _OutlineColor.rgb;
                return col;
            }
            ENDCG
        }
    }
}