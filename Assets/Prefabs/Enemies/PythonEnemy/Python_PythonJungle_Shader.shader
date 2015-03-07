Shader "Custom/Python_PythonJungle_Shader" {
// Source: Aaron Lanterman, June 14, 2014
// Source Notes: Cobbled together from numerous sources
// Editor: Luke Panayioto, Feb 28, 2015
// Editor Notes: Editted for Game
	Properties {
		_BaseTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_SpecColor ("Spec Color", Color) = (1,1,1,1)
	    _Shininess ("Shininess", Range(0.01,1)) = 0.7
	    _Ambience  ("Ambience", Color) = (1,1,1,1)
	}
		
    SubShader {
    	// Directional light colors aren't exposed in "ForwardBase" mode, so we try "Vertex" mode,
    	// which really should be called "Simple" mode, as we can still do custom per-pixel lighting
        Tags { "LightMode" = "Vertex" }
    	 
        Pass {
        	CGPROGRAM
			#include "UnityCG.cginc"
			 
            #pragma vertex vert_specmappixellit
            #pragma fragment frag_specmappixellit
           
           	uniform sampler2D _BaseTex;	
           	uniform float4 _BaseTex_ST;
           	
           	uniform float4 _SpecColor;
           	uniform float4 _Ambience;
           	uniform float _Shininess;
           	
           	struct a2v {    			
           		float4 v: POSITION;
           		float3 n: NORMAL;
           		float2 tc: TEXCOORD0;	
           	};
           	
			struct v2f {				
           		float4 sv: SV_POSITION;
           		float2 tc: TEXCOORD0;   
           		float3 vWorldPos: TEXCOORD1;     
           		float3 nWorld: TEXCOORD2;  
           	};
 
            v2f vert_specmappixellit(a2v input)  {
                v2f output;           
                output.sv = mul(UNITY_MATRIX_MVP, input.v);
                 // To transform normals, we want to use the inverse transpose of upper left 3x3
                // Putting input.n in first argument is like doing trans((float3x3)_World2Object) * input.n;
                output.vWorldPos = mul(_Object2World, input.v).xyz;
                output.nWorld = normalize(mul(input.n, (float3x3) _World2Object));
                output.tc = TRANSFORM_TEX(input.tc, _BaseTex);
                return output;
            }

     		float4 frag_specmappixellit(v2f input) : COLOR {
     			// Unity light position convention is:
                // w = 0, directional light, with x y z pointing in opposite of light direction 
                // w = 1, point light, with x y z indicating position coordinates
                float3 lightDir = normalize(_WorldSpaceLightPos0.xyz - input.vWorldPos * _WorldSpaceLightPos0.w);
                float3 eyeDir = normalize(_WorldSpaceCameraPos.xyz - input.vWorldPos);
   				float3 h = normalize(lightDir + eyeDir);
   				// renormalizing because the GPU's interpolator doesn't know this is a unit vector
   				float3 n = normalize(input.nWorld);
				float3 diff_almost = 2*unity_LightColor0.rgb * max(0, dot(n, lightDir));
				float ndoth = max(0, dot(n, h));
				float3 spec_almost = 2*unity_LightColor0.rgb * _SpecColor.rgb * pow(ndoth, _Shininess*128.0);
            	
     			float4 base = tex2D(_BaseTex, input.tc);
     			float3 output = (diff_almost + 2*UNITY_LIGHTMODEL_AMBIENT.rgb) * base.rgb * _Ambience + spec_almost.rgb * base.a;
     			return(float4(output,1));   
            }

            ENDCG
        }
    }
}