// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

/*
This shader is used by the Blaze rasteriser. It's mainly used to draw the SDF distance map for letters.
The distance maps then get used by PowerUI to display letters on a UI.
*/
Shader "Blaze Raster Draw" {
	Properties {
	}
	SubShader {
		
		Tags{"RenderType"="Transparent" Queue=Transparent}
		
		Lighting Off
		Blend SrcAlpha Zero
		Cull Off 
		ZWrite On 
		Fog { Mode Off }  
		
		Pass {
			Name "BASE"
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma glsl_no_auto_normalization
			
			#include "UnityCG.cginc"
			
			struct appdata_t {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
			};
			
			appdata_t vert (appdata_t v)
			{
				v.vertex = UnityObjectToClipPos(v.vertex);
				return v;
			}
			
			fixed4 frag (appdata_t i) : COLOR
			{
				return i.color;
			}
			ENDCG 
		}
	} 	

}
