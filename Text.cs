using System;
using System.Security.AccessControl;
using TMPro;
using UnityEngine;
using UnityEngine.UIElements;

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

                if (FlightData == flightData.missionTimeFormatted)
                {
                    textInstance.text = $"T+{Convert.ToInt32(Math.Floor(FlightGlobals.ActiveVessel.missionTime / 3600)).ToString("D2")}:{Convert.ToInt32(Math.Floor(FlightGlobals.ActiveVessel.missionTime / 60) % 60).ToString("D2")}:{Convert.ToInt32(Math.Floor(FlightGlobals.ActiveVessel.missionTime) % 60).ToString("D2")}";
                }
                else if (FlightData == flightData.missionTime)
                {
                    textInstance.text = $"T+{FlightGlobals.ActiveVessel.missionTime}";
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
            this.textInstance = Overlay.instance.CreateText(text, new Vector2(positionX, positionY), alignment, fontsize);
            instance.texts.Add(this);
            instance.textMeshGUIs.Add(this.textInstance);
        }
    }
}
