Shader "_Chessgeon/ChessgeonTransparency"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_Ambient("Ambient", Range(0, 1)) = 0.3
	}

	SubShader
	{
		Tags
		{
			"Queue" = "Transparent"
		}

		Pass
		{
			ZWrite Off
			Cull Front
			Blend SrcAlpha OneMinusSrcAlpha

			Tags
			{
				"LightMode" = "ForwardBase"
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			fixed _Ambient;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//fixed3 lighting = max(dot(i.normal, _WorldSpaceLightPos0), _Ambient);
				//float3 color = lighting * texColor.xyz * _Color.xyz;
				float4 texColor = tex2D(_MainTex, i.uv);
				return fixed4(max(dot(i.normal, _WorldSpaceLightPos0), _Ambient) * texColor.xyz * _Color.xyz, texColor.a);
			}
			ENDCG
		}



		Pass
		{
			ZWrite Off
			Cull Back
			Blend SrcAlpha OneMinusSrcAlpha

			Tags
			{
				"LightMode" = "ForwardBase"
			}

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float3 normal : NORMAL;
				float uv : TEXCOORD0;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;
			fixed _Ambient;

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.normal = UnityObjectToWorldNormal(v.normal);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f i) : SV_Target
			{
				//fixed3 lighting = max(dot(i.normal, _WorldSpaceLightPos0), _Ambient);
				//float3 color = lighting * texColor.xyz * _Color.xyz;
				float4 texColor = tex2D(_MainTex, i.uv);
				return fixed4(max(dot(i.normal, _WorldSpaceLightPos0), _Ambient) * texColor.xyz * _Color.xyz, texColor.a);
			}
			ENDCG
		}
	}
}
