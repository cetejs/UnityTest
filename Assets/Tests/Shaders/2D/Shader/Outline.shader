Shader "Custom/2D/Outline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineSize ("Outline Size", Range(0, 10)) = 1
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
            float _OutlineSize;

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

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float2 offset = _MainTex_TexelSize.xy * _OutlineSize;
                fixed4 col = tex2D(_MainTex, i.uv);

                float outline = 0.0; // 计算八个方向的采样值，剔除周围完全透明的像素
                outline += tex2D(_MainTex, i.uv + float2(-offset.x, 0)).a;
                outline += tex2D(_MainTex, i.uv + float2(offset.x, 0)).a;
                outline += tex2D(_MainTex, i.uv + float2(0, -offset.y)).a;
                outline += tex2D(_MainTex, i.uv + float2(0, offset.y)).a;
                outline += tex2D(_MainTex, i.uv + float2(-offset.x, -offset.y)).a;
                outline += tex2D(_MainTex, i.uv + float2(-offset.x, offset.y)).a;
                outline += tex2D(_MainTex, i.uv + float2(offset.x, -offset.y)).a;
                outline += tex2D(_MainTex, i.uv + float2(offset.x, offset.y)).a;

                float isOutline = step(0.05, outline) * (1 - col.a); // 如果周围有透明像素，则认为是轮廓，并且自身透明度越小越明显
                col = lerp(col, _OutlineColor, isOutline);
                return col;
            }
            
            ENDCG
        }
    }
}