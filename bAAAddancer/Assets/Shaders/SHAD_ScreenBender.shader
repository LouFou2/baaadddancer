Shader "SHAD_ScreenBender"
{
    Properties
    {
        _ScaleX("ScaleX", Range(0.001, 1)) = 0.1
        _ScaleY("ScaleY", Range(0.001, 1)) = 0.1
        _OffsetX("OffsetX", Range(-10, 10)) = 0.5
        _OffsetY("OffsetY", Range(-10, 10)) = 0.24
        _PowerZ("PowerZ", Range(0.1, 4)) = 1
        _PowerY("PowerY", Range(0.1, 4)) = 1
        [HDR]_ColorZ("ColorZ", Color) = (4, 0, 0, 0)
        [HDR]_ColorY("ColorY", Color) = (4, 0, 0, 0)
        _VertexShiftFactorZ("VertexShiftFactorZ", Float) = 1
        _ThumbPosL("ThumbPosL", Vector) = (0, 0, 0, 0)
        _ThumbPosR("ThumbPosR", Vector) = (0, 0, 0, 0)
        _Pow("Pow", Range(1, 16)) = 1
        _ThumbMagnitudeL("ThumbMagnitudeL", Float) = 0
        _ThumbMagnitudeR("ThumbMagnitudeR", Float) = 0
        [NoScaleOffset]_RendTex("RendTex", 2D) = "black" {}
        [HideInInspector]_QueueOffset("_QueueOffset", Float) = 0
        [HideInInspector]_QueueControl("_QueueControl", Float) = -1
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
    SubShader
    {
        HLSLINCLUDE

        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

        struct GeomData
        {
            float4 positionCS : SV_POSITION;
             float3 positionWS : INTERP0;
             float3 normalWS : INTERP1;
             float3 viewDirectionWS : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

        [maxvertexcount(3)] //***MAKE SURE THIS IS CORRECT AMOUNT
        void geom(triangle GeomData input[3], inout TriangleStream<GeomData> triStream)
        {
            GeomData vert1 = input[0];
            GeomData vert2 = input[1];
            GeomData vert3 = input[2];
            
            // a flat normal 
            //float3 normalizedEdge1 = normalize(vert2.positionWS - vert1.positionWS);
            //float3 normalizedEdge2 = normalize(vert3.positionWS - vert1.positionWS);
            //float3 flatNormal = normalize(cross(normalizedEdge1, normalizedEdge2));

            // to flatten the tri:
            //vert1.normalWS = flatNormal;
            //vert2.normalWS = flatNormal;
            //vert3.normalWS = flatNormal;

            // Compute centroid in world space
            float3 centroidWS = (vert1.positionWS + vert2.positionWS + vert3.positionWS) / 3;

            float shrinkAmount = vert1.positionWS.z; // we'll just use the first vert of the triangle's z position
            shrinkAmount = saturate(shrinkAmount); // ensure 0-1

            // "shrink" the triangles, relative to their z value in world space
            // Move the vertex toward the centroid
            float3 toCenter1 = centroidWS - vert1.positionWS;
            float3 toCenter2 = centroidWS - vert2.positionWS;
            float3 toCenter3 = centroidWS - vert3.positionWS;

            vert1.positionWS += toCenter1 * shrinkAmount;
            vert2.positionWS += toCenter2 * shrinkAmount;
            vert3.positionWS += toCenter3 * shrinkAmount;


            // make the tri:
            vert1.positionCS = TransformWorldToHClip(vert1.positionWS);
            vert2.positionCS = TransformWorldToHClip(vert2.positionWS);
            vert3.positionCS = TransformWorldToHClip(vert3.positionWS);

            triStream.Append(vert1);
            triStream.Append(vert2);
            triStream.Append(vert3);

            triStream.RestartStrip();
        }
        ENDHLSL
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Geometry"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalUnlitSubTarget"
        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                // LightMode: <None>
            }
        
        // Render State
        Cull Back
        Blend One Zero
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        #pragma geometry geom
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_UNLIT
        #define _FOG_FRAGMENT 1
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float3 viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 ObjectSpacePosition;
             float3 AbsoluteWorldSpacePosition;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float3 interp2 : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyz =  input.viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.viewDirectionWS = input.interp2.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _ScaleY;
        float _ScaleX;
        float _OffsetY;
        float _OffsetX;
        float _PowerY;
        float _PowerZ;
        float4 _ColorY;
        float4 _ColorZ;
        float _VertexShiftFactorZ;
        float2 _ThumbPosR;
        float2 _ThumbPosL;
        float _Pow;
        float _ThumbMagnitudeR;
        float _ThumbMagnitudeL;
        float4 _RendTex_TexelSize;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_RendTex);
        SAMPLER(sampler_RendTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Negate_float(float In, out float Out)
        {
            Out = -1 * In;
        }
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_446dc19484434c83967c65cfc4eb1fb5_R_1 = IN.ObjectSpacePosition[0];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_G_2 = IN.ObjectSpacePosition[1];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_B_3 = IN.ObjectSpacePosition[2];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_A_4 = 0;
            float2 _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0 = _ThumbPosL;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[0];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[1];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_B_3 = 0;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_A_4 = 0;
            float _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1;
            Unity_Negate_float(_Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1, _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1);
            float4 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4;
            float3 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5;
            float2 _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6;
            Unity_Combine_float(_Negate_2ad59c6a776a4f918862d473887f20f9_Out_1, _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2, 0, 0, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5, _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6);
            float _Split_defefc5a69434596b1aa452751bd1352_R_1 = IN.ObjectSpacePosition[0];
            float _Split_defefc5a69434596b1aa452751bd1352_G_2 = IN.ObjectSpacePosition[1];
            float _Split_defefc5a69434596b1aa452751bd1352_B_3 = IN.ObjectSpacePosition[2];
            float _Split_defefc5a69434596b1aa452751bd1352_A_4 = 0;
            float4 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4;
            float3 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5;
            float2 _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6;
            Unity_Combine_float(_Split_defefc5a69434596b1aa452751bd1352_R_1, _Split_defefc5a69434596b1aa452751bd1352_G_2, 0, 0, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6);
            float _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2;
            Unity_Distance_float2(_Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2);
            float _Remap_37127198c75b435ba2226a69a6929e67_Out_3;
            Unity_Remap_float(_Distance_c00ba968de284fb688fb5607c37c6c69_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_37127198c75b435ba2226a69a6929e67_Out_3);
            float _OneMinus_81594df369fa463c993993e3188fe57e_Out_1;
            Unity_OneMinus_float(_Remap_37127198c75b435ba2226a69a6929e67_Out_3, _OneMinus_81594df369fa463c993993e3188fe57e_Out_1);
            float _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0 = _Pow;
            float _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2;
            Unity_Power_float(_OneMinus_81594df369fa463c993993e3188fe57e_Out_1, _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0, _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2);
            float _Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0 = _VertexShiftFactorZ;
            float _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0 = _ThumbMagnitudeL;
            float _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2);
            float _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2;
            Unity_Multiply_float_float(_Power_989c374cd6fd4383b6901eb2a86fb148_Out_2, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2);
            float _Add_36318e66c25a4d24a221420cd5523e3b_Out_2;
            Unity_Add_float(_Split_446dc19484434c83967c65cfc4eb1fb5_B_3, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2, _Add_36318e66c25a4d24a221420cd5523e3b_Out_2);
            float2 _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0 = _ThumbPosR;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[0];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[1];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_B_3 = 0;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_A_4 = 0;
            float _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1;
            Unity_Negate_float(_Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1, _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1);
            float4 _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4;
            float3 _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5;
            float2 _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6;
            Unity_Combine_float(_Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1, _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2, 0, 0, _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4, _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5, _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6);
            float _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2;
            Unity_Distance_float2(_Combine_46940d00310044df99b10a7fb8bc4c33_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2);
            float _Remap_2fda5145689741abac020b9fe82f5615_Out_3;
            Unity_Remap_float(_Distance_eca9eb87869d4fa793b4337194ad776a_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_2fda5145689741abac020b9fe82f5615_Out_3);
            float _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1;
            Unity_OneMinus_float(_Remap_2fda5145689741abac020b9fe82f5615_Out_3, _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1);
            float _Property_5341cd1509214feeb193c9c00017ce81_Out_0 = _Pow;
            float _Power_4f2b65db1cb94455848606e911753371_Out_2;
            Unity_Power_float(_OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1, _Property_5341cd1509214feeb193c9c00017ce81_Out_0, _Power_4f2b65db1cb94455848606e911753371_Out_2);
            float _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0 = _ThumbMagnitudeR;
            float _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2);
            float _Multiply_7ba089fecfd1484894091913be08d114_Out_2;
            Unity_Multiply_float_float(_Power_4f2b65db1cb94455848606e911753371_Out_2, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2);
            float _Add_236176de907448c0a72c7d0f4b745891_Out_2;
            Unity_Add_float(_Add_36318e66c25a4d24a221420cd5523e3b_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2);
            float4 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4;
            float3 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            float2 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6;
            Unity_Combine_float(_Split_446dc19484434c83967c65cfc4eb1fb5_R_1, _Split_446dc19484434c83967c65cfc4eb1fb5_G_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2, 0, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6);
            description.Position = _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_e2a262c5b3164b00a013e7df72c6e5c0_Out_0 = UnityBuildTexture2DStructNoScale(_RendTex);
            float _Split_9cd4ad96b39949c8a0c57f3a60888cb4_R_1 = IN.AbsoluteWorldSpacePosition[0];
            float _Split_9cd4ad96b39949c8a0c57f3a60888cb4_G_2 = IN.AbsoluteWorldSpacePosition[1];
            float _Split_9cd4ad96b39949c8a0c57f3a60888cb4_B_3 = IN.AbsoluteWorldSpacePosition[2];
            float _Split_9cd4ad96b39949c8a0c57f3a60888cb4_A_4 = 0;
            float _Property_103b7ebbfb27412ba579bc799627f5a6_Out_0 = _ScaleX;
            float _Multiply_909373f9f70e4b6bab86b501b548395b_Out_2;
            Unity_Multiply_float_float(_Split_9cd4ad96b39949c8a0c57f3a60888cb4_R_1, _Property_103b7ebbfb27412ba579bc799627f5a6_Out_0, _Multiply_909373f9f70e4b6bab86b501b548395b_Out_2);
            float _Property_b81ebc64d10e4c6cbcb2107d6ad5f77f_Out_0 = _ScaleY;
            float _Multiply_becad06a9e004e05bbbfc9fdd0f031ef_Out_2;
            Unity_Multiply_float_float(_Split_9cd4ad96b39949c8a0c57f3a60888cb4_G_2, _Property_b81ebc64d10e4c6cbcb2107d6ad5f77f_Out_0, _Multiply_becad06a9e004e05bbbfc9fdd0f031ef_Out_2);
            float4 _Combine_e93674f2a9894a27ba9702ca067f92c6_RGBA_4;
            float3 _Combine_e93674f2a9894a27ba9702ca067f92c6_RGB_5;
            float2 _Combine_e93674f2a9894a27ba9702ca067f92c6_RG_6;
            Unity_Combine_float(_Multiply_909373f9f70e4b6bab86b501b548395b_Out_2, _Multiply_becad06a9e004e05bbbfc9fdd0f031ef_Out_2, 0, 0, _Combine_e93674f2a9894a27ba9702ca067f92c6_RGBA_4, _Combine_e93674f2a9894a27ba9702ca067f92c6_RGB_5, _Combine_e93674f2a9894a27ba9702ca067f92c6_RG_6);
            float _Property_c88029182e6646869bb6d1e054d35ec9_Out_0 = _OffsetX;
            float _Property_564c558b9e6d4cc9a923335a6d38f84b_Out_0 = _OffsetY;
            float2 _Vector2_82023f8651d344949a743a97386a233a_Out_0 = float2(_Property_c88029182e6646869bb6d1e054d35ec9_Out_0, _Property_564c558b9e6d4cc9a923335a6d38f84b_Out_0);
            float2 _TilingAndOffset_1308e39247e24997943404e5cc78eeb9_Out_3;
            Unity_TilingAndOffset_float(_Combine_e93674f2a9894a27ba9702ca067f92c6_RG_6, float2 (1, 1), _Vector2_82023f8651d344949a743a97386a233a_Out_0, _TilingAndOffset_1308e39247e24997943404e5cc78eeb9_Out_3);
            float4 _SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e2a262c5b3164b00a013e7df72c6e5c0_Out_0.tex, _Property_e2a262c5b3164b00a013e7df72c6e5c0_Out_0.samplerstate, _Property_e2a262c5b3164b00a013e7df72c6e5c0_Out_0.GetTransformedUV(_TilingAndOffset_1308e39247e24997943404e5cc78eeb9_Out_3));
            float _SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_R_4 = _SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_RGBA_0.r;
            float _SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_G_5 = _SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_RGBA_0.g;
            float _SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_B_6 = _SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_RGBA_0.b;
            float _SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_A_7 = _SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_RGBA_0.a;
            float4 _Combine_bf9c2558e432425fbe1d28f4682c1746_RGBA_4;
            float3 _Combine_bf9c2558e432425fbe1d28f4682c1746_RGB_5;
            float2 _Combine_bf9c2558e432425fbe1d28f4682c1746_RG_6;
            Unity_Combine_float(_SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_R_4, _SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_G_5, _SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_B_6, 0, _Combine_bf9c2558e432425fbe1d28f4682c1746_RGBA_4, _Combine_bf9c2558e432425fbe1d28f4682c1746_RGB_5, _Combine_bf9c2558e432425fbe1d28f4682c1746_RG_6);
            float4 _Property_5b951af995e84a6d843a86841bf18592_Out_0 = IsGammaSpace() ? LinearToSRGB(_ColorY) : _ColorY;
            float4 _Property_6ba6072d6ac0484f99751018f6d111f7_Out_0 = IsGammaSpace() ? LinearToSRGB(_ColorZ) : _ColorZ;
            float _Split_fa88f550f4564e0492f56d9710c18bed_R_1 = IN.ObjectSpacePosition[0];
            float _Split_fa88f550f4564e0492f56d9710c18bed_G_2 = IN.ObjectSpacePosition[1];
            float _Split_fa88f550f4564e0492f56d9710c18bed_B_3 = IN.ObjectSpacePosition[2];
            float _Split_fa88f550f4564e0492f56d9710c18bed_A_4 = 0;
            float _Remap_c38ab87c4109438796bf8f78a85e112c_Out_3;
            Unity_Remap_float(_Split_fa88f550f4564e0492f56d9710c18bed_G_2, float2 (-1, 1), float2 (0, 1), _Remap_c38ab87c4109438796bf8f78a85e112c_Out_3);
            float _Property_a189f388d34944c1abae6fe6953443d7_Out_0 = _PowerY;
            float _Power_3138866bb40243d7b78e0eba39fc7951_Out_2;
            Unity_Power_float(_Remap_c38ab87c4109438796bf8f78a85e112c_Out_3, _Property_a189f388d34944c1abae6fe6953443d7_Out_0, _Power_3138866bb40243d7b78e0eba39fc7951_Out_2);
            float4 _Lerp_db7f8453ee344183945de8600b4f3010_Out_3;
            Unity_Lerp_float4(_Property_5b951af995e84a6d843a86841bf18592_Out_0, _Property_6ba6072d6ac0484f99751018f6d111f7_Out_0, (_Power_3138866bb40243d7b78e0eba39fc7951_Out_2.xxxx), _Lerp_db7f8453ee344183945de8600b4f3010_Out_3);
            float _Property_bfd37c13048c4c2a95268a4bf4c6c67a_Out_0 = _PowerZ;
            float _Power_2523c3fd54b5403e8a1d7a5dbc9737f7_Out_2;
            Unity_Power_float(_Split_fa88f550f4564e0492f56d9710c18bed_B_3, _Property_bfd37c13048c4c2a95268a4bf4c6c67a_Out_0, _Power_2523c3fd54b5403e8a1d7a5dbc9737f7_Out_2);
            float3 _Lerp_933214f96d284e988bde391446524e45_Out_3;
            Unity_Lerp_float3(_Combine_bf9c2558e432425fbe1d28f4682c1746_RGB_5, (_Lerp_db7f8453ee344183945de8600b4f3010_Out_3.xyz), (_Power_2523c3fd54b5403e8a1d7a5dbc9737f7_Out_2.xxx), _Lerp_933214f96d284e988bde391446524e45_Out_3);
            surface.BaseColor = _Lerp_933214f96d284e988bde391446524e45_Out_3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.ObjectSpacePosition = TransformWorldToObject(input.positionWS);
            output.AbsoluteWorldSpacePosition = GetAbsolutePositionWS(input.positionWS);
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        ColorMask 0
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        //#pragma geometry geom
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _ScaleY;
        float _ScaleX;
        float _OffsetY;
        float _OffsetX;
        float _PowerY;
        float _PowerZ;
        float4 _ColorY;
        float4 _ColorZ;
        float _VertexShiftFactorZ;
        float2 _ThumbPosR;
        float2 _ThumbPosL;
        float _Pow;
        float _ThumbMagnitudeR;
        float _ThumbMagnitudeL;
        float4 _RendTex_TexelSize;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_RendTex);
        SAMPLER(sampler_RendTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Negate_float(float In, out float Out)
        {
            Out = -1 * In;
        }
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_446dc19484434c83967c65cfc4eb1fb5_R_1 = IN.ObjectSpacePosition[0];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_G_2 = IN.ObjectSpacePosition[1];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_B_3 = IN.ObjectSpacePosition[2];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_A_4 = 0;
            float2 _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0 = _ThumbPosL;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[0];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[1];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_B_3 = 0;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_A_4 = 0;
            float _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1;
            Unity_Negate_float(_Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1, _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1);
            float4 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4;
            float3 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5;
            float2 _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6;
            Unity_Combine_float(_Negate_2ad59c6a776a4f918862d473887f20f9_Out_1, _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2, 0, 0, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5, _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6);
            float _Split_defefc5a69434596b1aa452751bd1352_R_1 = IN.ObjectSpacePosition[0];
            float _Split_defefc5a69434596b1aa452751bd1352_G_2 = IN.ObjectSpacePosition[1];
            float _Split_defefc5a69434596b1aa452751bd1352_B_3 = IN.ObjectSpacePosition[2];
            float _Split_defefc5a69434596b1aa452751bd1352_A_4 = 0;
            float4 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4;
            float3 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5;
            float2 _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6;
            Unity_Combine_float(_Split_defefc5a69434596b1aa452751bd1352_R_1, _Split_defefc5a69434596b1aa452751bd1352_G_2, 0, 0, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6);
            float _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2;
            Unity_Distance_float2(_Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2);
            float _Remap_37127198c75b435ba2226a69a6929e67_Out_3;
            Unity_Remap_float(_Distance_c00ba968de284fb688fb5607c37c6c69_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_37127198c75b435ba2226a69a6929e67_Out_3);
            float _OneMinus_81594df369fa463c993993e3188fe57e_Out_1;
            Unity_OneMinus_float(_Remap_37127198c75b435ba2226a69a6929e67_Out_3, _OneMinus_81594df369fa463c993993e3188fe57e_Out_1);
            float _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0 = _Pow;
            float _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2;
            Unity_Power_float(_OneMinus_81594df369fa463c993993e3188fe57e_Out_1, _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0, _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2);
            float _Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0 = _VertexShiftFactorZ;
            float _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0 = _ThumbMagnitudeL;
            float _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2);
            float _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2;
            Unity_Multiply_float_float(_Power_989c374cd6fd4383b6901eb2a86fb148_Out_2, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2);
            float _Add_36318e66c25a4d24a221420cd5523e3b_Out_2;
            Unity_Add_float(_Split_446dc19484434c83967c65cfc4eb1fb5_B_3, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2, _Add_36318e66c25a4d24a221420cd5523e3b_Out_2);
            float2 _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0 = _ThumbPosR;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[0];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[1];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_B_3 = 0;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_A_4 = 0;
            float _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1;
            Unity_Negate_float(_Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1, _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1);
            float4 _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4;
            float3 _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5;
            float2 _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6;
            Unity_Combine_float(_Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1, _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2, 0, 0, _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4, _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5, _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6);
            float _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2;
            Unity_Distance_float2(_Combine_46940d00310044df99b10a7fb8bc4c33_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2);
            float _Remap_2fda5145689741abac020b9fe82f5615_Out_3;
            Unity_Remap_float(_Distance_eca9eb87869d4fa793b4337194ad776a_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_2fda5145689741abac020b9fe82f5615_Out_3);
            float _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1;
            Unity_OneMinus_float(_Remap_2fda5145689741abac020b9fe82f5615_Out_3, _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1);
            float _Property_5341cd1509214feeb193c9c00017ce81_Out_0 = _Pow;
            float _Power_4f2b65db1cb94455848606e911753371_Out_2;
            Unity_Power_float(_OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1, _Property_5341cd1509214feeb193c9c00017ce81_Out_0, _Power_4f2b65db1cb94455848606e911753371_Out_2);
            float _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0 = _ThumbMagnitudeR;
            float _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2);
            float _Multiply_7ba089fecfd1484894091913be08d114_Out_2;
            Unity_Multiply_float_float(_Power_4f2b65db1cb94455848606e911753371_Out_2, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2);
            float _Add_236176de907448c0a72c7d0f4b745891_Out_2;
            Unity_Add_float(_Add_36318e66c25a4d24a221420cd5523e3b_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2);
            float4 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4;
            float3 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            float2 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6;
            Unity_Combine_float(_Split_446dc19484434c83967c65cfc4eb1fb5_R_1, _Split_446dc19484434c83967c65cfc4eb1fb5_G_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2, 0, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6);
            description.Position = _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthNormalsOnly"
            Tags
            {
                "LightMode" = "DepthNormalsOnly"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        //#pragma geometry geom // **THIS BREAKS
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS;
             float4 tangentWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.normalWS;
            output.interp1.xyzw =  input.tangentWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.normalWS = input.interp0.xyz;
            output.tangentWS = input.interp1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _ScaleY;
        float _ScaleX;
        float _OffsetY;
        float _OffsetX;
        float _PowerY;
        float _PowerZ;
        float4 _ColorY;
        float4 _ColorZ;
        float _VertexShiftFactorZ;
        float2 _ThumbPosR;
        float2 _ThumbPosL;
        float _Pow;
        float _ThumbMagnitudeR;
        float _ThumbMagnitudeL;
        float4 _RendTex_TexelSize;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_RendTex);
        SAMPLER(sampler_RendTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Negate_float(float In, out float Out)
        {
            Out = -1 * In;
        }
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_446dc19484434c83967c65cfc4eb1fb5_R_1 = IN.ObjectSpacePosition[0];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_G_2 = IN.ObjectSpacePosition[1];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_B_3 = IN.ObjectSpacePosition[2];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_A_4 = 0;
            float2 _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0 = _ThumbPosL;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[0];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[1];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_B_3 = 0;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_A_4 = 0;
            float _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1;
            Unity_Negate_float(_Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1, _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1);
            float4 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4;
            float3 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5;
            float2 _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6;
            Unity_Combine_float(_Negate_2ad59c6a776a4f918862d473887f20f9_Out_1, _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2, 0, 0, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5, _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6);
            float _Split_defefc5a69434596b1aa452751bd1352_R_1 = IN.ObjectSpacePosition[0];
            float _Split_defefc5a69434596b1aa452751bd1352_G_2 = IN.ObjectSpacePosition[1];
            float _Split_defefc5a69434596b1aa452751bd1352_B_3 = IN.ObjectSpacePosition[2];
            float _Split_defefc5a69434596b1aa452751bd1352_A_4 = 0;
            float4 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4;
            float3 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5;
            float2 _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6;
            Unity_Combine_float(_Split_defefc5a69434596b1aa452751bd1352_R_1, _Split_defefc5a69434596b1aa452751bd1352_G_2, 0, 0, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6);
            float _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2;
            Unity_Distance_float2(_Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2);
            float _Remap_37127198c75b435ba2226a69a6929e67_Out_3;
            Unity_Remap_float(_Distance_c00ba968de284fb688fb5607c37c6c69_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_37127198c75b435ba2226a69a6929e67_Out_3);
            float _OneMinus_81594df369fa463c993993e3188fe57e_Out_1;
            Unity_OneMinus_float(_Remap_37127198c75b435ba2226a69a6929e67_Out_3, _OneMinus_81594df369fa463c993993e3188fe57e_Out_1);
            float _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0 = _Pow;
            float _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2;
            Unity_Power_float(_OneMinus_81594df369fa463c993993e3188fe57e_Out_1, _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0, _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2);
            float _Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0 = _VertexShiftFactorZ;
            float _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0 = _ThumbMagnitudeL;
            float _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2);
            float _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2;
            Unity_Multiply_float_float(_Power_989c374cd6fd4383b6901eb2a86fb148_Out_2, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2);
            float _Add_36318e66c25a4d24a221420cd5523e3b_Out_2;
            Unity_Add_float(_Split_446dc19484434c83967c65cfc4eb1fb5_B_3, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2, _Add_36318e66c25a4d24a221420cd5523e3b_Out_2);
            float2 _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0 = _ThumbPosR;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[0];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[1];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_B_3 = 0;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_A_4 = 0;
            float _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1;
            Unity_Negate_float(_Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1, _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1);
            float4 _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4;
            float3 _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5;
            float2 _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6;
            Unity_Combine_float(_Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1, _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2, 0, 0, _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4, _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5, _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6);
            float _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2;
            Unity_Distance_float2(_Combine_46940d00310044df99b10a7fb8bc4c33_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2);
            float _Remap_2fda5145689741abac020b9fe82f5615_Out_3;
            Unity_Remap_float(_Distance_eca9eb87869d4fa793b4337194ad776a_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_2fda5145689741abac020b9fe82f5615_Out_3);
            float _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1;
            Unity_OneMinus_float(_Remap_2fda5145689741abac020b9fe82f5615_Out_3, _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1);
            float _Property_5341cd1509214feeb193c9c00017ce81_Out_0 = _Pow;
            float _Power_4f2b65db1cb94455848606e911753371_Out_2;
            Unity_Power_float(_OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1, _Property_5341cd1509214feeb193c9c00017ce81_Out_0, _Power_4f2b65db1cb94455848606e911753371_Out_2);
            float _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0 = _ThumbMagnitudeR;
            float _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2);
            float _Multiply_7ba089fecfd1484894091913be08d114_Out_2;
            Unity_Multiply_float_float(_Power_4f2b65db1cb94455848606e911753371_Out_2, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2);
            float _Add_236176de907448c0a72c7d0f4b745891_Out_2;
            Unity_Add_float(_Add_36318e66c25a4d24a221420cd5523e3b_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2);
            float4 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4;
            float3 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            float2 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6;
            Unity_Combine_float(_Split_446dc19484434c83967c65cfc4eb1fb5_R_1, _Split_446dc19484434c83967c65cfc4eb1fb5_G_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2, 0, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6);
            description.Position = _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma vertex vert
        //#pragma geometry geom
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _ScaleY;
        float _ScaleX;
        float _OffsetY;
        float _OffsetX;
        float _PowerY;
        float _PowerZ;
        float4 _ColorY;
        float4 _ColorZ;
        float _VertexShiftFactorZ;
        float2 _ThumbPosR;
        float2 _ThumbPosL;
        float _Pow;
        float _ThumbMagnitudeR;
        float _ThumbMagnitudeL;
        float4 _RendTex_TexelSize;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_RendTex);
        SAMPLER(sampler_RendTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Negate_float(float In, out float Out)
        {
            Out = -1 * In;
        }
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_446dc19484434c83967c65cfc4eb1fb5_R_1 = IN.ObjectSpacePosition[0];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_G_2 = IN.ObjectSpacePosition[1];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_B_3 = IN.ObjectSpacePosition[2];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_A_4 = 0;
            float2 _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0 = _ThumbPosL;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[0];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[1];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_B_3 = 0;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_A_4 = 0;
            float _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1;
            Unity_Negate_float(_Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1, _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1);
            float4 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4;
            float3 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5;
            float2 _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6;
            Unity_Combine_float(_Negate_2ad59c6a776a4f918862d473887f20f9_Out_1, _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2, 0, 0, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5, _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6);
            float _Split_defefc5a69434596b1aa452751bd1352_R_1 = IN.ObjectSpacePosition[0];
            float _Split_defefc5a69434596b1aa452751bd1352_G_2 = IN.ObjectSpacePosition[1];
            float _Split_defefc5a69434596b1aa452751bd1352_B_3 = IN.ObjectSpacePosition[2];
            float _Split_defefc5a69434596b1aa452751bd1352_A_4 = 0;
            float4 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4;
            float3 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5;
            float2 _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6;
            Unity_Combine_float(_Split_defefc5a69434596b1aa452751bd1352_R_1, _Split_defefc5a69434596b1aa452751bd1352_G_2, 0, 0, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6);
            float _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2;
            Unity_Distance_float2(_Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2);
            float _Remap_37127198c75b435ba2226a69a6929e67_Out_3;
            Unity_Remap_float(_Distance_c00ba968de284fb688fb5607c37c6c69_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_37127198c75b435ba2226a69a6929e67_Out_3);
            float _OneMinus_81594df369fa463c993993e3188fe57e_Out_1;
            Unity_OneMinus_float(_Remap_37127198c75b435ba2226a69a6929e67_Out_3, _OneMinus_81594df369fa463c993993e3188fe57e_Out_1);
            float _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0 = _Pow;
            float _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2;
            Unity_Power_float(_OneMinus_81594df369fa463c993993e3188fe57e_Out_1, _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0, _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2);
            float _Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0 = _VertexShiftFactorZ;
            float _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0 = _ThumbMagnitudeL;
            float _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2);
            float _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2;
            Unity_Multiply_float_float(_Power_989c374cd6fd4383b6901eb2a86fb148_Out_2, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2);
            float _Add_36318e66c25a4d24a221420cd5523e3b_Out_2;
            Unity_Add_float(_Split_446dc19484434c83967c65cfc4eb1fb5_B_3, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2, _Add_36318e66c25a4d24a221420cd5523e3b_Out_2);
            float2 _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0 = _ThumbPosR;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[0];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[1];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_B_3 = 0;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_A_4 = 0;
            float _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1;
            Unity_Negate_float(_Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1, _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1);
            float4 _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4;
            float3 _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5;
            float2 _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6;
            Unity_Combine_float(_Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1, _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2, 0, 0, _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4, _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5, _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6);
            float _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2;
            Unity_Distance_float2(_Combine_46940d00310044df99b10a7fb8bc4c33_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2);
            float _Remap_2fda5145689741abac020b9fe82f5615_Out_3;
            Unity_Remap_float(_Distance_eca9eb87869d4fa793b4337194ad776a_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_2fda5145689741abac020b9fe82f5615_Out_3);
            float _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1;
            Unity_OneMinus_float(_Remap_2fda5145689741abac020b9fe82f5615_Out_3, _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1);
            float _Property_5341cd1509214feeb193c9c00017ce81_Out_0 = _Pow;
            float _Power_4f2b65db1cb94455848606e911753371_Out_2;
            Unity_Power_float(_OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1, _Property_5341cd1509214feeb193c9c00017ce81_Out_0, _Power_4f2b65db1cb94455848606e911753371_Out_2);
            float _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0 = _ThumbMagnitudeR;
            float _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2);
            float _Multiply_7ba089fecfd1484894091913be08d114_Out_2;
            Unity_Multiply_float_float(_Power_4f2b65db1cb94455848606e911753371_Out_2, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2);
            float _Add_236176de907448c0a72c7d0f4b745891_Out_2;
            Unity_Add_float(_Add_36318e66c25a4d24a221420cd5523e3b_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2);
            float4 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4;
            float3 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            float2 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6;
            Unity_Combine_float(_Split_446dc19484434c83967c65cfc4eb1fb5_R_1, _Split_446dc19484434c83967c65cfc4eb1fb5_G_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2, 0, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6);
            description.Position = _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
        // Render State
        Cull Back
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma vertex vert
        //#pragma geometry geom
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _ScaleY;
        float _ScaleX;
        float _OffsetY;
        float _OffsetX;
        float _PowerY;
        float _PowerZ;
        float4 _ColorY;
        float4 _ColorZ;
        float _VertexShiftFactorZ;
        float2 _ThumbPosR;
        float2 _ThumbPosL;
        float _Pow;
        float _ThumbMagnitudeR;
        float _ThumbMagnitudeL;
        float4 _RendTex_TexelSize;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_RendTex);
        SAMPLER(sampler_RendTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Negate_float(float In, out float Out)
        {
            Out = -1 * In;
        }
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_446dc19484434c83967c65cfc4eb1fb5_R_1 = IN.ObjectSpacePosition[0];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_G_2 = IN.ObjectSpacePosition[1];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_B_3 = IN.ObjectSpacePosition[2];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_A_4 = 0;
            float2 _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0 = _ThumbPosL;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[0];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[1];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_B_3 = 0;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_A_4 = 0;
            float _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1;
            Unity_Negate_float(_Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1, _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1);
            float4 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4;
            float3 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5;
            float2 _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6;
            Unity_Combine_float(_Negate_2ad59c6a776a4f918862d473887f20f9_Out_1, _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2, 0, 0, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5, _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6);
            float _Split_defefc5a69434596b1aa452751bd1352_R_1 = IN.ObjectSpacePosition[0];
            float _Split_defefc5a69434596b1aa452751bd1352_G_2 = IN.ObjectSpacePosition[1];
            float _Split_defefc5a69434596b1aa452751bd1352_B_3 = IN.ObjectSpacePosition[2];
            float _Split_defefc5a69434596b1aa452751bd1352_A_4 = 0;
            float4 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4;
            float3 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5;
            float2 _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6;
            Unity_Combine_float(_Split_defefc5a69434596b1aa452751bd1352_R_1, _Split_defefc5a69434596b1aa452751bd1352_G_2, 0, 0, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6);
            float _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2;
            Unity_Distance_float2(_Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2);
            float _Remap_37127198c75b435ba2226a69a6929e67_Out_3;
            Unity_Remap_float(_Distance_c00ba968de284fb688fb5607c37c6c69_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_37127198c75b435ba2226a69a6929e67_Out_3);
            float _OneMinus_81594df369fa463c993993e3188fe57e_Out_1;
            Unity_OneMinus_float(_Remap_37127198c75b435ba2226a69a6929e67_Out_3, _OneMinus_81594df369fa463c993993e3188fe57e_Out_1);
            float _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0 = _Pow;
            float _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2;
            Unity_Power_float(_OneMinus_81594df369fa463c993993e3188fe57e_Out_1, _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0, _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2);
            float _Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0 = _VertexShiftFactorZ;
            float _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0 = _ThumbMagnitudeL;
            float _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2);
            float _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2;
            Unity_Multiply_float_float(_Power_989c374cd6fd4383b6901eb2a86fb148_Out_2, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2);
            float _Add_36318e66c25a4d24a221420cd5523e3b_Out_2;
            Unity_Add_float(_Split_446dc19484434c83967c65cfc4eb1fb5_B_3, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2, _Add_36318e66c25a4d24a221420cd5523e3b_Out_2);
            float2 _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0 = _ThumbPosR;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[0];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[1];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_B_3 = 0;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_A_4 = 0;
            float _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1;
            Unity_Negate_float(_Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1, _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1);
            float4 _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4;
            float3 _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5;
            float2 _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6;
            Unity_Combine_float(_Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1, _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2, 0, 0, _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4, _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5, _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6);
            float _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2;
            Unity_Distance_float2(_Combine_46940d00310044df99b10a7fb8bc4c33_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2);
            float _Remap_2fda5145689741abac020b9fe82f5615_Out_3;
            Unity_Remap_float(_Distance_eca9eb87869d4fa793b4337194ad776a_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_2fda5145689741abac020b9fe82f5615_Out_3);
            float _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1;
            Unity_OneMinus_float(_Remap_2fda5145689741abac020b9fe82f5615_Out_3, _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1);
            float _Property_5341cd1509214feeb193c9c00017ce81_Out_0 = _Pow;
            float _Power_4f2b65db1cb94455848606e911753371_Out_2;
            Unity_Power_float(_OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1, _Property_5341cd1509214feeb193c9c00017ce81_Out_0, _Power_4f2b65db1cb94455848606e911753371_Out_2);
            float _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0 = _ThumbMagnitudeR;
            float _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2);
            float _Multiply_7ba089fecfd1484894091913be08d114_Out_2;
            Unity_Multiply_float_float(_Power_4f2b65db1cb94455848606e911753371_Out_2, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2);
            float _Add_236176de907448c0a72c7d0f4b745891_Out_2;
            Unity_Add_float(_Add_36318e66c25a4d24a221420cd5523e3b_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2);
            float4 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4;
            float3 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            float2 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6;
            Unity_Combine_float(_Split_446dc19484434c83967c65cfc4eb1fb5_R_1, _Split_446dc19484434c83967c65cfc4eb1fb5_G_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2, 0, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6);
            description.Position = _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormalsOnly"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 4.5
        #pragma exclude_renderers gles gles3 glcore
        #pragma multi_compile_instancing
        #pragma multi_compile _ DOTS_INSTANCING_ON
        #pragma vertex vert
        //#pragma geometry geom
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define VARYINGS_NEED_NORMAL_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.normalWS = input.interp0.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _ScaleY;
        float _ScaleX;
        float _OffsetY;
        float _OffsetX;
        float _PowerY;
        float _PowerZ;
        float4 _ColorY;
        float4 _ColorZ;
        float _VertexShiftFactorZ;
        float2 _ThumbPosR;
        float2 _ThumbPosL;
        float _Pow;
        float _ThumbMagnitudeR;
        float _ThumbMagnitudeL;
        float4 _RendTex_TexelSize;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_RendTex);
        SAMPLER(sampler_RendTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Negate_float(float In, out float Out)
        {
            Out = -1 * In;
        }
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_446dc19484434c83967c65cfc4eb1fb5_R_1 = IN.ObjectSpacePosition[0];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_G_2 = IN.ObjectSpacePosition[1];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_B_3 = IN.ObjectSpacePosition[2];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_A_4 = 0;
            float2 _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0 = _ThumbPosL;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[0];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[1];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_B_3 = 0;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_A_4 = 0;
            float _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1;
            Unity_Negate_float(_Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1, _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1);
            float4 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4;
            float3 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5;
            float2 _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6;
            Unity_Combine_float(_Negate_2ad59c6a776a4f918862d473887f20f9_Out_1, _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2, 0, 0, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5, _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6);
            float _Split_defefc5a69434596b1aa452751bd1352_R_1 = IN.ObjectSpacePosition[0];
            float _Split_defefc5a69434596b1aa452751bd1352_G_2 = IN.ObjectSpacePosition[1];
            float _Split_defefc5a69434596b1aa452751bd1352_B_3 = IN.ObjectSpacePosition[2];
            float _Split_defefc5a69434596b1aa452751bd1352_A_4 = 0;
            float4 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4;
            float3 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5;
            float2 _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6;
            Unity_Combine_float(_Split_defefc5a69434596b1aa452751bd1352_R_1, _Split_defefc5a69434596b1aa452751bd1352_G_2, 0, 0, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6);
            float _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2;
            Unity_Distance_float2(_Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2);
            float _Remap_37127198c75b435ba2226a69a6929e67_Out_3;
            Unity_Remap_float(_Distance_c00ba968de284fb688fb5607c37c6c69_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_37127198c75b435ba2226a69a6929e67_Out_3);
            float _OneMinus_81594df369fa463c993993e3188fe57e_Out_1;
            Unity_OneMinus_float(_Remap_37127198c75b435ba2226a69a6929e67_Out_3, _OneMinus_81594df369fa463c993993e3188fe57e_Out_1);
            float _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0 = _Pow;
            float _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2;
            Unity_Power_float(_OneMinus_81594df369fa463c993993e3188fe57e_Out_1, _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0, _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2);
            float _Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0 = _VertexShiftFactorZ;
            float _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0 = _ThumbMagnitudeL;
            float _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2);
            float _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2;
            Unity_Multiply_float_float(_Power_989c374cd6fd4383b6901eb2a86fb148_Out_2, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2);
            float _Add_36318e66c25a4d24a221420cd5523e3b_Out_2;
            Unity_Add_float(_Split_446dc19484434c83967c65cfc4eb1fb5_B_3, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2, _Add_36318e66c25a4d24a221420cd5523e3b_Out_2);
            float2 _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0 = _ThumbPosR;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[0];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[1];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_B_3 = 0;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_A_4 = 0;
            float _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1;
            Unity_Negate_float(_Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1, _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1);
            float4 _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4;
            float3 _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5;
            float2 _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6;
            Unity_Combine_float(_Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1, _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2, 0, 0, _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4, _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5, _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6);
            float _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2;
            Unity_Distance_float2(_Combine_46940d00310044df99b10a7fb8bc4c33_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2);
            float _Remap_2fda5145689741abac020b9fe82f5615_Out_3;
            Unity_Remap_float(_Distance_eca9eb87869d4fa793b4337194ad776a_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_2fda5145689741abac020b9fe82f5615_Out_3);
            float _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1;
            Unity_OneMinus_float(_Remap_2fda5145689741abac020b9fe82f5615_Out_3, _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1);
            float _Property_5341cd1509214feeb193c9c00017ce81_Out_0 = _Pow;
            float _Power_4f2b65db1cb94455848606e911753371_Out_2;
            Unity_Power_float(_OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1, _Property_5341cd1509214feeb193c9c00017ce81_Out_0, _Power_4f2b65db1cb94455848606e911753371_Out_2);
            float _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0 = _ThumbMagnitudeR;
            float _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2);
            float _Multiply_7ba089fecfd1484894091913be08d114_Out_2;
            Unity_Multiply_float_float(_Power_4f2b65db1cb94455848606e911753371_Out_2, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2);
            float _Add_236176de907448c0a72c7d0f4b745891_Out_2;
            Unity_Add_float(_Add_36318e66c25a4d24a221420cd5523e3b_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2);
            float4 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4;
            float3 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            float2 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6;
            Unity_Combine_float(_Split_446dc19484434c83967c65cfc4eb1fb5_R_1, _Split_446dc19484434c83967c65cfc4eb1fb5_G_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2, 0, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6);
            description.Position = _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    SubShader
    {
        Tags
        {
            "RenderPipeline"="UniversalPipeline"
            "RenderType"="Opaque"
            "UniversalMaterialType" = "Unlit"
            "Queue"="Geometry"
            "ShaderGraphShader"="true"
            "ShaderGraphTargetId"="UniversalUnlitSubTarget"
        }
        Pass
        {
            Name "Universal Forward"
            Tags
            {
                // LightMode: <None>
            }
        
        // Render State
        Cull Back
        Blend One Zero
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        #pragma geometry geom
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        #pragma multi_compile _ LIGHTMAP_ON
        #pragma multi_compile _ DIRLIGHTMAP_COMBINED
        #pragma shader_feature _ _SAMPLE_GI
        #pragma multi_compile_fragment _ _DBUFFER_MRT1 _DBUFFER_MRT2 _DBUFFER_MRT3
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_VIEWDIRECTION_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_UNLIT
        #define _FOG_FRAGMENT 1
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/DBuffer.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float3 normalWS;
             float3 viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float3 ObjectSpacePosition;
             float3 AbsoluteWorldSpacePosition;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float3 interp1 : INTERP1;
             float3 interp2 : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.positionWS;
            output.interp1.xyz =  input.normalWS;
            output.interp2.xyz =  input.viewDirectionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.positionWS = input.interp0.xyz;
            output.normalWS = input.interp1.xyz;
            output.viewDirectionWS = input.interp2.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _ScaleY;
        float _ScaleX;
        float _OffsetY;
        float _OffsetX;
        float _PowerY;
        float _PowerZ;
        float4 _ColorY;
        float4 _ColorZ;
        float _VertexShiftFactorZ;
        float2 _ThumbPosR;
        float2 _ThumbPosL;
        float _Pow;
        float _ThumbMagnitudeR;
        float _ThumbMagnitudeL;
        float4 _RendTex_TexelSize;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_RendTex);
        SAMPLER(sampler_RendTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Negate_float(float In, out float Out)
        {
            Out = -1 * In;
        }
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        void Unity_TilingAndOffset_float(float2 UV, float2 Tiling, float2 Offset, out float2 Out)
        {
            Out = UV * Tiling + Offset;
        }
        
        void Unity_Lerp_float4(float4 A, float4 B, float4 T, out float4 Out)
        {
            Out = lerp(A, B, T);
        }
        
        void Unity_Lerp_float3(float3 A, float3 B, float3 T, out float3 Out)
        {
            Out = lerp(A, B, T);
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_446dc19484434c83967c65cfc4eb1fb5_R_1 = IN.ObjectSpacePosition[0];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_G_2 = IN.ObjectSpacePosition[1];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_B_3 = IN.ObjectSpacePosition[2];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_A_4 = 0;
            float2 _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0 = _ThumbPosL;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[0];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[1];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_B_3 = 0;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_A_4 = 0;
            float _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1;
            Unity_Negate_float(_Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1, _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1);
            float4 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4;
            float3 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5;
            float2 _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6;
            Unity_Combine_float(_Negate_2ad59c6a776a4f918862d473887f20f9_Out_1, _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2, 0, 0, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5, _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6);
            float _Split_defefc5a69434596b1aa452751bd1352_R_1 = IN.ObjectSpacePosition[0];
            float _Split_defefc5a69434596b1aa452751bd1352_G_2 = IN.ObjectSpacePosition[1];
            float _Split_defefc5a69434596b1aa452751bd1352_B_3 = IN.ObjectSpacePosition[2];
            float _Split_defefc5a69434596b1aa452751bd1352_A_4 = 0;
            float4 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4;
            float3 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5;
            float2 _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6;
            Unity_Combine_float(_Split_defefc5a69434596b1aa452751bd1352_R_1, _Split_defefc5a69434596b1aa452751bd1352_G_2, 0, 0, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6);
            float _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2;
            Unity_Distance_float2(_Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2);
            float _Remap_37127198c75b435ba2226a69a6929e67_Out_3;
            Unity_Remap_float(_Distance_c00ba968de284fb688fb5607c37c6c69_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_37127198c75b435ba2226a69a6929e67_Out_3);
            float _OneMinus_81594df369fa463c993993e3188fe57e_Out_1;
            Unity_OneMinus_float(_Remap_37127198c75b435ba2226a69a6929e67_Out_3, _OneMinus_81594df369fa463c993993e3188fe57e_Out_1);
            float _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0 = _Pow;
            float _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2;
            Unity_Power_float(_OneMinus_81594df369fa463c993993e3188fe57e_Out_1, _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0, _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2);
            float _Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0 = _VertexShiftFactorZ;
            float _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0 = _ThumbMagnitudeL;
            float _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2);
            float _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2;
            Unity_Multiply_float_float(_Power_989c374cd6fd4383b6901eb2a86fb148_Out_2, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2);
            float _Add_36318e66c25a4d24a221420cd5523e3b_Out_2;
            Unity_Add_float(_Split_446dc19484434c83967c65cfc4eb1fb5_B_3, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2, _Add_36318e66c25a4d24a221420cd5523e3b_Out_2);
            float2 _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0 = _ThumbPosR;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[0];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[1];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_B_3 = 0;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_A_4 = 0;
            float _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1;
            Unity_Negate_float(_Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1, _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1);
            float4 _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4;
            float3 _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5;
            float2 _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6;
            Unity_Combine_float(_Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1, _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2, 0, 0, _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4, _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5, _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6);
            float _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2;
            Unity_Distance_float2(_Combine_46940d00310044df99b10a7fb8bc4c33_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2);
            float _Remap_2fda5145689741abac020b9fe82f5615_Out_3;
            Unity_Remap_float(_Distance_eca9eb87869d4fa793b4337194ad776a_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_2fda5145689741abac020b9fe82f5615_Out_3);
            float _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1;
            Unity_OneMinus_float(_Remap_2fda5145689741abac020b9fe82f5615_Out_3, _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1);
            float _Property_5341cd1509214feeb193c9c00017ce81_Out_0 = _Pow;
            float _Power_4f2b65db1cb94455848606e911753371_Out_2;
            Unity_Power_float(_OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1, _Property_5341cd1509214feeb193c9c00017ce81_Out_0, _Power_4f2b65db1cb94455848606e911753371_Out_2);
            float _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0 = _ThumbMagnitudeR;
            float _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2);
            float _Multiply_7ba089fecfd1484894091913be08d114_Out_2;
            Unity_Multiply_float_float(_Power_4f2b65db1cb94455848606e911753371_Out_2, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2);
            float _Add_236176de907448c0a72c7d0f4b745891_Out_2;
            Unity_Add_float(_Add_36318e66c25a4d24a221420cd5523e3b_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2);
            float4 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4;
            float3 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            float2 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6;
            Unity_Combine_float(_Split_446dc19484434c83967c65cfc4eb1fb5_R_1, _Split_446dc19484434c83967c65cfc4eb1fb5_G_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2, 0, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6);
            description.Position = _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
            float3 BaseColor;
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            UnityTexture2D _Property_e2a262c5b3164b00a013e7df72c6e5c0_Out_0 = UnityBuildTexture2DStructNoScale(_RendTex);
            float _Split_9cd4ad96b39949c8a0c57f3a60888cb4_R_1 = IN.AbsoluteWorldSpacePosition[0];
            float _Split_9cd4ad96b39949c8a0c57f3a60888cb4_G_2 = IN.AbsoluteWorldSpacePosition[1];
            float _Split_9cd4ad96b39949c8a0c57f3a60888cb4_B_3 = IN.AbsoluteWorldSpacePosition[2];
            float _Split_9cd4ad96b39949c8a0c57f3a60888cb4_A_4 = 0;
            float _Property_103b7ebbfb27412ba579bc799627f5a6_Out_0 = _ScaleX;
            float _Multiply_909373f9f70e4b6bab86b501b548395b_Out_2;
            Unity_Multiply_float_float(_Split_9cd4ad96b39949c8a0c57f3a60888cb4_R_1, _Property_103b7ebbfb27412ba579bc799627f5a6_Out_0, _Multiply_909373f9f70e4b6bab86b501b548395b_Out_2);
            float _Property_b81ebc64d10e4c6cbcb2107d6ad5f77f_Out_0 = _ScaleY;
            float _Multiply_becad06a9e004e05bbbfc9fdd0f031ef_Out_2;
            Unity_Multiply_float_float(_Split_9cd4ad96b39949c8a0c57f3a60888cb4_G_2, _Property_b81ebc64d10e4c6cbcb2107d6ad5f77f_Out_0, _Multiply_becad06a9e004e05bbbfc9fdd0f031ef_Out_2);
            float4 _Combine_e93674f2a9894a27ba9702ca067f92c6_RGBA_4;
            float3 _Combine_e93674f2a9894a27ba9702ca067f92c6_RGB_5;
            float2 _Combine_e93674f2a9894a27ba9702ca067f92c6_RG_6;
            Unity_Combine_float(_Multiply_909373f9f70e4b6bab86b501b548395b_Out_2, _Multiply_becad06a9e004e05bbbfc9fdd0f031ef_Out_2, 0, 0, _Combine_e93674f2a9894a27ba9702ca067f92c6_RGBA_4, _Combine_e93674f2a9894a27ba9702ca067f92c6_RGB_5, _Combine_e93674f2a9894a27ba9702ca067f92c6_RG_6);
            float _Property_c88029182e6646869bb6d1e054d35ec9_Out_0 = _OffsetX;
            float _Property_564c558b9e6d4cc9a923335a6d38f84b_Out_0 = _OffsetY;
            float2 _Vector2_82023f8651d344949a743a97386a233a_Out_0 = float2(_Property_c88029182e6646869bb6d1e054d35ec9_Out_0, _Property_564c558b9e6d4cc9a923335a6d38f84b_Out_0);
            float2 _TilingAndOffset_1308e39247e24997943404e5cc78eeb9_Out_3;
            Unity_TilingAndOffset_float(_Combine_e93674f2a9894a27ba9702ca067f92c6_RG_6, float2 (1, 1), _Vector2_82023f8651d344949a743a97386a233a_Out_0, _TilingAndOffset_1308e39247e24997943404e5cc78eeb9_Out_3);
            float4 _SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_RGBA_0 = SAMPLE_TEXTURE2D(_Property_e2a262c5b3164b00a013e7df72c6e5c0_Out_0.tex, _Property_e2a262c5b3164b00a013e7df72c6e5c0_Out_0.samplerstate, _Property_e2a262c5b3164b00a013e7df72c6e5c0_Out_0.GetTransformedUV(_TilingAndOffset_1308e39247e24997943404e5cc78eeb9_Out_3));
            float _SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_R_4 = _SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_RGBA_0.r;
            float _SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_G_5 = _SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_RGBA_0.g;
            float _SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_B_6 = _SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_RGBA_0.b;
            float _SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_A_7 = _SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_RGBA_0.a;
            float4 _Combine_bf9c2558e432425fbe1d28f4682c1746_RGBA_4;
            float3 _Combine_bf9c2558e432425fbe1d28f4682c1746_RGB_5;
            float2 _Combine_bf9c2558e432425fbe1d28f4682c1746_RG_6;
            Unity_Combine_float(_SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_R_4, _SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_G_5, _SampleTexture2D_0fff2fd8cc0541b1afbd9caacb05cc4d_B_6, 0, _Combine_bf9c2558e432425fbe1d28f4682c1746_RGBA_4, _Combine_bf9c2558e432425fbe1d28f4682c1746_RGB_5, _Combine_bf9c2558e432425fbe1d28f4682c1746_RG_6);
            float4 _Property_5b951af995e84a6d843a86841bf18592_Out_0 = IsGammaSpace() ? LinearToSRGB(_ColorY) : _ColorY;
            float4 _Property_6ba6072d6ac0484f99751018f6d111f7_Out_0 = IsGammaSpace() ? LinearToSRGB(_ColorZ) : _ColorZ;
            float _Split_fa88f550f4564e0492f56d9710c18bed_R_1 = IN.ObjectSpacePosition[0];
            float _Split_fa88f550f4564e0492f56d9710c18bed_G_2 = IN.ObjectSpacePosition[1];
            float _Split_fa88f550f4564e0492f56d9710c18bed_B_3 = IN.ObjectSpacePosition[2];
            float _Split_fa88f550f4564e0492f56d9710c18bed_A_4 = 0;
            float _Remap_c38ab87c4109438796bf8f78a85e112c_Out_3;
            Unity_Remap_float(_Split_fa88f550f4564e0492f56d9710c18bed_G_2, float2 (-1, 1), float2 (0, 1), _Remap_c38ab87c4109438796bf8f78a85e112c_Out_3);
            float _Property_a189f388d34944c1abae6fe6953443d7_Out_0 = _PowerY;
            float _Power_3138866bb40243d7b78e0eba39fc7951_Out_2;
            Unity_Power_float(_Remap_c38ab87c4109438796bf8f78a85e112c_Out_3, _Property_a189f388d34944c1abae6fe6953443d7_Out_0, _Power_3138866bb40243d7b78e0eba39fc7951_Out_2);
            float4 _Lerp_db7f8453ee344183945de8600b4f3010_Out_3;
            Unity_Lerp_float4(_Property_5b951af995e84a6d843a86841bf18592_Out_0, _Property_6ba6072d6ac0484f99751018f6d111f7_Out_0, (_Power_3138866bb40243d7b78e0eba39fc7951_Out_2.xxxx), _Lerp_db7f8453ee344183945de8600b4f3010_Out_3);
            float _Property_bfd37c13048c4c2a95268a4bf4c6c67a_Out_0 = _PowerZ;
            float _Power_2523c3fd54b5403e8a1d7a5dbc9737f7_Out_2;
            Unity_Power_float(_Split_fa88f550f4564e0492f56d9710c18bed_B_3, _Property_bfd37c13048c4c2a95268a4bf4c6c67a_Out_0, _Power_2523c3fd54b5403e8a1d7a5dbc9737f7_Out_2);
            float3 _Lerp_933214f96d284e988bde391446524e45_Out_3;
            Unity_Lerp_float3(_Combine_bf9c2558e432425fbe1d28f4682c1746_RGB_5, (_Lerp_db7f8453ee344183945de8600b4f3010_Out_3.xyz), (_Power_2523c3fd54b5403e8a1d7a5dbc9737f7_Out_2.xxx), _Lerp_933214f96d284e988bde391446524e45_Out_3);
            surface.BaseColor = _Lerp_933214f96d284e988bde391446524e45_Out_3;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
            output.ObjectSpacePosition = TransformWorldToObject(input.positionWS);
            output.AbsoluteWorldSpacePosition = GetAbsolutePositionWS(input.positionWS);
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/UnlitPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthOnly"
            Tags
            {
                "LightMode" = "DepthOnly"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        ColorMask 0
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        //#pragma geometry geom
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _ScaleY;
        float _ScaleX;
        float _OffsetY;
        float _OffsetX;
        float _PowerY;
        float _PowerZ;
        float4 _ColorY;
        float4 _ColorZ;
        float _VertexShiftFactorZ;
        float2 _ThumbPosR;
        float2 _ThumbPosL;
        float _Pow;
        float _ThumbMagnitudeR;
        float _ThumbMagnitudeL;
        float4 _RendTex_TexelSize;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_RendTex);
        SAMPLER(sampler_RendTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Negate_float(float In, out float Out)
        {
            Out = -1 * In;
        }
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_446dc19484434c83967c65cfc4eb1fb5_R_1 = IN.ObjectSpacePosition[0];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_G_2 = IN.ObjectSpacePosition[1];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_B_3 = IN.ObjectSpacePosition[2];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_A_4 = 0;
            float2 _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0 = _ThumbPosL;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[0];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[1];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_B_3 = 0;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_A_4 = 0;
            float _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1;
            Unity_Negate_float(_Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1, _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1);
            float4 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4;
            float3 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5;
            float2 _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6;
            Unity_Combine_float(_Negate_2ad59c6a776a4f918862d473887f20f9_Out_1, _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2, 0, 0, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5, _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6);
            float _Split_defefc5a69434596b1aa452751bd1352_R_1 = IN.ObjectSpacePosition[0];
            float _Split_defefc5a69434596b1aa452751bd1352_G_2 = IN.ObjectSpacePosition[1];
            float _Split_defefc5a69434596b1aa452751bd1352_B_3 = IN.ObjectSpacePosition[2];
            float _Split_defefc5a69434596b1aa452751bd1352_A_4 = 0;
            float4 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4;
            float3 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5;
            float2 _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6;
            Unity_Combine_float(_Split_defefc5a69434596b1aa452751bd1352_R_1, _Split_defefc5a69434596b1aa452751bd1352_G_2, 0, 0, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6);
            float _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2;
            Unity_Distance_float2(_Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2);
            float _Remap_37127198c75b435ba2226a69a6929e67_Out_3;
            Unity_Remap_float(_Distance_c00ba968de284fb688fb5607c37c6c69_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_37127198c75b435ba2226a69a6929e67_Out_3);
            float _OneMinus_81594df369fa463c993993e3188fe57e_Out_1;
            Unity_OneMinus_float(_Remap_37127198c75b435ba2226a69a6929e67_Out_3, _OneMinus_81594df369fa463c993993e3188fe57e_Out_1);
            float _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0 = _Pow;
            float _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2;
            Unity_Power_float(_OneMinus_81594df369fa463c993993e3188fe57e_Out_1, _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0, _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2);
            float _Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0 = _VertexShiftFactorZ;
            float _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0 = _ThumbMagnitudeL;
            float _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2);
            float _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2;
            Unity_Multiply_float_float(_Power_989c374cd6fd4383b6901eb2a86fb148_Out_2, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2);
            float _Add_36318e66c25a4d24a221420cd5523e3b_Out_2;
            Unity_Add_float(_Split_446dc19484434c83967c65cfc4eb1fb5_B_3, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2, _Add_36318e66c25a4d24a221420cd5523e3b_Out_2);
            float2 _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0 = _ThumbPosR;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[0];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[1];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_B_3 = 0;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_A_4 = 0;
            float _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1;
            Unity_Negate_float(_Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1, _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1);
            float4 _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4;
            float3 _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5;
            float2 _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6;
            Unity_Combine_float(_Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1, _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2, 0, 0, _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4, _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5, _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6);
            float _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2;
            Unity_Distance_float2(_Combine_46940d00310044df99b10a7fb8bc4c33_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2);
            float _Remap_2fda5145689741abac020b9fe82f5615_Out_3;
            Unity_Remap_float(_Distance_eca9eb87869d4fa793b4337194ad776a_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_2fda5145689741abac020b9fe82f5615_Out_3);
            float _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1;
            Unity_OneMinus_float(_Remap_2fda5145689741abac020b9fe82f5615_Out_3, _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1);
            float _Property_5341cd1509214feeb193c9c00017ce81_Out_0 = _Pow;
            float _Power_4f2b65db1cb94455848606e911753371_Out_2;
            Unity_Power_float(_OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1, _Property_5341cd1509214feeb193c9c00017ce81_Out_0, _Power_4f2b65db1cb94455848606e911753371_Out_2);
            float _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0 = _ThumbMagnitudeR;
            float _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2);
            float _Multiply_7ba089fecfd1484894091913be08d114_Out_2;
            Unity_Multiply_float_float(_Power_4f2b65db1cb94455848606e911753371_Out_2, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2);
            float _Add_236176de907448c0a72c7d0f4b745891_Out_2;
            Unity_Add_float(_Add_36318e66c25a4d24a221420cd5523e3b_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2);
            float4 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4;
            float3 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            float2 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6;
            Unity_Combine_float(_Split_446dc19484434c83967c65cfc4eb1fb5_R_1, _Split_446dc19484434c83967c65cfc4eb1fb5_G_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2, 0, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6);
            description.Position = _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthNormalsOnly"
            Tags
            {
                "LightMode" = "DepthNormalsOnly"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        //#pragma geometry geom
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD1
        #define VARYINGS_NEED_NORMAL_WS
        #define VARYINGS_NEED_TANGENT_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv1 : TEXCOORD1;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS;
             float4 tangentWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
             float4 interp1 : INTERP1;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.normalWS;
            output.interp1.xyzw =  input.tangentWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.normalWS = input.interp0.xyz;
            output.tangentWS = input.interp1.xyzw;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _ScaleY;
        float _ScaleX;
        float _OffsetY;
        float _OffsetX;
        float _PowerY;
        float _PowerZ;
        float4 _ColorY;
        float4 _ColorZ;
        float _VertexShiftFactorZ;
        float2 _ThumbPosR;
        float2 _ThumbPosL;
        float _Pow;
        float _ThumbMagnitudeR;
        float _ThumbMagnitudeL;
        float4 _RendTex_TexelSize;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_RendTex);
        SAMPLER(sampler_RendTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Negate_float(float In, out float Out)
        {
            Out = -1 * In;
        }
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_446dc19484434c83967c65cfc4eb1fb5_R_1 = IN.ObjectSpacePosition[0];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_G_2 = IN.ObjectSpacePosition[1];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_B_3 = IN.ObjectSpacePosition[2];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_A_4 = 0;
            float2 _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0 = _ThumbPosL;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[0];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[1];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_B_3 = 0;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_A_4 = 0;
            float _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1;
            Unity_Negate_float(_Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1, _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1);
            float4 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4;
            float3 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5;
            float2 _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6;
            Unity_Combine_float(_Negate_2ad59c6a776a4f918862d473887f20f9_Out_1, _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2, 0, 0, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5, _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6);
            float _Split_defefc5a69434596b1aa452751bd1352_R_1 = IN.ObjectSpacePosition[0];
            float _Split_defefc5a69434596b1aa452751bd1352_G_2 = IN.ObjectSpacePosition[1];
            float _Split_defefc5a69434596b1aa452751bd1352_B_3 = IN.ObjectSpacePosition[2];
            float _Split_defefc5a69434596b1aa452751bd1352_A_4 = 0;
            float4 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4;
            float3 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5;
            float2 _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6;
            Unity_Combine_float(_Split_defefc5a69434596b1aa452751bd1352_R_1, _Split_defefc5a69434596b1aa452751bd1352_G_2, 0, 0, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6);
            float _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2;
            Unity_Distance_float2(_Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2);
            float _Remap_37127198c75b435ba2226a69a6929e67_Out_3;
            Unity_Remap_float(_Distance_c00ba968de284fb688fb5607c37c6c69_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_37127198c75b435ba2226a69a6929e67_Out_3);
            float _OneMinus_81594df369fa463c993993e3188fe57e_Out_1;
            Unity_OneMinus_float(_Remap_37127198c75b435ba2226a69a6929e67_Out_3, _OneMinus_81594df369fa463c993993e3188fe57e_Out_1);
            float _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0 = _Pow;
            float _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2;
            Unity_Power_float(_OneMinus_81594df369fa463c993993e3188fe57e_Out_1, _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0, _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2);
            float _Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0 = _VertexShiftFactorZ;
            float _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0 = _ThumbMagnitudeL;
            float _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2);
            float _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2;
            Unity_Multiply_float_float(_Power_989c374cd6fd4383b6901eb2a86fb148_Out_2, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2);
            float _Add_36318e66c25a4d24a221420cd5523e3b_Out_2;
            Unity_Add_float(_Split_446dc19484434c83967c65cfc4eb1fb5_B_3, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2, _Add_36318e66c25a4d24a221420cd5523e3b_Out_2);
            float2 _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0 = _ThumbPosR;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[0];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[1];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_B_3 = 0;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_A_4 = 0;
            float _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1;
            Unity_Negate_float(_Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1, _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1);
            float4 _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4;
            float3 _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5;
            float2 _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6;
            Unity_Combine_float(_Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1, _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2, 0, 0, _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4, _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5, _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6);
            float _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2;
            Unity_Distance_float2(_Combine_46940d00310044df99b10a7fb8bc4c33_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2);
            float _Remap_2fda5145689741abac020b9fe82f5615_Out_3;
            Unity_Remap_float(_Distance_eca9eb87869d4fa793b4337194ad776a_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_2fda5145689741abac020b9fe82f5615_Out_3);
            float _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1;
            Unity_OneMinus_float(_Remap_2fda5145689741abac020b9fe82f5615_Out_3, _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1);
            float _Property_5341cd1509214feeb193c9c00017ce81_Out_0 = _Pow;
            float _Power_4f2b65db1cb94455848606e911753371_Out_2;
            Unity_Power_float(_OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1, _Property_5341cd1509214feeb193c9c00017ce81_Out_0, _Power_4f2b65db1cb94455848606e911753371_Out_2);
            float _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0 = _ThumbMagnitudeR;
            float _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2);
            float _Multiply_7ba089fecfd1484894091913be08d114_Out_2;
            Unity_Multiply_float_float(_Power_4f2b65db1cb94455848606e911753371_Out_2, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2);
            float _Add_236176de907448c0a72c7d0f4b745891_Out_2;
            Unity_Add_float(_Add_36318e66c25a4d24a221420cd5523e3b_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2);
            float4 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4;
            float3 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            float2 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6;
            Unity_Combine_float(_Split_446dc19484434c83967c65cfc4eb1fb5_R_1, _Split_446dc19484434c83967c65cfc4eb1fb5_G_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2, 0, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6);
            description.Position = _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "SceneSelectionPass"
            Tags
            {
                "LightMode" = "SceneSelectionPass"
            }
        
        // Render State
        Cull Off
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        //#pragma geometry geom
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENESELECTIONPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _ScaleY;
        float _ScaleX;
        float _OffsetY;
        float _OffsetX;
        float _PowerY;
        float _PowerZ;
        float4 _ColorY;
        float4 _ColorZ;
        float _VertexShiftFactorZ;
        float2 _ThumbPosR;
        float2 _ThumbPosL;
        float _Pow;
        float _ThumbMagnitudeR;
        float _ThumbMagnitudeL;
        float4 _RendTex_TexelSize;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_RendTex);
        SAMPLER(sampler_RendTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Negate_float(float In, out float Out)
        {
            Out = -1 * In;
        }
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_446dc19484434c83967c65cfc4eb1fb5_R_1 = IN.ObjectSpacePosition[0];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_G_2 = IN.ObjectSpacePosition[1];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_B_3 = IN.ObjectSpacePosition[2];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_A_4 = 0;
            float2 _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0 = _ThumbPosL;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[0];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[1];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_B_3 = 0;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_A_4 = 0;
            float _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1;
            Unity_Negate_float(_Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1, _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1);
            float4 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4;
            float3 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5;
            float2 _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6;
            Unity_Combine_float(_Negate_2ad59c6a776a4f918862d473887f20f9_Out_1, _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2, 0, 0, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5, _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6);
            float _Split_defefc5a69434596b1aa452751bd1352_R_1 = IN.ObjectSpacePosition[0];
            float _Split_defefc5a69434596b1aa452751bd1352_G_2 = IN.ObjectSpacePosition[1];
            float _Split_defefc5a69434596b1aa452751bd1352_B_3 = IN.ObjectSpacePosition[2];
            float _Split_defefc5a69434596b1aa452751bd1352_A_4 = 0;
            float4 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4;
            float3 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5;
            float2 _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6;
            Unity_Combine_float(_Split_defefc5a69434596b1aa452751bd1352_R_1, _Split_defefc5a69434596b1aa452751bd1352_G_2, 0, 0, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6);
            float _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2;
            Unity_Distance_float2(_Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2);
            float _Remap_37127198c75b435ba2226a69a6929e67_Out_3;
            Unity_Remap_float(_Distance_c00ba968de284fb688fb5607c37c6c69_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_37127198c75b435ba2226a69a6929e67_Out_3);
            float _OneMinus_81594df369fa463c993993e3188fe57e_Out_1;
            Unity_OneMinus_float(_Remap_37127198c75b435ba2226a69a6929e67_Out_3, _OneMinus_81594df369fa463c993993e3188fe57e_Out_1);
            float _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0 = _Pow;
            float _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2;
            Unity_Power_float(_OneMinus_81594df369fa463c993993e3188fe57e_Out_1, _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0, _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2);
            float _Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0 = _VertexShiftFactorZ;
            float _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0 = _ThumbMagnitudeL;
            float _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2);
            float _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2;
            Unity_Multiply_float_float(_Power_989c374cd6fd4383b6901eb2a86fb148_Out_2, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2);
            float _Add_36318e66c25a4d24a221420cd5523e3b_Out_2;
            Unity_Add_float(_Split_446dc19484434c83967c65cfc4eb1fb5_B_3, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2, _Add_36318e66c25a4d24a221420cd5523e3b_Out_2);
            float2 _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0 = _ThumbPosR;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[0];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[1];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_B_3 = 0;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_A_4 = 0;
            float _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1;
            Unity_Negate_float(_Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1, _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1);
            float4 _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4;
            float3 _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5;
            float2 _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6;
            Unity_Combine_float(_Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1, _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2, 0, 0, _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4, _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5, _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6);
            float _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2;
            Unity_Distance_float2(_Combine_46940d00310044df99b10a7fb8bc4c33_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2);
            float _Remap_2fda5145689741abac020b9fe82f5615_Out_3;
            Unity_Remap_float(_Distance_eca9eb87869d4fa793b4337194ad776a_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_2fda5145689741abac020b9fe82f5615_Out_3);
            float _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1;
            Unity_OneMinus_float(_Remap_2fda5145689741abac020b9fe82f5615_Out_3, _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1);
            float _Property_5341cd1509214feeb193c9c00017ce81_Out_0 = _Pow;
            float _Power_4f2b65db1cb94455848606e911753371_Out_2;
            Unity_Power_float(_OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1, _Property_5341cd1509214feeb193c9c00017ce81_Out_0, _Power_4f2b65db1cb94455848606e911753371_Out_2);
            float _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0 = _ThumbMagnitudeR;
            float _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2);
            float _Multiply_7ba089fecfd1484894091913be08d114_Out_2;
            Unity_Multiply_float_float(_Power_4f2b65db1cb94455848606e911753371_Out_2, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2);
            float _Add_236176de907448c0a72c7d0f4b745891_Out_2;
            Unity_Add_float(_Add_36318e66c25a4d24a221420cd5523e3b_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2);
            float4 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4;
            float3 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            float2 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6;
            Unity_Combine_float(_Split_446dc19484434c83967c65cfc4eb1fb5_R_1, _Split_446dc19484434c83967c65cfc4eb1fb5_G_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2, 0, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6);
            description.Position = _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "ScenePickingPass"
            Tags
            {
                "LightMode" = "Picking"
            }
        
        // Render State
        Cull Back
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma vertex vert
        //#pragma geometry geom
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHONLY
        #define SCENEPICKINGPASS 1
        #define ALPHA_CLIP_THRESHOLD 1
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _ScaleY;
        float _ScaleX;
        float _OffsetY;
        float _OffsetX;
        float _PowerY;
        float _PowerZ;
        float4 _ColorY;
        float4 _ColorZ;
        float _VertexShiftFactorZ;
        float2 _ThumbPosR;
        float2 _ThumbPosL;
        float _Pow;
        float _ThumbMagnitudeR;
        float _ThumbMagnitudeL;
        float4 _RendTex_TexelSize;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_RendTex);
        SAMPLER(sampler_RendTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Negate_float(float In, out float Out)
        {
            Out = -1 * In;
        }
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_446dc19484434c83967c65cfc4eb1fb5_R_1 = IN.ObjectSpacePosition[0];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_G_2 = IN.ObjectSpacePosition[1];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_B_3 = IN.ObjectSpacePosition[2];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_A_4 = 0;
            float2 _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0 = _ThumbPosL;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[0];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[1];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_B_3 = 0;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_A_4 = 0;
            float _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1;
            Unity_Negate_float(_Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1, _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1);
            float4 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4;
            float3 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5;
            float2 _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6;
            Unity_Combine_float(_Negate_2ad59c6a776a4f918862d473887f20f9_Out_1, _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2, 0, 0, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5, _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6);
            float _Split_defefc5a69434596b1aa452751bd1352_R_1 = IN.ObjectSpacePosition[0];
            float _Split_defefc5a69434596b1aa452751bd1352_G_2 = IN.ObjectSpacePosition[1];
            float _Split_defefc5a69434596b1aa452751bd1352_B_3 = IN.ObjectSpacePosition[2];
            float _Split_defefc5a69434596b1aa452751bd1352_A_4 = 0;
            float4 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4;
            float3 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5;
            float2 _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6;
            Unity_Combine_float(_Split_defefc5a69434596b1aa452751bd1352_R_1, _Split_defefc5a69434596b1aa452751bd1352_G_2, 0, 0, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6);
            float _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2;
            Unity_Distance_float2(_Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2);
            float _Remap_37127198c75b435ba2226a69a6929e67_Out_3;
            Unity_Remap_float(_Distance_c00ba968de284fb688fb5607c37c6c69_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_37127198c75b435ba2226a69a6929e67_Out_3);
            float _OneMinus_81594df369fa463c993993e3188fe57e_Out_1;
            Unity_OneMinus_float(_Remap_37127198c75b435ba2226a69a6929e67_Out_3, _OneMinus_81594df369fa463c993993e3188fe57e_Out_1);
            float _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0 = _Pow;
            float _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2;
            Unity_Power_float(_OneMinus_81594df369fa463c993993e3188fe57e_Out_1, _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0, _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2);
            float _Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0 = _VertexShiftFactorZ;
            float _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0 = _ThumbMagnitudeL;
            float _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2);
            float _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2;
            Unity_Multiply_float_float(_Power_989c374cd6fd4383b6901eb2a86fb148_Out_2, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2);
            float _Add_36318e66c25a4d24a221420cd5523e3b_Out_2;
            Unity_Add_float(_Split_446dc19484434c83967c65cfc4eb1fb5_B_3, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2, _Add_36318e66c25a4d24a221420cd5523e3b_Out_2);
            float2 _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0 = _ThumbPosR;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[0];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[1];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_B_3 = 0;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_A_4 = 0;
            float _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1;
            Unity_Negate_float(_Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1, _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1);
            float4 _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4;
            float3 _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5;
            float2 _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6;
            Unity_Combine_float(_Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1, _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2, 0, 0, _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4, _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5, _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6);
            float _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2;
            Unity_Distance_float2(_Combine_46940d00310044df99b10a7fb8bc4c33_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2);
            float _Remap_2fda5145689741abac020b9fe82f5615_Out_3;
            Unity_Remap_float(_Distance_eca9eb87869d4fa793b4337194ad776a_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_2fda5145689741abac020b9fe82f5615_Out_3);
            float _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1;
            Unity_OneMinus_float(_Remap_2fda5145689741abac020b9fe82f5615_Out_3, _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1);
            float _Property_5341cd1509214feeb193c9c00017ce81_Out_0 = _Pow;
            float _Power_4f2b65db1cb94455848606e911753371_Out_2;
            Unity_Power_float(_OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1, _Property_5341cd1509214feeb193c9c00017ce81_Out_0, _Power_4f2b65db1cb94455848606e911753371_Out_2);
            float _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0 = _ThumbMagnitudeR;
            float _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2);
            float _Multiply_7ba089fecfd1484894091913be08d114_Out_2;
            Unity_Multiply_float_float(_Power_4f2b65db1cb94455848606e911753371_Out_2, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2);
            float _Add_236176de907448c0a72c7d0f4b745891_Out_2;
            Unity_Add_float(_Add_36318e66c25a4d24a221420cd5523e3b_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2);
            float4 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4;
            float3 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            float2 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6;
            Unity_Combine_float(_Split_446dc19484434c83967c65cfc4eb1fb5_R_1, _Split_446dc19484434c83967c65cfc4eb1fb5_G_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2, 0, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6);
            description.Position = _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
            
        
        
        
        
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/SelectionPickingPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
        Pass
        {
            Name "DepthNormals"
            Tags
            {
                "LightMode" = "DepthNormalsOnly"
            }
        
        // Render State
        Cull Back
        ZTest LEqual
        ZWrite On
        
        // Debug
        // <None>
        
        // --------------------------------------------------
        // Pass
        
        HLSLPROGRAM
        
        // Pragmas
        #pragma target 2.0
        #pragma only_renderers gles gles3 glcore d3d11
        #pragma multi_compile_instancing
        #pragma multi_compile_fog
        #pragma instancing_options renderinglayer
        #pragma vertex vert
        //#pragma geometry geom
        #pragma fragment frag
        
        // DotsInstancingOptions: <None>
        // HybridV1InjectedBuiltinProperties: <None>
        
        // Keywords
        // PassKeywords: <None>
        // GraphKeywords: <None>
        
        // Defines
        
        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define VARYINGS_NEED_NORMAL_WS
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_DEPTHNORMALSONLY
        /* WARNING: $splice Could not find named fragment 'DotsInstancingVars' */
        
        
        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */
        
        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"
        
        // --------------------------------------------------
        // Structs and Packing
        
        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */
        
        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float3 interp0 : INTERP0;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        
        PackedVaryings PackVaryings (Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.interp0.xyz =  input.normalWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        Varyings UnpackVaryings (PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.normalWS = input.interp0.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }
        
        
        // --------------------------------------------------
        // Graph
        
        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float _ScaleY;
        float _ScaleX;
        float _OffsetY;
        float _OffsetX;
        float _PowerY;
        float _PowerZ;
        float4 _ColorY;
        float4 _ColorZ;
        float _VertexShiftFactorZ;
        float2 _ThumbPosR;
        float2 _ThumbPosL;
        float _Pow;
        float _ThumbMagnitudeR;
        float _ThumbMagnitudeL;
        float4 _RendTex_TexelSize;
        CBUFFER_END
        
        // Object and Global properties
        SAMPLER(SamplerState_Linear_Repeat);
        TEXTURE2D(_RendTex);
        SAMPLER(sampler_RendTex);
        
        // Graph Includes
        // GraphIncludes: <None>
        
        // -- Property used by ScenePickingPass
        #ifdef SCENEPICKINGPASS
        float4 _SelectionID;
        #endif
        
        // -- Properties used by SceneSelectionPass
        #ifdef SCENESELECTIONPASS
        int _ObjectId;
        int _PassValue;
        #endif
        
        // Graph Functions
        
        void Unity_Negate_float(float In, out float Out)
        {
            Out = -1 * In;
        }
        
        void Unity_Combine_float(float R, float G, float B, float A, out float4 RGBA, out float3 RGB, out float2 RG)
        {
            RGBA = float4(R, G, B, A);
            RGB = float3(R, G, B);
            RG = float2(R, G);
        }
        
        void Unity_Distance_float2(float2 A, float2 B, out float Out)
        {
            Out = distance(A, B);
        }
        
        void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
        {
            Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
        }
        
        void Unity_OneMinus_float(float In, out float Out)
        {
            Out = 1 - In;
        }
        
        void Unity_Power_float(float A, float B, out float Out)
        {
            Out = pow(A, B);
        }
        
        void Unity_Multiply_float_float(float A, float B, out float Out)
        {
            Out = A * B;
        }
        
        void Unity_Add_float(float A, float B, out float Out)
        {
            Out = A + B;
        }
        
        // Custom interpolators pre vertex
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */
        
        // Graph Vertex
        struct VertexDescription
        {
            float3 Position;
            float3 Normal;
            float3 Tangent;
        };
        
        VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
        {
            VertexDescription description = (VertexDescription)0;
            float _Split_446dc19484434c83967c65cfc4eb1fb5_R_1 = IN.ObjectSpacePosition[0];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_G_2 = IN.ObjectSpacePosition[1];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_B_3 = IN.ObjectSpacePosition[2];
            float _Split_446dc19484434c83967c65cfc4eb1fb5_A_4 = 0;
            float2 _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0 = _ThumbPosL;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[0];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2 = _Property_89406e44e9e940249c3d69015c2f0dc3_Out_0[1];
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_B_3 = 0;
            float _Split_2f1dc25dbfd34e4d8667bba09238f6b3_A_4 = 0;
            float _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1;
            Unity_Negate_float(_Split_2f1dc25dbfd34e4d8667bba09238f6b3_R_1, _Negate_2ad59c6a776a4f918862d473887f20f9_Out_1);
            float4 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4;
            float3 _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5;
            float2 _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6;
            Unity_Combine_float(_Negate_2ad59c6a776a4f918862d473887f20f9_Out_1, _Split_2f1dc25dbfd34e4d8667bba09238f6b3_G_2, 0, 0, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGBA_4, _Combine_6f8a79ecb4614627ab9f391b545045c2_RGB_5, _Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6);
            float _Split_defefc5a69434596b1aa452751bd1352_R_1 = IN.ObjectSpacePosition[0];
            float _Split_defefc5a69434596b1aa452751bd1352_G_2 = IN.ObjectSpacePosition[1];
            float _Split_defefc5a69434596b1aa452751bd1352_B_3 = IN.ObjectSpacePosition[2];
            float _Split_defefc5a69434596b1aa452751bd1352_A_4 = 0;
            float4 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4;
            float3 _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5;
            float2 _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6;
            Unity_Combine_float(_Split_defefc5a69434596b1aa452751bd1352_R_1, _Split_defefc5a69434596b1aa452751bd1352_G_2, 0, 0, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGBA_4, _Combine_3e91dc0523c44094a334ed2084b77ea1_RGB_5, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6);
            float _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2;
            Unity_Distance_float2(_Combine_6f8a79ecb4614627ab9f391b545045c2_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_c00ba968de284fb688fb5607c37c6c69_Out_2);
            float _Remap_37127198c75b435ba2226a69a6929e67_Out_3;
            Unity_Remap_float(_Distance_c00ba968de284fb688fb5607c37c6c69_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_37127198c75b435ba2226a69a6929e67_Out_3);
            float _OneMinus_81594df369fa463c993993e3188fe57e_Out_1;
            Unity_OneMinus_float(_Remap_37127198c75b435ba2226a69a6929e67_Out_3, _OneMinus_81594df369fa463c993993e3188fe57e_Out_1);
            float _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0 = _Pow;
            float _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2;
            Unity_Power_float(_OneMinus_81594df369fa463c993993e3188fe57e_Out_1, _Property_727f7a3a25644ff9aa56b5d008a2fa1b_Out_0, _Power_989c374cd6fd4383b6901eb2a86fb148_Out_2);
            float _Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0 = _VertexShiftFactorZ;
            float _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0 = _ThumbMagnitudeL;
            float _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_8ffb8e54d3cd43eb8ec83c039800bcdf_Out_0, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2);
            float _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2;
            Unity_Multiply_float_float(_Power_989c374cd6fd4383b6901eb2a86fb148_Out_2, _Multiply_30b444edc78b471b9f12452555b1f69c_Out_2, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2);
            float _Add_36318e66c25a4d24a221420cd5523e3b_Out_2;
            Unity_Add_float(_Split_446dc19484434c83967c65cfc4eb1fb5_B_3, _Multiply_809fce85f87545c68bbc6b513f26f182_Out_2, _Add_36318e66c25a4d24a221420cd5523e3b_Out_2);
            float2 _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0 = _ThumbPosR;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[0];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2 = _Property_967c3f6d9467481f8cdd7c0ee533326f_Out_0[1];
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_B_3 = 0;
            float _Split_b3dd43678c8c4517a5bbf65cc84a93f0_A_4 = 0;
            float _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1;
            Unity_Negate_float(_Split_b3dd43678c8c4517a5bbf65cc84a93f0_R_1, _Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1);
            float4 _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4;
            float3 _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5;
            float2 _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6;
            Unity_Combine_float(_Negate_3365af7c57704ad68a9fab1dcb7e6f33_Out_1, _Split_b3dd43678c8c4517a5bbf65cc84a93f0_G_2, 0, 0, _Combine_46940d00310044df99b10a7fb8bc4c33_RGBA_4, _Combine_46940d00310044df99b10a7fb8bc4c33_RGB_5, _Combine_46940d00310044df99b10a7fb8bc4c33_RG_6);
            float _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2;
            Unity_Distance_float2(_Combine_46940d00310044df99b10a7fb8bc4c33_RG_6, _Combine_3e91dc0523c44094a334ed2084b77ea1_RG_6, _Distance_eca9eb87869d4fa793b4337194ad776a_Out_2);
            float _Remap_2fda5145689741abac020b9fe82f5615_Out_3;
            Unity_Remap_float(_Distance_eca9eb87869d4fa793b4337194ad776a_Out_2, float2 (0, 3.6), float2 (0, 1), _Remap_2fda5145689741abac020b9fe82f5615_Out_3);
            float _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1;
            Unity_OneMinus_float(_Remap_2fda5145689741abac020b9fe82f5615_Out_3, _OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1);
            float _Property_5341cd1509214feeb193c9c00017ce81_Out_0 = _Pow;
            float _Power_4f2b65db1cb94455848606e911753371_Out_2;
            Unity_Power_float(_OneMinus_bb5c68d850c34db78fee4c8ab50429a4_Out_1, _Property_5341cd1509214feeb193c9c00017ce81_Out_0, _Power_4f2b65db1cb94455848606e911753371_Out_2);
            float _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0 = _ThumbMagnitudeR;
            float _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2;
            Unity_Multiply_float_float(_Property_05c57bdf613b4ea48ca088ee6f4c6607_Out_0, _Property_1904c4b9a42d4c9693fdb9eff9b96f4d_Out_0, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2);
            float _Multiply_7ba089fecfd1484894091913be08d114_Out_2;
            Unity_Multiply_float_float(_Power_4f2b65db1cb94455848606e911753371_Out_2, _Multiply_65e292505a1b495591ce55cfcebc1e51_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2);
            float _Add_236176de907448c0a72c7d0f4b745891_Out_2;
            Unity_Add_float(_Add_36318e66c25a4d24a221420cd5523e3b_Out_2, _Multiply_7ba089fecfd1484894091913be08d114_Out_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2);
            float4 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4;
            float3 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            float2 _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6;
            Unity_Combine_float(_Split_446dc19484434c83967c65cfc4eb1fb5_R_1, _Split_446dc19484434c83967c65cfc4eb1fb5_G_2, _Add_236176de907448c0a72c7d0f4b745891_Out_2, 0, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGBA_4, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5, _Combine_7d93c03fc39040c4b3218ce980a88ee4_RG_6);
            description.Position = _Combine_7d93c03fc39040c4b3218ce980a88ee4_RGB_5;
            description.Normal = IN.ObjectSpaceNormal;
            description.Tangent = IN.ObjectSpaceTangent;
            return description;
        }
        
        // Custom interpolators, pre surface
        #ifdef FEATURES_GRAPH_VERTEX
        Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
        {
        return output;
        }
        #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
        #endif
        
        // Graph Pixel
        struct SurfaceDescription
        {
        };
        
        SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
        {
            SurfaceDescription surface = (SurfaceDescription)0;
            return surface;
        }
        
        // --------------------------------------------------
        // Build Graph Inputs
        #ifdef HAVE_VFX_MODIFICATION
        #define VFX_SRP_ATTRIBUTES Attributes
        #define VFX_SRP_VARYINGS Varyings
        #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
        #endif
        VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
        {
            VertexDescriptionInputs output;
            ZERO_INITIALIZE(VertexDescriptionInputs, output);
        
            output.ObjectSpaceNormal =                          input.normalOS;
            output.ObjectSpaceTangent =                         input.tangentOS.xyz;
            output.ObjectSpacePosition =                        input.positionOS;
        
            return output;
        }
        SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
        {
            SurfaceDescriptionInputs output;
            ZERO_INITIALIZE(SurfaceDescriptionInputs, output);
        
        #ifdef HAVE_VFX_MODIFICATION
            // FragInputs from VFX come from two places: Interpolator or CBuffer.
            /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */
        
        #endif
        
        
        #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
        #else
        #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        #endif
        #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
        
                return output;
        }
        
        // --------------------------------------------------
        // Main
        
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/DepthNormalsOnlyPass.hlsl"
        
        // --------------------------------------------------
        // Visual Effect Vertex Invocations
        #ifdef HAVE_VFX_MODIFICATION
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
        #endif
        
        ENDHLSL
        }
    }
    CustomEditorForRenderPipeline "UnityEditor.ShaderGraphUnlitGUI" "UnityEngine.Rendering.Universal.UniversalRenderPipelineAsset"
    CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
    FallBack "Hidden/Shader Graph/FallbackError"
}