// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "PowerUI/StandardUI Cull/Extruded" {
	Properties {
		_MainTex  ("Texture", 2D) = "white" {}
	}

	SubShader {

		Tags{"RenderType"="Transparent" Queue=Transparent}
		
		Lighting Off 
		Blend SrcAlpha OneMinusSrcAlpha
		Cull Back
		ZWrite On 
		Fog { Mode Off }  
		
		Pass {
			Name "BASE"
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest

			#include "UnityCG.cginc"

			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				half2 texcoord : TEXCOORD0;
			};
			
			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			
			appdata_t vert (appdata_t v)
			{
				v.vertex = UnityObjectToClipPos(v.vertex);
				v.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				return v;
			}

			fixed4 frag (appdata_t i) : COLOR
			{
				return i.color*tex2D(_MainTex, i.texcoord);
			}
			ENDCG 
		}
	} 	

}
