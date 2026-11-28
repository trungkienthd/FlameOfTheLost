Shader "Unlit/MagicCircle5"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _AuraTex ("Aura Texture", 2D) = "white"
        _RootTransform ("Root Transform", Vector) = (0,0,0,0)
        _TexScale ("Texture Scale", Vector) = (1,1,1,1)

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
            sampler2D _AuraTex;
            float4 _AuraTex_ST;
            float3 _TexScale;
            float3 _RootTransform;

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

            void Unity_PolarCoordinates_float(float2 UV, float2 Center, float RadialScale, float LengthScale, out float2 Out)
            {
                float2 delta = UV - Center;
                float radius = length(delta) * 2 * RadialScale;
                float angle = atan2(delta.x, delta.y) * 1.0/6.28 * LengthScale;
                Out = float2(radius, angle);
            }

            fixed4 frag (v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv);

                float2 polCoord; 
                Unity_PolarCoordinates_float(i.worldPos.xz, _RootTransform.xz, 1, 1 / _TexScale, polCoord);

                fixed4 aura1 = tex2D(_AuraTex, (polCoord * _TexScale.xz) + float2(_Time.y * -0.4, 0));
                fixed4 aura2 = tex2D(_AuraTex, (i.worldPos.xz * _TexScale.xz * float2(0.3, 1)) + float2(_Time.y * 0.2, 0));

                fixed4 aura = aura1 * aura2;
                aura.rgba *= 2;

                col.g += aura;
                col.r += aura;

                col.r += col.g;
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
