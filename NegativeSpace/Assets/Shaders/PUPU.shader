Shader "Projector/Sticker" {
	Properties{
		_ShadowTex("Cookie", 2D) = "gray" {}
	_ProjectorForward("Projector Forward", Vector) = (0, 0, 0)
		_ProjectorPosition("Projector Position", Vector) = (0, 0, 0)
		_Distance("Projector Distance", Float) = 5

		_Reflection("Reflection Intence", Float) = 1
	}
		Subshader{
		Tags{ "Queue" = "Geometry" }
		Pass{
		ColorMask RGB
		//Offset - 1, -1

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma multi_compile_fwdbase nolightmap nodirlightmap nodynlightmap novertexlight
#include "UnityCG.cginc"
#include "UnityLightingCommon.cginc"
#include "AutoLight.cginc"

	struct v2f {
		float4 uvShadow : TEXCOORD0;
		float3 normal : NORMAL;
		float4 pos : SV_POSITION;
		float3 worldPos : NORMAL1;
		fixed3 diff : COLOR;
	};

	float4x4 unity_Projector;
	float4x4 unity_ProjectorClip;

	v2f vert(appdata_base v)
	{
		v2f o;
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
		o.worldPos = mul(unity_ObjectToWorld, v.vertex).xyz;
		o.uvShadow = mul(unity_Projector, v.vertex);
		o.normal = mul(unity_ObjectToWorld, v.normal);

		//Light
		half nl = min(max(0.3, dot(o.normal, _WorldSpaceLightPos0.xyz) * 0.5 + 0.5), 0.8);
		o.diff = nl * _LightColor0;

		return o;
	}

	sampler2D _ShadowTex;
	sampler2D _FalloffTex;
	float3 _ProjectorForward;
	float3 _ProjectorPosition;
	float _Distance;
	float _Reflection;

	fixed4 frag(v2f i) : SV_Target
	{
		fixed4 projectedCord = UNITY_PROJ_COORD(i.uvShadow);
	clip(projectedCord.z);

	if (distance(_ProjectorPosition, i.worldPos) > _Distance)
		return float4(0, 0, 0, 0);
	if (i.uvShadow.x > 1 || i.uvShadow.x < 0)
		return float4(0, 0, 0, 0);
	if (i.uvShadow.y > 1 || i.uvShadow.y < 0)
		return float4(0, 0, 0, 0);

	if (dot(i.normal, _ProjectorForward) < 0.0) // in front of projector?
	{
		fixed4 res = tex2Dproj(_ShadowTex, UNITY_PROJ_COORD(i.uvShadow));
		UNITY_APPLY_FOG_COLOR(i.fogCoord, res, fixed4(1, 1, 1, 1));

		if (res.a < 0.5)
			discard;

		return res * fixed4(i.diff, 1);
	}
	else // behind projector
	{
		return float4(1.0, 1.0, 1.0, 0.0);
	}
	}
		ENDCG
	}
	}
}