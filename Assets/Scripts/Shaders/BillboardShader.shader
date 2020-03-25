Shader "Custom/Billboard"
{
    Properties
    {
        _MainTex("Noise", 2D) = "white" {}
    }

    SubShader
    {
        Tags 
        {
            "Queue" = "Transparent"
            "DisableBatching" = "True"
        }

        // Render the object with the texture generated above, and invert the colors
        Pass
        {
            ZTest Always

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            // Properties
            sampler2D _MainTex;
			float4 _MainTex_ST;

            struct vertexInput
            {
                float4 vertex : POSITION;
                float2 texCoord : TEXCOORD0;
            };

            struct vertexOutput
            {
                float4 pos : SV_POSITION;
                float2 texCoord : TEXCOORD0;
            };

            vertexOutput vert(vertexInput input)
            {
                vertexOutput output;
                
                // billboard to camera
                float4 pos = input.vertex;
                pos = mul(UNITY_MATRIX_P, 
                      mul(UNITY_MATRIX_MV, float4(0, 0, 0, 1))
                          + float4(pos.x, pos.z, 0, 0));
                output.pos = pos;                
				output.texCoord = TRANSFORM_TEX(input.texCoord, _MainTex);

                return output;
            }

            float4 frag(vertexOutput input) : COLOR
            {
                //return float4(1,1,1,1); // billboard test
                return tex2D(_MainTex, input.texCoord);
            }

            ENDCG
        }

    }
}