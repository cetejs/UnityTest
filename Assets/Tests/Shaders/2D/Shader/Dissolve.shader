Shader "Custom/2D/Dissolve"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        [NoScaleOffset]
        _DissolveTex ("Dissolve Noise", 2D) = "white" {}
        _DissolveAmount ("Dissolve Amount", Range(0, 1)) = 0.5
        _EdgeColor ("Edge Color", Color) = (1, 1, 1, 1)
        _EdgeWidth ("Edge Width", Range(0, 1)) = 0.1
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
            sampler2D _DissolveTex;
            float _DissolveAmount;
            float4 _EdgeColor;
            float _EdgeWidth;

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
                fixed4 col = tex2D(_MainTex, i.uv);
                fixed4 dissolve = tex2D(_DissolveTex, i.uv).r; // 采样噪声贴图
                float edge = smoothstep(_DissolveAmount, _DissolveAmount + _EdgeWidth, dissolve); // 计算边缘效果 smoothstep(a, b, x)， x = [a, b] => [0, 1]
                float alpha = step(_DissolveAmount, dissolve); // 判断是否在消失范围内 step(a, b) b >= a ? 1 : 0
                col.a *= alpha;
                col.rgb = lerp(_EdgeColor, col.rgb, edge);
                return col;
            }
            
            ENDCG
        }
    }
}