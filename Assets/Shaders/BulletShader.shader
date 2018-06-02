Shader "Custom/BulletShader"{

	Properties{
		_CenterColor ("Center Color", Color) = (0,0,1,1)
		_RimColor ("Rim Color", Color) = (1,0,0,1)
		_LowerBound ("Lower Bound", Range(0,1)) = 0
		_UpperBound ("Upper Bound", Range(0,1)) = 1
	}

	SubShader{

		Tags { "RenderType"="Opaque" "Queue"="Geometry"}

		Pass{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma multi_compile_fog

			#include "UnityCG.cginc"

			fixed4 _CenterColor;
			fixed4 _RimColor;
			float _LowerBound;
			float _UpperBound;

			struct appdata{
				float4 vertex : POSITION;
				float3 normal : NORMAL; 
			};

			struct v2f{
				float4 vertex : SV_POSITION;
				float3 worldNormal : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
				UNITY_FOG_COORDS(2)
			};

			v2f vert (appdata v){
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target{
				float viewDot = dot(normalize(_WorldSpaceCameraPos - i.worldPos), normalize(i.worldNormal));
				float lerpFactor = saturate((viewDot - _LowerBound) / (_UpperBound - _LowerBound));
				fixed4 col = lerp(_RimColor, _CenterColor, lerpFactor);
				UNITY_APPLY_FOG(i.fogCoord, col);
				return col;
			}

			ENDCG
		}
	}
}
