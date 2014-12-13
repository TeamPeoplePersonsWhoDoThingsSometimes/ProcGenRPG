Shader "IndieEffects/ShaderDir/BoxBlur" {
	Properties {
		_MainTex ("Base (RGB)", 2D) = "white" { }
	}
	
	CGINCLUDE
	#include "UnityCG.cginc"
	
	sampler2D _MainTex;
	float4 _MainTex_TexelSize;
	
	struct v2f {
		float4 pos : POSITION;
		float2 uv : TEXCOORD0;
	};
	
	v2f vert (appdata_img v)
	{
		v2f o;
		
		o.pos = mul (UNITY_MATRIX_MVP, v.vertex);

		o.uv = v.texcoord.xy;
		
		return o;
	}
	
	half4 fragV (v2f i) : COLOR
	{
		
		float4 col = tex2D(_MainTex, i.uv);
		int radius = 10;
		int count = 0;
		float4 newCol;
		newCol.rgb = (0, 0, 0);
		for (int pix = 0; pix < radius; pix ++) {
			
			float2 uvOff = float2(_MainTex_TexelSize.x, 0.0) * pix;
			if (i.uv.x + uvOff.x <= 1.0) {
				newCol += tex2D(_MainTex, i.uv + uvOff);
				count ++;
			}
			if (i.uv.x - uvOff.x >= 0.0) {
				newCol += tex2D(_MainTex, i.uv - uvOff);
				count ++;
			}
			
		}
		
		newCol = (newCol + col) / (count + 1);
		
		return newCol;
	}
	
	half4 fragH (v2f i) : COLOR
	{
		
		float4 col = tex2D(_MainTex, i.uv);
		int radius = 10;
		int count = 0;
		float4 newCol;
		newCol.rgb = (0, 0, 0);
		for (int pix = 0; pix < radius; pix ++) {
			float2 uvOff = float2(0.0, _MainTex_TexelSize.y) * pix;
			
			if (i.uv.y + uvOff.y <= 1.0) {
				newCol += tex2D(_MainTex, i.uv + uvOff);
				count ++;
			}
			if (i.uv.y - uvOff.y >= 0.0) {
				newCol += tex2D(_MainTex, i.uv - uvOff);
				count ++;
			}
			
		}
		
		newCol = (newCol + col) / (count + 1);
		
		return newCol;
	}
	ENDCG
	
	SubShader {
		Tags { "Queue"="Overlay" "RenderType"="Overlay"}
		
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragH
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			
			ENDCG
		}
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragV
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			
			ENDCG
		}
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragH
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			
			ENDCG
		}
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragV
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			
			ENDCG
		}
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragH
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			
			ENDCG
		}
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragV
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			
			ENDCG
		}
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragV
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			
			ENDCG
		}
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragV
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			
			ENDCG
		}
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragV
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			
			ENDCG
		}
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragV
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			
			ENDCG
		}
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragV
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			
			ENDCG
		}
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragV
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			
			ENDCG
		}
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragV
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			
			ENDCG
		}
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragV
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			
			ENDCG
		}
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragV
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			
			ENDCG
		}
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragV
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			
			ENDCG
		}
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragV
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			
			ENDCG
		}
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragV
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			
			ENDCG
		}
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragV
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			
			ENDCG
		}
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			Lighting Off
			
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragV
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma target 3.0
			
			ENDCG
		}
		
	}
	
	SubShader {
		Tags { "Queue" = "Overlay" }
		
		Pass {
			ZTest Always Cull Off ZWrite Off
			Fog { Mode off }
			Lighting Off
			
			SetTexture [_MainTex] {
				Combine texture
			}
		}
	}
	Fallback off
}
