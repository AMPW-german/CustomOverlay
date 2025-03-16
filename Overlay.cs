using KSP.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using KSP.UI.Screens;

namespace CustomOverlay
{
    public enum textAlignment
    {
        left,
        center,
        right
    }

    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class Overlay : MonoBehaviour
    {
        private AssetBundle assetBundle;
        private Shader gaugeShader;
        private Material gaugeMaterial;
        private RenderTexture renderTexture;

        public List<CustomUI> UIs;
        public CustomUI activeUI;

        private Camera uiCamera;

        public static Overlay instance;


        protected void Awake()
        {
            assetBundle = AssetBundle.LoadFromFile($"{KSPUtil.ApplicationRootPath}GameData/CustomOverlay/OverlayShader-windows.unity3d");

            instance = this;

            UIs = new List<CustomUI> { };
            activeUI = new CustomUI();

            Settings.Load();

            renderTexture = new RenderTexture(Settings.OverlaySize.x, Settings.OverlaySize.y, 32);
        }

        void Start()
        {
            GameEvents.onShowUI.Add(showUi);
            GameEvents.onHideUI.Add(hideUI);

            reload();
        }

        public void showUi()
        {
            Settings.ShowCustomUI = false;
        }

        public void hideUI()
        {
            Settings.ShowCustomUI = true;
        }

        /// <summary>
        /// position is relativ on the rendertexture
        /// </summary>
        /// <param name="textLength">Only needed for aligment, needs to be manually checked</param>
        /// <returns></returns>
        public TextMeshProUGUI CreateText(string text, Vector2 position, textAlignment alignment, float fontSize = 1)
        {
            GameObject textObj = new GameObject("TextMeshPro");
            textObj.transform.SetParent(GameObject.Find("Text Canvas").transform);
            textObj.layer = LayerMask.NameToLayer("UI"); // Set the text to the "UI" layer

            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            tmp.ForceMeshUpdate();

            RectTransform textRect = tmp.GetComponent<RectTransform>();
            //textRect.sizeDelta = new Vector2(Settings.OverLaySize.x, Settings.OverLaySize.y); // Match render texture size
            switch (alignment)
            {
                case textAlignment.left:
                    textRect.localPosition = new Vector2(position.x * Settings.OverlaySize.x - Settings.OverlaySize.x / 2 + tmp.preferredWidth * 12, position.y * Settings.OverlaySize.y - Settings.OverlaySize.y / 2);
                    break;
                case textAlignment.center:
                    textRect.localPosition = new Vector2(position.x * Settings.OverlaySize.x - Settings.OverlaySize.x / 2, position.y * Settings.OverlaySize.y - Settings.OverlaySize.y / 2);
                    break;
                case textAlignment.right:
                    textRect.localPosition = new Vector2(position.x * Settings.OverlaySize.x - Settings.OverlaySize.x / 2 - tmp.preferredWidth * 12, position.y * Settings.OverlaySize.y - Settings.OverlaySize.y / 2);
                    break;
            }

            Debug.Log($"Text Position: {textRect.anchoredPosition}"); // Debugging

            return tmp;
        }

        public void reload()
        {
            Shader[] shaders = assetBundle.LoadAllAssets<Shader>();
            foreach (Shader shader in shaders)
            {
                gaugeShader = shader;
            }

            if (shaders.Length == 1)
            {
                gaugeShader = shaders.First();
            }
            else if (shaders.Length == 0)
            {
                Debug.LogError("Failed to load Gauge Shader!");
            }
            else
            {
                Debug.LogError("Too many shaders loaded");
            }

            // Create a material using the shader
            gaugeMaterial = new Material(gaugeShader);
            gaugeMaterial.SetFloat("_AspectRatio", Settings.OverlaySize.x / Settings.OverlaySize.y);


            GameObject camObj = new GameObject("UI Render Camera");
            uiCamera = camObj.AddComponent<Camera>();
            uiCamera.orthographic = true;
            uiCamera.clearFlags = CameraClearFlags.Nothing;
            uiCamera.backgroundColor = new Color(0, 0, 0, 0); // Transparent background

            // Use the "UI" layer to render UI elements
            uiCamera.cullingMask = LayerMask.GetMask("UI"); // Render only UI elements
            uiCamera.targetTexture = renderTexture; // Directly render to the RenderTexture

            // Step 2.2: Create Canvas and TextMeshProUGUI
            GameObject canvasObj = new GameObject("Text Canvas");
            Canvas canvas = canvasObj.AddComponent<Canvas>();
            canvas.renderMode = RenderMode.ScreenSpaceCamera;
            canvas.worldCamera = uiCamera; // Assign the camera to the canvas
            canvasObj.layer = LayerMask.NameToLayer("UI"); // Use the "UI" layer

            RectTransform canvasRect = canvas.GetComponent<RectTransform>();
            canvasRect.sizeDelta = new Vector2(3440, 256); // Match render texture size

            //Circles = new List<circle>();
            //Rectangles = new List<rectangle>();
            //barGauges = new List<barGauge>();

            //oxGauge = new barGauge("LOX", 0, new Vector2(0.2f, 0.3f), new Vector2(0.1f, 0.05f), new Vector4(0.35f, 0.35f, 0.35f, 1), new Vector4(1, 1, 1, 1), new Vector4(0.1f, 0.1f, 0.1f, 1f));
            //lfGauge = new barGauge("CH4", 0, new Vector2(0.2f, 0.15f), new Vector2(0.1f, 0.05f), new Vector4(0.35f, 0.35f, 0.35f, 1), new Vector4(1, 1, 1, 1), new Vector4(0.1f, 0.1f, 0.1f, 1f));
            //barGauges.Add(lfGauge);
            //barGauges.Add(oxGauge);

            //speedGauge = new circleGauge("m/s", "Speed", 0, 320, 0, 1, new Vector2(0.05f, 0.45f), new Vector2(0.4f, 0.4f), new Vector4(255, 255, 255, 1));
            //throttleGauge = new circleGauge("%", "Throttle", 0, 100, 0, 0, new Vector2(0.15f, 0.45f), new Vector2(0.4f, 0.4f), new Vector4(255, 255, 255, 1));
            //machGauge = new circleGauge("M", "Mach", 0, 3, 0, 3, new Vector2(0.25f, 0.45f), new Vector2(0.4f, 0.4f), new Vector4(255, 255, 255, 1));
            //gGauge = new circleGauge("G", "G-Forces", 0, 5, 0, 3, new Vector2(0.95f, 0.45f), new Vector2(0.4f, 0.4f), new Vector4(255, 255, 255, 1));

            // Superheavy engines
            Vector2 midPoint = new Vector2(0.075f, 0.5f);
            float circleSize = 0.05f;
            float innerSize = 0.15f;
            float middleSize = 0.4f;
            float outerSize = 0.7f;

            //innerCircles = new List<circle> { };
            //middleCircles = new List<circle> { };
            //outerCircles = new List<circle> { };

            //circleSymetrie(midPoint, innerSize, 0, 3).ForEach(v => innerCircles.Add(new circle(v, new Vector2(circleSize, 0), new Vector4(1, 1, 1, 1), 0, 360, 360)));
            //circleSymetrie(midPoint, middleSize, 0, 10).ForEach(v => middleCircles.Add(new circle(v, new Vector2(circleSize, 0), new Vector4(1, 1, 1, 1), 0, 360, 360)));
            //circleSymetrie(midPoint, outerSize, 9, 20).ForEach(v => outerCircles.Add(new circle(v, new Vector2(circleSize, 0), new Vector4(1, 1, 1, 1), 0, 360, 360)));

            //innerCircles.ForEach(c => Circles.Add(c));
            //middleCircles.ForEach(c => Circles.Add(c));
            //outerCircles.ForEach(c => Circles.Add(c));





            //speedText = CreateText("SPEED", new Vector2(0.13f, 0.6f), textAlignment.left, 1.2f);
            //altitudeText = CreateText("ALTITUDE", new Vector2(0.13f, 0.45f), textAlignment.left, 1.2f);
            //speedValue = CreateText("0 KM/H", new Vector2(0.23f, 0.6f), textAlignment.center, 1.2f);
            //altitudeValue = CreateText("0 KM", new Vector2(0.23f, 0.45f), textAlignment.center, 1.2f);
            //timeText = CreateText("T+00:00:00", new Vector2(0.5f, 0.5f), textAlignment.center, 3f);
            //text = CreateText("STARSHIP FLIGHT TEST", new Vector2(0.5f, 0.25f), textAlignment.center, 1.4f);

            //activeUI = new CustomUI();
            //activeUI.circleGauges.Add(new CircleGauge(activeUI, "m/s", "Speed", valueMode.flightData, flightData.airspeed, 0, 340, 0, 2, new Vector2(0.3f, 0.5f), new Vector2(0.5f, 0.5f), Vector4.one));
            //activeUI.circleGauges.Add(new CircleGauge(activeUI, "LF", "Liquid Fuel", valueMode.resource, "LiquidFuel", 0, 340, 0, 2, new Vector2(0.5f, 0.5f), new Vector2(0.5f, 0.5f), Vector4.one));

            UIs = Settings.LoadUIs();
            activeUI = UIs.First();

            RenderTexture.active = renderTexture;
            GL.Clear(true, true, new Color(0, 0, 0, 0.5f)); // Ensure transparency

            Graphics.Blit(null, renderTexture, gaugeMaterial);

            RenderTexture.active = null;
        }




        void Update()
        {            
            //speedGauge.Upddate((float)FlightGlobals.ActiveVessel.speed);
            //throttleGauge.Upddate(FlightGlobals.ActiveVessel.ctrlState.mainThrottle * 100);
            //machGauge.Upddate((float)FlightGlobals.ActiveVessel.mach);
            //gGauge.Upddate((float)FlightGlobals.ActiveVessel.geeForce);
            //gGauge.Max = Math.Max(gGauge.Max, (float)FlightGlobals.ActiveVessel.geeForce);

            //if (rsList.ContainsKey(PartResourceLibrary.Instance.GetDefinition("Oxidizer").id))
            //{
            //    oxGauge.Percent = (float)(rsList[PartResourceLibrary.Instance.GetDefinition("Oxidizer").id].AmountValue / rsList[PartResourceLibrary.Instance.GetDefinition("Oxidizer").id].MaxAmountValue);
            //}

            //if (rsList.ContainsKey(PartResourceLibrary.Instance.GetDefinition("LiquidFuel").id))
            //{
            //    lfGauge.Percent = (float)(rsList[PartResourceLibrary.Instance.GetDefinition("LiquidFuel").id].AmountValue / rsList[PartResourceLibrary.Instance.GetDefinition("LiquidFuel").id].MaxAmountValue);
            //}

            //if (rsList.ContainsKey(PartResourceLibrary.Instance.GetDefinition("LqdMethane").id))
            //{
            //    lfGauge.Percent = (float)(rsList[PartResourceLibrary.Instance.GetDefinition("LqdMethane").id].AmountValue / rsList[PartResourceLibrary.Instance.GetDefinition("LqdMethane").id].MaxAmountValue);
            //}

            //lfGauge.Percent = (float)(rsList[PartResourceLibrary.Instance.GetDefinition("LqdMethane").id].AmountValue / rsList[PartResourceLibrary.Instance.GetDefinition("LqdMethane").id].MaxAmountValue);



            if (Settings.ShowCustomUI)
            {

                //if (FlightGlobals.ActiveVessel.ActionGroups[KSPActionGroup.Custom10])
                //{
                //    innerCircles.ForEach(c => c.size.y = 0.045f);
                //}
                //else
                //{
                //    innerCircles.ForEach(c => c.size.y = 0.0f);
                //}

                //if (FlightGlobals.ActiveVessel.ActionGroups[KSPActionGroup.Custom09])
                //{
                //    middleCircles.ForEach(c => c.size.y = 0.045f);
                //}
                //else
                //{
                //    middleCircles.ForEach(c => c.size.y = 0.0f);
                //}

                //if (FlightGlobals.ActiveVessel.ActionGroups[KSPActionGroup.Custom08])
                //{
                //    outerCircles.ForEach(c => c.size.y = 0.045f);
                //}
                //else
                //{
                //    outerCircles.ForEach(c => c.size.y = 0.0f);
                //}

                //speedValue.text = $"{Math.Round(FlightGlobals.ActiveVessel.speed * 3.6)} KM/H";
                //altitudeValue.text = $"{Math.Floor(FlightGlobals.ActiveVessel.altitude / 1000)} KM";
                //timeText.text = $"T+{Convert.ToInt32(Math.Floor(FlightGlobals.ActiveVessel.missionTime / 3600)).ToString("D2")}:{Convert.ToInt32(Math.Floor(FlightGlobals.ActiveVessel.missionTime / 60) % 60).ToString("D2")}:{Convert.ToInt32(Math.Floor(FlightGlobals.ActiveVessel.missionTime) % 60).ToString("D2")}";
                ResourceManager.update();

                activeUI.Update();

                activeUI.GetCircleVectors(out Vector4[] vectorCircles, out Vector4[] circleFill, out Vector4[] cirlceColors);
                gaugeMaterial.SetInt("_CircleCount", 100);
                gaugeMaterial.SetVectorArray("_Circles", vectorCircles);
                gaugeMaterial.SetVectorArray("_CircleFill", circleFill);
                gaugeMaterial.SetVectorArray("_CircleColors", cirlceColors);

                activeUI.GetVectorRectangles(out Vector4[] vectorRectangles, out float[] rotations, out Vector4[] rectangleColors);
                gaugeMaterial.SetInt("_RectangleCount", 50);
                gaugeMaterial.SetVectorArray("_Rectangles", vectorRectangles);
                gaugeMaterial.SetFloatArray("_RectangleRotation", rotations);
                gaugeMaterial.SetVectorArray("_RectangleColors", rectangleColors);

                activeUI.GetVectorBars(out Vector4[] vectorBars, out Vector4[] startColor, out Vector4[] endColor, out float[] barRounding, out float[] barThickness);
                gaugeMaterial.SetInt("_barCount", 40);
                gaugeMaterial.SetVectorArray("_barPosition", vectorBars);
                gaugeMaterial.SetVectorArray("_barStartColor", startColor);
                gaugeMaterial.SetVectorArray("_barEndColor", endColor);
                gaugeMaterial.SetFloatArray("_barRounding", barRounding);
                gaugeMaterial.SetFloatArray("_barThickness", barThickness);

                uiCamera.enabled = true;

                RenderTexture.active = renderTexture;
                GL.Clear(true, true, new Color(activeUI.backgroundColor.x, activeUI.backgroundColor.y, activeUI.backgroundColor.z, activeUI.backgroundColor.w)); // Ensure transparency

                Graphics.Blit(null, renderTexture, gaugeMaterial);

                RenderTexture.active = null;
            }
            else
            {
                uiCamera.enabled = false;

                RenderTexture.active = renderTexture;
                GL.Clear(true, true, new Color(0, 0, 0, 0));

                RenderTexture.active = null;
            }
        }

        void OnGUI()
        {
            GUI.DrawTexture(new Rect(0, Settings.ScreenSize.y - Settings.OverlaySize.y, Settings.OverlaySize.x, Settings.OverlaySize.y), renderTexture, ScaleMode.ScaleToFit, true);
        }

        //void RenderText(string text)

        public static UrlDir.UrlConfig[] GetConfigsByName(string name)
        {
            return GameDatabase.Instance.GetConfigs(name);
        }

        private void OnDestroy()
        {
            GameEvents.onShowUI.Remove(showUi);
            GameEvents.onHideUI.Remove(hideUI);

            assetBundle.Unload(true);

        }
    }
}
