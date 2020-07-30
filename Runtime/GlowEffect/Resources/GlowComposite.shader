// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/GlowComposite"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;

				UNITY_VERTEX_INPUT_INSTANCE_ID
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			float2 _MainTex_TexelSize;

			v2f vert (appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v); //Insert
				UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv0 = v.uv;
				o.uv1 = v.uv;

				#if UNITY_UV_STARTS_AT_TOP
				if (_MainTex_TexelSize.y < 0)
					o.uv1.y = 1 - o.uv1.y;
				#endif

				return o;
			}
			
			//sampler2D _MainTex;
			UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
			sampler2D _GlowPrePassTex;
			sampler2D _GlowBlurredTex;
			sampler2D _TempTex0;
			float _Intensity;
			//UNITY_DECLARE_SCREENSPACE_TEXTURE( _MainTex); //Insert

			fixed4 frag (v2f i) : SV_Target
			{ 
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i); //Insert
				fixed4 col = UNITY_SAMPLE_SCREENSPACE_TEXTURE( _MainTex, i.uv0);
				fixed4 glow = max(0, tex2D(_GlowBlurredTex, i.uv1) - tex2D(_GlowPrePassTex, i.uv1));
				return col + glow * _Intensity;
			}
			ENDCG
		}
	}
}
