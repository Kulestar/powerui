Shader "PowerUI/StandardUI Cull Lit/Normal" {
	Properties {
		_Font ("Font Texture", 2D) = "white" {}
		_MainTex  ("Graphical Atlas", 2D) = "white" {}
	}
	
	SubShader {
		
		Tags{"RenderType"="Transparent" Queue=Transparent}
		
		Cull Back
		
		CGPROGRAM
		
		#pragma surface surf Lambert alpha:blend vertex:vert
		
		struct Input {
			float2 texcoord0;
			float2 texcoord1;
			float2 texcoord2;
			fixed4 color : COLOR;
		};
		
		sampler2D _Font;
		sampler2D _MainTex;
		
		void vert(inout appdata_full i, out Input o){
			
			UNITY_INITIALIZE_OUTPUT(Input, o);
			
			o.texcoord0=i.texcoord;
			o.texcoord1=i.texcoord1;
			o.texcoord2=i.texcoord2;
			
		}
		
		void surf (Input IN, inout SurfaceOutput o) {
			
			fixed4 col = IN.color;
			
			if(IN.texcoord0.y<=1){
				col *= tex2D(_MainTex, IN.texcoord0);
			}
			
			if(IN.texcoord1.y<=1){
				col.a *= smoothstep(IN.texcoord2.x,IN.texcoord2.y,tex2D(_Font,IN.texcoord1).a);
			}
			
			o.Albedo = col.rgb;
			o.Alpha=col.a;
			
		}
		
		ENDCG
	}
	
	Fallback "Diffuse"
}