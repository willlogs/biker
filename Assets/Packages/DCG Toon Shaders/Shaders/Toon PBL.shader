// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Shader created with Shader Forge v1.32 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.32;sub:START;pass:START;ps:flbk:Standard,iptp:0,cusa:False,bamd:0,lico:1,lgpr:1,limd:3,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:True,enco:False,rmgx:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:False,igpj:False,qofs:1,qpre:2,rntp:3,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False;n:type:ShaderForge.SFN_Final,id:3138,x:31259,y:32547,varname:node_3138,prsc:2|diff-3674-OUT,spec-2959-OUT,gloss-3417-OUT,normal-324-OUT,emission-6316-OUT,difocc-2746-OUT,olwid-6578-OUT,olcol-9646-RGB;n:type:ShaderForge.SFN_Color,id:7241,x:29186,y:32747,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.7058823,c2:0.7058823,c3:0.7058823,c4:1;n:type:ShaderForge.SFN_LightVector,id:9322,x:29330,y:31935,varname:node_9322,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:3177,x:29330,y:32083,prsc:2,pt:True;n:type:ShaderForge.SFN_Dot,id:6355,x:29519,y:31935,varname:node_6355,prsc:2,dt:1|A-9322-OUT,B-3177-OUT;n:type:ShaderForge.SFN_Smoothstep,id:8748,x:29713,y:31778,varname:node_8748,prsc:2|A-4065-OUT,B-7201-OUT,V-1601-OUT;n:type:ShaderForge.SFN_Vector1,id:4065,x:28889,y:31779,varname:node_4065,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:7201,x:28889,y:31833,varname:node_7201,prsc:2,v1:0.025;n:type:ShaderForge.SFN_RemapRange,id:3209,x:29519,y:32129,varname:node_3209,prsc:2,frmn:0,frmx:1,tomn:-3,tomx:2|IN-6355-OUT;n:type:ShaderForge.SFN_Smoothstep,id:7613,x:29727,y:32083,varname:node_7613,prsc:2|A-4065-OUT,B-7201-OUT,V-3209-OUT;n:type:ShaderForge.SFN_Clamp01,id:5149,x:29947,y:32083,varname:node_5149,prsc:2|IN-7613-OUT;n:type:ShaderForge.SFN_Clamp01,id:9440,x:29942,y:31778,varname:node_9440,prsc:2|IN-8748-OUT;n:type:ShaderForge.SFN_Add,id:6060,x:30151,y:31932,varname:node_6060,prsc:2|A-5149-OUT,B-5430-OUT;n:type:ShaderForge.SFN_Vector1,id:5430,x:29942,y:31932,varname:node_5430,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:6031,x:30151,y:31778,varname:node_6031,prsc:2|A-9440-OUT,B-6060-OUT;n:type:ShaderForge.SFN_Clamp01,id:8285,x:30235,y:32242,varname:node_8285,prsc:2|IN-6031-OUT;n:type:ShaderForge.SFN_RemapRange,id:1601,x:29519,y:31778,varname:node_1601,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:2|IN-6355-OUT;n:type:ShaderForge.SFN_Tex2d,id:1159,x:29186,y:32548,ptovrint:False,ptlb:Albedo,ptin:_Albedo,varname:node_1159,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:3674,x:30308,y:32571,varname:node_3674,prsc:2|A-4784-OUT,B-8285-OUT,C-8150-OUT;n:type:ShaderForge.SFN_Slider,id:5243,x:29430,y:33193,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:node_5243,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Tex2d,id:233,x:30739,y:31709,ptovrint:False,ptlb:Normal Map,ptin:_NormalMap,varname:node_233,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Lerp,id:324,x:30940,y:31709,varname:node_324,prsc:2|A-5770-OUT,B-233-RGB,T-9922-OUT;n:type:ShaderForge.SFN_Vector3,id:5770,x:30940,y:31612,varname:node_5770,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Slider,id:9922,x:30874,y:31953,ptovrint:False,ptlb:Normal Intensity,ptin:_NormalIntensity,varname:node_9922,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.6,max:1;n:type:ShaderForge.SFN_Tex2d,id:5873,x:31826,y:31540,ptovrint:False,ptlb:Emission Map,ptin:_EmissionMap,varname:node_5873,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:7134,x:31826,y:31728,ptovrint:False,ptlb:HDR Emission Color,ptin:_HDREmissionColor,varname:node_7134,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Multiply,id:6316,x:32047,y:31661,varname:node_6316,prsc:2|A-5873-RGB,B-7134-RGB;n:type:ShaderForge.SFN_Tex2d,id:2017,x:30091,y:33096,ptovrint:False,ptlb:Metallic Map,ptin:_MetallicMap,varname:node_2017,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:2959,x:30585,y:33090,varname:node_2959,prsc:2|A-2017-R,B-8404-OUT;n:type:ShaderForge.SFN_Slider,id:8404,x:30310,y:33307,ptovrint:False,ptlb:Metalness,ptin:_Metalness,varname:node_8404,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:3417,x:30365,y:32910,varname:node_3417,prsc:2|A-5243-OUT,B-2017-A;n:type:ShaderForge.SFN_Tex2d,id:2756,x:31142,y:32209,ptovrint:False,ptlb:Occlussion Map,ptin:_OcclussionMap,varname:node_2756,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:9743,x:31296,y:32402,ptovrint:False,ptlb:Occlussion Intensity,ptin:_OcclussionIntensity,varname:node_9743,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Lerp,id:8150,x:31436,y:32261,varname:node_8150,prsc:2|A-6550-OUT,B-2756-R,T-9743-OUT;n:type:ShaderForge.SFN_Vector1,id:6550,x:31354,y:32173,varname:node_6550,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:2746,x:30625,y:32619,varname:node_2746,prsc:2|A-8285-OUT,B-8150-OUT;n:type:ShaderForge.SFN_Color,id:9646,x:30965,y:32887,ptovrint:False,ptlb:Outline Color,ptin:_OutlineColor,varname:node_9646,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Slider,id:6578,x:30921,y:33091,ptovrint:False,ptlb:Outline Width,ptin:_OutlineWidth,varname:node_6578,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.025;n:type:ShaderForge.SFN_RemapRange,id:7845,x:28178,y:32646,varname:node_7845,prsc:2,frmn:0,frmx:1,tomn:0.25,tomx:11|IN-6923-OUT;n:type:ShaderForge.SFN_Exp,id:7955,x:28178,y:32495,varname:node_7955,prsc:2,et:0|IN-7845-OUT;n:type:ShaderForge.SFN_Power,id:4076,x:28382,y:32477,varname:node_4076,prsc:2|VAL-6355-OUT,EXP-7955-OUT;n:type:ShaderForge.SFN_RemapRange,id:5238,x:28609,y:32550,varname:node_5238,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-4076-OUT;n:type:ShaderForge.SFN_Clamp01,id:5205,x:29171,y:32358,varname:node_5205,prsc:2|IN-4038-OUT;n:type:ShaderForge.SFN_Smoothstep,id:7730,x:28609,y:32410,varname:node_7730,prsc:2|A-4065-OUT,B-7201-OUT,V-5238-OUT;n:type:ShaderForge.SFN_Slider,id:2351,x:28178,y:32937,ptovrint:False,ptlb:Albedo Gloss Intensity,ptin:_AlbedoGlossIntensity,varname:node_6941,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:3;n:type:ShaderForge.SFN_Multiply,id:4038,x:28963,y:32384,varname:node_4038,prsc:2|A-7730-OUT,B-2351-OUT;n:type:ShaderForge.SFN_Slider,id:6923,x:27848,y:32756,ptovrint:False,ptlb:Albedo Gloss,ptin:_AlbedoGloss,varname:node_6923,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Multiply,id:3906,x:29496,y:32376,varname:node_3906,prsc:2|A-5205-OUT,B-1779-OUT;n:type:ShaderForge.SFN_Add,id:4784,x:29892,y:32516,varname:node_4784,prsc:2|A-1779-OUT,B-3906-OUT;n:type:ShaderForge.SFN_Multiply,id:1779,x:29489,y:32643,varname:node_1779,prsc:2|A-1159-RGB,B-7241-RGB;n:type:ShaderForge.SFN_Slider,id:8280,x:30241,y:31146,ptovrint:False,ptlb:CutOut,ptin:_CutOut,varname:node_4472,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Relay,id:9070,x:30528,y:31402,varname:node_9070,prsc:2|IN-1159-A;n:type:ShaderForge.SFN_Multiply,id:24,x:30685,y:31390,varname:node_24,prsc:2|A-9240-OUT,B-9070-OUT;n:type:ShaderForge.SFN_RemapRange,id:9240,x:30642,y:31153,varname:node_9240,prsc:2,frmn:0,frmx:1,tomn:1,tomx:0|IN-8280-OUT;n:type:ShaderForge.SFN_Add,id:6138,x:30890,y:31251,varname:node_6138,prsc:2|A-9240-OUT,B-24-OUT;n:type:ShaderForge.SFN_Clamp01,id:1568,x:31190,y:31282,varname:node_1568,prsc:2|IN-6138-OUT;proporder:7241-1159-8280-233-9922-2017-8404-5243-6923-2351-2756-9743-5873-7134-6578-9646;pass:END;sub:END;*/

Shader "DCG/Toon Shaders/Toon PBL" {
    Properties {      
            [Header(Color Settings)]
       [Space()]
        _Color ("Color", Color) = (0.7058823,0.7058823,0.7058823,1)
        _Albedo ("Albedo", 2D) = "white" {}
        [HideInInspector]_CutOut ("CutOut", Range(0, 1)) = 0
                 [Space()]
            [Header(Normal Settings)]
       [Space()]
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalIntensity ("Normal Intensity", Range(0, 1)) = 0.6
                 [Space()]
            [Header(PBL Settings)]
       [Space()]
        _MetallicMap ("Metallic Map", 2D) = "white" {}
        _Metalness ("Metalness", Range(0, 1)) = 1
        _Gloss ("Gloss", Range(0, 1)) = 0.5
        _AlbedoGloss ("Albedo Gloss", Range(0, 1)) = 0
        _AlbedoGlossIntensity ("Albedo Gloss Intensity", Range(0, 3)) = 1
                 [Space()]
            [Header(Occlussion Settings)]
       [Space()]
        _OcclussionMap ("Occlussion Map", 2D) = "white" {}
        _OcclussionIntensity ("Occlussion Intensity", Range(0, 1)) = 1
                 [Space()]
            [Header(Emission Settings)]
       [Space()]
        _EmissionMap ("Emission Map", 2D) = "white" {}
        [HDR]_HDREmissionColor ("HDR Emission Color", Color) = (0,0,0,1)

                 [Space()]
            [Header(Outline Settings)]
       [Space()]
        _OutlineWidth ("Outline Width", Range(0, 0.025)) = 0
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
    }
    SubShader {
        Tags {
            "Queue"="AlphaTest+1"
            "RenderType"="TransparentCutout"
        }
        Pass {
            Name "Outline"
            Tags {
            }
            Cull Front
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma fragmentoption ARB_precision_hint_fastest
            #pragma multi_compile_shadowcaster
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _OutlineColor;
            uniform float _OutlineWidth;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.pos = UnityObjectToClipPos(float4(v.vertex.xyz + v.normal*_OutlineWidth,1) );
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                return fixed4(_OutlineColor.rgb,0);
            }
            ENDCG
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _Albedo; uniform float4 _Albedo_ST;
            uniform float _Gloss;
            uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
            uniform float _NormalIntensity;
            uniform sampler2D _EmissionMap; uniform float4 _EmissionMap_ST;
            uniform float4 _HDREmissionColor;
            uniform sampler2D _MetallicMap; uniform float4 _MetallicMap_ST;
            uniform float _Metalness;
            uniform sampler2D _OcclussionMap; uniform float4 _OcclussionMap_ST;
            uniform float _OcclussionIntensity;
            uniform float _AlbedoGlossIntensity;
            uniform float _AlbedoGloss;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _NormalMap_var = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(i.uv0, _NormalMap)));
                float3 normalLocal = lerp(float3(0,0,1),_NormalMap_var.rgb,_NormalIntensity);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 viewReflectDirection = reflect( -viewDirection, normalDirection );
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _MetallicMap_var = tex2D(_MetallicMap,TRANSFORM_TEX(i.uv0, _MetallicMap));
                float gloss = (_Gloss*_MetallicMap_var.a);
                float specPow = exp2( gloss * 10.0+1.0);
/////// GI Data:
                UnityLight light;
                #ifdef LIGHTMAP_OFF
                    light.color = lightColor;
                    light.dir = lightDirection;
                    light.ndotl = LambertTerm (normalDirection, light.dir);
                #else
                    light.color = half3(0.f, 0.f, 0.f);
                    light.ndotl = 0.0f;
                    light.dir = half3(0.f, 0.f, 0.f);
                #endif
                UnityGIInput d;
                d.light = light;
                d.worldPos = i.posWorld.xyz;
                d.worldViewDir = viewDirection;
                d.atten = attenuation;
                #if UNITY_SPECCUBE_BLENDING || UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMin[0] = unity_SpecCube0_BoxMin;
                    d.boxMin[1] = unity_SpecCube1_BoxMin;
                #endif
                #if UNITY_SPECCUBE_BOX_PROJECTION
                    d.boxMax[0] = unity_SpecCube0_BoxMax;
                    d.boxMax[1] = unity_SpecCube1_BoxMax;
                    d.probePosition[0] = unity_SpecCube0_ProbePosition;
                    d.probePosition[1] = unity_SpecCube1_ProbePosition;
                #endif
                d.probeHDR[0] = unity_SpecCube0_HDR;
                d.probeHDR[1] = unity_SpecCube1_HDR;
                Unity_GlossyEnvironmentData ugls_en_data;
                ugls_en_data.roughness = 1.0 - gloss;
                ugls_en_data.reflUVW = viewReflectDirection;
                UnityGI gi = UnityGlobalIllumination(d, 1, normalDirection, ugls_en_data );
                lightDirection = gi.light.dir;
                lightColor = gi.light.color;
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float3 specularColor = (_MetallicMap_var.r*_Metalness);
                float specularMonochrome;
                float4 _Albedo_var = tex2D(_Albedo,TRANSFORM_TEX(i.uv0, _Albedo));
                float3 node_1779 = (_Albedo_var.rgb*_Color.rgb);
                float node_4065 = 0.0;
                float node_7201 = 0.025;
                float node_6355 = max(0,dot(lightDirection,normalDirection));
                float node_8285 = saturate((saturate(smoothstep( node_4065, node_7201, (node_6355*3.0+-1.0) ))*(saturate(smoothstep( node_4065, node_7201, (node_6355*5.0+-3.0) ))+0.5)));
                float4 _OcclussionMap_var = tex2D(_OcclussionMap,TRANSFORM_TEX(i.uv0, _OcclussionMap));
                float node_8150 = lerp(1.0,_OcclussionMap_var.r,_OcclussionIntensity);
                float3 diffuseColor = ((node_1779+(saturate((smoothstep( node_4065, node_7201, (pow(node_6355,exp((_AlbedoGloss*10.75+0.25)))*2.0+-1.0) )*_AlbedoGlossIntensity))*node_1779))*node_8285*node_8150); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, GGXTerm(NdotH, 1.0-gloss));
                float specularPBL = (NdotL*visTerm*normTerm) * (UNITY_PI / 4);
                if (IsGammaSpace())
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                specularPBL = max(0, specularPBL * NdotL);
                float3 directSpecular = (floor(attenuation) * _LightColor0.xyz)*specularPBL*FresnelTerm(specularColor, LdotH);
                half grazingTerm = saturate( gloss + specularMonochrome );
                float3 indirectSpecular = (gi.indirect.specular);
                indirectSpecular *= FresnelLerp (specularColor, grazingTerm, NdotV);
                float3 specular = (directSpecular + indirectSpecular);
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 indirectDiffuse = float3(0,0,0);
                indirectDiffuse += UNITY_LIGHTMODEL_AMBIENT.rgb; // Ambient Light
                indirectDiffuse *= (node_8285*node_8150); // Diffuse AO
                float3 diffuse = (directDiffuse + indirectDiffuse) * diffuseColor;
////// Emissive:
                float4 _EmissionMap_var = tex2D(_EmissionMap,TRANSFORM_TEX(i.uv0, _EmissionMap));
                float3 emissive = (_EmissionMap_var.rgb*_HDREmissionColor.rgb);
/// Final Color:
                float3 finalColor = diffuse + specular + emissive;
                return fixed4(finalColor,1);
            }
            ENDCG
        }
        Pass {
            Name "FORWARD_DELTA"
            Tags {
                "LightMode"="ForwardAdd"
            }
            Blend One One
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDADD
            #define _GLOSSYENV 1
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "UnityPBSLighting.cginc"
            #include "UnityStandardBRDF.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _Albedo; uniform float4 _Albedo_ST;
            uniform float _Gloss;
            uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
            uniform float _NormalIntensity;
            uniform sampler2D _EmissionMap; uniform float4 _EmissionMap_ST;
            uniform float4 _HDREmissionColor;
            uniform sampler2D _MetallicMap; uniform float4 _MetallicMap_ST;
            uniform float _Metalness;
            uniform sampler2D _OcclussionMap; uniform float4 _OcclussionMap_ST;
            uniform float _OcclussionIntensity;
            uniform float _AlbedoGlossIntensity;
            uniform float _AlbedoGloss;
            struct VertexInput {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 tangent : TANGENT;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                float3 tangentDir : TEXCOORD3;
                float3 bitangentDir : TEXCOORD4;
                LIGHTING_COORDS(5,6)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                o.tangentDir = normalize( mul( unity_ObjectToWorld, float4( v.tangent.xyz, 0.0 ) ).xyz );
                o.bitangentDir = normalize(cross(o.normalDir, o.tangentDir) * v.tangent.w);
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos(v.vertex );
                TRANSFER_VERTEX_TO_FRAGMENT(o)
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                i.normalDir = normalize(i.normalDir);
                float3x3 tangentTransform = float3x3( i.tangentDir, i.bitangentDir, i.normalDir);
                float3 viewDirection = normalize(_WorldSpaceCameraPos.xyz - i.posWorld.xyz);
                float3 _NormalMap_var = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(i.uv0, _NormalMap)));
                float3 normalLocal = lerp(float3(0,0,1),_NormalMap_var.rgb,_NormalIntensity);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
                float3 halfDirection = normalize(viewDirection+lightDirection);
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 attenColor = attenuation * _LightColor0.xyz;
                float Pi = 3.141592654;
                float InvPi = 0.31830988618;
///////// Gloss:
                float4 _MetallicMap_var = tex2D(_MetallicMap,TRANSFORM_TEX(i.uv0, _MetallicMap));
                float gloss = (_Gloss*_MetallicMap_var.a);
                float specPow = exp2( gloss * 10.0+1.0);
////// Specular:
                float NdotL = max(0, dot( normalDirection, lightDirection ));
                float LdotH = max(0.0,dot(lightDirection, halfDirection));
                float3 specularColor = (_MetallicMap_var.r*_Metalness);
                float specularMonochrome;
                float4 _Albedo_var = tex2D(_Albedo,TRANSFORM_TEX(i.uv0, _Albedo));
                float3 node_1779 = (_Albedo_var.rgb*_Color.rgb);
                float node_4065 = 0.0;
                float node_7201 = 0.025;
                float node_6355 = max(0,dot(lightDirection,normalDirection));
                float node_8285 = saturate((saturate(smoothstep( node_4065, node_7201, (node_6355*3.0+-1.0) ))*(saturate(smoothstep( node_4065, node_7201, (node_6355*5.0+-3.0) ))+0.5)));
                float4 _OcclussionMap_var = tex2D(_OcclussionMap,TRANSFORM_TEX(i.uv0, _OcclussionMap));
                float node_8150 = lerp(1.0,_OcclussionMap_var.r,_OcclussionIntensity);
                float3 diffuseColor = ((node_1779+(saturate((smoothstep( node_4065, node_7201, (pow(node_6355,exp((_AlbedoGloss*10.75+0.25)))*2.0+-1.0) )*_AlbedoGlossIntensity))*node_1779))*node_8285*node_8150); // Need this for specular when using metallic
                diffuseColor = DiffuseAndSpecularFromMetallic( diffuseColor, specularColor, specularColor, specularMonochrome );
                specularMonochrome = 1.0-specularMonochrome;
                float NdotV = max(0.0,dot( normalDirection, viewDirection ));
                float NdotH = max(0.0,dot( normalDirection, halfDirection ));
                float VdotH = max(0.0,dot( viewDirection, halfDirection ));
                float visTerm = SmithJointGGXVisibilityTerm( NdotL, NdotV, 1.0-gloss );
                float normTerm = max(0.0, GGXTerm(NdotH, 1.0-gloss));
                float specularPBL = (NdotL*visTerm*normTerm) * (UNITY_PI / 4);
                if (IsGammaSpace())
                    specularPBL = sqrt(max(1e-4h, specularPBL));
                specularPBL = max(0, specularPBL * NdotL);
                float3 directSpecular = attenColor*specularPBL*FresnelTerm(specularColor, LdotH);
                float3 specular = directSpecular;
/////// Diffuse:
                NdotL = max(0.0,dot( normalDirection, lightDirection ));
                half fd90 = 0.5 + 2 * LdotH * LdotH * (1-gloss);
                float nlPow5 = Pow5(1-NdotL);
                float nvPow5 = Pow5(1-NdotV);
                float3 directDiffuse = ((1 +(fd90 - 1)*nlPow5) * (1 + (fd90 - 1)*nvPow5) * NdotL) * attenColor;
                float3 diffuse = directDiffuse * diffuseColor;
/// Final Color:
                float3 finalColor = diffuse + specular;
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Standard"
//    CustomEditor "ShaderForgeMaterialInspector"
}
