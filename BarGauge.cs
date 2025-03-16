using System;
using TMPro;
using UnityEngine;

namespace CustomOverlay
{
    public class BarGauge
    {
        private string name;
        private float percent;
        private Vector2 position;
        private Vector2 size;
        private Vector4 startColor;
        private Vector4 endColor;
        private Vector4 backgroundColor;

        private TextMeshProUGUI textMeshProUGUI;
        public float Percent { get { return percent; } set { percent = Math.Max(Math.Min(value, 1.0f), 0.0f); } }

        public Vector2 primaryPosition { get { return new Vector2(position.x - size.x / 2.0f + (size.x * percent) / 2.0f, position.y); } }
        public Vector2 primarySize { get { return new Vector2(size.x * percent, size.y - 0.02f); } }

        public Vector2 Position { get { return position; } }
        public Vector2 Size { get { return size; } }

        public Vector4 StartColor { get { return startColor; } }
        public Vector4 EndColor { get { return endColor; } }
        public Vector4 BackgroundColor { get { return backgroundColor; } }

        public float Rounding { get { return percent > 0.2f ? 0.16f : 0.2f; } }
        public float thickness { get { return percent > 0.2f ? 0.02f : 0.2f; } }

        public static BarGauge GetEmpty()
        {
            return new BarGauge();
        }

        public BarGauge()
        {
            this.percent = 0;
            this.position = Vector2.zero;
            this.size = Vector2.zero;
            this.startColor = Vector4.zero;
            this.endColor = Vector4.zero;
            this.backgroundColor = Vector4.zero;
        }

        public BarGauge(string name, float percent, Vector2 position, Vector2 size, Vector4 startColor, Vector4 endColor, Vector4 backgroundColor)
        {
            this.name = name;
            this.percent = percent;
            this.position = position;
            this.size = size;
            this.startColor = startColor;
            this.endColor = endColor;
            this.backgroundColor = backgroundColor;

            textMeshProUGUI = Overlay.instance.CreateText(name, new Vector2(position.x - size.x / 2.0f - 0.02f, position.y), textAlignment.left);
        }

        public BarGauge(ConfigNode node)
        {

        }
    }
}
