﻿using System;
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
    public class Text : valueInterface
    {
        TextMeshProUGUI textInstance;

        string text;
        public valueMode Mode { get; private set; }
        public flightData FlightData { get; private set; }
        public bool autoScale { get; private set; }
        public float value { get; private set; }
        public string valueString { get; private set; }
        public float maxValue { get; private set; }
        public string resourceType { get; private set; }
        public int decimals { get; private set; }
        public float multiplier { get; private set; }

        public void updateValue()
        {
            if (Mode == valueMode.resource)
            {
                value = (float)ResourceManager.getResource(PartResourceLibrary.Instance.GetDefinition(resourceType));
                maxValue = (float)ResourceManager.getResourceMax(PartResourceLibrary.Instance.GetDefinition(resourceType));
                textInstance.text = $"{Math.Round(value * multiplier, decimals)}/{Math.Round(maxValue * multiplier, decimals)} {text}";
            }
            else if (Mode == valueMode.flightData)
            {
                if (FlightData == flightData.None) { return; }
                value = FlightDataManager.FlightData(FlightData);

                if (FlightData == flightData.missionTimeFormatted || FlightData == flightData.timeToAPFormatted || FlightData == flightData.timeToPEFormatted)
                {
                    int time = (int) FlightDataManager.FlightData(FlightData);
                    textInstance.text = $"T+{Convert.ToInt32(Math.Floor(time / 3600f)).ToString("D2")}:{Convert.ToInt32(Math.Floor(time / 60f) % 60).ToString("D2")}:{Convert.ToInt32(Math.Floor((float)time) % 60).ToString("D2")}";
                }
                else if (FlightData == flightData.missionTime || FlightData == flightData.timeToAP || FlightData == flightData.timeToPE)
                {
                    int time = (int) FlightDataManager.FlightData(FlightData);
                    textInstance.text = $"T+{time}";
                }
                else
                {
                    textInstance.text = $"{Math.Round(value * multiplier, decimals)}{text}";
                }
            }
        }

        public Text(string text, Vector2 position, textAlignment alignment, float fontSize = 1)
        {
            Mode = valueMode.None;

            textInstance = Overlay.instance.CreateText(text, position, alignment, fontSize);
        }

        public Text(CustomUI instance, ConfigNode node)
        {
            text = node.GetValue("text");
            float.TryParse(node.GetValue("positionX"), out float positionX);
            float.TryParse(node.GetValue("positionY"), out float positionY);

            float colorR = 1;
            float colorG = 1;
            float colorB = 1;
            float colorA = 1;

            if (node.HasValue("colorR")) colorR = float.Parse(node.GetValue("colorR"));
            if (node.HasValue("colorG")) colorG = float.Parse(node.GetValue("colorG"));
            if (node.HasValue("colorB")) colorB = float.Parse(node.GetValue("colorB"));
            if (node.HasValue("colorA")) colorA = float.Parse(node.GetValue("colorA"));

            Color color = new Color(colorR, colorG, colorB, colorA);

            textAlignment alignment = textAlignment.center;
            switch (node.GetValue("alignment"))
            {
                case "left":
                    alignment = textAlignment.left;
                    break;
                case "right":
                    alignment = textAlignment.right;
                    break;
                case "center":
                default:
                    alignment = textAlignment.center;
                    break;
            }
            float.TryParse(node.GetValue("fontsize"), out float fontsize);

            string modeText = node.GetValue("mode");

            if (modeText == "none")
            {
                Mode = valueMode.None;
            }
            else if (modeText == "resource")
            {
                Mode = valueMode.resource;
                resourceType = node.GetValue("resource");
            }
            else if (modeText == "flightdata")
            {
                Mode = valueMode.flightData;
                FlightData = FlightDataManager.stringToValue(node.GetValue("source"));
            }
            if (node.HasValue("decimals"))
            {
                decimals = int.Parse(node.GetValue("decimals"));
            }
            else
            {
                decimals = 0;
            }
            if (node.HasValue("multiplier"))
            {
                multiplier = float.Parse(node.GetValue("multiplier"));
            }
            else
            {
                multiplier = 1;
            }
            this.textInstance = Overlay.instance.CreateText(text, new Vector2(positionX, positionY), alignment, fontsize, color);
            instance.texts.Add(this);
            instance.textMeshGUIs.Add(this.textInstance);
        }
    }
}
