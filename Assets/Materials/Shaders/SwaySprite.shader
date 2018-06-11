Shader "Sprites/Sway Sprite"
{
	Properties
	{
		[PerRendererData] _MainTex("Sprite Texture", 2D) = "white" {}
		_Color("Tint", Color) = (1,1,1,1)
		_SwayAmount("Sway Amount", Float) = 1.0
		_SwaySpeed("Sway Speed", Float) = 1.0
		[PerRendererData] _PushAmountX("Push Amount X", Float) = 0.0
		[PerRendererData] _PushAmountY("Push Amount Y", Float) = 0.0
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
			"DisableBatching" = "True"
		}

		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			#pragma multi_compile _ UNITY_ETC1_EXTERNAL_ALPHA

			#include "UnityCG.cginc"
			#include "PixelPerfect.cginc"

			struct appdata_t
			{
				float4 vertex   : POSITION;
				float4 color    : COLOR;
				float2 texcoord : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex   : SV_POSITION;
				fixed4 color : COLOR;
				float2 texcoord  : TEXCOORD0;
			};

			fixed4 _Color;
			float _SwayAmount;
			float _SwaySpeed;
			float _PushAmountX;
			float _PushAmountY;

			v2f vert(appdata_t IN)
			{
				//Align to pixel grid before we manipulate vertices
				float4 alignedPos = AlignToPixelGrid(IN.vertex);

				//Get vertex world coordinates
				float4 worldV = mul(unity_ObjectToWorld, alignedPos);

				float height = sqrt(alignedPos.y * alignedPos.y);
				float offset = (sin(_Time.y * _SwaySpeed + ((worldV.x + worldV.z) / 2)))
					+ (sin(_Time.y * _SwaySpeed + ((worldV.x + worldV.z) / 2) / 4)) + _PushAmountX;

				//Move world vertex position
				worldV += float4(offset * height * _SwayAmount, height * _PushAmountY, 0.0f, 0.0f);

				//Set object vertex pos
				float4 selfV = mul(unity_WorldToObject, worldV);

				v2f OUT;
				float4 v = UnityObjectToClipPos(selfV);
				OUT.texcoord = IN.texcoord;
				OUT.color = IN.color * _Color;
				OUT.vertex = v;

				return OUT;
			}

			sampler2D _MainTex;
			sampler2D _AlphaTex;

			fixed4 SampleSpriteTexture(float2 uv)
			{
				fixed4 color = tex2D(_MainTex, uv);

				#if ETC1_EXTERNAL_ALPHA
				color.a = tex2D(_AlphaTex, uv).r;
				#endif

				return color;
			}

			fixed4 frag(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture(IN.texcoord) * IN.color;
				c.rgb *= c.a;

				return c;
			}
			ENDCG
		}
	}
}