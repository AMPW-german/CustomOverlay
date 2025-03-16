using System;
using System.Collections.Generic;
using UnityEngine;

namespace CustomOverlay
{
    public class CircleSymetrie
    {
        List<Circle> circles;

        public float Size;
        public float EmptySize;

        public void Enable()
        {
            circles.ForEach(circle => circle.size = new Vector2(Size, 0));
        }

        public void Disable()
        {
            circles.ForEach(circle => circle.size = new Vector2(Size, EmptySize));
        }

        public CircleSymetrie(CustomUI instance, int count, Vector2 midPoint, float Size, float EmptySize, float startDegreeOffset, Vector4 color)
        {
            circles = new List<Circle>();

            int degreeOffset = 360 / count;

            for (int i = 0; i < count; i++)
            {
                Vector2 circlePoint = new Vector2((float)(Size * Math.Sin((degreeOffset * i + startDegreeOffset) * 0.01745329252f) * Settings.OverlaySize.y / Settings.OverlaySize.x) + midPoint.x, (float)(Size * Math.Cos((degreeOffset * i + startDegreeOffset) * 0.01745329252f)) + midPoint.y);
                circles.Add(new Circle(circlePoint, new Vector2(Size, 0), color, 0, 360, 360));
            }

            circles.ForEach(circle => instance.circles.Add(circle));

            this.Size = Size;
            this.EmptySize = EmptySize;
        }
    }
}
