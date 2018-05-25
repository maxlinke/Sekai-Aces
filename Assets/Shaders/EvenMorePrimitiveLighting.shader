Shader "Custom/EvenMorePrimitiveLighting"{

	Properties{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_LightDir ("Light Vector", Vector) = (0,1,0,0)
		_LightCol ("Light Color", Color) = (1,1,1,1)
		_Ambient ("Ambient Light", Color) = (0.25, 0.25, 0.25, 0.25)
		_WorldSpace ("World Space Light Direction", Range(0,1)) = 0.0
	}

	SubShader{

		Tags { "RenderType"="Opaque" }

		Pass{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			float4 _LightDir;
			fixed4 _LightCol;
			fixed4 _Ambient;
			float _WorldSpace;

			struct appdata{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 normal : TEXCOORD1;
			};
			
			v2f vert (appdata v){
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				float3 objNormal = v.normal;
				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				o.normal = lerp(objNormal, worldNormal, _WorldSpace);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target{
				fixed4 diff = (saturate(dot(normalize(_LightDir.xyz), normalize(i.normal))) * _LightCol) + _Ambient;
				fixed4 col = (tex2D(_MainTex, i.uv) * _Color) * diff;
				col.a = 1.0;
				return col;
			}
			ENDCG
		}
	}
}
