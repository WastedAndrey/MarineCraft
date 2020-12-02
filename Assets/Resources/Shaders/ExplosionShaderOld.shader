Shader "Custom/ExplosionShader"
{
    Properties
    {
		_MainTex("Color (RGB) Alpha (A)", 2D) = "white" {}
        _Color ("Color", Color) = (1,1,1,1)
		_Speed("Speed", Range(0,100)) = 1
		_Intencity("Intencity", Range(0,10)) = 1
		_Alpha("Alpha", Range(0,1)) = 1
		_AlphaInCenter("AlphaInCenter", Range(0,10)) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
        LOD 100
		Cull off

        CGPROGRAM

		#pragma surface surf NoLighting noforwardadd novertexlights vertex:vert alpha 
		struct Input {
			  float2 uv_MainTex;
			  float4 color: COLOR;
			  float2 uv : TEXCOORD0;
		};


		fixed4 _Color;
		float _Speed;
		float _Intencity;
		float _Alpha;
		float _AlphaInCenter;
		float _MyTimer;
		sampler2D _MainTex;

		fixed4 LightingNoLighting(SurfaceOutput s, fixed3 lightDir, fixed atten)
		{
			fixed4 c;
			c.rgb = s.Albedo;
			c.a = s.Alpha;
			return c;
		}

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

		float3 shape(float frame, float3 normal, float2 texcoord)
		{
			float3 result = float3(0,0,0);

			float2 transformedXY = float2 ((texcoord.x - 0.5) * 100, (texcoord.y - 0.5) * 100);

			if (abs(transformedXY.y) < 35)
			{
				result = normal * frame / 100 * 1.3;
				result.y = (0.02 + frame / 50000) * sign(texcoord.y - 0.5);
				result.y += -(texcoord.y - 0.5) / 50;
			}
			else
			{
				result = normal / 30 + normal * sin(0.1 + frame / 40)/5;
			}

			return result;
		}

		float4 calcColor(float frame, float4 vertex, float2 texcoord)
		{
			float4 result = float4(0, 0, 0, 0);

			float2 transformedXY = float2 ((texcoord.x - 0.5) * 100, (texcoord.y - 0.5) * 100);

			if (abs(transformedXY.y) < 35)
			{
				result.x = abs(vertex.y);
				result.y = clamp(1 - frame / 70, 0, 1);
				result.z = sin((texcoord.x * 100) % 2 * (texcoord.y * 100) % 2 * _Time.x * 100) + 0.55;
			}
			else
			{
				result.x = abs(vertex.y);
				result.y = clamp(1 - frame / 100, 0, 1);
				result.z = sin((texcoord.x * 100) % 2 * (texcoord.y * 100) % 2 * _Time.x * 100) + 1.15;
			}

			return result;
		}

		void vert(inout appdata_full v)
		{
			float frame = ((_Time.y - _MyTimer) * _Speed) % 100;

			v.vertex.xyz = shape(frame, v.normal, v.texcoord.xy);
			v.color = calcColor(frame, v.vertex, v.texcoord.xy);
			

			//v.vertex.z = 0 + abs(v.texcoord.y - 0.5) * sign(v.texcoord.x - 0.5);
		}


		void surf(Input IN, inout SurfaceOutput o) {
			fixed4 tex = tex2D(_MainTex, IN.uv_MainTex);

			//tex.x = 0.9;
			//tex.y = 0.9;
			//tex.z = 0.9;
			/**/


			tex.x = (IN.color.b + 0.9) * _Intencity * (0.4 + _Color.rgb);
			tex.y = (IN.color.b * 0.75 + 0.8) * _Intencity * (0.4 + _Color.rgb);
			tex.z = (IN.color.b * 0.5 + 0.65) * _Intencity * (0.4 + _Color.rgb);
			tex.a = _Alpha;

			o.Albedo = tex.rgb * _Color.rgb;
			o.Alpha = tex.a * _Alpha * IN.color.g;

			if (IN.color.r < 0.02 * _AlphaInCenter) o.Alpha *= 0.10;
		}

        ENDCG
    }
    FallBack "Diffuse"
}
