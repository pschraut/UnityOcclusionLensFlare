//
// Occlusion Lens Flare for Unity. Copyright (c) 2021 Peter Schraut (www.console-dev.de). See LICENSE.md
// https://github.com/pschraut/UnityOcclusionLensFlare
//

Shader "Hidden/FX/Occlusion Lens Flare - OcclusionTexture"
{
	CGINCLUDE
	#pragma target 2.0
	#include "UnityCG.cginc"

	struct appdata
	{
		float4 vertex : POSITION;
		float2 uv : TEXCOORD0;
	};

	struct v2f
	{
		float4 vertex : SV_POSITION;
		float2 uv : TEXCOORD0;
	};

	sampler2D _MainTex;
	float4 _MainTex_ST;
	float _Cutoff;
	float4 _Color;

	v2f vert(appdata v)
	{
		v2f o;
		UNITY_INITIALIZE_OUTPUT(v2f, o);

		o.vertex = UnityObjectToClipPos(v.vertex);
		o.uv = TRANSFORM_TEX(v.uv, _MainTex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		return fixed4(0,0,0,0);
	}

	fixed4 frag_cutout(v2f i) : SV_Target
	{
		fixed4 col = tex2D(_MainTex, i.uv) * _Color;
		clip(col.a - _Cutoff);
		return fixed4(0,0,0,0);
	}

	fixed4 frag_sun(v2f i) : SV_Target
	{
		return fixed4(1,1,1,1);
	}
	ENDCG

	SubShader
	{
		Tags{ "RenderType" = "OcclusionLensFlareSun" "IgnoreProjector" = "True" }
		Pass
		{
			Fog { Mode Off }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_sun
			ENDCG
		}
	}

	SubShader
	{
		Tags { "RenderType" = "Opaque" "IgnoreProjector" = "True" }
		Pass
		{
			Fog { Mode Off }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			ENDCG
		}
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout" "Queue" = "AlphaTest" "IgnoreProjector" = "True" }
		Pass
		{
			Fog { Mode Off }
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag_cutout
			ENDCG
		}
	}
}

