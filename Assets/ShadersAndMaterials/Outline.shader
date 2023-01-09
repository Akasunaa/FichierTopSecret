Shader "Unlit/Outline"
{
	Properties{
		_MainTex("Texture",2D) = "white" {}
		_Color("First Color",Color) = (1,1,1,1)
		_OutlineSize("Outline Size",Int) = 1
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
			o.pos = UnityObjectToClipPos(v.vertex * 1.5f);
			o.uv = (v.texcoord * 1.5f - 0.25f);
			return o;
		}

		fixed4 _Color;
		fixed4 _SecondColor;
		int _OutlineSize;
		float4 _MainTex_TexelSize;

		//About pixel
		fixed4 fragmentFunc(v2f i) : COLOR {
			half4 c = tex2D(_MainTex, i.uv); // couleur des pixels
			half4 outlineC = _Color;
			bool check = false;
			for(int x = -_OutlineSize; x < _OutlineSize + 1; x++)
			{
				for (int y = -_OutlineSize; y < _OutlineSize + 1; y++)
				{
					if (x == 0 && y == 0)
					{
						continue;
					}
					float2 pos = float2(i.uv.x + _MainTex_TexelSize.x * x, i.uv.y + _MainTex_TexelSize.y * y);
					if (pos.x <= 1 && pos.x >= 0 && pos.y <= 1 && pos.y >= 0)
					{
						if (tex2D(_MainTex, pos).a != 0)
						{
							check = true;
						}
					}
				}
			}

			if ((c.a == 0 || i.uv.x > 1 || i.uv.x < 0 || i.uv.y > 1 || i.uv.y < 0) && check == true)
			{
				return outlineC;
			}
			
			return c * c.a * (i.uv.x >= 0 && i.uv.x <= 1) * (i.uv.y >= 0 && i.uv.y <= 1);
		}
			
		ENDCG
		}
	}
}
