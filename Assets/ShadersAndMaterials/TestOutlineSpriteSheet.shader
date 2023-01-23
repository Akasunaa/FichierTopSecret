Shader "Unlit/TestOutlineSpriteSheet"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("First Color",Color) = (1,1,1,1)
    	_SecondColor("Second Color",Color) = (1,1,1,1)
        _OutlineSize("Outline Size",Int) = 1
    }
    SubShader
    {
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

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            float4 _Color;
            float4 _SecondColor;
            float _OutlineSize;

            uniform int _numberOfSprite = 0;
            #define FACTOR 1.5f

            sampler2D _MainTex;
	        float4 _MainTex_ST;
	        float4 _MainTex_TexelSize;

            struct v2f
            {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 vert : TEXCOORD1;
            };

            v2f vert (appdata_base v)
            {
                v2f o;
            	if (_numberOfSprite > 0)
            	{
            		o.pos = UnityObjectToClipPos(v.vertex * float4(FACTOR, FACTOR, 1, 1));
            	}
            	else
            	{
            		o.pos = UnityObjectToClipPos(v.vertex);
            	}
                o.vert = v.vertex;
                o.uv = v.texcoord;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                if (_numberOfSprite > 0)
                {
                	i.uv.y = i.uv.y * FACTOR - (FACTOR - 1) / 2;
                    int nSprite = (int) ceil(i.uv.x * (float) _numberOfSprite) - 1;
                    float a = (float) nSprite / (float) _numberOfSprite;
                    float b = _numberOfSprite;
                    i.uv.x -= a;
                    i.uv.x *= b;
                    i.uv.x *= FACTOR;
                	float2 centeredUv = float2(i.uv.x - (FACTOR - 1) / 2 , i.uv.y);
                    i.uv.x /= b;
                    i.uv.x += a;
                    i.uv.x -= (FACTOR - 1) / 2 / b;
                    fixed4 col = tex2D(_MainTex, i.uv);

                    bool check = false;
					bool last = false;
					for(int x = -_OutlineSize; x < _OutlineSize + 1; x++)
					{
						for (int y = -_OutlineSize; y < _OutlineSize + 1; y++)
						{
							if (x == 0 && y == 0)
							{
								continue;
							}
							float2 pos = float2(i.uv.x + _MainTex_TexelSize.x * x, i.uv.y + _MainTex_TexelSize.y * y);
							float a = ((float)(nSprite + 1)) / ((float) _numberOfSprite);
							float b = ((float)(nSprite)) / ((float) _numberOfSprite);
							if (pos.x <= a && pos.x >= b && pos.y <= 1 && pos.y >= 0)
							{
								if (tex2D(_MainTex, pos).a != 0)
								{
									check = true;
									if (abs(y) < _OutlineSize && abs(x) < _OutlineSize) {
										last = true;
									}
								}
							}
						}
					}

                	// bool inSprite = (i.vert.x >= - (1 - (FACTOR - 1) / 2) && i.vert.x <= 1 - (FACTOR - 1) / 2);
                	if ((col.a == 0 || i.uv.x > 1 || i.uv.x < 0 || i.uv.y > 1 || i.uv.y < 0) && check == true)
					{
                		if (last == false) {
							return _SecondColor; // * (i.vert.x >= - (1 - (FACTOR - 1) / 2) && i.vert.x <= 1 - (FACTOR - 1) / 2);
						}	
						return _Color; // * (i.vert.x >= - (1 - (FACTOR - 1) / 2) && i.vert.x <= 1 - (FACTOR - 1) / 2);
					}

                    return col * col.a * (centeredUv.x >= 0 && centeredUv.x <= 1) * (i.uv.y >= 0 && i.uv.y <= 1);
                }

            	fixed4 col = tex2D(_MainTex, i.uv);
                return col * col.a;
            }
            ENDCG
        }
    }
}
