Shader "Sleek Render/Post Process/Compose" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_PreComposeTex ("PreCompose Texture", 2D) = "black" {}
		_Colorize ("Colorize", Vector) = (1,1,1,0)
		_ContrastBrightness ("Contrast And Brightness", Vector) = (1,0.5,0,0)
		_LuminanceConst ("Luminance Const", Vector) = (0.2126,0.7152,0.0722,0)
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