Shader "UI/SquareColorTransition"
{
    Properties
    {
        _MainTex("Dummy Texture", 2D) = "white" {} 
        _BaseColor("Base Color", Color) = (1,1,1,1)
        _NewColor("New Color", Color) = (1,1,1,1)
        _Progress("Transition Progress", Range(0, 1)) = 0
        _Softness("Edge Softness", Range(0, 0.5)) = 0.1
        [Toggle] _Reverse("Reverse Direction", Float) = 0
    }

        SubShader
    {
        Tags
        {
            "Queue" = "Transparent"
            "IgnoreProjector" = "True"
            "RenderType" = "Transparent"
            "PreviewType" = "Plane"
        }

        Blend SrcAlpha OneMinusSrcAlpha
        Cull Off
        ZWrite Off

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
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            float4 _BaseColor;
            float4 _NewColor;
            float _Progress;
            float _Softness;
            float _Reverse;

            v2f vert(appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                // Calculate square distance from center
                float2 center = float2(0.5, 0.5);
                float2 direction = abs(i.uv - center) * 2; // Range 0-1
                float squareDistance = max(direction.x, direction.y);

                // Adjust progress based on reverse toggle
                float adjustedProgress = _Reverse ? (1 - _Progress) : _Progress;

                // Create smooth transition with square shape
                float transition = smoothstep(
                    adjustedProgress - _Softness,
                    adjustedProgress + _Softness,
                    squareDistance
                );

                // Mix colors based on transition
                fixed4 finalColor = lerp(_NewColor, _BaseColor, transition);
                finalColor.a = lerp(_NewColor.a, _BaseColor.a, transition);

                return finalColor;
            }
            ENDCG
        }
    }
}