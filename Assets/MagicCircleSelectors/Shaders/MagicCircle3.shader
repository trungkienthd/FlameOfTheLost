Shader "Unlit/MagicCircle3"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AuraTex ("Texture", 2D) = "white" {}

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
        Cull Off

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
            sampler2D _AuraTex;
            float4 _AuraTex_ST;

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

            void Unity_Rotate_Degrees_float(float2 UV, float2 Center, float Rotation, out float2 Out)
            {
                Rotation = Rotation * (3.1415926f/180.0f);
                UV -= Center;
                float s = sin(Rotation);
                float c = cos(Rotation);
                float2x2 rMatrix = float2x2(c, -s, s, c);
                rMatrix *= 0.5;
                rMatrix += 0.5;
                rMatrix = rMatrix * 2 - 1;
                UV.xy = mul(UV.xy, rMatrix);
                UV += Center;
                Out = UV;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);
                float2 auraUV;

                Unity_Rotate_Degrees_float(i.uv, float2(0.5f, 0.5f), 30, auraUV);
                float aura1 = tex2D(_AuraTex, auraUV * float2(.11, 1) + float2(_Time.y * -0.19, 0));

                Unity_Rotate_Degrees_float(i.uv, float2(0.5f, 0.5f), 150, auraUV);
                float aura2 = tex2D(_AuraTex, auraUV * float2(.11, 1) + float2(_Time.y * -0.19, 0));

                Unity_Rotate_Degrees_float(i.uv, float2(0.5f, 0.5f), 270, auraUV);
                float aura3 = tex2D(_AuraTex, auraUV * float2(.11, 1) + float2(_Time.y * -0.19, 0));

                float aura = aura1 + aura2 + aura3;
                float mask = col.g * aura;

                col.a = col.r;
                col.r += aura;
                col.r += mask;

                col.rgb = col.r;

                col *= i.color;
                col.rgb *= col.a;

                UNITY_APPLY_FOG(i.fogCoord, col);

                return col;
            }
            ENDCG
        }
    }
}
