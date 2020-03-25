Shader "Unlit/EnvironmentShader"
{
    Properties
    {
		_MainTex ("Main Texture", 2D) = "white" {}
		_FOWTex ("Fog of War Texture", 2D) = "white" {}
        _MapWidthHeight ("Width and Height of the map", Vector) = (1, 1, 1, 1)
		_BaseIntensity ("Base Light Intensity", Range(0, 1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
			
			float4 _MapWidthHeight;
			float _BaseIntensity;

            struct appdata
            {
                float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
            };

            struct v2f
            {
				float2 uv : TEXCOORD0;
                float2 fowUV : TEXCOORD1;
				float intensity : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

			sampler2D _MainTex;
            sampler2D _FOWTex;
			float4 _MainTex_ST;
            float4 _FOWTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.fowUV = mul(unity_ObjectToWorld, v.vertex).xz;
				
				half3 world_normal = UnityObjectToWorldNormal(v.normal);
				o.intensity = _BaseIntensity + (max(0.4, dot(world_normal, _WorldSpaceLightPos0.xyz))) * (1 - _BaseIntensity);
				
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
				float4 col = tex2D(_MainTex, i.uv);
                col = lerp(float4(0, 0, 0, 0), col, tex2D(_FOWTex, i.fowUV / _MapWidthHeight.xy).a) * i.intensity;
                return col;
            }
            ENDCG
        }
    }
}
