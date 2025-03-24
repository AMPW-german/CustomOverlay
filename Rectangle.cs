using System;
using UnityEngine;

// Custom Overlay
// This mod allows you to use fully functional UIs in KSP
// Copyright (C) 2025 AMPW

// This program is free software: you can redistribute it and/or modify
// it under the terms of the GNU General Public License as published by
// the Free Software Foundation, either version 3 of the License, or
// (at your option) any later version.

// This program is distributed in the hope that it will be useful,
// but WITHOUT ANY WARRANTY; without even the implied warranty of
// MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
// GNU General Public License for more details.

// You should have received a copy of the GNU General Public License
// along with this program.  If not, see <https://www.gnu.org/licenses/

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
