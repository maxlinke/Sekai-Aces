Shader "Custom/Wireframe/UnlitAdditiveWireframe"{

	Properties{
		_Color ("Color", Color) = (0,0,0,1)
		_WireColor ("Wireframe Color", Color) = (1,1,1,1)
		_WireWidth ("Wire Width", Range(0,10)) = 1.0
		_WireSmoothing ("Wire Smoothing", Range(0,10)) = 0.0
	}

	SubShader{

		Tags { "Queue" = "Transparent" "RenderType"="Opaque" }
		LOD 100

		ZWrite Off
		Blend One One

		Pass{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom
			
			#include "UnityCG.cginc"

			fixed4 _Color;
			fixed4 _WireColor;
			float _WireWidth;
			float _WireSmoothing;

			struct appdata{
				float4 vertex : POSITION;
			};

			struct v2f{
				float4 vertex : SV_POSITION;
			};

			struct g2f{
				v2f data;
				float3 baryCoords : TEXCOORD9;
			};
			
			v2f vert (appdata v){
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}

			[maxvertexcount(3)]
			void geom(triangle v2f i[3], inout TriangleStream<g2f> stream){
				g2f g0, g1, g2;

				g0.data = i[0];
				g1.data = i[1];
				g2.data = i[2];

				g0.baryCoords = float3(1,0,0);
				g1.baryCoords = float3(0,1,0);
				g2.baryCoords = float3(0,0,1);

				stream.Append(g0);
				stream.Append(g1);
				stream.Append(g2);
			}
			
			fixed4 frag (g2f i) : SV_Target{
				float3 bary = i.baryCoords;
				float3 deltas = fwidth(bary);
				float3 smoothing = deltas * _WireSmoothing;
				float3 width = deltas * 0.5 * _WireWidth;
				bary = smoothstep(width, width + smoothing, bary);
				float minBary = min(bary.x, min(bary.y, bary.z));
				fixed4 col = lerp(_WireColor, _Color, minBary);
				return col;
			}

			ENDCG
		}
	}
}
