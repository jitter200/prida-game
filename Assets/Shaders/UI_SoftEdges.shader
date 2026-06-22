Shader "UI/SoftEdges"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _EdgeSoftness ("Edge Softness", Range(0.001, 0.5)) = 0.08
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "RenderType"="Transparent"
            "IgnoreProjector"="True"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        Lighting Off
        ZWrite Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            sampler2D _MainTex;
            float4 _MainTex_ST;
            float _EdgeSoftness;

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;

                float left = smoothstep(0.0, _EdgeSoftness, i.uv.x);
                float right = smoothstep(0.0, _EdgeSoftness, 1.0 - i.uv.x);
                float bottom = smoothstep(0.0, _EdgeSoftness, i.uv.y);
                float top = smoothstep(0.0, _EdgeSoftness, 1.0 - i.uv.y);

                float alpha = left * right * bottom * top;

                col.a *= alpha;

                return col;
            }

            ENDCG
        }
    }
}