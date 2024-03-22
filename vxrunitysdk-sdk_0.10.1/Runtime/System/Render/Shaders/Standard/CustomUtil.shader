
Shader "Vivo/CustomUtil"
{
    Properties
    {
		[HideInInspector]_BlendOp("__blendop", Float) = 0.0
		[Enum(UnityEngine.Rendering.BlendMode)]_SrcBlend("SrcBlend", int) = 1
        [Enum(UnityEngine.Rendering.BlendMode)]_DestBlend("DestBlend", int) = 0
        [Enum(UnityEngine.Rendering.CullMode)]_Cull("CullMode", int) = 2
        [Enum(LEqual, 4, Always, 8)]_ZAlways("ZTest", int) = 4
		[HideInInspector] _ZWrite("__zw", Float) = 1.0

        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)

        [MaterialToggle(_USELIGHT_ON)] _USELIGHTON("LightOn", float) = 1
        [MaterialToggle(_ALPHATEST_ON)] _ALPHATESTON("AlphaTest", float) = 0
        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        [HideInInspector] _Color("Color", Color) = (1,1,1,1)
        [HideInInspector] _MainTex("Albedo", 2D) = "white" {}
    }

    SubShader
    {
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" "IgnoreProjector" = "True"}
        LOD 100

        BlendOp[_BlendOp]
		Blend[_SrcBlend][_DestBlend]
		Cull[_Cull]
		ZTest[_ZAlways]
        ZWrite[_ZWrite]
        Lighting Off 

        Pass
        {
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}

            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma target 2.0

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer

            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _USELIGHT_ON

            #pragma multi_compile_fog

            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl" 

            CBUFFER_START(UnityPerMaterial)
                float4 _BaseMap_ST;
                float4 _BaseColor;
                float _Cutoff;
            CBUFFER_END
            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);

            struct Attributes_C {
                float4 positionOS : POSITION;
                float3 normalOS:NORMAL;
                float4 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings_C {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float3 positionWS : TEXCOORD1;
                float3 normalWS : TEXCOORD2;

                float3 viewDirWS : TEXCOORD3;
                #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
                    half  fogFactor : TEXCOORD4;
                #endif

                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            Varyings_C Vertex(Attributes_C input) {
                Varyings_C output = (Varyings_C)0;

                // Instance
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.positionWS =   TransformObjectToWorld(input.positionOS.xyz);
                output.uv.xy = TRANSFORM_TEX(input.uv.xy,_BaseMap);
                output.normalWS = normalize(TransformObjectToWorldNormal(input.normalOS.xyz));
                output.viewDirWS = _WorldSpaceCameraPos - TransformObjectToWorld(input.positionOS.xyz);
                #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
                    output.fogFactor = ComputeFogFactor(output.positionCS.z); 
                #endif         

                return output;
            }

            half4 Fragment(Varyings_C input) : SV_Target{
                UNITY_SETUP_INSTANCE_ID(input);

                float4 baseMap = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,input.uv.xy)*_BaseColor;

                #if defined(_ALPHATEST_ON)
                    clip(baseMap.a - _Cutoff);
                #endif 

                #if defined(_USELIGHT_ON)
                    half3 normalWS=input.normalWS;
                    Light mainLight = GetMainLight();
                    float NDotL = dot(mainLight.direction,normalWS)*0.5+0.5;
                    float3 h = normalize(normalize(input.viewDirWS)+mainLight.direction);
                    float NDotH =saturate(dot(h,normalWS));
                    NDotH=pow(NDotH,4);
                    float l =saturate(NDotL+NDotH*0.3);
                    baseMap.rgb = baseMap.rgb*l;
                #endif 

                half4 finColor=baseMap;

                #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
                    half fogIntensity = ComputeFogIntensity(input.fogFactor);
	                finColor.rgb = lerp(unity_FogColor.rgb, finColor.rgb , fogIntensity);
                #endif

                return finColor;
            }

            ENDHLSL
        }

    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
        LOD 100

        BlendOp[_BlendOp]
		Blend[_SrcBlend][_DestBlend]
		Cull[_Cull]
		ZTest[_ZAlways]
        ZWrite[_ZWrite]
        Lighting Off 

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM
            #pragma target 2.0

            #pragma shader_feature_local _ _ALPHATEST_ON
            #pragma shader_feature_local _ _USELIGHT_ON
          
            #pragma multi_compile_fog

            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"

            half4       _BaseColor;
            half        _Cutoff;
            sampler2D   _BaseMap;
            float4      _BaseMap_ST;

            struct VertexInput_C
            {
				float4 vertex 	: POSITION;
				float2 uv 		: TEXCOORD0;
				float3 normal   : NORMAL;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput_C
            {
                float4 vertex 		: SV_POSITION;
                float3 worldPos 	: TEXCOORD0;
                float3 worldNormal  : TEXCOORD1;
                float3 viewDir 		: TEXCOORD5;
                float2 uv 		: TEXCOORD6;
                UNITY_FOG_COORDS(7)

                UNITY_VERTEX_OUTPUT_STEREO
            };

            VertexOutput_C Vertex (VertexInput_C v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                VertexOutput_C o;
				o = (VertexOutput_C)0;
                UNITY_INITIALIZE_OUTPUT(VertexOutput_C, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
				o.worldPos = mul ((float3x3)unity_ObjectToWorld, v.vertex.xyz );
				o.vertex 		= UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                o.viewDir = _WorldSpaceCameraPos.xyz - o.worldPos.xyz;
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                return o;
            }

            half4 Fragment (VertexOutput_C i) : SV_Target
            {
                float4 baseMap = tex2D(_BaseMap,i.uv)*_BaseColor;

                #ifdef _ALPHATEST_ON
                    clip(baseMap.a - _Cutoff);
                #endif

                #ifdef _USELIGHT_ON
                    half3 normalWS=i.worldNormal;
                    half3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                    float3 lightDirection = _WorldSpaceLightPos0.xyz;
                    float NDotL = dot(lightDirection,normalWS)*0.5+0.5;
                    float3 h = normalize(worldViewDir+lightDirection);
                    float NDotH =saturate(dot(h,normalWS));
                    NDotH=pow(NDotH,4);
                    float l =saturate(NDotL+NDotH*0.3);
                    baseMap.rgb = baseMap.rgb*l;
                #endif

                half4 finalRGBA = baseMap;

                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);

                return finalRGBA;
            }

            ENDCG
        }

    }

}
