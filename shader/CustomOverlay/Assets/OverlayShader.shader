Shader "Unlit/OverlayShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _CircleCount("Circle Count", Int) = 1
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
            uniform float4 _CircleFill[20]; // (startDegree, endDegree, fillPercent, direction (1 = clockwise, 0 = counterClockwise))
            uniform float4 _CircleColors[20]; // (r, g, b, alpha)

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

                        // if (angle < circleFill.y){
                        //     finalColor = _CircleColors[j];
                        // }

                        // if (circleFill.w == 1){
                        if (circleFill.x < circleFill.y){
                            float fillDegree = circleFill.x + circleFill.z * (circleFill.y - circleFill.x);

                            if (circleFill.x <= angle && angle <= fillDegree) {
                                finalColor = _CircleColors[j];
                            }
                        }
                        //     else {
                        //         if (circleFill.x < angle){
                        //             finalColor = _CircleColors[j];
                        //         }
                        //         else{
                        //             float fillDegree = (1 - circleFill.x + circleFill.z * (circleFill.y + 1 - circleFill.x) % 1);
                        //             if (angle < fillDegree){
                        //                 finalColor = _CircleColors[j];
                        //             }
                        //         }
                        //     }
                        // }


                        // Not working edge wrapping

                        // if (circleFill.w == 1){
                        //     if (circleFill.x < circleFill.y){
                        //         float fillDegree = circleFill.x + circleFill.z * (circleFill.y - circleFill.x);

                        //         if (circleFill.x <= angle && angle <= fillDegree) {
                        //             finalColor = _CircleColors[j];
                        //         }
                        //     }
                        //     else {
                        //         float fillDegree = 1 - circleFill.x + circleFill.z * (circleFill.y + 1 - circleFill.x);
                        //         if (circleFill.x < angle && angle < fillDegree){
                        //             finalColor = _CircleColors[j];
                        //         }
                        //         else{
                        //             if (angle < fmod(fillDegree, 1)){
                        //                 finalColor = _CircleColors[j];
                        //             }
                        //         }
                        //     }
                        // }
                        // else {
                        //     if (circleFill.y < circleFill.x){
                        //         float fillDegree = circleFill.x - circleFill.z * (circleFill.x - circleFill.y);

                        //         if (angle <= fillDegree) {
                        //             finalColor = _CircleColors[j];
                        //         }
                        //     }
                        //     else {
                        //         float fillDegree = 1 - circleFill.y + circleFill.z * (circleFill.x + 1 - circleFill.y);
                        //         if (angle < circleFill.x && fillDegree < angle){
                        //             finalColor = _CircleColors[j];
                        //         }
                        //         else{
                        //             if (angle > fmod(fillDegree, 1)){
                        //                 finalColor = _CircleColors[j];
                        //             }
                        //         }
                        //     }
                        // }
                    }
                }
                return tex2D(_MainTex, i.uv) * finalColor;
            }
            ENDCG
        }
    }
}