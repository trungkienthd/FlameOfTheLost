Shader "Unlit/Flame"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _MainColor ("Main Color", Color) = (1,1,1,1)
        _AuraTex ("Aura Texture", 2D) = "white"
        _FlameTex ("Flame Texture", 2D) = "white"
        _RootTransform ("Root Transform", Vector) = (0,0,0,0)
        _TexScale ("Texture Scale", Vector) = (1,1,1,1)
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
                float4 worldPos : TEXCOORD1;
                fixed4 color: COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 worldPos : TEXCOORD1;
                float4 objPos : TEXCOORD2;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            sampler2D _AuraTex;
            float4 _AuraTex_ST;
            sampler2D _FlameTex;
            float4 _FlameTex_ST;
            float3 _TexScale;
            float3 _RootTransform;
            float _Intensity;
            float4 _MainColor;

            v2f vert (appdata v)
            {
                v2f o;
                o.objPos = v.vertex;
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

            void Unity_Remap_float4(float4 In, float2 InMinMax, float2 OutMinMax, out float4 Out)
            {
                Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            }

            float4 Unity_Ellipse_float(float2 UV, float Width, float Height)
            {
                float d = length((UV * 2 - 1) / float2(Width, Height));
                return saturate((1 - d) / fwidth(d));
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float2 polCoord;
                float origDistance = distance(float3(0,0,0), i.objPos);

                Unity_PolarCoordinates_float(i.uv, float2(0.5, 0.5), 0.88, 3.56, polCoord);
                Unity_Rotate_Degrees_float(polCoord, float2(0.5, 0.5), 90, polCoord);
                fixed4 flame1 = tex2Dlod(_FlameTex, float4((polCoord * float2(0.84, 2.52)) + float2(-0.2, 0.42) * _Intensity * _Time.y, 0,0));
                flame1 *= 1 - (origDistance + 0.47);
                flame1 *= 2;

                Unity_PolarCoordinates_float(i.uv, float2(0.5, 0.5), 1, 3.37, polCoord);
                Unity_Rotate_Degrees_float(polCoord, float2(0.5, 0.5), 90, polCoord);
                fixed4 flame2 = tex2Dlod(_FlameTex, float4((polCoord * float2(0.59, 3.25)) + float2(0.18, -0.66) * _Intensity * _Time.y,0,0));
                flame2 *= origDistance - 0.38;
                flame2 *= 2;

                flame1 -= flame2;

                fixed4 col = clamp(0,1,flame1.r);
                fixed colDDXY = fwidth(col.r);
                colDDXY *= 0.5;

                Unity_Remap_float4(col, float2(0.261 - colDDXY, 0.261 + colDDXY), float2(0, 0.79), col);
                col = saturate(col);
                col *= step(1, 1 - Unity_Ellipse_float(i.uv, 0.6, 0.6));
                
                float alpha = col * i.color.a;
                 
                Unity_PolarCoordinates_float(i.worldPos.xz, _RootTransform.xz, 1, 1 / _TexScale, polCoord);

                fixed4 aura1 = tex2D(_AuraTex, (polCoord * _TexScale.xz) + float2(_Time.y * -0.4, 0));
                fixed4 aura2 = tex2D(_AuraTex, (i.worldPos.xz * _TexScale.xz * float2(0.3, 1)) + float2(_Time.y * 0.2, 0));

                fixed4 aura = aura1 * aura2;
                aura.rgba *= 2;

                col += aura;
                col *= 2;

                col *= i.color;
                col.a = alpha;

                col.rgb *= col.a; 

                UNITY_APPLY_FOG(i.fogCoord, col);
                
                return col;
            }
            ENDCG
        }
    }
}
