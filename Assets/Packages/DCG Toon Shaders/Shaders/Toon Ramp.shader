// Shader created with Shader Forge v1.37 
// Shader Forge (c) Neat Corporation / Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.37;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,lico:1,lgpr:1,limd:0,spmd:1,trmd:0,grmd:0,uamb:True,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:0,bdst:1,dpts:2,wrdp:True,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:False,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:False,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:3138,x:32858,y:32100,varname:node_3138,prsc:2|normal-324-OUT,emission-9313-OUT,custl-4562-OUT,olwid-2802-OUT,olcol-6565-RGB;n:type:ShaderForge.SFN_Color,id:7241,x:30416,y:32731,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_7241,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0.7058823,c2:0.7058823,c3:0.7058823,c4:1;n:type:ShaderForge.SFN_LightVector,id:9322,x:28978,y:32141,varname:node_9322,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:3177,x:28978,y:32289,prsc:2,pt:True;n:type:ShaderForge.SFN_Dot,id:6355,x:29268,y:32227,varname:node_6355,prsc:2,dt:1|A-9322-OUT,B-3177-OUT;n:type:ShaderForge.SFN_Vector1,id:4065,x:29152,y:31966,varname:node_4065,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:7201,x:29152,y:32020,varname:node_7201,prsc:2,v1:0.025;n:type:ShaderForge.SFN_Multiply,id:4747,x:32068,y:32050,varname:node_4747,prsc:2|A-8246-RGB,B-8166-OUT;n:type:ShaderForge.SFN_LightColor,id:8246,x:31714,y:31925,varname:node_8246,prsc:2;n:type:ShaderForge.SFN_LightAttenuation,id:8166,x:31714,y:32060,varname:node_8166,prsc:2;n:type:ShaderForge.SFN_Tex2d,id:1159,x:30416,y:32548,ptovrint:False,ptlb:Diffuse,ptin:_Diffuse,varname:node_1159,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Multiply,id:3674,x:30669,y:32548,varname:node_3674,prsc:2|A-1159-RGB,B-7241-RGB;n:type:ShaderForge.SFN_AmbientLight,id:2545,x:31020,y:32513,varname:node_2545,prsc:2;n:type:ShaderForge.SFN_Multiply,id:7553,x:31206,y:32513,varname:node_7553,prsc:2|A-2545-RGB,B-3154-OUT,C-3674-OUT;n:type:ShaderForge.SFN_Vector1,id:3154,x:31020,y:32664,varname:node_3154,prsc:2,v1:0.4;n:type:ShaderForge.SFN_Add,id:1525,x:31670,y:32273,varname:node_1525,prsc:2|A-8586-OUT,B-6539-OUT;n:type:ShaderForge.SFN_Slider,id:5243,x:28768,y:32767,ptovrint:False,ptlb:Gloss,ptin:_Gloss,varname:node_5243,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.5,max:1;n:type:ShaderForge.SFN_RemapRange,id:4196,x:29101,y:32767,varname:node_4196,prsc:2,frmn:0,frmx:1,tomn:0.25,tomx:11|IN-5243-OUT;n:type:ShaderForge.SFN_Exp,id:4203,x:29101,y:32616,varname:node_4203,prsc:2,et:0|IN-4196-OUT;n:type:ShaderForge.SFN_Power,id:3468,x:29305,y:32598,varname:node_3468,prsc:2|VAL-6355-OUT,EXP-4203-OUT;n:type:ShaderForge.SFN_Add,id:9101,x:31009,y:32272,varname:node_9101,prsc:2|A-3918-OUT,B-860-OUT;n:type:ShaderForge.SFN_Multiply,id:3918,x:30628,y:32270,varname:node_3918,prsc:2|A-4482-RGB,B-3674-OUT;n:type:ShaderForge.SFN_Multiply,id:8586,x:31252,y:32272,varname:node_8586,prsc:2|A-8378-OUT,B-9101-OUT;n:type:ShaderForge.SFN_RemapRange,id:4568,x:29532,y:32671,varname:node_4568,prsc:2,frmn:0,frmx:1,tomn:-1,tomx:1|IN-3468-OUT;n:type:ShaderForge.SFN_Clamp01,id:860,x:30094,y:32479,varname:node_860,prsc:2|IN-3003-OUT;n:type:ShaderForge.SFN_Smoothstep,id:9028,x:29532,y:32531,varname:node_9028,prsc:2|A-4065-OUT,B-7201-OUT,V-4568-OUT;n:type:ShaderForge.SFN_Tex2d,id:233,x:32863,y:31748,ptovrint:False,ptlb:Normal Map,ptin:_NormalMap,varname:node_233,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:3,isnm:True;n:type:ShaderForge.SFN_Lerp,id:324,x:33064,y:31748,varname:node_324,prsc:2|A-5770-OUT,B-233-RGB,T-9922-OUT;n:type:ShaderForge.SFN_Vector3,id:5770,x:33064,y:31651,varname:node_5770,prsc:2,v1:0,v2:0,v3:1;n:type:ShaderForge.SFN_Slider,id:9922,x:32998,y:31992,ptovrint:False,ptlb:Normal Intensity,ptin:_NormalIntensity,varname:node_9922,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0.6,max:1;n:type:ShaderForge.SFN_Tex2d,id:5873,x:31538,y:31474,ptovrint:False,ptlb:Emission Map,ptin:_EmissionMap,varname:node_5873,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Color,id:7134,x:31538,y:31662,ptovrint:False,ptlb:HDR Emission Color,ptin:_HDREmissionColor,varname:node_7134,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Multiply,id:6316,x:31759,y:31595,varname:node_6316,prsc:2|A-5873-RGB,B-7134-RGB;n:type:ShaderForge.SFN_Slider,id:6941,x:29022,y:33025,ptovrint:False,ptlb:Gloss Intensity,ptin:_GlossIntensity,varname:node_6941,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:1,max:1;n:type:ShaderForge.SFN_Multiply,id:3003,x:29886,y:32505,varname:node_3003,prsc:2|A-9028-OUT,B-6941-OUT;n:type:ShaderForge.SFN_Tex2d,id:9940,x:31465,y:32584,ptovrint:False,ptlb:Occlussion Map,ptin:_OcclussionMap,varname:node_9940,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False;n:type:ShaderForge.SFN_Slider,id:2871,x:31308,y:32787,ptovrint:False,ptlb:Occlussion Intensity,ptin:_OcclussionIntensity,varname:node_2871,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Lerp,id:7893,x:31793,y:32731,varname:node_7893,prsc:2|A-9675-OUT,B-9940-RGB,T-2871-OUT;n:type:ShaderForge.SFN_Vector1,id:9675,x:31707,y:32647,varname:node_9675,prsc:2,v1:1;n:type:ShaderForge.SFN_Multiply,id:4562,x:32339,y:32223,varname:node_4562,prsc:2|A-1525-OUT,B-7893-OUT,C-4747-OUT;n:type:ShaderForge.SFN_Color,id:6565,x:32523,y:32427,ptovrint:False,ptlb:Outline Color,ptin:_OutlineColor,varname:node_6565,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,c1:0,c2:0,c3:0,c4:1;n:type:ShaderForge.SFN_Slider,id:2802,x:32577,y:32706,ptovrint:False,ptlb:Outline Width,ptin:_OutlineWidth,varname:node_2802,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:0.025;n:type:ShaderForge.SFN_Slider,id:4472,x:31795,y:31446,ptovrint:False,ptlb:CutOut,ptin:_CutOut,varname:node_4472,prsc:2,glob:False,taghide:True,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,min:0,cur:0,max:1;n:type:ShaderForge.SFN_Relay,id:3704,x:31818,y:31799,varname:node_3704,prsc:2|IN-1159-A;n:type:ShaderForge.SFN_Multiply,id:2014,x:32239,y:31690,varname:node_2014,prsc:2|A-9176-OUT,B-3704-OUT;n:type:ShaderForge.SFN_RemapRange,id:9176,x:32196,y:31453,varname:node_9176,prsc:2,frmn:0,frmx:1,tomn:1,tomx:0|IN-4472-OUT;n:type:ShaderForge.SFN_Add,id:7282,x:32494,y:31513,varname:node_7282,prsc:2|A-9176-OUT,B-2014-OUT;n:type:ShaderForge.SFN_Clamp01,id:5992,x:32494,y:31653,varname:node_5992,prsc:2|IN-7282-OUT;n:type:ShaderForge.SFN_Tex2d,id:4482,x:30026,y:31967,ptovrint:False,ptlb:Lighting Ramp,ptin:_LightingRamp,varname:node_4482,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,ntxv:0,isnm:False|UVIN-8043-OUT;n:type:ShaderForge.SFN_Append,id:8043,x:29736,y:32167,varname:node_8043,prsc:2|A-6355-OUT,B-6672-OUT;n:type:ShaderForge.SFN_Vector1,id:6672,x:29504,y:32302,varname:node_6672,prsc:2,v1:0;n:type:ShaderForge.SFN_Vector1,id:8378,x:30950,y:32171,varname:node_8378,prsc:2,v1:1;n:type:ShaderForge.SFN_Vector1,id:6539,x:31635,y:32467,varname:node_6539,prsc:2,v1:0;n:type:ShaderForge.SFN_Multiply,id:6379,x:32313,y:32367,varname:node_6379,prsc:2|A-1525-OUT,B-7893-OUT,C-4848-RGB;n:type:ShaderForge.SFN_AmbientLight,id:4848,x:32198,y:32549,varname:node_4848,prsc:2;n:type:ShaderForge.SFN_Add,id:9313,x:32374,y:31919,varname:node_9313,prsc:2|A-6316-OUT,B-6379-OUT;proporder:4482-7241-1159-4472-233-9922-5243-6941-9940-2871-5873-7134-2802-6565;pass:END;sub:END;*/

Shader "DCG/Toon Shaders/Toon Custom Ramp" {
    Properties {
        _LightingRamp ("Lighting Ramp", 2D) = "white" {}
        _Color ("Color", Color) = (0.7058823,0.7058823,0.7058823,1)
        _Diffuse ("Diffuse", 2D) = "white" {}
        [HideInInspector]_CutOut ("CutOut", Range(0, 1)) = 0
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
            uniform sampler2D _LightingRamp; uniform float4 _LightingRamp_ST;
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
                float node_6355 = max(0,dot(lightDirection,normalDirection));
                float2 node_8043 = float2(node_6355,0.0);
                float4 _LightingRamp_var = tex2D(_LightingRamp,TRANSFORM_TEX(node_8043, _LightingRamp));
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float3 node_3674 = (_Diffuse_var.rgb*_Color.rgb);
                float3 node_1525 = ((1.0*((_LightingRamp_var.rgb*node_3674)+saturate((smoothstep( 0.0, 0.025, (pow(node_6355,exp((_Gloss*10.75+0.25)))*2.0+-1.0) )*_GlossIntensity))))+0.0);
                float node_9675 = 1.0;
                float4 _OcclussionMap_var = tex2D(_OcclussionMap,TRANSFORM_TEX(i.uv0, _OcclussionMap));
                float3 node_7893 = lerp(float3(node_9675,node_9675,node_9675),_OcclussionMap_var.rgb,_OcclussionIntensity);
                float3 emissive = ((_EmissionMap_var.rgb*_HDREmissionColor.rgb)+(node_1525*node_7893*UNITY_LIGHTMODEL_AMBIENT.rgb));
                float3 finalColor = emissive + (node_1525*node_7893*(_LightColor0.rgb*attenuation));
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
            uniform sampler2D _LightingRamp; uniform float4 _LightingRamp_ST;
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
                float node_6355 = max(0,dot(lightDirection,normalDirection));
                float2 node_8043 = float2(node_6355,0.0);
                float4 _LightingRamp_var = tex2D(_LightingRamp,TRANSFORM_TEX(node_8043, _LightingRamp));
                float4 _Diffuse_var = tex2D(_Diffuse,TRANSFORM_TEX(i.uv0, _Diffuse));
                float3 node_3674 = (_Diffuse_var.rgb*_Color.rgb);
                float3 node_1525 = ((1.0*((_LightingRamp_var.rgb*node_3674)+saturate((smoothstep( 0.0, 0.025, (pow(node_6355,exp((_Gloss*10.75+0.25)))*2.0+-1.0) )*_GlossIntensity))))+0.0);
                float node_9675 = 1.0;
                float4 _OcclussionMap_var = tex2D(_OcclussionMap,TRANSFORM_TEX(i.uv0, _OcclussionMap));
                float3 node_7893 = lerp(float3(node_9675,node_9675,node_9675),_OcclussionMap_var.rgb,_OcclussionIntensity);
                float3 finalColor = (node_1525*node_7893*(_LightColor0.rgb*attenuation));
                return fixed4(finalColor * 1,0);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
