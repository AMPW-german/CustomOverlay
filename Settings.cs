using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

namespace CustomOverlay
{
    public static class Settings
    {
        public static bool ShowCustomUI = false;

        public static bool loaded = false;
        public static Vector2Int ScreenSize;
        public static Vector2Int OverlaySize;
        public static float textSizeMultiplier = 12.0f;

        public static void Load()
        {
            ScreenSize = new Vector2Int(GameSettings.SCREEN_RESOLUTION_WIDTH, GameSettings.SCREEN_RESOLUTION_HEIGHT);

            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/CustomUISettings.cfg";

            ConfigNode node = ConfigNode.Load(path);
            if ((node == null) || (node.GetNodes().Length == 0))
            {
                return;
            }
            ConfigNode[] Nodes = node.GetNodes();
            ConfigNode[] nodes = Nodes[0].GetNodes("Layout");


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
                UISizeX = (int) (float.Parse(nodes[0].GetValue("UIPercentX")) / 100.0f * ScreenSize.x);
            }
            if (nodes[0].HasValue("UIPercentY"))
            {
                UISizeY = (int)(float.Parse(nodes[0].GetValue("UIPercentY")) / 100.0f * ScreenSize.y);
            }
            OverlaySize = new Vector2Int(UISizeX, UISizeY);

            float.TryParse(nodes[0].GetValue("textSizeMultiplier"), out textSizeMultiplier);
            textSizeMultiplier *= OverlaySize.y / 256f;

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

                UIs.Add(layout);
            }

            return UIs;
        }
    }
}
