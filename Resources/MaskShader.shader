// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "PowerUI/Camera Mask" {
	Properties {
		_Mask  ("Mask", 2D) = "white" {}
	}

	SubShader {

		Tags{"RenderType"="Transparent" Queue=Transparent}
		
		ColorMask 0
		ZWrite On
		AlphaTest Less .5
		
		BindChannels {
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}
		
		Pass {	
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
			};
			
			sampler2D _Mask;
			uniform float4 _Mask_ST;
			
			v2f vert (appdata_t v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = v.texcoord;
				return o;
			}

			fixed4 frag (v2f i) : COLOR
			{
				fixed4 col = tex2D(_Mask, i.texcoord);
				
				return col;
			}
			ENDCG 
		}
	} 	

}
