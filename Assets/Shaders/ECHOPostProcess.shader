Shader "Hidden/TheGPUMan/ECHO"
{
	HLSLINCLUDE

#include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

		TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
	TEXTURE2D_SAMPLER2D(_GlobalEchoTex, sampler_GlobalEchoTex);
	float4 _MainTex_TexelSize;

	float _Opacity;
	float _HDRMultiplier;

	float nrand(float2 uv)
	{
		return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
	}

	float4 Frag(VaryingsDefault i) : SV_Target
	{
		float2 echoUV = i.texcoord;

		// Some randomization to create the glitchy effect
		float pulse = nrand(_Time.xy);

		// You probably want to optimize this someday....
		float sinBand = sin(i.texcoord.y * 80 + _Time.z);
		float sinBand2 = sin(i.texcoord.y * 380 + _Time.z * 100);
		float sinBand3 = sin(i.texcoord.y * 800 + _Time.z * 10);
		float cosBand = cos(i.texcoord.x * 10 + _Time.z * 10);

		float distortion = (1.0 - _Opacity) * cosBand;

		// Use the pulse and previous bands to shift the UVs
		float UVshiftX = (pulse > 0.975 && abs(pulse - sinBand) < 0.05) ? sin(i.texcoord.y) : 0;
		UVshiftX += sinBand2 * 0.05;
		UVshiftX += distortion * (cosBand + sinBand2) * 100;
		echoUV.x += UVshiftX * 0.01;

		float4 echo = SAMPLE_TEXTURE2D(_GlobalEchoTex, sampler_GlobalEchoTex, echoUV);
		float4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, i.texcoord);

		float smallBands = clamp(sinBand3, cos(_Time.y) + sin(echoUV.x) + sin(echoUV.y), 1);

		return color + echo * _HDRMultiplier * pow(max(_Opacity, 0), 50) * smallBands;
	}

		ENDHLSL

		SubShader
	{
		Cull Off
			ZWrite Off
			ZTest Always

			Pass
		{
			HLSLPROGRAM

				#pragma vertex VertDefault
				#pragma fragment Frag

			ENDHLSL
		}
	}
}