Shader "Vivo/Hide/OverlayConvert"
{
    Properties
    {
        _MainTex("Albedo", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque"}
        LOD 100

        Pass
        {
            Name "Blit OverlayConvert BuiltIn"
            ZTest Always
            ZWrite Off
            Cull Off

            CGPROGRAM
            #pragma vertex Vertex
            #pragma fragment Fragment
            #pragma multi_compile _ _NEEDLINEAR_TO_SRGB_CONVERSION
            #pragma multi_compile _ _USE_DRAW_PROCEDURAL

            #include "UnityCG.cginc"

            sampler2D   _MainTex;

            struct ver_v
            {
                #if _USE_DRAW_PROCEDURAL
                    uint vertexID     : SV_VertexID;
                #else
                    float4 positionOS : POSITION;
                    float2 uv         : TEXCOORD0;
                #endif
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct ver_o
            {
                float4 positionCS: SV_POSITION;
                float2 uv:TEXCOORD0;

                UNITY_VERTEX_OUTPUT_STEREO
            };

            float4 GetQuadVertexPositionBuilIn(uint vertexID, float z = 1.0)
            {
                uint topBit = vertexID >> 1;
                uint botBit = (vertexID & 1);
                float x = topBit;
                float y = 1 - (topBit + botBit) & 1;
                return float4(x, y, z, 1.0);
            }

            float2 GetQuadTexCoordBuilIn(uint vertexID)
            {
                uint topBit = vertexID >> 1;
                uint botBit = (vertexID & 1);
                float u = topBit;
                float v = (topBit + botBit) & 1;
                #if UNITY_UV_STARTS_AT_TOP
                    v = 1.0 - v;
                #endif
                return float2(u, v);
            }

            //RGB > Linear
            void RGB_Linear_float(float4 In, out float4 Out)
            {
                float4 linearRGBLo = In / 12.92;;
                float4 linearRGBHi = pow(max(abs((In + 0.055) / 1.055), 1.192092896e-07), float4(2.4, 2.4, 2.4,2.4));
                Out = float4(In <= 0.04045) ? linearRGBLo : linearRGBHi;
            }

            //Linear > RGB
            void Linear_RGB_float(float4 In, out float4 Out)
            {
                float4 sRGBLo = In * 12.92;
                float4 sRGBHi = (pow(max(abs(In), 1.192092896e-07), float4(1.0 / 2.4, 1.0 / 2.4, 1.0 / 2.4, 1.0 / 2.4)) * 1.055) - 0.055;
                Out = float4(In <= 0.0031308) ? sRGBLo : sRGBHi;
            }

            ver_o Vertex(ver_v v) {
                ver_o o = (ver_o)0;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                #if _USE_DRAW_PROCEDURAL
                    o.positionCS = GetQuadVertexPositionBuilIn(v.vertexID);
                    o.positionCS.xy = o.positionCS.xy * float2(2.0f, -2.0f) + float2(-1.0f, 1.0f); //convert to -1..1
                    //o.uv = GetQuadTexCoord(v.vertexID) * _ScaleBias.xy + _ScaleBias.zw;
                    o.uv = GetQuadTexCoordBuilIn(v.vertexID);
                #else
                    o.positionCS = UnityObjectToClipPos(v.positionOS);
                    o.uv = v.uv;
                #endif

                return o;
            }

            half4 Fragment(ver_o input) : SV_Target
            {
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                float2 uv = input.uv;
                //uv.y = 1-uv.y;

                half4 col = tex2D(_MainTex,uv);

                #ifdef _NEEDLINEAR_TO_SRGB_CONVERSION
                    Linear_RGB_float(col, col);
                #endif

                col.rgb = col.rgb*col.a;

                return col;
            }

            ENDCG
        }
    }
}
