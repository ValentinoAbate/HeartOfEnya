// My city now. Based on Unity's default unlit shader

Shader "UI/Unlit/MulticolorTile"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        [Header(Colors and Textures)] [Space]
        [PerRendererData] _ColorTex ("Color Lookup Texture", 2D) = "white" {}
        [PerRendererData] _NumColors ("Number of Active Colors", Range(1.0,10.0)) = 1

        [Header(Animation Options)] [Space]
        _ScrollSpeed ("Scroll Speed", Range(0.0,10.0)) = 0.0

		[Header(View Options)] [Space]
		[IntRange] _Divisions ("Divisions", Range(1.0, 10.0)) = 2.0
		_Angle("Skew Angle", Range(0.0, 360.0)) = 0.0
		_IScale("Grid X Scale", Range(0.1, 10.0)) = 1.0

		[Header(Worldspace Options)] [Space]
		[Toggle] _WorldSpace("Worldspace", Range(0.0, 1.0)) = 1.0
		_WScale("Worldspace X Scale", Range(0.1, 10.0)) = 1.0
		_Offset("Worldspace Offset", Range(0.0, 10.0)) = 0.0

		[Header(Stencil Properties)] [Space]
        _StencilComp ("Stencil Comparison", Float) = 8
        _Stencil ("Stencil ID", Float) = 0
        _StencilOp ("Stencil Operation", Float) = 0
        _StencilWriteMask ("Stencil Write Mask", Float) = 255
        _StencilReadMask ("Stencil Read Mask", Float) = 255

        _ColorMask ("Color Mask", Float) = 15

        [Toggle(UNITY_UI_ALPHACLIP)] _UseUIAlphaClip ("Use Alpha Clip", Float) = 0
    }

    SubShader
    {
        LOD 100

        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType"="Plane"
        }

        Stencil
        {
            Ref [_Stencil]
            Comp [_StencilComp]
            Pass [_StencilOp]
            ReadMask [_StencilReadMask]
            WriteMask [_StencilWriteMask]
        }

        Cull Off
        Lighting Off
        ZWrite Off
        ZTest [unity_GUIZTestMode]
        Blend SrcAlpha OneMinusSrcAlpha
        ColorMask [_ColorMask]

        Pass
        {
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "UnityUI.cginc"

            #pragma multi_compile_local _ UNITY_UI_CLIP_RECT
            #pragma multi_compile_local _ UNITY_UI_ALPHACLIP

            struct appdata_t
            {
                float4 vertex : POSITION;
                float2 texcoord : TEXCOORD0;
                float2 texcoord2 : TEXCOORD1;
                fixed4 color : COLOR;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float2 moduv : TEXCOORD1;
                float4 worldPosition : TEXCOORD2;
                fixed4 color : COLOR;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            sampler2D _MainTex;
            
            float4 _MainTex_ST;

            sampler2D _ColorTex;
            float4 _ColorTex_TexelSize;
            fixed _NumActiveColors;
			
            float _ScrollSpeed;
			float _Divisions;
			float _Angle;
			float _IScale;
			float _WScale;
			float _Offset;
			bool _WorldSpace;

            fixed4 _TextureSampleAdd;

            bool _UseClipRect;
            float4 _ClipRect;

            bool _UseAlphaClip;

            v2f vert (appdata_t v)
            {
				
                v2f o;
                UNITY_SETUP_INSTANCE_ID(v);
                UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
                o.worldPosition = v.vertex;
                o.vertex = UnityObjectToClipPos(o.worldPosition);

				// set up angle
				float ws = floor(_WorldSpace);
				float ang = _Angle + (1 - ws)*135;
				ang = (360 - ang) * 3.14159265/180;
				float sint = sin(ang);
				float cost = cos(ang);

				float2 wofs = mul(UNITY_MATRIX_MV, float4(v.vertex.x, v.vertex.y, 0.0, 1.0)).xy;
				wofs.x += _Offset;

				// transformation matrices
				float3x3 rot = { cost, -sint, 0,
								 sint,  cost, 0,
								 0,     0,    1};
				float3x3 scale = { lerp(_IScale, _WScale, ws), 0.f, 0.f,
								   0.f, 1.f, 0.f,
								   0.f, 0.f, 1.f};

				float3x3 transform = mul(scale, rot);
				float3 newtex = mul(transform, float3(v.texcoord.x, v.texcoord.y, 0));
				newtex += (ws * float3(wofs.x, wofs.y, 0));

				o.moduv = float2(newtex.x, newtex.y);

                o.moduv = TRANSFORM_TEX(o.moduv, _MainTex);
                o.moduv.x += _ScrollSpeed * _Time;

                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.color = v.color;

                return o;
            }

            fixed4 frag(v2f i) : COLOR
            {
                float div =  1 / _Divisions;
                float floorMod = floor(i.moduv.x / div) % (_NumActiveColors);
                fixed4 color;
                float x = floorMod / _ColorTex_TexelSize.z + 0.5 / _ColorTex_TexelSize.z;
                color = tex2D(_ColorTex, float2(x, 0));
                //fixed4 detail;
                //detail = tex2D(_DetailTex, i.uv);
				//color.rgb = lerp(color.rgb, color.rgb * detail.rgb, detail.a * _Strength);

                #ifdef UNITY_UI_CLIP_RECT
                color.a *= UnityGet2DClipping(i.worldPosition.xy, _ClipRect);
                #endif

                #ifdef UNITY_UI_ALPHACLIP
                clip (color.a - 0.001);
                #endif

                return color;
            }
            ENDCG

        }
    }
}