Shader "Custom/VeryPrimitiveLighting"{

	Properties{
		_MainTex ("Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_Ambient ("Ambient Light", Color) = (0.25, 0.25, 0.25, 0.25)
	}

	SubShader{

		Tags { "RenderType"="Opaque" }

		Pass{

			Tags{"lightMode" = "ForwardBase"}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fwdbase
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed4 _Color;
			fixed4 _Ambient;
			fixed4 _LightColor0;

			struct appdata{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldNormal : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float3 lightDir : TEXCOORD3;
			};
			
			v2f vert (appdata v){
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.lightDir = WorldSpaceLightDir(v.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target{
				fixed4 diff = (saturate(dot(normalize(i.lightDir), normalize(i.worldNormal))) * _LightColor0) + _Ambient;
				fixed4 col = (tex2D(_MainTex, i.uv) * _Color) * diff;
				return col;
			}
			ENDCG
		}
	}
}
