using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using Unity.Jobs;
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
    public static class Settings
    {
        public static bool ShowCustomUI = false;

        public static bool loaded = false;
        public static Vector2Int ScreenSize;
        public static Vector2Int OverlaySize;
        public static Vector2Int OverlayOffset;
        public static float textSizeMultiplier = 12.0f;
        public static float aspectRatio = 1.0f;
        public static KeyCode hotKey = KeyCode.None;

        public static void Load()
        {
            ScreenSize = new Vector2Int(GameSettings.SCREEN_RESOLUTION_WIDTH, GameSettings.SCREEN_RESOLUTION_HEIGHT);
            aspectRatio = (float)ScreenSize.x / (float)ScreenSize.y;

            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/OverLaySettings.cfg";

            ConfigNode node = ConfigNode.Load(path);
            if ((node == null) || (node.GetNodes().Length == 0))
            {
                return;
            }
            ConfigNode[] nodes = node.GetNodes();

            if ((nodes == null) || (nodes.Length == 0))
            {
                return;
            }

            int UISizeY = ScreenSize.y;
            int UISizeX = ScreenSize.x;
            if (nodes[0].HasValue("UISizeX"))
            {
                int.TryParse(nodes[0].GetValue("UISizeX"), out UISizeX);
            }
            if (nodes[0].HasValue("UISizeY"))
            {
                int.TryParse(nodes[0].GetValue("UISizeY"), out UISizeY);
            }
            if (nodes[0].HasValue("UIPercentX"))
            {
                UISizeX = (int)(float.Parse(nodes[0].GetValue("UIPercentX")) / 100.0f * ScreenSize.x);
            }
            if (nodes[0].HasValue("UIPercentY"))
            {
                UISizeY = (int)(float.Parse(nodes[0].GetValue("UIPercentY")) / 100.0f * ScreenSize.y);
            }
            OverlaySize = new Vector2Int(UISizeX, UISizeY);


            int UIOffsetX = 0;
            int UIOffsetY = 0;
            if (nodes[0].HasValue("UIOffsetX"))
            {
                int.TryParse(nodes[0].GetValue("UIOffsetX"), out UIOffsetX);
            }
            if (nodes[0].HasValue("UIOffsetY"))
            {
                int.TryParse(nodes[0].GetValue("UIOffsetY"), out UIOffsetY);
            }
            if (nodes[0].HasValue("UIOffsetPercentX"))
            {
                UIOffsetX = (int)(float.Parse(nodes[0].GetValue("UIOffsetPercentX")) / 100.0f * ScreenSize.x);
            }
            if (nodes[0].HasValue("UIOffsetPercentY"))
            {
                UIOffsetY = (int)(float.Parse(nodes[0].GetValue("UIOffsetPercentY")) / 100.0f * ScreenSize.y);
            }
            OverlayOffset = new Vector2Int(UIOffsetX, UIOffsetY);


            float.TryParse(nodes[0].GetValue("textSizeMultiplier"), out textSizeMultiplier);
            textSizeMultiplier *= OverlaySize.y / 256f;

            if (nodes[0].HasValue("hotkey"))
            {
                if (Enum.TryParse(nodes[0].GetValue("hotkey"), true, out KeyCode hotkey)) hotKey = hotkey;
            }

            loaded = true;
        }

        public static List<CustomUI> LoadUIs()
        {
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/Overlays.cfg";

            ConfigNode node = ConfigNode.Load(path);

            if ((node == null) || (node.GetNodes().Length == 0))
            {
                return new List<CustomUI> { };
            }
            ConfigNode[] nodes = node.GetNodes();

            ConfigNode[] LayoutNodes = nodes[0].GetNodes("Layout");

            List<CustomUI> UIs = new List<CustomUI>();

            foreach (ConfigNode itemNode in LayoutNodes)
            {
                CustomUI layout = new CustomUI();
                layout.name = itemNode.GetValue("name");

                float.TryParse(itemNode.GetValue("colorR"), out layout.backgroundColor.x);
                float.TryParse(itemNode.GetValue("colorG"), out layout.backgroundColor.y);
                float.TryParse(itemNode.GetValue("colorB"), out layout.backgroundColor.z);
                float.TryParse(itemNode.GetValue("colorA"), out layout.backgroundColor.w);

                foreach (ConfigNode item in itemNode.GetNodes("circlegauge"))
                {
                    layout.circleGauges.Add(new CircleGauge(layout, item));
                }
                foreach (ConfigNode item in itemNode.GetNodes("bargauge"))
                {
                    layout.barGauges.Add(new BarGauge(layout, item));
                }
                foreach (ConfigNode item in itemNode.GetNodes("rectangle"))
                {
                    layout.rectangles.Add(new Rectangle(item));
                }
                foreach (ConfigNode item in itemNode.GetNodes("circle"))
                {
                    layout.circles.Add(new Circle(item));
                }
                foreach (ConfigNode item in itemNode.GetNodes("text"))
                {
                    layout.texts.Add(new Text(layout, item));
                }
                foreach (ConfigNode item in itemNode.GetNodes("circlesymetrie"))
                {
                    new CircleSymetrie(layout, item);
                }
                foreach (ConfigNode item in itemNode.GetNodes("picture"))
                {
                    layout.pictures.Add(new picture(item));
                }

                UIs.Add(layout);
            }

            return UIs;
        }
    }
}
