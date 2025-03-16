using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomOverlay
{
    public class Circle
    {
        public Vector2 position;
        public Vector2 size;
        public Vector4 color;
        public float startDegree;
        public float endDegree;
        public float degree;

        public float start { get { return startDegree / 360; } }
        public float end { get { return endDegree / 360; } }
        public float deg { get { return degree / 360; } }

        public static Circle getEmpty()
        {
            return new Circle(Vector2.zero, Vector2.zero, Vector4.zero, 0.0f, 0.0f, 0.0f);
        }

        public Circle(Vector2 position, Vector2 size, Vector4 color, float startDegree, float endDegree, float degree)
        {
            this.position = position;
            this.size = size;
            this.color = color;
            this.startDegree = startDegree;
            this.endDegree = endDegree;
            this.degree = degree;
        }

        public Circle(ConfigNode node)
        {
            float.TryParse(node.GetValue("positionX"), out position.x);
            float.TryParse(node.GetValue("positionY"), out position.y);
            float.TryParse(node.GetValue("outerSize"), out size.x);
            float.TryParse(node.GetValue("innerSize"), out size.y);
            float.TryParse(node.GetValue("colorR"), out color.x);
            float.TryParse(node.GetValue("colorG"), out color.y);
            float.TryParse(node.GetValue("colorB"), out color.z);
            float.TryParse(node.GetValue("colorA"), out color.w);
            float.TryParse(node.GetValue("startDegree"), out startDegree);
            float.TryParse(node.GetValue("endDegree"), out endDegree);
            degree = endDegree;

        }
    }
}
