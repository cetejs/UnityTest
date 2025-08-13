Shader "Custom/2D/Glitch"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _JitterDisplacement ("Jitter Displacement", Range(0, 1)) = 0.05
        _JitterThreshold ("Jitter Threshold", Range(0, 1)) = 0
        _VerticalJumpAmount ("Vertical Jump Amount", Range(0, 1)) = 0
        _VerticalJumpTime ("Vertical Jump Time", Range(0, 1)) = 0
        _HorizontalShake ("Horizontal Shake", Range(0, 1)) = 0.05
        _ColorDriftAmount ("Color Drift Amount", Range(0, 1)) = 0
        _ColorDriftTime ("Color Drift Time", Range(0, 1)) = 0
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

            float _JitterDisplacement;
            float _JitterThreshold;
            float _VerticalJumpAmount;
            float _VerticalJumpTime;
            float _HorizontalShake;
            float _ColorDriftAmount;
            float _ColorDriftTime;

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

            float rand(float2 value)
            {
                return frac(sin(dot(value, float2(12.9898, 78.233))) * 43758.5453);
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
                float u = i.uv.x;
                float v = i.uv.y;

                // Scan line jitter
                float jitter = rand(float2(v, _Time.x)) * 2 - 1;
                jitter *= step(_JitterThreshold, abs(jitter)) * _JitterDisplacement;

                // Vertical jump
                float jump = lerp(v, frac(v + _VerticalJumpTime), _VerticalJumpAmount);

                // Horizontal shake
                float shake = (rand(float2(_Time.x, 2)) - 0.5) * _HorizontalShake;

                // Color drift
                float drift = sin(jump + _ColorDriftTime) * _ColorDriftAmount;

                half4 src1 = tex2D(_MainTex, frac(float2(u + jitter + shake, jump)));
                half4 src2 = tex2D(_MainTex, frac(float2(u + jitter + shake + drift, jump)));

                return half4(src1.r, src2.g, src1.b, src1.a);
            }
            ENDCG
        }
    }
}