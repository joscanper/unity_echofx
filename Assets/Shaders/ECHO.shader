Shader "TheGPUMan/ECHO"
{
	Properties
	{
		_Color("Color", Color) = (1,0,0,1)
	}

		SubShader
	{
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct EchoPoint
			{
				float4 position;
				float4 normal;
			};

			sampler2D_float _CameraDepthTexture;

			float3 _InteractorPos;
			float _InteractorStrength;
			float _InteractorRadius;
			float3 _LightDir;

			float4 _Color;

			float nrand(float2 uv)
			{
				return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
			}

			struct appdata
			{
				float4 position : POSITION;
				float4 normal : NORMAL;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float4 normal : NORMAL;
				float4 projPos : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				float4 vertex = v.position;
				vertex.w = 1;
				float4 worldPos = mul(unity_ObjectToWorld, vertex);
				float distFromInt = 1.0 - clamp(distance(_InteractorPos, worldPos.xyz) / _InteractorRadius, 0, 1);
				float3 dirFromInt = normalize(worldPos.xyz - _InteractorPos);

				o.vertex = UnityWorldToClipPos(worldPos + dirFromInt * distFromInt * _InteractorStrength * nrand(v.position.xz));
				o.normal = v.normal;
				o.projPos = ComputeScreenPos(o.vertex);
				o.projPos.z = -UnityObjectToViewPos(vertex).z;

				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				float sceneEyeDepth = DECODE_EYEDEPTH(tex2D(_CameraDepthTexture, i.projPos.xy / i.projPos.w));
				clip(sceneEyeDepth - i.projPos.z);

				float nDotL = max(dot(normalize(i.normal), -normalize(_LightDir)), 0.1);

				return float4(_Color.rgb * nDotL, 1);
			}
		ENDCG
	}
	}
}