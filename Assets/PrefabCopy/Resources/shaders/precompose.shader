Shader "Sleek Render/Post Process/PreCompose" {
	Properties {
		_MainTex ("Main Texture", 2D) = "black" {}
		_BloomTex ("Bloom", 2D) = "black" {}
		_BloomIntencity ("Bloom Intensity", Float) = 0.672
		_VignetteShape ("Vignette Form", Vector) = (1,1,1,1)
		_VignetteColor ("Vignette Color", Vector) = (0,0,0,1)
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}