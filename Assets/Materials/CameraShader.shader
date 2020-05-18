Shader "Shaders101/Basic"
{
    Properties 
    {
        _MainTex("Texture", 2D) = "white" {}
        _DisplaceTex("Displacement Texture", 2D) = "white" {}
        _Magnitude("Magnitude", Range(0, 0.1)) = 0.1
        _AnimationOffset("Animation Offset", Range(0, 1)) = 0
    }
	SubShader
	{
		Tags
		{
			"PreviewType" = "Plane"
		}
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
				return o;
			}
            
            sampler2D _MainTex;
            sampler2D _DisplaceTex;
            float _Magnitude;
            float _AnimationOffset;

			float4 frag(v2f i) : SV_Target
			{
                float2 disp = tex2D(_DisplaceTex, float2(i.uv.x / 2, i.uv.y) + _Time.x).xy;
                disp = ((disp * 2) - 1) * _Magnitude * _AnimationOffset;
                float4 color = tex2D(_MainTex, i.uv + disp);
                color *= float4(1, 1, 1, 1) - (float4(0.3, 0.1, 0, 0) * _AnimationOffset);
				return color;
			}
			ENDCG
		}
	}
}