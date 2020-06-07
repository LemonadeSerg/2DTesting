// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'
// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

Shader "Custom/CurvedWorld" {
	Properties{
		_Color("Color", Color) = (1,1,1,1)
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_BumpMap("Normalmap", 2D) = "bump" {}
		_Glossiness("Smoothness", Range(0,1)) = 0.5
		_Metallic("Metallic", Range(0,1)) = 0.0
		_Curvature("Curvature", Float) = 0.001
	   _Offset("Offset", Float) = 0.001
	}
		SubShader{
			Tags { "RenderType" = "Opaque" }
			LOD 200

			CGPROGRAM
			// Physically based Standard lighting model, and enable shadows on all light types
			#pragma surface surf Standard vertex:vert fullforwardshadows

			// Use shader model 3.0 target, to get nicer looking lighting
			#pragma target 3.0

			sampler2D _MainTex;
			sampler2D _BumpMap;
			float _Curvature;
			float _Offset;

			struct Input {
				float2 uv_MainTex;
				float2 uv_BumpMap;
			};

			half _Glossiness;
			half _Metallic;
			fixed4 _Color;
			struct v2f
			{
				float2 uv : TEXCOORD0;
				UNITY_FOG_COORDS(1)
					float4 vertex : SV_POSITION;
				float4 scrPos : TEXCOORD2;
			};
			 void vert(inout appdata_full v)
			{
				 v2f o;
				 o.vertex = UnityObjectToClipPos(v.vertex);
				 o.scrPos = ComputeScreenPos(o.vertex);

				float4 vv = mul(unity_ObjectToWorld, v.vertex);
				vv.xyz -= _WorldSpaceCameraPos.xyz;
					vv = float4(0.0f, -((o.scrPos.x + _Offset) * (o.scrPos.x + _Offset)) * _Curvature, 0.0f, 0.0f);

				v.vertex += mul(unity_WorldToObject, vv);
			}

			void surf(Input IN, inout SurfaceOutputStandard o) {
				// Albedo comes from a texture tinted by color
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Normal = UnpackNormal(tex2D(_BumpMap, IN.uv_BumpMap));
				// Metallic and smoothness come from slider variables
				o.Metallic = _Metallic;
				o.Smoothness = _Glossiness;
				o.Alpha = c.a;
			}
			ENDCG
		}
			FallBack "Diffuse"
}