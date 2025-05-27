Shader "URP/Particles/BloodEffectURP"
{
    Properties
    {
        [HDR] _BaseColor("Base Color Mult", Color) = (1,1,1,1)
        _MainTex("Mask Texture", 2D) = "white" {}
        _NoiseTex("Noise Texture", 2D) = "white" {}
        _ChannelMask("Channel Mask", Vector) = (1,0,0,0)
        _ChannelMask2("Channel Mask 2", Vector) = (1,0,0,0)
        _AlphaMin("Alpha Clip Min", Range(-0.01, 1.01)) = 0.1
        _AlphaSoft("Alpha Clip Softness", Range(0,1)) = 0.022
        _ProcMask("Procedural Mask Strength", float) = 1.0
        _Columns("Flipbook Columns", Int) = 1
        _Rows("Flipbook Rows", Int) = 1
        _NoiseAlphaStr("Noise Strength", float) = 0.8
        [Toggle] _FlipU("Flip U Randomly", float) = 0
        [Toggle] _FlipV("Flip V Randomly", float) = 0
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 100
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            Name "BloodEffect"
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
                float3 customData : TEXCOORD1; // X = Pan Offset, Y = Warp Strength, Z = Random
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 uvNoise : TEXCOORD1;
                float4 color : COLOR;
                float3 customData : TEXCOORD2;
            };

            TEXTURE2D(_MainTex); SAMPLER(sampler_MainTex);
            TEXTURE2D(_NoiseTex); SAMPLER(sampler_NoiseTex);

            float4 _BaseColor;
            float4 _MainTex_ST;
            float4 _NoiseTex_ST;
            float4 _ChannelMask;
            float4 _ChannelMask2;
            float _AlphaMin;
            float _AlphaSoft;
            float _ProcMask;
            float _Columns;
            float _Rows;
            float _NoiseAlphaStr;
            float _FlipU;
            float _FlipV;

            Varyings vert(Attributes IN)
            {
                Varyings OUT;

                float2 uvFlip = round(frac(float2(IN.customData.z * 13, IN.customData.z * 8))) * 2 - 1;
                uvFlip = lerp(float2(1,1), uvFlip, float2(_FlipU, _FlipV));

                float2 uv = IN.uv * uvFlip;
                float2 uvNoise = uv * float2(_Columns, _Rows) + IN.customData.z * float2(3,8);

                OUT.uv = TRANSFORM_TEX(uv, _MainTex);
                OUT.uvNoise = TRANSFORM_TEX(uvNoise, _NoiseTex);
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.color = IN.color;
                OUT.customData = IN.customData;

                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {
                float4 baseColor = _BaseColor * IN.color;
                float4 mask = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);
                float alphaMask = saturate(dot(mask, _ChannelMask));

                float2 tempUV = frac(IN.uv * float2(_Columns, _Rows)) - 0.5;
                tempUV *= tempUV * 4;
                float edgeMask = 1.0 - saturate(tempUV.x + tempUV.y);
                edgeMask = saturate(lerp(1.0, edgeMask * edgeMask, _ProcMask));

                alphaMask *= edgeMask;

                float noiseSample = dot(SAMPLE_TEXTURE2D(_NoiseTex, sampler_NoiseTex, IN.uvNoise), _ChannelMask2);
                float noise = saturate(lerp(1, noiseSample, _NoiseAlphaStr));
                
                float preClipAlpha = alphaMask * noise * baseColor.a;
                float clippedAlpha = saturate((preClipAlpha - _AlphaMin) / _AlphaSoft);

                half4 finalColor = baseColor;
                finalColor.a = clippedAlpha;
                finalColor.rgb *= preClipAlpha;

                return finalColor;
            }
            ENDHLSL
        }
    }
}
