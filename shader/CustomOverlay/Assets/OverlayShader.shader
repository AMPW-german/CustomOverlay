Shader "Unlit/OverlayShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CircleCount("Circle Count", Int) = 1
        _RectangleCount("Rectangle Count", Int) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Overlay" }
        Pass
        {
            Cull Off
            Lighting Off
            ZWrite Off
            Blend SrcAlpha OneMinusSrcAlpha
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            uniform int _CircleCount;
            uniform float4 _Circles[20]; // (x, y, radius, innerRadius)
            uniform float4 _CircleFill[20]; // (startDegree, endDegree, fillPercent, direction (1 = clockwise, 0 = counterClockwise)) // (rotation in degree/360)
            uniform float4 _CircleColors[20]; // (r, g, b, alpha)
            uniform int _RectangleCount;
            uniform float4 _Rectangles[40]; // (x, y, width, height)
            uniform float _RectangleRotation[40]; // (rotation in radians)
            uniform float4 _RectangleColors[40]; // (r, g, b, alpha)

            uniform float _AspectRatio;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            fixed4 frag(v2f i) : SV_Target {
                float2 uv = i.uv;

                // Adjust UV for aspect ratio correction
                uv.x *= _AspectRatio;

                float4 finalColor = float4(0, 0, 0, 0); // Transparent background

                for (int j = 0; j < _CircleCount; j++) {
                    float2 center = _Circles[j].xy;
                    center.x *= _AspectRatio; // Apply aspect ratio correction to center

                    float radius = _Circles[j].z;
                    float innerRadius = _Circles[j].w;
                    float distance = length(uv - center);

                    if (innerRadius < distance && distance < radius) {
                        float4 circleFill = _CircleFill[j];
                        float angle = atan2(uv.x - center.x, uv.y - center.y) / 6.28318 + 0.5;

                        // 0° is down, 90° is left, ...
                        if (circleFill.x < circleFill.y){
                            float fillDegree = circleFill.x + circleFill.z * (circleFill.y - circleFill.x);

                            if (circleFill.x <= angle && angle <= fillDegree) {
                                finalColor = _CircleColors[j];
                                // break;
                            }
                        }
                    }
                }
                // Rectangles
                // calculate the x/y distance from the center of the rectangle to the uv
                if (all(finalColor == float4(0, 0, 0, 0))){
                    for (int j = 0; j < _RectangleCount; j++){
                        float2 center = _Rectangles[j].xy;
                        center.x *= _AspectRatio; // Apply aspect ratio correction to center
                        float2 wh = _Rectangles[j].zw; //width, height
                        wh.x *= _AspectRatio;

                        float2 uvXY = float2(uv.x - center.x, uv.y - center.y);

                        float cos_theta = cos(-_RectangleRotation[j]);
                        float sin_theta = sin(-_RectangleRotation[j]);

                        float local_x = cos_theta * uvXY.x - sin_theta * uvXY.y;
                        float local_y = sin_theta * uvXY.x + cos_theta * uvXY.y;

                        if (abs(local_x) <= wh.x / 2 && abs(local_y) <= wh.y / 2){
                            finalColor = _RectangleColors[j];
                            break;
                        }
                    }
                }
                return tex2D(_MainTex, i.uv) * finalColor;
                }
            ENDCG
        }
    }
}