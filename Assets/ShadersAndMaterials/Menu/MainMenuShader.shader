Shader "Unlit/MainMenuShader"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Main Color", Color) = (1,1,1,1)

//        _DetailTex ("Detail (RGB)", 2D) = "white" {}
//        _Strength ("Detail Strength", Range(0.0, 1.0)) = 0.2

//        _StencilComp ("Stencil Comparison", Float) = 8
//        _Stencil ("Stencil ID", Float) = 0
//        _StencilOp ("Stencil Operation", Float) = 0
//        _StencilWriteMask ("Stencil Write Mask", Float) = 255
//        _StencilReadMask ("Stencil Read Mask", Float) = 255

//        _ColorMask ("Color Mask", Float) = 15

//        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
//        LOD 100

        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType"="Plane"
        }

//        Stencil
//        {
//            Ref [_Stencil]
//            Comp [_StencilComp]
//            Pass [_StencilOp]
//            ReadMask [_StencilReadMask]
//            WriteMask [_StencilWriteMask]
//        }

        Cull Off
        Lighting Off
        ZWrite Off
//        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
//        ColorMask [_ColorMask]

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            // #include "UnityUI.cginc"

            // #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            // #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            // struct appdata_t
            // {
            //     float4 vertex : POSITION;
            //     float2 texcoord : TEXCOORD0;
            //     float2 texcoord2 : TEXCOORD1;
            //     fixed4 color : COLOR;
            //     UNITY_VERTEX_INPUT_INSTANCE_ID
            // };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 texcoord : TEXCOORD0;
                // float2 texcoord2 : TEXCOORD1;
                // float4 worldPosition : TEXCOORD2;
                fixed4 color : COLOR;
                // UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            // sampler2D _DetailTex;
            float4 _MainTex_ST;
            // float4 _DetailTex_ST;
            // float4 _DetailTex_TexelSize;
            fixed4 _Color;
            uniform int _state = 0;
            // fixed _Strength;

            // fixed4 _TextureSampleAdd;

            // bool _UseClipRect;
            // float4 _ClipRect;

            // bool _UseAlphaClip;

            v2f vert (appdata_full v)
            {
                v2f o;
                // o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(v.vertex);

                o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color * _Color;

                return o;
            }

            fixed4 frag (v2f i) : COLOR
            {
                fixed4 color = tex2D(_MainTex, i.texcoord) * i.color;
                switch (_state)
                {
                    case 1:
                        if (color.a == 0)
                        {
                            return float4(1, 1, 1, 0.1);
                        }
                        return color;
                    case 2:
                        if (color.a == 0)
                        {
                            return float4(1, 1, 1, 0.3);
                        }
                        return color;
                }
                return color;
            }
            ENDCG

        }
    }
}
