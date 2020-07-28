// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Hidden/Blur"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		// Horizontal
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
				float2 uv : TEXCOORD0;
				UNITY_VERTEX_OUTPUT_STEREO
			};

			v2f vert (appdata v)
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v); //Insert
				UNITY_INITIALIZE_OUTPUT(v2f, o); //Insert
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o); //Insert
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			//sampler2D _MainTex;
			UNITY_DECLARE_SCREENSPACE_TEXTURE(_MainTex);
			float2 _BlurSize;

			 //Insert

			fixed4 frag (v2f i) : SV_Target
			{
				UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i); //Insert
				fixed4 s = UNITY_SAMPLE_SCREENSPACE_TEXTURE(_MainTex, i.uv) * 0.38774;
				s += tex2D(_MainTex, i.uv + float2(_BlurSize.x * 2, 0)) * 0.06136;
				s += tex2D(_MainTex, i.uv + float2(_BlurSize.x, 0)) * 0.24477;
				s += tex2D(_MainTex, i.uv + float2(_BlurSize.x * -1, 0)) * 0.24477;
				s += tex2D(_MainTex, i.uv + float2(_BlurSize.x * -2, 0)) * 0.06136;

				return s;
			}
			ENDCG
		}

		// Vertical
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
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			float2 _BlurSize;

			fixed4 frag (v2f i) : SV_Target
			{
				fixed4 s = tex2D(_MainTex, i.uv) * 0.38774;
				s += tex2D(_MainTex, i.uv + float2(0, _BlurSize.y * 2)) * 0.06136;
				s += tex2D(_MainTex, i.uv + float2(0, _BlurSize.y)) * 0.24477;			
				s += tex2D(_MainTex, i.uv + float2(0, _BlurSize.y * -1)) * 0.24477;
				s += tex2D(_MainTex, i.uv + float2(0, _BlurSize.y * -2)) * 0.06136;

				return s;
			}
			ENDCG
		}
	}
}
