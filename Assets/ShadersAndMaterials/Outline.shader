Shader "Unlit/Outline"
{
	Properties{
		_MainTex("Texture",2D) = "white" {}
		_Color("First Color",Color) = (1,1,1,1)
		_SecondColor("Second Color",Color) = (1,1,1,1)
	}
	SubShader{
		Tags
		{
			"Queue" = "Transparent"
			"IgnoreProjector" = "True"
			"RenderType" = "Transparent"
			"PreviewType" = "Plane"
			"CanUseSpriteAtlas" = "True"
		}
		Cull Off
		Lighting Off
		ZWrite Off
		Blend One OneMinusSrcAlpha

		Pass{
		CGPROGRAM
		#pragma vertex vertexFunc
		#pragma fragment fragmentFunc
		#pragma target 4.0
		#include "UnityCG.cginc"

		sampler2D _MainTex;
		struct v2f {
			float4 pos : SV_POSITION;
			half2 uv : TEXCOORD0; //half est un equivalent de float
		};

		//About sommet
		v2f vertexFunc(appdata_base v) {
			v2f o;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord;
			return o;
		}

		fixed4 _Color;
		fixed4 _SecondColor;
		float4 _MainTex_TexelSize;

		//About pixel
		fixed4 fragmentFunc(v2f i) : COLOR{
			half4 c = tex2D(_MainTex, i.uv); //couleur des pixel 
			c.rgb *= c.a; //rendre transparente toute partie de la texture qui n'est pas opaque 
			half4 outlineC = _Color;
			half4 secondOutlineC = _SecondColor; 


			//rendre transparente toute partie de l'ombrage qui n'est pas opaque
			outlineC.a *= ceil(c.a);
			outlineC.rgb *= outlineC.a;


			//valeurs alpha des pixels de la texture situés au-dessus, en dessous, à droite et à gauche du pixel en cours de traitement
			fixed upAlpha = tex2D(_MainTex, i.uv + fixed2(0, _MainTex_TexelSize.y)).a;
			fixed downAlpha = tex2D(_MainTex, i.uv - fixed2(0, _MainTex_TexelSize.y)).a;
			fixed rightAlpha = tex2D(_MainTex, i.uv + fixed2(_MainTex_TexelSize.x, 0)).a;
			fixed leftAlpha = tex2D(_MainTex, i.uv - fixed2(_MainTex_TexelSize.x, 0)).a;
			fixed edgeX = (1 - floor(i.uv.r + _MainTex_TexelSize.x)) * ceil(i.uv.r - _MainTex_TexelSize.x);
			fixed edgeY = (1 - floor(i.uv.g + _MainTex_TexelSize.y)) * ceil(i.uv.g - _MainTex_TexelSize.y);

			return lerp(outlineC, c, ceil(upAlpha * downAlpha * rightAlpha * leftAlpha * edgeX * edgeY ));

		}
			
		ENDCG
		}
	}
}
