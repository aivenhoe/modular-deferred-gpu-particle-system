

// https://forum.unity3d.com/threads/billboard-geometry-shader.169415/
Shader "aivo/BillboardParticleDeferred"
{
	Properties
	{
		_MainTex("Albedo (RGB)", 2D) = "white" {}
		_Cutoff("Cutoff", Range(0,1)) = 0.2 

		_Size ("Size", Range(0, 3)) = 1.5
		_MinSizeFactor ("MinSizeFactor", Range(0.001,0.01)) = 0.001

		_Color("Color", Color) = (1,1,1,1)
		_Specular("SpecularColor", Color) = (0.5,0.5,0.5,1)
		_Emission("Emission", Color) = (0.5,0.5,0.5,1)
		_Normal("normal", Color) = (0.5,0.5,0.5,1)
		_Smoothness("Smoothness", Range(0,1)) = 0.5
		_ShadowZOffset("ShadowZOffset", Range(-1,1)) = -0.001

	}

	SubShader
	{

		Pass
		{
			Tags { "LightMode" = "Deferred" }
			
	
			//stenciling check here as a start:https://docs.unity3d.com/Manual/SL-Stencil.html
			Stencil {
				Ref 128
				Comp always
				Pass replace
			}

			CGPROGRAM
				#pragma target 5.0
				#pragma vertex vert
				#pragma geometry geom
				#pragma fragment frag
				#include "UnityCG.cginc"
				#include "UnityStandardUtils.cginc"	

				struct FragmentOutput {
					float4 gBuffer0 : SV_Target0;
					float4 gBuffer1 : SV_Target1;
					float4 gBuffer2 : SV_Target2;
					float4 gBuffer3 : SV_Target3;
				};



				struct v2g
				{
					float4 pos	: POSITION;
					float4 col : COLOR;

				};				
				struct g2f
				{
					float4 pos	: POSITION;
					float3 normal: NORMAL;
					float4 col : COLOR;
					float2 uv : TEXCOORD0;

				};
				struct particle
				{
					float3 pos;
					float3 acc;
					float3 vel;
					float mass;
					int age;
					float4 col;
				};

				StructuredBuffer<particle> buf_Points;

				sampler2D _MainTex;
				half _Smoothness;
				float _Cutoff;
				float4 _Specular;
				float4 _Color;
				float4 _Emission;
				float4 _Normal;
				float _Size;
				float _MinSizeFactor;
				float4x4 _VP;
				v2g vert(uint id : SV_VertexID)
				{
					v2g output = (v2g)0;
					output.pos = float4(buf_Points[id].pos, 1.0f);
					if (buf_Points[id].age > 0)
						output.col = buf_Points[id].col;
					else
						output.col = float4(0, 0, 0, 0);
					return output;
				}


				[maxvertexcount(4)]
				void geom(point v2g p[1], inout TriangleStream<g2f> triStream)
				{
					float3 up = UNITY_MATRIX_IT_MV[1].xyz;
					float3 right = -UNITY_MATRIX_IT_MV[0].xyz;
					float dist = length(ObjSpaceViewDir(p[0].pos));

					float halfS = 0.5f * (_Size + (dist * _MinSizeFactor));
					float4 v[4];
					float2 n[4];
					v[0] = float4(p[0].pos - halfS * right - halfS * up, 1.0f);
					v[1] = float4(p[0].pos + halfS * right - halfS * up, 1.0f);
					v[2] = float4(p[0].pos - halfS * right + halfS * up, 1.0f);
					v[3] = float4(p[0].pos + halfS * right + halfS * up, 1.0f);
					
					n[0] = float2(1, 0);
					n[1] = float2(0, 0);
					n[2] = float2(1, 1);
					n[3] = float2(0, 1);

					float4x4 vp = UNITY_MATRIX_VP;
					g2f pIn;
 
					pIn.col = p[0].col;
					pIn.normal = -1 + (_Normal.rgb * 2);


					pIn.pos = mul(vp, v[0]);
					pIn.uv = n[0];
					triStream.Append(pIn);

					pIn.pos =  mul(vp, v[1]);
					pIn.uv = n[1];
					triStream.Append(pIn);

					pIn.pos =  mul(vp, v[2]);
					pIn.uv = n[2];
					triStream.Append(pIn);

					pIn.pos =  mul(vp, v[3]);
					pIn.uv = n[3];
					triStream.Append(pIn);

				}

				FragmentOutput frag(g2f input) : COLOR
				{
					//return input.col;
					FragmentOutput output;
					
					//input.col.a determines, whether particle has been emitted. if so, input.col.a >0
					float4 c = tex2D(_MainTex, input.uv) * _Color * input.col.a;
					c.rgb += input.col.rgb;
					clip(c.a<_Cutoff?-1:1);
					output.gBuffer0 = float4(c.rgb, c.a);  // Diffuse color (RGB), occlusion (A).
					output.gBuffer1 = float4(_Specular.rgb, _Smoothness); //Specular color (RGB), roughness (A).
					output.gBuffer2 = float4(input.normal * 0.5 + 0.5, 1); ////World space normal (RGB), unused (A). +reflection probes buffer.
					output.gBuffer3 = float4(_Emission.rgb, 1);	// Emission + lighting + lightmaps
					
					return output;
				}

			ENDCG
		}

		Pass
		{
			Tags { 
				"LightMode" = "ShadowCaster" }
			CGPROGRAM

				#include "UnityCG.cginc"
				#pragma target 5.0
				#pragma vertex vert
				#pragma geometry geom
				#pragma addshadow nometa

				#pragma alphatest:_Cutoff
				#pragma fragment frag
				#pragma multi_compile_shadowcaster

				struct v2g
				{
					float4 pos	: POSITION;
					float4 col : COLOR;

				};
				struct g2f
				{
					float4 pos	: POSITION;
					float3 normal: NORMAL;
					float4 col : COLOR;
					float2 uv : TEXCOORD0;

				};
				struct particle
				{
					float3 pos;
					float3 acc;
					float3 vel;
					float mass;
					int age;
					float4 col;
				};

				StructuredBuffer<particle> buf_Points;

				sampler2D _MainTex;
				half _Smoothness;
				float4 _Specular;
				float4 _Color;
				float _Cutoff;
				float4 _Emission;
				float4 _Normal;
				float _Size;
				float _MinSizeFactor;
				float4x4 _VP;
				float _ShadowZOffset;

				v2g vert(uint id : SV_VertexID)
				{
					v2g output = (v2g)0;
					output.pos = float4(buf_Points[id].pos, 1.0f);
					output.col = buf_Points[id].col;
					return output;
				}

				[maxvertexcount(4)]
				void geom(point v2g p[1], inout TriangleStream<g2f> triStream)
				{
					float3 up = UNITY_MATRIX_IT_MV[1].xyz;
					float3 right = -UNITY_MATRIX_IT_MV[0].xyz;
					float dist = length(ObjSpaceViewDir(p[0].pos));

					float halfS = 0.5f * (_Size + (dist * _MinSizeFactor));
					float4 v[4];
					float2 n[4];
					v[0] = float4(p[0].pos - halfS * right - halfS * up, 1.0f);
					v[1] = float4(p[0].pos + halfS * right - halfS * up, 1.0f);
					v[2] = float4(p[0].pos - halfS * right + halfS * up, 1.0f);
					v[3] = float4(p[0].pos + halfS * right + halfS * up, 1.0f);

					n[0] = float2(1, 0);
					n[1] = float2(0, 0);
					n[2] = float2(1, 1);
					n[3] = float2(0, 1);

					float4x4 vp = UNITY_MATRIX_VP;
					g2f pIn;

					pIn.col = p[0].col;
					pIn.normal = -1 + (_Normal.rgb * 2);

					float4 shadowZOffset = float4(0,0, _ShadowZOffset,0); //cheap workaround. plz let me know if you find another way to do this
					pIn.pos = mul(vp, v[0]);
					pIn.pos += shadowZOffset;
					pIn.uv = n[0];
					triStream.Append(pIn);

					pIn.pos = mul(vp, v[1]);
					pIn.pos += shadowZOffset;
					pIn.uv = n[1];
					triStream.Append(pIn);

					pIn.pos = mul(vp, v[2]);
					pIn.pos += shadowZOffset;
					pIn.uv = n[2];
					triStream.Append(pIn);

					pIn.pos = mul(vp, v[3]);
					pIn.pos += shadowZOffset;
					pIn.uv = n[3];
					triStream.Append(pIn);

				}

			float4 frag(g2f input) : COLOR {		
				float4 c = tex2D(_MainTex, input.uv) * input.col.a;
				clip(c.a < _Cutoff ? -1 : 1);
				SHADOW_CASTER_FRAGMENT(input);
			}

			ENDCG
		}

	}
}
