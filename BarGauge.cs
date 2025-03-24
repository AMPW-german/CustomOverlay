using TMPro;
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
    public class BarGauge : valueInterface
    {
        public valueMode Mode { get; private set; }
        public flightData FlightData { get; private set; }
        public bool autoScale { get; private set; }
        public float value { get; private set; }
        public float maxValue { get; private set; }
        public string resourceType { get; private set; }


        private string name;
        private Vector2 position;
        private Vector2 size;
        private Vector4 startColor;
        private Vector4 endColor;
        private Vector4 backgroundColor;

        private TextMeshProUGUI textMeshProUGUI;
        public float Percent { get { return value / maxValue; } set { this.value = maxValue * value; } }

        public Vector2 primaryPosition { get { return new Vector2(position.x - size.x / 2.0f + (size.x * Percent) / 2.0f, position.y); } }
        public Vector2 primarySize { get { return new Vector2(size.x * Percent, size.y); } }

        public Vector2 Position { get { return position; } }
        public Vector2 Size { get { return size; } }

        public Vector4 StartColor { get { return startColor; } }
        public Vector4 EndColor { get { return endColor; } }
        public Vector4 BackgroundColor { get { return backgroundColor; } }

        /// <summary>
        /// Needs reworking because the aspect ratio the rounding is stretched
        /// </summary>
        //public float Rounding { get { return Percent > 0.2f ? 0.16f : 0.2f; } }
        //public float thickness { get { return Percent > 0.2f ? 0.02f : 0.2f; } }

        public float Rounding { get { return 0.01f; } }
        public float thickness { get { return 0.01f; } }

        public void updateValue()
        {
            if (Mode == valueMode.resource)
            {
                value = (float)ResourceManager.getResource(PartResourceLibrary.Instance.GetDefinition(resourceType));
                maxValue = (float)ResourceManager.getResourceMax(PartResourceLibrary.Instance.GetDefinition(resourceType));
            }
            else
            {
                if (FlightData == flightData.None || FlightData == flightData.missionTime || FlightData == flightData.missionTimeFormatted) { return; }
                value = FlightDataManager.FlightData(FlightData);

                if (autoScale && value > maxValue)
                {
                    maxValue = value;
                }
            }
        }

        public static BarGauge GetEmpty()
        {
            return new BarGauge();
        }

        public BarGauge()
        {
            this.Percent = 0;
            this.position = Vector2.zero;
            this.size = Vector2.zero;
            this.startColor = Vector4.zero;
            this.endColor = Vector4.zero;
            this.backgroundColor = Vector4.zero;
        }

        public BarGauge(string name, float percent, Vector2 position, Vector2 size, Vector4 startColor, Vector4 endColor, Vector4 backgroundColor)
        {
            this.name = name;
            this.Percent = percent;
            this.position = position;
            this.size = size;
            this.startColor = startColor;
            this.endColor = endColor;
            this.backgroundColor = backgroundColor;

            textMeshProUGUI = Overlay.instance.CreateText(name, new Vector2(position.x - size.x / 2.0f - 0.02f, position.y), textAlignment.left);
        }

        public BarGauge(CustomUI instance, ConfigNode node)
        {
            name = node.GetValue("name");

            string modeText = node.GetValue("mode");
            if (modeText == "resource")
            {
                Mode = valueMode.resource;
                resourceType = node.GetValue("resource");
            }
            else if (modeText == "flightdata")
            {
                Mode = valueMode.flightData;
                FlightData = FlightDataManager.stringToValue(node.GetValue("source"));

                if (node.HasValue("autoscale"))
                {
                    autoScale = bool.Parse(node.GetValue("autoscale"));
                }

                float.TryParse(node.GetValue("maximum"), out float tempMax);
                maxValue = tempMax;
            }

            value = 0;
            float.TryParse(node.GetValue("positionX"), out position.x);
            float.TryParse(node.GetValue("positionY"), out position.y);
            float.TryParse(node.GetValue("sizeX"), out size.x);
            float.TryParse(node.GetValue("sizeY"), out size.y);

            float.TryParse(node.GetValue("startColorR"), out startColor.x);
            float.TryParse(node.GetValue("startColorG"), out startColor.y);
            float.TryParse(node.GetValue("startColorB"), out startColor.z);
            float.TryParse(node.GetValue("startColorA"), out startColor.w);

            float.TryParse(node.GetValue("endColorR"), out endColor.x);
            float.TryParse(node.GetValue("endColorG"), out endColor.y);
            float.TryParse(node.GetValue("endColorB"), out endColor.z);
            float.TryParse(node.GetValue("endColorA"), out endColor.w);

            float.TryParse(node.GetValue("bgColorR"), out backgroundColor.x);
            float.TryParse(node.GetValue("bgColorG"), out backgroundColor.y);
            float.TryParse(node.GetValue("bgColorB"), out backgroundColor.z);
            float.TryParse(node.GetValue("bgColorA"), out backgroundColor.w);

            textMeshProUGUI = Overlay.instance.CreateText(name, new Vector2(position.x - size.x / 2.0f - 0.02f, position.y), textAlignment.left);

            instance.textMeshGUIs.Add(textMeshProUGUI);
        }
    }
}
