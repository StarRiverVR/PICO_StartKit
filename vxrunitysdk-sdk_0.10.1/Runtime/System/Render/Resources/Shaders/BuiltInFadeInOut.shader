
Shader "VXR/Pipeline/BuiltInFadeInOut"
{
    Properties
    {

        [MainTexture] _BaseMap("Albedo", 2D) = "white" {}
        [MainColor] _BaseColor("Color", Color) = (1,1,1,1)

        [HideInInspector] _Color("Color", Color) = (1,1,1,1)
        [HideInInspector] _MainTex("Albedo", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "PerformanceChecks"="False" }
        LOD 100

        Blend One OneMinusSrcAlpha
		Cull Off
		ZWrite Off
		Lighting Off 

        Pass
        {
            Name "FORWARD"
            Tags { "LightMode" = "ForwardBase" }

            CGPROGRAM
            #pragma target 2.0

            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "UnityCG.cginc"

            half4       _BaseColor;
            sampler2D   _BaseMap;
            float4      _BaseMap_ST;

            struct VertexInput_C
            {
				float4 vertex 	: POSITION;
				float2 uv 		: TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct VertexOutput_C
            {
                float4 vertex 		: SV_POSITION;
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
                o.vertex 		=  float4(v.vertex.xz,1, 1.0);
                o.uv = TRANSFORM_TEX(v.uv, _BaseMap);
                return o;
            }

            half4 Fragment (VertexOutput_C i) : SV_Target
            {
                half4 finishColor = _BaseColor;
                finishColor =  tex2D(_BaseMap,i.uv)*finishColor;
                return finishColor;
            }

            ENDCG
        }

    }

}
