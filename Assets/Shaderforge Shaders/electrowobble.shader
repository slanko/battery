// Shader created with Shader Forge v1.40 
// Shader Forge (c) Freya Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:1.40;sub:START;pass:START;ps:flbk:,iptp:0,cusa:False,bamd:0,cgin:,cpap:True,lico:0,lgpr:1,limd:1,spmd:1,trmd:0,grmd:0,uamb:False,mssp:True,bkdf:False,hqlp:False,rprd:False,enco:False,rmgx:True,imps:True,rpth:0,vtps:0,hqsc:True,nrmq:1,nrsp:0,vomd:0,spxs:False,tesm:0,olmd:1,culm:0,bsrc:3,bdst:7,dpts:2,wrdp:False,dith:0,atcv:False,rfrpo:True,rfrpn:Refraction,coma:15,ufog:True,aust:True,igpj:True,qofs:0,qpre:3,rntp:2,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,stcl:False,atwp:False,stva:128,stmr:255,stmw:255,stcp:6,stps:0,stfa:0,stfz:0,ofsf:0,ofsu:0,f2p0:False,fnsp:True,fnfb:False,fsmp:False;n:type:ShaderForge.SFN_Final,id:4013,x:32719,y:32712,varname:node_4013,prsc:2|diff-1304-RGB,emission-7403-RGB,alpha-9018-OUT,voffset-1539-OUT;n:type:ShaderForge.SFN_Color,id:1304,x:32133,y:32768,ptovrint:False,ptlb:Color,ptin:_Color,varname:node_1304,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_Time,id:1794,x:31605,y:32922,varname:node_1794,prsc:2;n:type:ShaderForge.SFN_NormalVector,id:955,x:32000,y:33221,prsc:2,pt:False;n:type:ShaderForge.SFN_Multiply,id:1955,x:32337,y:33007,varname:node_1955,prsc:2|A-2882-OUT,B-955-OUT,C-3569-OUT;n:type:ShaderForge.SFN_ValueProperty,id:3569,x:32000,y:33169,ptovrint:False,ptlb:Amount,ptin:_Amount,varname:node_3569,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.15;n:type:ShaderForge.SFN_ValueProperty,id:1588,x:31429,y:33208,ptovrint:False,ptlb:Speed,ptin:_Speed,varname:node_1588,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:1;n:type:ShaderForge.SFN_Multiply,id:5998,x:31785,y:32940,varname:node_5998,prsc:2|A-1794-T,B-2677-OUT;n:type:ShaderForge.SFN_Noise,id:2882,x:32133,y:32934,varname:node_2882,prsc:2|XY-2740-UVOUT;n:type:ShaderForge.SFN_Panner,id:2740,x:31967,y:32934,varname:node_2740,prsc:2,spu:1,spv:1|UVIN-6875-UVOUT,DIST-5998-OUT;n:type:ShaderForge.SFN_TexCoord,id:6875,x:31785,y:32794,varname:node_6875,prsc:2,uv:0,uaff:False;n:type:ShaderForge.SFN_ValueProperty,id:9018,x:32357,y:32886,ptovrint:False,ptlb:Opacity,ptin:_Opacity,varname:node_9018,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0.1;n:type:ShaderForge.SFN_Color,id:7403,x:32133,y:32595,ptovrint:False,ptlb:Emission,ptin:_Emission,varname:node_7403,prsc:2,glob:False,taghide:False,taghdr:True,tagprd:False,tagnsco:False,tagnrm:False,c1:1,c2:1,c3:1,c4:1;n:type:ShaderForge.SFN_ValueProperty,id:8546,x:32306,y:33201,ptovrint:False,ptlb:Offset,ptin:_Offset,varname:node_8546,prsc:2,glob:False,taghide:False,taghdr:False,tagprd:False,tagnsco:False,tagnrm:False,v1:0;n:type:ShaderForge.SFN_Multiply,id:8477,x:32306,y:33250,varname:node_8477,prsc:2|A-8546-OUT,B-955-OUT;n:type:ShaderForge.SFN_Add,id:1539,x:32539,y:33073,varname:node_1539,prsc:2|A-1955-OUT,B-8477-OUT;n:type:ShaderForge.SFN_Multiply,id:2677,x:31681,y:33126,varname:node_2677,prsc:2|A-1588-OUT,B-7217-OUT;n:type:ShaderForge.SFN_Vector1,id:7217,x:31429,y:33268,varname:node_7217,prsc:2,v1:0.001;proporder:1304-3569-1588-9018-7403-8546;pass:END;sub:END;*/

Shader "Shader Forge/funky" {
    Properties {
        [HDR]_Color ("Color", Color) = (1,1,1,1)
        _Amount ("Amount", Float ) = 0.15
        _Speed ("Speed", Float ) = 1
        _Opacity ("Opacity", Float ) = 0.1
        [HDR]_Emission ("Emission", Color) = (1,1,1,1)
        _Offset ("Offset", Float ) = 0
        [HideInInspector]_Cutoff ("Alpha cutoff", Range(0,1)) = 0.5
    }
    SubShader {
        Tags {
            "IgnoreProjector"="True"
            "Queue"="Transparent"
            "RenderType"="Transparent"
        }
        Pass {
            Name "FORWARD"
            Tags {
                "LightMode"="ForwardBase"
            }
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase
            #pragma multi_compile_fog
            #pragma target 3.0
            uniform float4 _LightColor0;
            UNITY_INSTANCING_BUFFER_START( Props )
                UNITY_DEFINE_INSTANCED_PROP( float4, _Color)
                UNITY_DEFINE_INSTANCED_PROP( float, _Amount)
                UNITY_DEFINE_INSTANCED_PROP( float, _Speed)
                UNITY_DEFINE_INSTANCED_PROP( float, _Opacity)
                UNITY_DEFINE_INSTANCED_PROP( float4, _Emission)
                UNITY_DEFINE_INSTANCED_PROP( float, _Offset)
            UNITY_INSTANCING_BUFFER_END( Props )
            struct VertexInput {
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 texcoord0 : TEXCOORD0;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                UNITY_VERTEX_INPUT_INSTANCE_ID
                float2 uv0 : TEXCOORD0;
                float4 posWorld : TEXCOORD1;
                float3 normalDir : TEXCOORD2;
                UNITY_FOG_COORDS(3)
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o = (VertexOutput)0;
                UNITY_SETUP_INSTANCE_ID( v );
                UNITY_TRANSFER_INSTANCE_ID( v, o );
                o.uv0 = v.texcoord0;
                o.normalDir = UnityObjectToWorldNormal(v.normal);
                float4 node_1794 = _Time;
                float _Speed_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Speed );
                float2 node_2740 = (o.uv0+(node_1794.g*(_Speed_var*0.001))*float2(1,1));
                float2 node_2882_skew = node_2740 + 0.2127+node_2740.x*0.3713*node_2740.y;
                float2 node_2882_rnd = 4.789*sin(489.123*(node_2882_skew));
                float node_2882 = frac(node_2882_rnd.x*node_2882_rnd.y*(1+node_2882_skew.x));
                float _Amount_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Amount );
                float3 node_1955 = (node_2882*v.normal*_Amount_var);
                float _Offset_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Offset );
                v.vertex.xyz += (node_1955+(_Offset_var*v.normal));
                o.posWorld = mul(unity_ObjectToWorld, v.vertex);
                float3 lightColor = _LightColor0.rgb;
                o.pos = UnityObjectToClipPos( v.vertex );
                UNITY_TRANSFER_FOG(o,o.pos);
                return o;
            }
            float4 frag(VertexOutput i) : COLOR {
                UNITY_SETUP_INSTANCE_ID( i );
                i.normalDir = normalize(i.normalDir);
                float3 normalDirection = i.normalDir;
                float3 lightDirection = normalize(_WorldSpaceLightPos0.xyz);
                float3 lightColor = _LightColor0.rgb;
////// Lighting:
                float attenuation = 1;
                float3 attenColor = attenuation * _LightColor0.xyz;
/////// Diffuse:
                float NdotL = max(0.0,dot( normalDirection, lightDirection ));
                float3 directDiffuse = max( 0.0, NdotL) * attenColor;
                float4 _Color_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Color );
                float3 diffuseColor = _Color_var.rgb;
                float3 diffuse = directDiffuse * diffuseColor;
////// Emissive:
                float4 _Emission_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Emission );
                float3 emissive = _Emission_var.rgb;
/// Final Color:
                float3 finalColor = diffuse + emissive;
                float _Opacity_var = UNITY_ACCESS_INSTANCED_PROP( Props, _Opacity );
                fixed4 finalRGBA = fixed4(finalColor,_Opacity_var);
                UNITY_APPLY_FOG(i.fogCoord, finalRGBA);
                return finalRGBA;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
