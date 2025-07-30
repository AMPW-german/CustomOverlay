using System;
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
    public class CircleGauge : valueInterface
    {
        public valueMode Mode { get; private set; }
        public flightData FlightData { get; private set; }
        public bool autoScale { get; private set; }
        public float value { get; private set; }
        public float maxValue { get; private set; }
        public float multiplier { get; private set; } = 1f;

        public string resourceType { get; private set; }

        string unitName;
        string text;

        float current;
        int decimals;

        public float Max { get { return maxValue; } set { maxValue = value; } }

        Vector2 position;
        float size;
        Vector4 color;

        TextMeshProUGUI valueTextMesh;
        TextMeshProUGUI descTextMesh;

        Circle innerCircle;
        Circle outerCircle;

        Rectangle endcapLeft;
        Rectangle endcapRight;

        private bool disabled;

        Vector2? innerCircleSave;
        Vector2? outerCircleSave;
        Vector2? endcapLeftSave;
        Vector2? endcapRightSave;

        private CustomUI instance;

        public void Upddate()
        {
            if (disabled) { return; }
            current = value;
            innerCircle.degree = Math.Min(current / maxValue, 1) * 360;
            valueTextMesh.text = $"{Math.Round(Mode == valueMode.resource ? current / maxValue * 100 : current, decimals)}{unitName}";
        }

        public void disable()
        {
            disabled = true;
            valueTextMesh.text = "";
            valueTextMesh.color = Color.clear;
            descTextMesh.text = "";
            descTextMesh.color = Color.clear;

            innerCircleSave = innerCircle.size;
            outerCircleSave = outerCircle.size;
            endcapLeftSave = endcapLeft.size;
            endcapRightSave = endcapRight.size;

            innerCircle.size = new Vector2(0, 0);
            innerCircle.color = Color.clear;
            outerCircle.size = new Vector2(0, 0);
            outerCircle.color = Color.clear;
            endcapLeft.size = new Vector2(0, 0);
            endcapLeft.color = Color.clear;
            endcapRight.size = new Vector2(0, 0);
            endcapRight.color = Color.clear;
        }

        public void enable()
        {
            disabled = false;

            valueTextMesh.text = $"{Math.Round(current, decimals)}{unitName}";
            descTextMesh.text = text;

            valueTextMesh.color = color;
            descTextMesh.color = color;
            innerCircle.color = color;
            outerCircle.color = color;
            endcapLeft.color = color;
            endcapRight.color = color;

            if (innerCircleSave != null)
            {
                innerCircle.size = (Vector2)innerCircleSave;
                innerCircleSave = null;
            }
            if (outerCircleSave != null)
            {
                outerCircle.size = (Vector2)outerCircleSave;
                outerCircleSave = null;
            }
            if (endcapLeftSave != null)
            {
                endcapLeft.size = (Vector2)endcapLeftSave;
                endcapLeftSave = null;
            }
            if (endcapRightSave != null)
            {
                endcapRight.size = (Vector2)endcapRightSave;
                endcapRightSave = null;
            }
        }

        public void updateValue()
        {
            if (Mode == valueMode.resource)
            {
                value = (float)ResourceManager.getResource(PartResourceLibrary.Instance.GetDefinition(resourceType)) * multiplier;
                maxValue = (float)ResourceManager.getResourceMax(PartResourceLibrary.Instance.GetDefinition(resourceType)) * multiplier;
            }
            else
            {
                if (FlightData == flightData.None || FlightData == flightData.missionTime || FlightData == flightData.missionTimeFormatted) { return; }
                value = FlightDataManager.FlightData(FlightData) * multiplier;

                if (autoScale && value > maxValue)
                {
                    maxValue = value;
                }
            }
            Upddate();
        }

        private void create()
        {
            valueTextMesh = Overlay.instance.CreateText($"{Math.Round(current, decimals)}{unitName}", new Vector2(position.x, position.y), textAlignment.center, 2.8f * size);
            descTextMesh = Overlay.instance.CreateText($"{text}", new Vector2(position.x, position.y - 0.6f * size), textAlignment.center, 2.2f * size);

            innerCircle = new Circle(position, new Vector2(size * 0.9f, size * 0.75f), color, 50, 310, Math.Min(current / maxValue, 1) * 360);
            outerCircle = new Circle(position, new Vector2(size, size * 0.95f), color, 50, 310, 360);

            endcapLeft = new Rectangle(new Vector2(position.x - (float)Math.Sin(46 * 0.01745329252) * (size * 0.875f) / Settings.OverlaySize.x * Settings.OverlaySize.y, position.y - (float)Math.Cos(46 * 0.01745329252) * (size * 0.875f)), new Vector2(0.008f * (size * Settings.OverlaySize.y / 256) / 0.4f, 0.022f * (size * Settings.OverlaySize.y / 256) / 0.4f), 40, color);
            endcapRight = new Rectangle(new Vector2(position.x - (float)Math.Sin(314 * 0.01745329252) * (size * 0.875f) / Settings.OverlaySize.x * Settings.OverlaySize.y, position.y - (float)Math.Cos(314 * 0.01745329252) * (size * 0.875f)), new Vector2(0.008f * (size * Settings.OverlaySize.y / 256) / 0.4f, 0.022f * (size * Settings.OverlaySize.y / 256) / 0.4f), 140, color);

            instance.textMeshGUIs.Add(valueTextMesh);
            instance.textMeshGUIs.Add(descTextMesh);

            instance.circles.Add(innerCircle);
            instance.circles.Add(outerCircle);

            instance.rectangles.Add(endcapLeft);
            instance.rectangles.Add(endcapRight);
        }

        public CircleGauge(CustomUI instance, string unitName, string text, valueMode mode, flightData FlightData, float max, float current, int decimals, Vector2 position, float size, Vector4 color)
        {
            this.instance = instance;

            this.unitName = unitName;
            this.text = text;
            this.Mode = mode;
            this.FlightData = FlightData;
            this.maxValue = max;
            this.current = current;
            this.decimals = decimals;
            this.position = position;
            this.size = size;
            this.color = color;

            create();
        }

        public CircleGauge(CustomUI instance, string unitName, string text, valueMode mode, string resource, float max, float current, int decimals, Vector2 position, float size, Vector4 color)
        {
            this.instance = instance;

            this.unitName = unitName;
            this.text = text;
            this.Mode = mode;
            this.FlightData = flightData.None;
            this.resourceType = resource;
            this.maxValue = max;
            this.current = current;
            this.decimals = decimals;
            this.position = position;
            this.size = size;
            this.color = color;

            create();
        }

        public CircleGauge(CustomUI instance, ConfigNode node)
        {
            this.instance = instance;

            unitName = node.GetValue("unitname");
            text = node.GetValue("text");

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

            if (node.HasValue("multiplier")) multiplier = float.Parse(node.GetValue("multiplier"));
            else multiplier = 1;

            current = 0;
            int.TryParse(node.GetValue("decimals"), out decimals);
            float.TryParse(node.GetValue("positionX"), out position.x);
            float.TryParse(node.GetValue("positionY"), out position.y);
            float.TryParse(node.GetValue("size"), out size);

            float.TryParse(node.GetValue("colorR"), out color.x);
            float.TryParse(node.GetValue("colorG"), out color.y);
            float.TryParse(node.GetValue("colorB"), out color.z);
            float.TryParse(node.GetValue("colorA"), out color.w);

            create();
        }
    }
}
