Shader "Custom/Terrain/surf_Water" {
	Properties {
		_Color ("Main Color", Color) = (1,1,1,1)
		_SpecColor ("Specular Color", Color) = (0.5, 0.5, 0.5, 1)
		_Shininess ("Shininess", Range (0.03, 1)) = 0.078125
		_MainTex ("Base (RGB) Gloss (A)", 2D) = "white" {}
		_BumpMap1 ("Normalmap A", 2D) = "bump" {}
		_BumpMap2 ("Normalmap B", 2D) = "bump" {}
		_TexOff ("Texture Offset (multiplied with time)", Vector) = (1,1,-1,-1)
	}
	
	CGINCLUDE
	sampler2D _MainTex;
	sampler2D _BumpMap1;
	sampler2D _BumpMap2;
	fixed4 _Color;
	half _Shininess;
	float4 _TexOff;
	
	struct Input {
		float2 uv_MainTex;
		float2 uv_BumpMap1;
		float2 uv_BumpMap2;
	};
		
	void surf (Input IN, inout SurfaceOutput o) {
		fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);
		o.Albedo = tex.rgb * _Color.rgb;
		o.Gloss = tex.a;
		o.Alpha = tex.a * _Color.a;
		o.Specular = _Shininess;
		float2 texOff1 = _TexOff.xy * _Time.y;
		float2 texOff2 = _TexOff.zw * _Time.y;
		float3 normal1 = UnpackNormal(tex2D(_BumpMap1, IN.uv_BumpMap1 + texOff1));
		float3 normal2 = UnpackNormal(tex2D(_BumpMap2, IN.uv_BumpMap2 + texOff2));
		o.Normal = normalize(normal1 + normal2);
	}
	ENDCG
	
	SubShader { 
		Tags { "RenderType"="Opaque" }
		LOD 400
		
		CGPROGRAM
		#pragma surface surf BlinnPhong
		#pragma target 3.0
		ENDCG
	}

	SubShader { 
		Tags { "RenderType"="Opaque" }
		LOD 400
		
		CGPROGRAM
		#pragma surface surf BlinnPhong nodynlightmap
		ENDCG
	}
	
	FallBack "Legacy Shaders/Specular"
}