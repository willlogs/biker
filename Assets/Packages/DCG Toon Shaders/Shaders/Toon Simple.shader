// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32858,y:32100,varname:node_3138,prsc:2|normal-324-OUT,emission-3530-OUT,custl-2469-OUT,olwid-2802-OUT,olcol-6565-RGB;n:type:ShaderForge.SFN_Color,id:7241,x:30344,y:32904,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.7058823,c2:0.7058823,c3:0.7058823,c4:1;n:type:ShaderForge.SFN_LightColor,id:8246,x:30959,y:31967,varname:node_8246,prsc:2;n:type:ShaderForge.SFN_LightAttenuation,id:8166,x:30959,y:32100,varname:node_8166,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:1159,x:30344,y:32719,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:node_1159,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:3674,x:30688,y:32792,varname:node_3674,prsc:2|A-1159-RGB,B-7241-RGB;n:type:ShaderForge.SFN_Slider,id:5243,x:31358,y:33194,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:node_5243,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_Tex2d,id:233,x:32863,y:31748,ptovrint:False,ptlb:Normal Map,ptin:_NormalMap,varname:node_233,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Lerp,id:324,x:33064,y:31748,varname:node_324,prsc:2|A-5770-OUT,B-233-RGB,T-9922-OUT;n:type:ShaderForge.SFN_Vector3,id:5770,x:33064,y:31651,varname:node_5770,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Slider,id:9922,x:32998,y:31992,ptovrint:False,ptlb:Normal Intensity,ptin:_NormalIntensity,varname:node_9922,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.6,max:1;n:type:ShaderForge.SFN_Tex2d,id:5873,x:31813,y:31716,ptovrint:False,ptlb:Emission Map,ptin:_EmissionMap,varname:node_5873,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:7134,x:31813,y:31904,ptovrint:False,ptlb:HDR Emission Color,ptin:_HDREmissionColor,varname:node_7134,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Slider,id:6941,x:31372,y:33327,ptovrint:False,ptlb:Gloss Intensity,ptin:_GlossIntensity,varname:node_6941,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Tex2d,id:9940,x:30358,y:33184,ptovrint:False,ptlb:Occlussion Map,ptin:_OcclussionMap,varname:node_9940,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:2871,x:30358,y:33361,ptovrint:False,ptlb:Occlussion Intensity,ptin:_OcclussionIntensity,varname:node_2871,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:7893,x:30751,y:33306,varname:node_7893,prsc:2|A-9675-OUT,B-9940-RGB,T-2871-OUT;n:type:ShaderForge.SFN_Vector1,id:9675,x:30515,y:33282,varname:node_9675,prsc:2,v1:1;n:type:ShaderForge.SFN_Color,id:6565,x:32925,y:32599,ptovrint:False,ptlb:Outline Color,ptin:_OutlineColor,varname:node_6565,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Slider,id:2802,x:32867,y:32802,ptovrint:False,ptlb:Outline Width,ptin:_OutlineWidth,varname:node_2802,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.025;n:type:ShaderForge.SFN_Multiply,id:4345,x:31412,y:31993,varname:node_4345,prsc:2|A-8246-RGB,B-8166-OUT;n:type:ShaderForge.SFN_LightVector,id:6770,x:29684,y:32211,varname:node_6770,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:3559,x:29684,y:32352,prsc:2,pt:True;n:type:ShaderForge.SFN_Dot,id:5654,x:29890,y:32268,varname:node_5654,prsc:2,dt:1|A-6770-OUT,B-3559-OUT;n:type:ShaderForge.SFN_RemapRange,id:9804,x:29866,y:32437,varname:node_9804,prsc:2,frmn:0,frmx:0.1,tomn:0,tomx:6|IN-5654-OUT;n:type:ShaderForge.SFN_Clamp01,id:6849,x:30104,y:32437,varname:node_6849,prsc:2|IN-9804-OUT;n:type:ShaderForge.SFN_RemapRange,id:3485,x:29890,y:32080,varname:node_3485,prsc:2,frmn:0,frmx:1,tomn:-12,tomx:16|IN-5654-OUT;n:type:ShaderForge.SFN_Clamp01,id:2483,x:30113,y:32115,varname:node_2483,prsc:2|IN-3485-OUT;n:type:ShaderForge.SFN_Multiply,id:8664,x:30377,y:32401,varname:node_8664,prsc:2|A-6849-OUT,B-9707-OUT;n:type:ShaderForge.SFN_Vector1,id:9707,x:30113,y:32312,varname:node_9707,prsc:2,v1:0.5;n:type:ShaderForge.SFN_Multiply,id:975,x:30377,y:32215,varname:node_975,prsc:2|A-2483-OUT,B-9707-OUT;n:type:ShaderForge.SFN_Add,id:8440,x:30567,y:32215,varname:node_8440,prsc:2|A-975-OUT,B-8664-OUT;n:type:ShaderForge.SFN_Multiply,id:8617,x:31065,y:32553,varname:node_8617,prsc:2|A-3674-OUT,B-7893-OUT;n:type:ShaderForge.SFN_Multiply,id:7548,x:31335,y:32378,varname:node_7548,prsc:2|A-8440-OUT,B-8617-OUT;n:type:ShaderForge.SFN_LightVector,id:9309,x:31751,y:32792,varname:node_9309,prsc:2;n:type:ShaderForge.SFN_Dot,id:4669,x:31979,y:32949,varname:node_4669,prsc:2,dt:1|A-9309-OUT,B-6011-OUT;n:type:ShaderForge.SFN_RemapRange,id:3974,x:32572,y:32962,varname:node_3974,prsc:2,frmn:0,frmx:1,tomn:-6,tomx:6|IN-3082-OUT;n:type:ShaderForge.SFN_Clamp01,id:3462,x:32198,y:32701,varname:node_3462,prsc:2|IN-3974-OUT;n:type:ShaderForge.SFN_RemapRange,id:610,x:32029,y:33302,varname:node_610,prsc:2,frmn:0,frmx:1,tomn:1,tomx:11|IN-5243-OUT;n:type:ShaderForge.SFN_Exp,id:1105,x:32316,y:33270,varname:node_1105,prsc:2,et:1|IN-610-OUT;n:type:ShaderForge.SFN_Power,id:3082,x:32311,y:32960,varname:node_3082,prsc:2|VAL-4669-OUT,EXP-1105-OUT;n:type:ShaderForge.SFN_Add,id:7508,x:31762,y:32383,varname:node_7508,prsc:2|A-7548-OUT,B-9955-OUT;n:type:ShaderForge.SFN_NormalVector,id:6011,x:31751,y:32976,prsc:2,pt:True;n:type:ShaderForge.SFN_Multiply,id:9955,x:32449,y:32749,varname:node_9955,prsc:2|A-3462-OUT,B-6941-OUT;n:type:ShaderForge.SFN_Multiply,id:2469,x:32394,y:32285,varname:node_2469,prsc:2|A-4345-OUT,B-7508-OUT;n:type:ShaderForge.SFN_Multiply,id:3159,x:32240,y:32038,varname:node_3159,prsc:2|A-4337-RGB,B-7508-OUT;n:type:ShaderForge.SFN_AmbientLight,id:4337,x:31965,y:32020,varname:node_4337,prsc:2;n:type:ShaderForge.SFN_Multiply,id:4933,x:32212,y:31749,varname:node_4933,prsc:2|A-5873-RGB,B-7134-RGB;n:type:ShaderForge.SFN_Add,id:3530,x:32548,y:31877,varname:node_3530,prsc:2|A-4933-OUT,B-3159-OUT;proporder:1159-7241-233-9922-5243-6941-9940-2871-5873-7134-2802-6565;pass:END;sub:END;*/

Shader "DCG/Toon Shaders/Toon Simple" {
    Properties {
        _Diffuse ("Diffuse", 2D) = "white" {}
        _Color ("Color", Color) = (0.7058823,0.7058823,0.7058823,1)
        _NormalMap ("Normal Map", 2D) = "bump" {}
        _NormalIntensity ("Normal Intensity", Range(0, 1)) = 0.6
        _Gloss ("Gloss", Range(0, 1)) = 0.5
        _GlossIntensity ("Gloss Intensity", Range(0, 1)) = 1
        _OcclussionMap ("Occlussion Map", 2D) = "white" {}
        _OcclussionIntensity ("Occlussion Intensity", Range(0, 1)) = 0
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
                float3 _NormalMap_var = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(i.uv0, _NormalMap)));
                float3 normalLocal = lerp(float3(0,0,1),_NormalMap_var.rgb,_NormalIntensity);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
////// Emissive:
                float4 _EmissionMap_var = tex2D(_EmissionMap,TRANSFORM_TEX(i.uv0, _EmissionMap));
                float node_5654 = max(0,dot(lightDirection,normalDirection));
                float node_9707 = 0.5;
                float node_8440 = ((saturate((node_5654*28.0+-12.0))*node_9707)+(saturate((node_5654*60.0+0.0))*node_9707));
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float3 node_3674 = (_Diffuse_var.rgb*_Color.rgb);
                float node_9675 = 1.0;
                float4 _OcclussionMap_var = tex2D(_OcclussionMap,TRANSFORM_TEX(i.uv0, _OcclussionMap));
                float3 node_7893 = lerp(float3(node_9675,node_9675,node_9675),_OcclussionMap_var.rgb,_OcclussionIntensity);
                float3 node_7508 = ((node_8440*(node_3674*node_7893))+(saturate((pow(max(0,dot(lightDirection,normalDirection)),exp2((_Gloss*10.0+1.0)))*12.0+-6.0))*_GlossIntensity));
                float3 emissive = ((_EmissionMap_var.rgb*_HDREmissionColor.rgb)+(UNITY_LIGHTMODEL_AMBIENT.rgb*node_7508));
                float3 node_4345 = (_LightColor0.rgb*attenuation);
                float3 finalColor = emissive + (node_4345*node_7508);
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
                float3 _NormalMap_var = UnpackNormal(tex2D(_NormalMap,TRANSFORM_TEX(i.uv0, _NormalMap)));
                float3 normalLocal = lerp(float3(0,0,1),_NormalMap_var.rgb,_NormalIntensity);
                float3 normalDirection = normalize(mul( normalLocal, tangentTransform )); // Perturbed normals
                float3 lightDirection = normalize(lerp(_WorldSpaceLightPos0.xyz, _WorldSpaceLightPos0.xyz - i.posWorld.xyz,_WorldSpaceLightPos0.w));
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = LIGHT_ATTENUATION(i);
                float3 node_4345 = (_LightColor0.rgb*attenuation);
                float node_5654 = max(0,dot(lightDirection,normalDirection));
                float node_9707 = 0.5;
                float node_8440 = ((saturate((node_5654*28.0+-12.0))*node_9707)+(saturate((node_5654*60.0+0.0))*node_9707));
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float3 node_3674 = (_Diffuse_var.rgb*_Color.rgb);
                float node_9675 = 1.0;
                float4 _OcclussionMap_var = tex2D(_OcclussionMap,TRANSFORM_TEX(i.uv0, _OcclussionMap));
                float3 node_7893 = lerp(float3(node_9675,node_9675,node_9675),_OcclussionMap_var.rgb,_OcclussionIntensity);
                float3 node_7508 = ((node_8440*(node_3674*node_7893))+(saturate((pow(max(0,dot(lightDirection,normalDirection)),exp2((_Gloss*10.0+1.0)))*12.0+-6.0))*_GlossIntensity));
                float3 finalColor = (node_4345*node_7508);
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
