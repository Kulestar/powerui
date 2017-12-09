// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "PowerUI/StandardUI Cull/Normal" {
	Properties {
		_Font ("Font Texture", 2D) = "white" {}
		_MainTex  ("Graphical Atlas", 2D) = "white" {}
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
			#pragma glsl_no_auto_normalization
			
			struct appdata_t {
				float4 vertex : POSITION;
				half2 texcoord0 : TEXCOORD0;
				half2 texcoord1 : TEXCOORD1;
				half2 texcoord2 : TEXCOORD2;
				fixed4 color : COLOR;
			};
			
			sampler2D _Font;
			uniform float4 _Font_ST;
			
			sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			
			appdata_t vert (appdata_t v) {
				v.vertex = UnityObjectToClipPos( v.vertex );
				return v;
			}
			
			fixed4 frag (appdata_t i) : COLOR {
				fixed4 col = i.color;
				
				if(i.texcoord0.y<=1){
					col *= tex2D(_MainTex, i.texcoord0);
				}
				
				if(i.texcoord1.y<=1){
					col.a *= smoothstep(i.texcoord2.x, i.texcoord2.y,tex2D(_Font,i.texcoord1).a);
				}
				
				return col;
			}
			
			ENDCG
		}
	}
}