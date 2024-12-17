Shader "Custom/PortalSurface2"
{
    Properties
    {
        _LeftEyeTex ("Left Eye Texture", 2D) = "white" {}
        _RightEyeTex ("Right Eye Texture", 2D) = "white" {}
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        
        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_local _ _MAIN_LIGHT_SHADOWS _MAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile_local_fragment _ _SHADOWS_SOFT
            #pragma multi_compile_instancing
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DeclareDepthTexture.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float4 screenPos : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                UNITY_VERTEX_OUTPUT_STEREO
            };

            TEXTURE2D_X(_LeftEyeTex);
            TEXTURE2D_X(_RightEyeTex);
            SAMPLER(sampler_LeftEyeTex);
            SAMPLER(sampler_RightEyeTex);
            
            Varyings vert(Attributes input)
            {
                Varyings output;
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_TRANSFER_INSTANCE_ID(input, output);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(output);
                
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.screenPos = ComputeScreenPos(output.positionCS);
                return output;
            }

            float4 frag(Varyings input) : SV_Target
            {
                UNITY_SETUP_INSTANCE_ID(input);
                UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(input);
                
                float2 screenUV = input.screenPos.xy / input.screenPos.w;
                uint viewIndex = unity_StereoEyeIndex;
                
                float4 color;
                if (viewIndex == 0)
                {
                    color = SAMPLE_TEXTURE2D_X(_LeftEyeTex, sampler_LeftEyeTex, screenUV);
                }
                else
                {
                    color = SAMPLE_TEXTURE2D_X(_RightEyeTex, sampler_RightEyeTex, screenUV);
                }
                
                return color;
            }
            ENDHLSL
        }
    }
}