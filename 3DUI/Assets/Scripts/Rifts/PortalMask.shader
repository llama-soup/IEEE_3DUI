Shader "Portals/PortalMask"
{
    Properties
    {
        _MainTex("Main Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags 
        { 
            "RenderType" = "Opaque"
            "Queue" = "Geometry"
            "RenderPipeline" = "UniversalPipeline"
        }

        HLSLINCLUDE
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        ENDHLSL

        Pass
        {
            Name "Mask"

            Stencil
            {
                Ref 1
                Pass replace
            }

            HLSLPROGRAM
                #pragma vertex vert
                #pragma fragment frag
                #pragma multi_compile _ UNITY_SINGLE_PASS_STEREO STEREO_INSTANCING_ON STEREO_MULTIVIEW_ON

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

                v2f vert(appdata v)
                {
                    v2f o;
                    UNITY_SETUP_INSTANCE_ID(v);
                    UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                    o.vertex = TransformObjectToHClip(v.vertex.xyz);
                    o.uv = v.uv;
                    return o;
                }

                uniform sampler2D _MainTex;

                float4 frag(v2f i) : SV_Target
                {
                    UNITY_SETUP_STEREO_EYE_INDEX_POST_VERTEX(i);
                    float2 uv = i.uv;
                    
                    #if defined(UNITY_SINGLE_PASS_STEREO)
                        // Adjust UV for stereo rendering
                        uv.x = uv.x * 0.5 + (unity_StereoEyeIndex * 0.5);
                    #endif
                    
                    // Flip UVs if needed
                    // uv.y = 1 - uv.y; // Uncomment if image appears upside down
                    
                    return tex2D(_MainTex, uv);
                }
                
            ENDHLSL
        }
    }
}