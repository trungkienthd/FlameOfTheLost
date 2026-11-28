Shader "Unlit/ElectricRing"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainColor ("Color", Color) = (0,0,0,1)
        _GradientTex ("Gradient", 2D) = "white" {}
        _ElectricityTex1 ("ElectricityTex1", 2D) = "white" {}
        _ElectricityTex2 ("ElectricityTex2", 2D) = "white" {}
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
            sampler2D _ElectricityTex1;
            float4 _ElectricityTex1_ST;
            sampler2D _ElectricityTex2;
            float4 _ElectricityTex2_ST;
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
                float2 uv1 = i.uv + (_Time.y * float2(-1, 0));
                float2 uv2 = i.uv * float2(2, -0.02) + (_Time.y * float2(1.36, 0.63));

                float noise = 0;
                Unity_GradientNoise_float(uv2, 6, noise);
                noise = sin(smoothstep(0.3, 1.45, noise));

                float2 gradient = tex2D(_GradientTex, i.uv) * float2(0, 0.3);

                uv1 = lerp(uv1, noise, gradient);

                fixed4 electricity1 = tex2D(_ElectricityTex1, uv1);

                uv1 = i.uv + (_Time.y * float2(-1.5, 0));
                uv2 = i.uv * float2(1, -0.02) + (_Time.y * float2(1.36, 0.63));

                Unity_GradientNoise_float(uv2, 6, noise);
                noise = sin(noise);

                gradient = tex2D(_GradientTex, i.uv) * float2(0, 0.35);

                uv1 = lerp(uv1, noise, gradient);

                fixed4 electricity2 = tex2D(_ElectricityTex2, uv1);

                fixed4 col = smoothstep(0.08, 0.53, electricity1 + electricity2);

                

                col.a = col.r * _MainColor.a;
                col *= lerp(_MainColor, float4(1,1,1,1), 0.3);
                col.rgb *= col.a;

                UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
            }
            ENDCG
        }
    }
}
