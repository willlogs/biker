// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32762,y:32084,varname:node_3138,prsc:2|normal-324-OUT,emission-951-OUT,custl-4747-OUT,olwid-2802-OUT,olcol-6565-RGB;n:type:ShaderForge.SFN_Color,id:7241,x:30309,y:32732,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.7058823,c2:0.7058823,c3:0.7058823,c4:1;n:type:ShaderForge.SFN_LightVector,id:9322,x:29593,y:32122,varname:node_9322,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:3177,x:29593,y:32270,prsc:2,pt:True;n:type:ShaderForge.SFN_Dot,id:6355,x:29782,y:32122,varname:node_6355,prsc:2,dt:1|A-9322-OUT,B-3177-OUT;n:type:ShaderForge.SFN_Smoothstep,id:8748,x:29975,y:31965,varname:node_8748,prsc:2|A-4065-OUT,B-7201-OUT,V-1601-OUT;n:type:ShaderForge.SFN_Vector1,id:4065,x:29152,y:31966,varname:node_4065,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:7201,x:29152,y:32020,varname:node_7201,prsc:2,v1:0.025;n:type:ShaderForge.SFN_RemapRange,id:3209,x:29782,y:32270,varname:node_3209,prsc:2,frmn:0,frmx:1,tomn:-2,tomx:1|IN-6355-OUT;n:type:ShaderForge.SFN_Smoothstep,id:7613,x:29990,y:32270,varname:node_7613,prsc:2|A-4065-OUT,B-7201-OUT,V-3209-OUT;n:type:ShaderForge.SFN_Clamp01,id:5149,x:30210,y:32270,varname:node_5149,prsc:2|IN-7613-OUT;n:type:ShaderForge.SFN_Clamp01,id:9440,x:30205,y:31965,varname:node_9440,prsc:2|IN-8748-OUT;n:type:ShaderForge.SFN_Add,id:6060,x:30414,y:32119,varname:node_6060,prsc:2|A-5149-OUT,B-5430-OUT;n:type:ShaderForge.SFN_Vector1,id:5430,x:30205,y:32119,varname:node_5430,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:6031,x:30414,y:31965,varname:node_6031,prsc:2|A-9440-OUT,B-6060-OUT;n:type:ShaderForge.SFN_Clamp01,id:8285,x:30414,y:32270,varname:node_8285,prsc:2|IN-6031-OUT;n:type:ShaderForge.SFN_RemapRange,id:1601,x:29782,y:31965,varname:node_1601,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:3|IN-6355-OUT;n:type:ShaderForge.SFN_Multiply,id:4747,x:32403,y:32233,varname:node_4747,prsc:2|A-8246-RGB,B-8166-OUT,C-4562-OUT;n:type:ShaderForge.SFN_LightColor,id:8246,x:30959,y:31967,varname:node_8246,prsc:2;n:type:ShaderForge.SFN_LightAttenuation,id:8166,x:30959,y:32100,varname:node_8166,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:1159,x:30309,y:32545,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:node_1159,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:3674,x:30908,y:32458,varname:node_3674,prsc:2|A-4720-OUT,B-7241-RGB;n:type:ShaderForge.SFN_Slider,id:5243,x:28647,y:32902,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:node_5243,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_RemapRange,id:4196,x:29101,y:32767,varname:node_4196,prsc:2,frmn:0,frmx:1,tomn:0.25,tomx:11|IN-5243-OUT;n:type:ShaderForge.SFN_Exp,id:4203,x:29101,y:32616,varname:node_4203,prsc:2,et:0|IN-4196-OUT;n:type:ShaderForge.SFN_Power,id:3468,x:29305,y:32598,varname:node_3468,prsc:2|VAL-6355-OUT,EXP-4203-OUT;n:type:ShaderForge.SFN_Add,id:9101,x:31009,y:32272,varname:node_9101,prsc:2|A-3918-OUT,B-860-OUT;n:type:ShaderForge.SFN_Multiply,id:3918,x:30628,y:32270,varname:node_3918,prsc:2|A-8285-OUT,B-3674-OUT;n:type:ShaderForge.SFN_RemapRange,id:4568,x:29532,y:32671,varname:node_4568,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-3468-OUT;n:type:ShaderForge.SFN_Clamp01,id:860,x:30094,y:32479,varname:node_860,prsc:2|IN-3003-OUT;n:type:ShaderForge.SFN_Smoothstep,id:9028,x:29532,y:32531,varname:node_9028,prsc:2|A-4065-OUT,B-7201-OUT,V-4568-OUT;n:type:ShaderForge.SFN_Tex2d,id:233,x:32863,y:31748,ptovrint:False,ptlb:Normal Map,ptin:_NormalMap,varname:node_233,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Lerp,id:324,x:33064,y:31748,varname:node_324,prsc:2|A-5770-OUT,B-233-RGB,T-9922-OUT;n:type:ShaderForge.SFN_Vector3,id:5770,x:33064,y:31651,varname:node_5770,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Slider,id:9922,x:32998,y:31992,ptovrint:False,ptlb:Normal Intensity,ptin:_NormalIntensity,varname:node_9922,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.6,max:1;n:type:ShaderForge.SFN_Tex2d,id:5873,x:31974,y:31866,ptovrint:False,ptlb:Emission Map,ptin:_EmissionMap,varname:node_5873,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:7134,x:31974,y:32054,ptovrint:False,ptlb:HDR Emission Color,ptin:_HDREmissionColor,varname:node_7134,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Multiply,id:6316,x:32195,y:31987,varname:node_6316,prsc:2|A-5873-RGB,B-7134-RGB;n:type:ShaderForge.SFN_Slider,id:6941,x:29348,y:32920,ptovrint:False,ptlb:Gloss Intensity,ptin:_GlossIntensity,varname:node_6941,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:3003,x:29886,y:32505,varname:node_3003,prsc:2|A-9028-OUT,B-6941-OUT;n:type:ShaderForge.SFN_Tex2d,id:9940,x:31969,y:32454,ptovrint:False,ptlb:Occlussion Map,ptin:_OcclussionMap,varname:node_9940,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:2871,x:31812,y:32657,ptovrint:False,ptlb:Occlussion Intensity,ptin:_OcclussionIntensity,varname:node_2871,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:7893,x:32297,y:32601,varname:node_7893,prsc:2|A-9675-OUT,B-9940-RGB,T-2871-OUT;n:type:ShaderForge.SFN_Vector1,id:9675,x:32211,y:32517,varname:node_9675,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:4562,x:31704,y:32287,varname:node_4562,prsc:2|A-9101-OUT,B-7893-OUT;n:type:ShaderForge.SFN_Color,id:6565,x:32523,y:32427,ptovrint:False,ptlb:Outline Color,ptin:_OutlineColor,varname:node_6565,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Slider,id:2802,x:32577,y:32706,ptovrint:False,ptlb:Outline Width,ptin:_OutlineWidth,varname:node_2802,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.025;n:type:ShaderForge.SFN_Slider,id:4472,x:31795,y:31446,ptovrint:False,ptlb:CutOut,ptin:_CutOut,varname:node_4472,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Relay,id:3704,x:32082,y:31702,varname:node_3704,prsc:2|IN-1159-A;n:type:ShaderForge.SFN_Multiply,id:2014,x:32239,y:31690,varname:node_2014,prsc:2|A-9176-OUT,B-3704-OUT;n:type:ShaderForge.SFN_RemapRange,id:9176,x:32196,y:31453,varname:node_9176,prsc:2,frmn:0,frmx:1,tomn:1,tomx:0|IN-4472-OUT;n:type:ShaderForge.SFN_Add,id:7282,x:32444,y:31551,varname:node_7282,prsc:2|A-9176-OUT,B-2014-OUT;n:type:ShaderForge.SFN_Clamp01,id:5992,x:32613,y:31389,varname:node_5992,prsc:2|IN-7282-OUT;n:type:ShaderForge.SFN_Cubemap,id:772,x:30425,y:33123,ptovrint:False,ptlb:Reflection Cubemap,ptin:_ReflectionCubemap,varname:node_772,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,cube:cb6423272b9355b4daa2c58f3bd66a40,pvfc:0|DIR-5917-OUT,MIP-3766-OUT;n:type:ShaderForge.SFN_Tex2d,id:9142,x:30425,y:32889,ptovrint:False,ptlb:Reflection Mask,ptin:_ReflectionMask,varname:node_9142,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:3082,x:30804,y:33102,varname:node_3082,prsc:2|A-9142-R,B-772-RGB;n:type:ShaderForge.SFN_ViewReflectionVector,id:5917,x:30101,y:32884,varname:node_5917,prsc:2;n:type:ShaderForge.SFN_Slider,id:6923,x:30382,y:33604,ptovrint:False,ptlb:Reflection Intensity,ptin:_ReflectionIntensity,varname:node_6923,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.8,max:1;n:type:ShaderForge.SFN_Blend,id:683,x:30751,y:32664,varname:node_683,prsc:2,blmd:6,clmp:True|SRC-1159-RGB,DST-2588-OUT;n:type:ShaderForge.SFN_OneMinus,id:1153,x:30791,y:32821,varname:node_1153,prsc:2|IN-9142-R;n:type:ShaderForge.SFN_Add,id:4689,x:31020,y:32885,varname:node_4689,prsc:2|A-1153-OUT,B-2588-OUT;n:type:ShaderForge.SFN_Multiply,id:7934,x:31230,y:32885,varname:node_7934,prsc:2|A-4689-OUT,B-2671-OUT;n:type:ShaderForge.SFN_Vector1,id:2671,x:31020,y:33008,varname:node_2671,prsc:2,v1:1.6;n:type:ShaderForge.SFN_Multiply,id:4720,x:31008,y:32736,varname:node_4720,prsc:2|A-683-OUT,B-7934-OUT;n:type:ShaderForge.SFN_RemapRange,id:1279,x:31279,y:33399,varname:node_1279,prsc:2,frmn:0,frmx:1,tomn:0.4,tomx:0|IN-6923-OUT;n:type:ShaderForge.SFN_Add,id:4620,x:31152,y:33290,varname:node_4620,prsc:2|A-3082-OUT,B-1279-OUT;n:type:ShaderForge.SFN_Multiply,id:2588,x:31268,y:33092,varname:node_2588,prsc:2|A-9142-R,B-4620-OUT;n:type:ShaderForge.SFN_RemapRange,id:3766,x:29896,y:33229,varname:node_3766,prsc:2,frmn:0,frmx:1,tomn:7,tomx:0.5|IN-5315-OUT;n:type:ShaderForge.SFN_Multiply,id:5315,x:29729,y:33205,varname:node_5315,prsc:2|A-9142-A,B-4932-OUT;n:type:ShaderForge.SFN_Slider,id:4932,x:29291,y:33220,ptovrint:False,ptlb:Reflection Sharpness,ptin:_ReflectionSharpness,varname:node_4932,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:3593,x:32165,y:32277,varname:node_3593,prsc:2|A-4562-OUT,B-9509-RGB;n:type:ShaderForge.SFN_AmbientLight,id:9509,x:31939,y:32267,varname:node_9509,prsc:2;n:type:ShaderForge.SFN_Add,id:951,x:32475,y:31948,varname:node_951,prsc:2|A-6316-OUT,B-3593-OUT;proporder:7241-1159-4472-233-9922-5243-6941-9940-2871-772-9142-6923-4932-5873-7134-2802-6565;pass:END;sub:END;*/

Shader "DCG/Toon Shaders/Toon IBL" {
    Properties {
        _Color ("Color", Color) = (0.7058823,0.7058823,0.7058823,1)
        _Diffuse ("Diffuse", 2D) = "white" {}
        [HideInInspector]_CutOut ("CutOut", Range(0, 1)) = 0
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalIntensity ("Normal Intensity", Range(0, 1)) = 0.6
        _Gloss ("Gloss", Range(0, 1)) = 0.5
        _GlossIntensity ("Gloss Intensity", Range(0, 1)) = 1
        _OcclussionMap ("Occlussion Map", 2D) = "white" {}
        _OcclussionIntensity ("Occlussion Intensity", Range(0, 1)) = 0
        _ReflectionCubemap ("Reflection Cubemap", Cube) = "_Skybox" {}
        _ReflectionMask ("Reflection Mask", 2D) = "white" {}
        _ReflectionIntensity ("Reflection Intensity", Range(0, 1)) = 0.8
        _ReflectionSharpness ("Reflection Sharpness", Range(0, 1)) = 1
        _EmissionMap ("Emission Map", 2D) = "white" {}
        [HDR]_HDREmissionColor ("HDR Emission Color", Color) = (0,0,0,1)
        _OutlineWidth ("Outline Width", Range(0, 0.025)) = 0
        _OutlineColor ("Outline Color", Color) = (0,0,0,1)
    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        Pass {
            Name "Outline"
            Tags {
            }
            Cull Front
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
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
                o.pos = UnityObjectToClipPos( float4(v.vertex.xyz + v.normal*_OutlineWidth,1) );
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
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _Gloss;
            uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
            uniform float _NormalIntensity;
            uniform sampler2D _EmissionMap; uniform float4 _EmissionMap_ST;
            uniform float4 _HDREmissionColor;
            uniform float _GlossIntensity;
            uniform sampler2D _OcclussionMap; uniform float4 _OcclussionMap_ST;
            uniform float _OcclussionIntensity;
            uniform samplerCUBE _ReflectionCubemap;
            uniform sampler2D _ReflectionMask; uniform float4 _ReflectionMask_ST;
            uniform float _ReflectionIntensity;
            uniform float _ReflectionSharpness;
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
                o.pos = UnityObjectToClipPos( v.vertex );
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
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
////// Emissive:
                float4 _EmissionMap_var = tex2D(_EmissionMap,TRANSFORM_TEX(i.uv0, _EmissionMap));
                float node_4065 = 0.0;
                float node_7201 = 0.025;
                float node_6355 = max(0,dot(lightDirection,normalDirection));
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float4 _ReflectionMask_var = tex2D(_ReflectionMask,TRANSFORM_TEX(i.uv0, _ReflectionMask));
                float4 _ReflectionCubemap_var = texCUBElod(_ReflectionCubemap,float4(viewReflectDirection,((_ReflectionMask_var.a*_ReflectionSharpness)*-6.5+7.0)));
                float3 node_3082 = (_ReflectionMask_var.r*_ReflectionCubemap_var.rgb);
                float3 node_2588 = (_ReflectionMask_var.r*(node_3082+(_ReflectionIntensity*-0.4+0.4)));
                float node_9675 = 1.0;
                float4 _OcclussionMap_var = tex2D(_OcclussionMap,TRANSFORM_TEX(i.uv0, _OcclussionMap));
                float3 node_4562 = (((saturate((saturate(smoothstep( node_4065, node_7201, (node_6355*4.0+-1.0) ))*(saturate(smoothstep( node_4065, node_7201, (node_6355*3.0+-2.0) ))+0.5)))*((saturate((1.0-(1.0-_Diffuse_var.rgb)*(1.0-node_2588)))*(((1.0 - _ReflectionMask_var.r)+node_2588)*1.6))*_Color.rgb))+saturate((smoothstep( node_4065, node_7201, (pow(node_6355,exp((_Gloss*10.75+0.25)))*2.0+-1.0) )*_GlossIntensity)))*lerp(float3(node_9675,node_9675,node_9675),_OcclussionMap_var.rgb,_OcclussionIntensity));
                float3 emissive = ((_EmissionMap_var.rgb*_HDREmissionColor.rgb)+(node_4562*UNITY_LIGHTMODEL_AMBIENT.rgb));
                float3 finalColor = emissive + (_LightColor0.rgb*attenuation*node_4562);
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
            #include "UnityCG.cginc"
            #include "AutoLight.cginc"
            #include "Lighting.cginc"
            #pragma multi_compile_fwdadd_fullshadows
            #pragma only_renderers d3d9 d3d11 glcore gles 
            #pragma target 3.0
            uniform float4 _Color;
            uniform sampler2D _Diffuse; uniform float4 _Diffuse_ST;
            uniform float _Gloss;
            uniform sampler2D _NormalMap; uniform float4 _NormalMap_ST;
            uniform float _NormalIntensity;
            uniform sampler2D _EmissionMap; uniform float4 _EmissionMap_ST;
            uniform float4 _HDREmissionColor;
            uniform float _GlossIntensity;
            uniform sampler2D _OcclussionMap; uniform float4 _OcclussionMap_ST;
            uniform float _OcclussionIntensity;
            uniform samplerCUBE _ReflectionCubemap;
            uniform sampler2D _ReflectionMask; uniform float4 _ReflectionMask_ST;
            uniform float _ReflectionIntensity;
            uniform float _ReflectionSharpness;
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
                o.pos = UnityObjectToClipPos( v.vertex );
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
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float node_4065 = 0.0;
                float node_7201 = 0.025;
                float node_6355 = max(0,dot(lightDirection,normalDirection));
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float4 _ReflectionMask_var = tex2D(_ReflectionMask,TRANSFORM_TEX(i.uv0, _ReflectionMask));
                float4 _ReflectionCubemap_var = texCUBElod(_ReflectionCubemap,float4(viewReflectDirection,((_ReflectionMask_var.a*_ReflectionSharpness)*-6.5+7.0)));
                float3 node_3082 = (_ReflectionMask_var.r*_ReflectionCubemap_var.rgb);
                float3 node_2588 = (_ReflectionMask_var.r*(node_3082+(_ReflectionIntensity*-0.4+0.4)));
                float node_9675 = 1.0;
                float4 _OcclussionMap_var = tex2D(_OcclussionMap,TRANSFORM_TEX(i.uv0, _OcclussionMap));
                float3 node_4562 = (((saturate((saturate(smoothstep( node_4065, node_7201, (node_6355*4.0+-1.0) ))*(saturate(smoothstep( node_4065, node_7201, (node_6355*3.0+-2.0) ))+0.5)))*((saturate((1.0-(1.0-_Diffuse_var.rgb)*(1.0-node_2588)))*(((1.0 - _ReflectionMask_var.r)+node_2588)*1.6))*_Color.rgb))+saturate((smoothstep( node_4065, node_7201, (pow(node_6355,exp((_Gloss*10.75+0.25)))*2.0+-1.0) )*_GlossIntensity)))*lerp(float3(node_9675,node_9675,node_9675),_OcclussionMap_var.rgb,_OcclussionIntensity));
                float3 finalColor = (_LightColor0.rgb*attenuation*node_4562);
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
