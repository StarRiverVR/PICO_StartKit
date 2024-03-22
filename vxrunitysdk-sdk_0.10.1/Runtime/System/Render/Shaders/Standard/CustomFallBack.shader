
Shader "Vivo/Hide/CustomFallBack"
{
    Properties
    {
        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)

        _BumpScale("Scale", Float) = 1.0
        _BumpMap("Normal Map", 2D) = "bump" {}

        [HDR]_EmissionColor("Color", Color) = (0,0,0)
        _EmissionMap("Emission", 2D) = "white" {}

        _Cutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5

        [HideInInspector] _SrcBlend ("__src", Float) = 1.0
        [HideInInspector] _DstBlend ("__dst", Float) = 0.0
        [HideInInspector] _ZWrite ("__zw", Float) = 1.0
        [HideInInspector]_Cull("__cull", Float) = 2.0

        [HideInInspector] _Color("Color", Color) = (1,1,1,1)
        [HideInInspector] _MainTex("Albedo", 2D) = "white" {}
    }

    SubShader
    {
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" "IgnoreProjector" = "True"}
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]

            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma target 2.0

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _EMISSION

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
                float _BumpScale;
                float4 _EmissionColor;
                float _Cutoff;
            CBUFFER_END
            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            TEXTURE2D(_BumpMap); SAMPLER(sampler_BumpMap);
            TEXTURE2D(_EmissionMap); SAMPLER(sampler_EmissionMap);

            struct Attributes_C {
                float4 positionOS : POSITION;
                float3 normalOS:NORMAL;
                float4 uv : TEXCOORD0;
                float4 tangentOS : TANGENT;
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

                float3 tangentWS : TEXCOORD5;
                float3 bitangentWS : TEXCOORD6;


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

                #if defined(_NORMALMAP)
                    VertexNormalInputs normalInput = GetVertexNormalInputs(input.normalOS, input.tangentOS);
                    half sign = input.tangentOS.w * GetOddNegativeScale();
                    output.tangentWS = half3(normalInput.tangentWS.xyz);
                    output.bitangentWS = half3(sign * cross(normalInput.normalWS.xyz, normalInput.tangentWS.xyz));
                    output.normalWS = normalInput.normalWS.xyz;    
                #endif   

                return output;
            }

            half4 Fragment(Varyings_C input) : SV_Target{
                UNITY_SETUP_INSTANCE_ID(input);

                float4 baseMap = SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,input.uv.xy)*_BaseColor;

                #if defined(_ALPHATEST_ON)
                    clip(baseMap.a - _Cutoff);
                #endif 

                half3 normalWS=input.normalWS;

                #if defined(_NORMALMAP)
                    half4 normalTex=SAMPLE_TEXTURE2D(_BumpMap,sampler_BumpMap,input.uv);
                    half3 normalTS = UnpackNormalScale(normalTex,_BumpScale);
                    half3x3 TBN = half3x3(input.tangentWS.xyz, input.bitangentWS.xyz, input.normalWS.xyz);
                    normalWS = normalize(mul(normalTS,TBN));
                #endif 

                Light mainLight = GetMainLight();
                float NDotL = dot(mainLight.direction,normalWS)*0.5+0.5;
                float3 h = normalize(normalize(input.viewDirWS)+mainLight.direction);
                float NDotH =saturate(dot(h,normalWS));
                NDotH=pow(NDotH,4);
                float l =saturate(NDotL+NDotH*0.3);

                baseMap.rgb = baseMap.rgb*l;

                #if defined(_EMISSION)
                    float3 emissionMap = SAMPLE_TEXTURE2D(_EmissionMap,sampler_EmissionMap,input.uv.xy).rgb*_EmissionColor.rgb;
                    baseMap.rgb =  baseMap.rgb+emissionMap;
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
        Tags{"RenderType" = "Opaque" "RenderPipeline" = "UniversalPipeline" "UniversalMaterialType" = "Lit" "IgnoreProjector" = "True"}
        LOD 100

        Pass
        {
            Name "ForwardLit"
            Tags{"LightMode" = "UniversalForward"}

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]

            HLSLPROGRAM
            #pragma only_renderers gles gles3 glcore d3d11
            #pragma target 2.0

            //--------------------------------------
            // GPU Instancing
            #pragma multi_compile_instancing
            #pragma instancing_options renderinglayer

            // -------------------------------------
            // Material Keywords
            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local_fragment _ALPHATEST_ON
            #pragma shader_feature_local_fragment _EMISSION

            #pragma multi_compile_fog

            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            CBUFFER_START(UnityPerMaterial)

            CBUFFER_END

            struct Attributes_C {
                float4 positionOS : POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings_C {
                float4 positionCS : SV_POSITION;
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
                #if defined(FOG_LINEAR) || defined(FOG_EXP) || defined(FOG_EXP2)
                    output.fogFactor = ComputeFogFactor(output.positionCS.z); 
                #endif         
                return output;
            }

            half4 Fragment(Varyings_C input) : SV_Target{
                UNITY_SETUP_INSTANCE_ID(input);

                half4 finColor= half4(1,0,1,1);

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

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]

            CGPROGRAM
            #pragma target 2.0

            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ _ALPHATEST_ON
            #pragma shader_feature_fragment _EMISSION
          
            #pragma multi_compile_fog

            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"

            half4       _BaseColor;
            half        _Cutoff;
            sampler2D   _BaseMap;
            float4      _BaseMap_ST;
            sampler2D   _BumpMap;
            half        _BumpScale;
            half4       _EmissionColor;
            sampler2D   _EmissionMap;

            struct VertexInput_C
            {
				float4 vertex 	: POSITION;
				float2 uv 		: TEXCOORD0;
				float3 normal   : NORMAL;
				float4 tangent  : TANGENT;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput_C
            {
                float4 vertex 		: SV_POSITION;
                float3 worldPos 	: TEXCOORD0;
                float3 worldNormal  : TEXCOORD1;
                half3 tspace0 		: TEXCOORD2; 
				half3 tspace1 		: TEXCOORD3; 
				half3 tspace2 		: TEXCOORD4;
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
                float4 wTangent;
                wTangent.xyz    = UnityObjectToWorldDir ( v.tangent );
                wTangent.w		= -1;

                half  tangentSign 	= v.tangent.w * unity_WorldTransformParams.w;
                half3 wBitangent 	= cross ( o.worldNormal, wTangent )  * tangentSign;

                o.tspace0 = half3(wTangent.x, wBitangent.x, o.worldNormal.x);
                o.tspace1 = half3(wTangent.y, wBitangent.y, o.worldNormal.y);
                o.tspace2 = half3(wTangent.z, wBitangent.z, o.worldNormal.z);

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
                half3 normalWS=i.worldNormal;

                #ifdef _NORMALMAP
                    float4 normalTex = tex2D(_BumpMap,i.uv);
                    half3 normalTS = UnpackScaleNormal(normalTex,_BumpScale);
                    normalWS.x = dot ( i.tspace0, normalTS );
                    normalWS.y = dot ( i.tspace1, normalTS );
                    normalWS.z = dot ( i.tspace2, normalTS );
                #endif

                half3 worldViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
                float3 lightDirection = _WorldSpaceLightPos0.xyz;
                float NDotL = dot(lightDirection,normalWS)*0.5+0.5;
                float3 h = normalize(worldViewDir+lightDirection);
                float NDotH =saturate(dot(h,normalWS));
                NDotH=pow(NDotH,4);
                float l =saturate(NDotL+NDotH*0.3);

                baseMap.rgb = baseMap.rgb*l;

                #ifdef _ALPHATEST_ON
                    float3 emissionMap = tex2D(_EmissionMap,i.uv).rgb*_EmissionColor.rgb;
                    baseMap.rgb =  baseMap.rgb+emissionMap;
                #endif

                half4 finalRGBA = baseMap;

                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);

                return finalRGBA;
            }

            ENDCG
        }

    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
        LOD 100

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            Blend[_SrcBlend][_DstBlend]
            ZWrite[_ZWrite]
            Cull[_Cull]

            CGPROGRAM
            #pragma target 2.0

            #pragma shader_feature_local _NORMALMAP
            #pragma shader_feature_local _ _ALPHATEST_ON
            #pragma shader_feature_fragment _EMISSION
          
            #pragma multi_compile_fog

            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"

            struct VertexInput_C
            {
				float4 vertex 	: POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput_C
            {
                float4 vertex 		: SV_POSITION;
                UNITY_FOG_COORDS(1)
                UNITY_VERTEX_OUTPUT_STEREO
            };

            VertexOutput_C Vertex (VertexInput_C v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                VertexOutput_C o;
				o = (VertexOutput_C)0;
                UNITY_INITIALIZE_OUTPUT(VertexOutput_C, o);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex 		= UnityObjectToClipPos(v.vertex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            half4 Fragment (VertexOutput_C i) : SV_Target
            {
                half4 finalRGBA = half4(1,0,1,1);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }

            ENDCG
        }

    }

}
