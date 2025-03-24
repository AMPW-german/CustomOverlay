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
    public class Circle : valueInterface
    {
        public valueMode Mode { get; private set; }
        public flightData FlightData { get; private set; }
        public bool autoScale { get; private set; }
        public float value { get; private set; }
        public float maxValue { get; private set; }
        public string resourceType { get; private set; }

        public Vector2 position;
        public Vector2 size;
        public Vector4 color;
        public float startDegree;
        public float endDegree;
        public float degree;

        public float start { get { return startDegree / 360; } }
        public float end { get { return endDegree / 360; } }
        public float deg { get { return degree / 360; } }

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

            degree = value / maxValue * (endDegree - startDegree) + startDegree;
        }

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
            if (node.HasValue("mode"))
            {
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
                }
            }
            else
            {
                Mode = valueMode.None;
            }


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
