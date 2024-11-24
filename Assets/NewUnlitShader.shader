Shader "Unlit/Color"
{
    Properties
    {
        _Color ("Main Color", Color) = (0.5, 0.5, 1, 1) // Цвет без прозрачности
    }
    SubShader
    {
        Tags { "Queue"="Geometry" "RenderType"="Opaque" } // Устанавливаем очередь "Geometry" для непрозрачных объектов
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                float4 pos : POSITION;
            };

            fixed4 _Color;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                return _Color; // Используем заданный цвет
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
