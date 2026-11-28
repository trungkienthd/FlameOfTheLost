Shader "Unlit/MagicCircle1"
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
        Blend One One
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
                fixed4 aura1 = tex2D(_AuraTex, (i.worldPos.xz * _TexScale.xz) + float2(_Time.y * -0.4, 0));
                fixed4 aura2 = tex2D(_AuraTex, (i.worldPos.xz * _TexScale.xz) + float2(_Time.y * -0.23, 0.1));

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
