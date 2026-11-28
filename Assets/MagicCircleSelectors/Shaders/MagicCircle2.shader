Shader "Unlit/MagicCircle2"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GlowColor ("Glow Color", Color) = (0,0,0,0)

    }
    SubShader
    {
        Tags { 
        "RenderType"="Transparent"
         "Queue"="Transparent"
         }
        Blend One OneMinusSrcAlpha
        ZWrite Off
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
                fixed4 color: COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _GlowColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.worldPos = mul (unity_ObjectToWorld, v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float glowTime = (sin(_Time.y * 3.82f) + 1) / 2;
                fixed4 glow = saturate(col.g - glowTime);
                float smoothcol = smoothstep(.73f, .88f, col.r);
                float outerglow = col.r - smoothcol;

                col = outerglow * glow * 6;
                col.a = outerglow * glow + smoothcol;

                glow.rgb *= _GlowColor;
                glow.a = 1;

                glow += i.color;
                glow = saturate(glow);
                glow *= smoothcol;

                col *= i.color;

                col.rgb += glow.rgb;

                col.rgb *= col.a;

                UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
            }
            ENDCG
        }
    }
}
