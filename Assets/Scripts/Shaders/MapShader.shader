Shader "Unlit/MapShader" {

	Properties {
		_XTex ("Texture", 2D) = "white" {}
		_YTex ("Texture", 2D) = "white" {}
		_ZTex ("Texture", 2D) = "white" {}
		_FluidGradient ("Fluid Gradient Color", 2D) = "white" {}
		_FluidTex ("Fluid Detail", 2D) = "white" {}
		_FOWTex ("Fog of War Texture", 2D) = "white" {}
		_Oscillation ("Fluid Oscillation Level", Range(0, 1)) = 0
		_Detail ("Fluid Detail Weight", Range(0, 1)) = 0
		_Bias ("Fluid Level Bias", Range(-1, 1)) = 0
		_BaseIntensity ("Base Light Intensity", Range(0, 1)) = 0
		_Flow ("Fluid Flowing Speed", Range(0, 10)) = 1
		_Height ("Fluid height", Range(0, 25)) = 10
		_BaseLightColor ("Base Light Color", Color) = (1, 1, 1, 1)
		_MapWidthHeight ("Width and Height of the map", Vector) = (1, 1, 1, 1)
	}

    SubShader {
	
		Tags {"RenderType"="Opaque" "LightMode"="ForwardBase"}
		
        Pass {
		
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
			
            #include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc"
			
			sampler2D _XTex;
			sampler2D _YTex;
			sampler2D _ZTex;
			sampler2D _FluidGradient;
			sampler2D _FluidTex;
			sampler2D _FOWTex;
			half _Oscillation;
			half _Detail;
			half _Bias;
			half _BaseIntensity;
			half _Flow;
			half _Height;
			float4 _BaseLightColor;
			float2 _MapWidthHeight;
			
			struct i2v {
				float4 pos : POSITION;
				half3 normal : NORMAL;
			};
			
            struct v2f {
                float4 pos : SV_POSITION;
				half3 normal : NORMAL;
				half3 world : TEXCOORD0;
				//UNITY_FOG_COORDS(3)
            };
			
			float4 _XTex_ST;
			float4 _YTex_ST;
			float4 _ZTex_ST;
			float4 _FluidTex_ST;
			float4 _FluidGradient_ST;
			float4 _FOWTex_ST;
            
            v2f vert (i2v v)
            {
                v2f o;
				o.world = v.pos.xyz;
                o.pos = UnityObjectToClipPos(v.pos);
				o.normal = v.normal;
				
                return o;
            }

            float4 frag (v2f i) : SV_Target 
			{
				half detail = (tex2D(_FluidTex, (_Flow * (_Time[0] * half2(1, 1))) + TRANSFORM_TEX(i.world.xz, _FluidTex)).r - 0.5) * 2 * _Detail;
				half time_osc = _SinTime[3] * _Oscillation;
				
				half height = abs(i.world.y / _Height);
				half weight = clamp(height + time_osc + detail + _Bias, 0.01, 0.99);
				
				float4 grd_col = tex2D(_FluidGradient, half2(weight, 0));
				
				float3 norm = abs(i.normal);
				norm = norm / (norm.x + norm.y + norm.z);
				float4 colX = tex2D(_XTex, TRANSFORM_TEX(i.world.zy, _XTex));
				float4 colY = tex2D(_YTex, TRANSFORM_TEX(i.world.xz, _YTex));
				float4 colZ = tex2D(_ZTex, TRANSFORM_TEX(i.world.xy, _ZTex));
				float4 tex_col = (colX * norm.x + colY * norm.y + colZ * norm.z);
				
				float4 col = lerp(grd_col, tex_col * _BaseLightColor, 1 - (grd_col.a));
				col = lerp(float4(0, 0, 0, 0), col, tex2D(_FOWTex, i.world.xz / _MapWidthHeight.xy).a);
				
				return col;
			}
            ENDCG
        }
    } 
	FallBack "Diffuse"
}
