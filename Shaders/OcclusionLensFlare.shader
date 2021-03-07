// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

//
// Occlusion Lens Flare for Unity. Copyright (c) 2021 Peter Schraut (www.console-dev.de). See LICENSE.md
// https://github.com/pschraut/UnityOcclusionLensFlare
//

Shader "Hidden/Occlusion Lens Flare"
{
    SubShader {

        Tags {"RenderType"="Overlay"}
        ZWrite Off ZTest Always
        Cull Off
        Blend One One
        //ColorMask RGB // according to unity docs, this can be bad on mobile gpu's https://docs.unity3d.com/Manual/SL-ShaderPerformance.html

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0

            #include "UnityCG.cginc"

            sampler2D _FlareTexture;
            float4 _FlareTexture_ST;
            sampler2D _FlareOcclusionTexture;

            struct appdata_t {
                float4 vertex : POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f {
                float4 vertex : SV_POSITION;
                fixed4 color : COLOR;
                float2 texcoord : TEXCOORD0;
                UNITY_VERTEX_OUTPUT_STEREO
            };


            v2f vert (appdata_t v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.texcoord = TRANSFORM_TEX(v.texcoord, _FlareTexture);
                o.color = v.color;

                // sample lowest mipmap from occlusion buffer
                o.color *= tex2Dlod(_FlareOcclusionTexture, float4(0.5, 0.5, 0, 16)).r;
                o.color.a = 1;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return tex2D(_FlareTexture, i.texcoord) * i.color;
            }
            ENDCG
        }
    }
}
