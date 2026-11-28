Shader "Unlit/ElectricBall"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _GradientTex ("Gradient", 2D) = "white" {}
        _Intensity ("Intensity", float) = 1

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
                fixed4 color: COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _ElectricityTex;
            float4 _ElectricityTex_ST;
            sampler2D _GradientTex;
            float _Intensity;
            float4 _MainColor;

            float2 unity_gradientNoise_dir(float2 p)
            {
                p = p % 289;
                float x = (34 * p.x + 1) * p.x % 289 + p.y;
                x = (34 * x + 1) * x % 289;
                x = frac(x / 41) * 2 - 1;
                return normalize(float2(x - floor(x + 0.5), abs(x) - 0.5));
            }

            float unity_gradientNoise(float2 p)
            {
                float2 ip = floor(p);
                float2 fp = frac(p);
                float d00 = dot(unity_gradientNoise_dir(ip), fp);
                float d01 = dot(unity_gradientNoise_dir(ip + float2(0, 1)), fp - float2(0, 1));
                float d10 = dot(unity_gradientNoise_dir(ip + float2(1, 0)), fp - float2(1, 0));
                float d11 = dot(unity_gradientNoise_dir(ip + float2(1, 1)), fp - float2(1, 1));
                fp = fp * fp * fp * (fp * (fp * 6 - 15) + 10);
                return lerp(lerp(d00, d01, fp.y), lerp(d10, d11, fp.y), fp.x);
            }

            void Unity_GradientNoise_float(float2 UV, float Scale, out float Out)
            {
                Out = unity_gradientNoise(UV * Scale) + 0.5;
            }

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 uv1 = i.uv * float2(0.37, 1) + (_Time.y * float2(-1, 0));
                float2 uv2 = i.uv * float2(1, -0.02) + (_Time.y * float2(1.36, 0.63));

                float noise = 0;
                Unity_GradientNoise_float(uv2, 6, noise);
                noise = sin(smoothstep(0.06, 0.92, noise));

                float2 gradient = tex2D(_GradientTex, i.uv) * (float2(0, 0.3) * _Intensity);

                uv1 = lerp(uv1, noise, gradient);

                fixed4 col = tex2D(_MainTex, uv1);

                

                col.a = col.r * i.color.a;
                col *= i.color;
                col.rgb *= col.a;

                UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
            }
            ENDCG
        }
    }
}
