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

            ConfigNode[] nodes = GameDatabase.Instance.GetConfigNodes("CustomUISettings");

            if ((nodes == null) || (nodes.Length == 0))
            {
                return;
            }

            int.TryParse(nodes[0].GetValue("UISizeX"), out int screenSizeX);
            int.TryParse(nodes[0].GetValue("UISizeY"), out int screenSizeY);
            OverlaySize = new Vector2Int(screenSizeX, screenSizeY);

            float.TryParse(nodes[0].GetValue("textSizeMultiplier"), out textSizeMultiplier);

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

                foreach (ConfigNode item in itemNode.GetNodes())
                {
                    switch (item.GetValue("type").ToLower())
                    {
                        case "circlegauge":
                            layout.circleGauges.Add(new CircleGauge(layout, item));
                            break;
                    }
                }

                UIs.Add(layout);
            }

            return UIs;
        }
    }
}
