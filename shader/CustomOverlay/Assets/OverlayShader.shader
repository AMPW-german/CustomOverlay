Shader "Unlit/OverlayShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CircleCount("Circle Count", Int) = 1
        _RectangleCount("Rectangle Count", Int) = 1
        _barCount("bar Count", Int) = 1
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
            uniform float4 _Circles[100]; // (x, y, radius, innerRadius)
            uniform float4 _CircleFill[100]; // (startDegree, endDegree, fillPercent, direction (1 = clockwise, 0 = counterClockwise)) // (rotation in degree/360)
            uniform float4 _CircleColors[100]; // (r, g, b, alpha)
            uniform int _RectangleCount;
            uniform float4 _Rectangles[50]; // (x, y, width, height)
            uniform float _RectangleRotation[50]; // (rotation in radians)
            uniform float4 _RectangleColors[50]; // (r, g, b, alpha)
            //horizontal bar
            uniform int _barCount;
            uniform float4 _barStartColor[40];
            uniform float4 _barEndColor[40];
            uniform float4 _barPosition[40]; //x, y, width, height
            uniform float _barRounding[40];
            uniform float _barThickness[40];

            uniform float _AspectRatio;

            v2f vert (appdata_t v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float roundedRectangle (float2 currentXY, float2 pos, float2 size, float radius, float thickness)
            {
                float d = length(max(abs(currentXY - pos),size) - size) - radius;
                return smoothstep(0.66, 0.33, d / thickness * 5.0);
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

                // Bars
                if (all(finalColor == float4(0, 0, 0, 0))){
                    for (int j = 0; j < _barCount; j++){

                        float2 uv2 = (2.0 * i.uv - 1.0) * _AspectRatio;             // -1.0 .. 1.0
                        float2 center = (2.0 * _barPosition[j].xy - 1.0) * _AspectRatio;
                        float2 size = _barPosition[j].zw * _AspectRatio;
                        float alpha = roundedRectangle (uv2, center, size, _barRounding[j], _barThickness[j]);

                        if (alpha > 0){
                            float uvPercent = (i.uv.x - (_barPosition[j].x - _barPosition[j].z / 2)) / _barPosition[j].z;

                            // finalColor.x = _barStartColor[j].x * (1- i.uv.x) + _barEndColor[j].x * i.uv.x;
                            // finalColor.y = _barStartColor[j].y * (1- i.uv.x) + _barEndColor[j].y * i.uv.x;
                            // finalColor.z = _barStartColor[j].z * (1- i.uv.x) + _barEndColor[j].z * i.uv.x;

                            finalColor.x = lerp(_barStartColor[j].x, _barEndColor[j].x, uvPercent);
                            finalColor.y = lerp(_barStartColor[j].y, _barEndColor[j].y, uvPercent);
                            finalColor.z = lerp(_barStartColor[j].z, _barEndColor[j].z, uvPercent);
                            finalColor.w = alpha;
                            break;
                            // finalColor.w = alpha;
                        }
                    }
                }

                return tex2D(_MainTex, i.uv) * finalColor;
                }
            ENDCG
        }
    }
}