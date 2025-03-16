using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomOverlay
{
    public class Rectangle
    {
        public Vector2 position;
        public Vector2 size;
        public float degree;
        public Vector4 color;

        public float getRadians()
        {
            return (float)(degree * (Math.PI / 180));
        }

        public static Rectangle getEmpty()
        {
            return new Rectangle(Vector2.zero, Vector2.zero, 0.0f, Vector4.zero);
        }

        public Rectangle(Vector2 position, Vector2 size, float degree, Vector4 color)
        {
            this.position = position;
            this.size = size;
            this.degree = degree;
            this.color = color;
        }

        public Rectangle(ConfigNode node)
        {
            float.TryParse(node.GetValue("positionX"), out position.x);
            float.TryParse(node.GetValue("positionY"), out position.y);
            float.TryParse(node.GetValue("width"), out size.x);
            float.TryParse(node.GetValue("height"), out size.y);
            float.TryParse(node.GetValue("rotation"), out degree);

            float.TryParse(node.GetValue("colorR"), out color.x);
            float.TryParse(node.GetValue("colorG"), out color.y);
            float.TryParse(node.GetValue("colorB"), out color.z);
            float.TryParse(node.GetValue("colorA"), out color.w);
        }
    }
}
