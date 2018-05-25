Shader "Custom/PerRendererData/SimpleGlowCustomVec"{

	Properties{
		[PerRendererData] _Color ("Color", Color) = (0,0,1,1)
		[PerRendererData] _Intensity ("Intensity", float) = 1
		[PerRendererData] _DotVector ("Vector", Vector) = (0,1,0,0)
	}

	SubShader{

		Tags { "RenderType"="Transparent" "Queue"="Transparent"}

		Pass{

			ZWrite Off
			Blend One One
			Cull Off
			ZTest LEqual

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			float4 _Color;
			float _Intensity;
			float4 _DotVector;

			struct appdata{
				float4 vertex : POSITION;
				float3 normal : NORMAL; 
			};

			struct v2f{
				float4 vertex : SV_POSITION;
				float dotValue : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
				UNITY_FOG_COORDS(2)
			};

			v2f vert (appdata v){
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				o.dotValue = saturate(dot(normalize(_DotVector.xyz), v.normal));
				UNITY_TRANSFER_FOG(o,o.vertex);

				#if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
					#if !defined(FOG_DISTANCE)
						#define FOG_DEPTH 1
					#endif
					#define FOG_ON 1
				#endif

				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target{
				fixed4 col = _Color * (_Intensity * saturate(i.dotValue));

				#if FOG_ON
					UNITY_CALC_FOG_FACTOR_RAW(length(_WorldSpaceCameraPos - i.worldPos.xyz));
					col.rgb = lerp(fixed3(0,0,0), col.rgb, saturate(unityFogFactor));
				#endif

				return col;
			}

			ENDCG
		}
	}
}
