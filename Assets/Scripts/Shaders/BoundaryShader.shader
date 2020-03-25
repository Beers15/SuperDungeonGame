// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/BoundaryShader"
{
    Properties
    {
		_FluidGradient ("Fluid Gradient Color", 2D) = "white" {}
		_FluidTex ("Fluid Detail", 2D) = "white" {}
		_FOWTex ("Fog of War", 2D) = "white" {}
		_Oscillation ("Fluid Oscillation Level", Range(0, 1)) = 0
		_Detail ("Fluid Detail Weight", Range(0, 1)) = 0
		_Bias ("Fluid Level Bias", Range(-1, 1)) = 0
		_Flow ("Fluid Flowing Speed", Range(0, 10)) = 1
		_MapWidthHeight ("Map width and height", Vector) = (1, 1, 0, 0)
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
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
				float3 world : TEXCOORD0;
                //UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _FluidTex;
			sampler2D _FluidGradient;
			sampler2D _FOWTex;
            float4 _FluidTex_ST;
			float4 _FluidGradient_ST;
			float4 _FOWTex_ST;
			
			half _Oscillation;
			half _Detail;
			half _Bias;
			half _Flow;
			float4 _MapWidthHeight;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.world = mul(unity_ObjectToWorld, v.vertex).xyz;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half detail = (tex2D(_FluidTex, (_Flow * (_Time[0] * half2(1, 1))) + TRANSFORM_TEX(i.world.xz, _FluidTex)).r - 0.5) * 2 * _Detail;
				half time_osc = _SinTime[3] * _Oscillation;
				half weight = clamp(1 + time_osc + detail + _Bias, 0.01, 0.99);
				float4 col = tex2D(_FluidGradient, half2(weight, 0));
				col = lerp(float4(0, 0, 0, 0), col, tex2D(_FOWTex, i.world.xz / _MapWidthHeight.xy).a); 
				
                return col;
            }
            ENDCG
        }
    }
}
