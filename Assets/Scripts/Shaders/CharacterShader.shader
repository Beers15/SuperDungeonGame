Shader "Unlit/CharacterShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_BaseIntensity ("Base Light Intensity", Range(0, 1)) = 0
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
            #pragma multi_compile_fog

            #include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
			
			half _BaseIntensity;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
				half3 normal : NORMAL;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				
				half3 world_normal = UnityObjectToWorldNormal(v.normal);
				half intensity = _BaseIntensity + (max(0.4, dot(world_normal, _WorldSpaceLightPos0.xyz))) * (1 - _BaseIntensity);
                o.color = _LightColor0 * intensity;
				
				UNITY_TRANSFER_FOG(o, o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
				col.a = 1;
                return col;
            }
            ENDCG
        }
    }
}
