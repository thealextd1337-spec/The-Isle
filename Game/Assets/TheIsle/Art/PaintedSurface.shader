Shader "TheIsle/Painted Surface"
{
    Properties
    {
        _BaseMap("Brush texture", 2D) = "white" {}
        _BaseColor("Pigment", Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Tags { "LightMode"="UniversalForward" }
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            CBUFFER_START(UnityPerMaterial)
            float4 _BaseColor;
            float4 _BaseMap_ST;
            CBUFFER_END
            struct Attributes { float4 positionOS:POSITION; float3 normalOS:NORMAL; float2 uv:TEXCOORD0; };
            struct Varyings { float4 positionCS:SV_POSITION; float3 normalWS:TEXCOORD0; float2 uv:TEXCOORD1; };
            Varyings vert(Attributes a)
            {
                Varyings v; v.positionCS=TransformObjectToHClip(a.positionOS.xyz);
                v.normalWS=TransformObjectToWorldNormal(a.normalOS); v.uv=TRANSFORM_TEX(a.uv,_BaseMap); return v;
            }
            half4 frag(Varyings v):SV_Target
            {
                Light light=GetMainLight();
                float diffuse=dot(normalize(v.normalWS),light.direction)*.5+.5;
                diffuse=smoothstep(.08,.94,diffuse);
                half3 shade=lerp(half3(.45,.57,.62),half3(1.06,1.01,.86),diffuse);
                half3 pigment=SAMPLE_TEXTURE2D(_BaseMap,sampler_BaseMap,v.uv).rgb*_BaseColor.rgb;
                return half4(pigment*shade,1);
            }
            ENDHLSL
        }
    }
}
