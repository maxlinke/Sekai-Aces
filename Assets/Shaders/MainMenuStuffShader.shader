Shader "Custom/MainMenuStuffShader"{

	Properties{
		_MainTex ("Main Texture", 2D) = "white" {}
		_Color ("Color", Color) = (1,1,1,1)
		_WireColor ("Wireframe Color", Color) = (0,0,0,1)
		_WireWidth ("Wire Width", Range(0,10)) = 1.0
		_WireSmoothing ("Wire Smoothing", Range(0,10)) = 0.0
	}

	SubShader{

		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass{

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma geometry geom
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _MainTex_ST;

			fixed4 _Color;
			fixed4 _WireColor;
			float _WireWidth;
			float _WireSmoothing;

			struct appdata{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float viewDot : TEXCOORD1;
				UNITY_FOG_COORDS(2)
			};

			struct g2f{
				v2f data;
				float3 baryCoords : TEXCOORD9;
			};
			
			v2f vert (appdata v){
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				float3 worldPos = mul(unity_ObjectToWorld, v.vertex);
				float3 worldNormal = UnityObjectToWorldNormal(v.normal);
				o.viewDot = saturate(dot(normalize(_WorldSpaceCameraPos - worldPos), worldNormal));
				UNITY_TRANSFER_FOG(o,o.vertex);
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
				fixed4 baryCol = lerp(_WireColor, _Color, minBary);
				fixed4 tex = tex2D(_MainTex, i.data.uv);
				fixed4 col = tex * baryCol * i.data.viewDot;
				UNITY_APPLY_FOG(i.data.fogCoord, col);
				return col;
			}

			ENDCG
		}
	}
}
