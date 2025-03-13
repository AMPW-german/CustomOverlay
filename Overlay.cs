using KSP.UI;
using KSPAlternateResourcePanel;
using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;

namespace CustomOverlay
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class Overlay : MonoBehaviour
    {
        private AssetBundle assetBundle;
        private Shader gaugeShader;
        private Material gaugeMaterial;
        private RenderTexture renderTexture;

        private Vector2 overlaySize;
        public List<circle> Circles;
        public List<rectangle> Rectangles;
        public List<barGauge> barGauges;

        private Camera uiCamera;

        public static Overlay instance;

        bool spaceReached = false;

        circleGauge throttleGauge;
        circleGauge speedGauge;
        circleGauge machGauge;
        circleGauge gGauge;
        barGauge lfGauge;
        barGauge oxGauge;


        private void OnEnable()
        {
            assetBundle = AssetBundle.LoadFromFile($"{KSPUtil.ApplicationRootPath}GameData/CustomOverlay/OverlayShader-windows.unity3d");

            instance = this;

            overlaySize = new Vector2(3440, 256);
            renderTexture = new RenderTexture((int)overlaySize.x, (int)overlaySize.y, 32);
        }

        void Start()
        {
            reload();
        }

        /// <summary>
        /// position is relativ on the rendertexture
        /// </summary>
        /// <param name="text"></param>
        /// <param name="position">relativ position on the rendertexture</param>
        /// <returns></returns>
        public TextMeshProUGUI CreateText(string text, Vector3 position, float fontSize = 1)
        {
            GameObject textObj = new GameObject("TextMeshPro");
            textObj.transform.SetParent(GameObject.Find("Text Canvas").transform);
            textObj.layer = LayerMask.NameToLayer("UI"); // Set the text to the "UI" layer

            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = fontSize;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            RectTransform textRect = tmp.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(overlaySize.x, overlaySize.y); // Match render texture size
            textRect.localPosition = new Vector3(position.x * overlaySize.x - overlaySize.x / 2, position.y * overlaySize.y - overlaySize.y / 2);

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
            gaugeMaterial.SetFloat("_AspectRatio", overlaySize.x / overlaySize.y);


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

            Circles = new List<circle>();
            Rectangles = new List<rectangle>();
            barGauges = new List<barGauge>();

            lfGauge = new barGauge("LF", 0, new Vector2(0.2f, 0.15f), new Vector2(0.1f, 0.05f), new Vector4(0.35f, 0.35f, 0.35f, 1), new Vector4(1, 1, 1, 1), new Vector4(0.1f, 0.1f, 0.1f, 1f));
            oxGauge = new barGauge("OX", 0, new Vector2(0.2f, 0.3f), new Vector2(0.1f, 0.05f), new Vector4(0.35f, 0.35f, 0.35f, 1), new Vector4(1, 1, 1, 1), new Vector4(0.1f, 0.1f, 0.1f, 1f));
            barGauges.Add(lfGauge);
            barGauges.Add(oxGauge);

            //speedGauge = new circleGauge("m/s", "Speed", 0, 320, 0, 1, new Vector2(0.05f, 0.45f), new Vector2(0.4f, 0.4f), new Vector4(255, 255, 255, 1));
            //throttleGauge = new circleGauge("%", "Throttle", 0, 100, 0, 0, new Vector2(0.15f, 0.45f), new Vector2(0.4f, 0.4f), new Vector4(255, 255, 255, 1));
            //machGauge = new circleGauge("M", "Mach", 0, 3, 0, 3, new Vector2(0.25f, 0.45f), new Vector2(0.4f, 0.4f), new Vector4(255, 255, 255, 1));
            //gGauge = new circleGauge("G", "G-Forces", 0, 5, 0, 3, new Vector2(0.95f, 0.45f), new Vector2(0.4f, 0.4f), new Vector4(255, 255, 255, 1));

            // Superheavy engines
            Vector2 midPoint = new Vector2(0.1f, 0.5f);
            float circleSize = 0.1f;
            float innerSize = 0.3f;
            float middleSize = 0.5f;
            float outerSize = 0.8f;

            circleSymetrie(midPoint, innerSize, 0, 3).ForEach(v => Circles.Add(new circle(v, new Vector2(circleSize, 0), new Vector4(1, 1, 1, 1), 0, 360, 360)));
            circleSymetrie(midPoint, middleSize, 0, 10).ForEach(v => Circles.Add(new circle(v, new Vector2(circleSize, 0), new Vector4(1, 1, 1, 1), 0, 360, 360)));
            circleSymetrie(midPoint, outerSize, 9, 20).ForEach(v => Circles.Add(new circle(v, new Vector2(circleSize, 0), new Vector4(1, 1, 1, 1), 0, 360, 360)));



            List<Vector4> vectorCircles = new List<Vector4> { };  // Each (x, y, radius, innerRadius)
            List<Vector4> circleFill = new List<Vector4> { }; // Each fill amount
            List<Vector4> colors = new List<Vector4> { };   // Each (r, g, b, alpha)

            foreach (circle circle in Circles)
            {
                vectorCircles.Add(new Vector4(circle.position.x, circle.position.y, circle.size.x, circle.size.y));
                circleFill.Add(new Vector4(circle.start, circle.end, circle.deg, 0));
                colors.Add(circle.color);
            }

            gaugeMaterial.SetInt("_CircleCount", vectorCircles.Count);
            if (vectorCircles.Count > 0)
            {
                gaugeMaterial.SetVectorArray("_Circles", vectorCircles.ToArray());
                gaugeMaterial.SetVectorArray("_CircleFill", circleFill.ToArray());
                gaugeMaterial.SetVectorArray("_CircleColors", colors.ToArray());
            }

            List<Vector4> rectangles = new List<Vector4> { };
            List<float> rectangleRotations = new List<float> { };
            List<Vector4> rectangleColors = new List<Vector4> { };

            foreach (rectangle rect in Rectangles)
            {
                rectangles.Add(new Vector4(rect.position.x, rect.position.y, rect.size.x, rect.size.y));
                rectangleRotations.Add(rect.getRadians());
                rectangleColors.Add(rect.color);
            }

            gaugeMaterial.SetInt("_RectangleCount", rectangles.Count);
            if (rectangles.Count > 0)
            {
                gaugeMaterial.SetVectorArray("_Rectangles", rectangles.ToArray());
                gaugeMaterial.SetFloatArray("_RectangleRotation", rectangleRotations.ToArray());
                gaugeMaterial.SetVectorArray("_RectangleColors", rectangleColors.ToArray());
            }


            List<Vector4> barPositions = new List<Vector4> { };
            List<float> barRounding = new List<float> { };
            List<Vector4> barStartColor = new List<Vector4> { };
            List<Vector4> barEndColor = new List<Vector4> { };

            barGauges.ForEach(barGauge =>
            {
                Vector2 primaryPosition = barGauge.primaryPosition;
                Vector2 primarySize = barGauge.primarySize;

                barPositions.Add(new Vector4(primaryPosition.x, primaryPosition.y, primarySize.x, primarySize.y));
                barRounding.Add(barGauge.Rounding);
                barStartColor.Add(barGauge.StartColor);
                barEndColor.Add(barGauge.EndColor);

                barPositions.Add(new Vector4(barGauge.Position.x, barGauge.Position.y, barGauge.Size.x, barGauge.Size.y));
                barRounding.Add(0.02f);
                barStartColor.Add(barGauge.BackgroundColor);
                barEndColor.Add(barGauge.BackgroundColor);
            });

            gaugeMaterial.SetInt("_barCount", barPositions.Count);
            if (barPositions.Count > 0)
            {
                gaugeMaterial.SetVectorArray("_barStartColor", barStartColor.ToArray());
                gaugeMaterial.SetVectorArray("_barEndColor", barEndColor.ToArray());
                gaugeMaterial.SetVectorArray("_barPosition", barPositions.ToArray());
                gaugeMaterial.SetFloatArray("_barRounding", barRounding.ToArray());
            }

            RenderTexture.active = renderTexture;
            GL.Clear(true, true, new Color(0, 0, 0, 0.5f)); // Ensure transparency

            Graphics.Blit(null, renderTexture, gaugeMaterial);

            RenderTexture.active = null;
        }

        public List<Vector2> circleSymetrie(Vector2 midPoint, float size, float startDegreeOffset, int count)
        {
            int degreeOffset = 360 / count;
            List<Vector2> points = new List<Vector2>();

            size /= 2;

            for (int i = 0; i < count; i++)
            {
                points.Add(new Vector2((float)(size * Math.Cos(degreeOffset * i + startDegreeOffset)) + midPoint.x, (float)(size * Math.Sin(degreeOffset * i + startDegreeOffset)) + midPoint.y));
            }

            return points;
        }

        public class circle
        {
            public Vector2 position;
            public Vector2 size;
            public Vector4 color;
            public float startDegree;
            public float endDegree;
            public float degree;

            public float start { get { return startDegree / 360; } }
            public float end { get { return endDegree / 360; } }
            public float deg { get { return degree / 360; } }

            public circle(Vector2 position, Vector2 size, Vector4 color, float startDegree, float endDegree, float degree)
            {
                this.position = position;
                this.size = size;
                this.color = color;
                this.startDegree = startDegree;
                this.endDegree = endDegree;
                this.degree = degree;
            }
        }

        public class rectangle
        {
            public Vector2 position;
            public Vector2 size;
            public float degree;
            public Vector4 color;

            public float getRadians()
            {
                return (float)(degree * (Math.PI / 180));
            }

            public rectangle(Vector2 position, Vector2 size, float degree, Vector4 color)
            {
                this.position = position;
                this.size = size;
                this.degree = degree;
                this.color = color;
            }
        }

        public class circleGauge
        {
            string unitName;
            string text;

            float min;
            float max;
            float current;
            int decimals;

            public float Max { get { return max; } set { max = value; } }

            Vector2 position;
            Vector2 size;
            Vector4 color;

            TextMeshProUGUI valueTextMesh;
            TextMeshProUGUI descTextMesh;

            circle innerCircle;
            circle outerCircle;

            rectangle endcapLeft;
            rectangle endcapRight;

            private bool disabled;

            Vector2? innerCircleSave;
            Vector2? outerCircleSave;
            Vector2? endcapLeftSave;
            Vector2? endcapRightSave;

            public void UpddateValue(float value)
            {
                if (disabled) { return; }
                current = value;
                innerCircle.degree = Math.Min(current / max, 1) * 360;
                valueTextMesh.text = $"{Math.Round(current, decimals)}{unitName}";
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

            public circleGauge(string unitName, string text, float min, float max, float current, int decimals, Vector2 position, Vector2 size, Vector4 color)
            {
                this.unitName = unitName;
                this.text = text;
                this.min = min;
                this.max = max;
                this.current = current;
                this.decimals = decimals;
                this.position = position;
                this.size = size;
                this.color = color;

                valueTextMesh = instance.CreateText($"{Math.Round(current, decimals)}{unitName}", new Vector3(position.x, position.y), 1.4f);
                descTextMesh = instance.CreateText($"{text}", new Vector3(position.x, position.y - 0.3f), 1.1f);

                innerCircle = new circle(position, new Vector2(size.x - 0.04f, size.y - 0.09f), color, 50, 310, Math.Min(current / max, 1) * 360);
                outerCircle = new circle(position, new Vector2(size.x, size.y - 0.02f), color, 50, 310, 360);

                endcapLeft = new rectangle(new Vector2(position.x - 0.0185f, position.y - 0.25f), new Vector2(0.008f, 0.025f), 40, color);
                endcapRight = new rectangle(new Vector2(position.x + 0.0185f, position.y - 0.25f), new Vector2(0.008f, 0.025f), 140, color);

                instance.Circles.Add(innerCircle);
                instance.Circles.Add(outerCircle);

                instance.Rectangles.Add(endcapLeft);
                instance.Rectangles.Add(endcapRight);
            }
        }

        public class barGauge
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

            public barGauge(string name, float percent, Vector2 position, Vector2 size, Vector4 startColor, Vector4 endColor, Vector4 backgroundColor)
            {
                this.name = name;
                this.percent = percent;
                this.position = position;
                this.size = size;
                this.startColor = startColor;
                this.endColor = endColor;
                this.backgroundColor = backgroundColor;

                textMeshProUGUI = instance.CreateText(name, new Vector3(position.x - size.x / 2.0f - 0.015f, position.y));
                //textMeshProUGUI.alignment = TextAlignmentOptions.Right;
            }
        }

        public class resourceInfo
        {
            public float amount;
            public float maxAmount;

            public resourceInfo(float amount, float maxAmount)
            {
                this.amount = amount;
                this.maxAmount = maxAmount;
            }
        }

        void Update()
        {
            //speedGauge.UpddateValue((float)FlightGlobals.ActiveVessel.speed);
            //throttleGauge.UpddateValue(FlightGlobals.ActiveVessel.ctrlState.mainThrottle * 100);
            //machGauge.UpddateValue((float)FlightGlobals.ActiveVessel.mach);
            //gGauge.UpddateValue((float)FlightGlobals.ActiveVessel.geeForce);
            //gGauge.Max = Math.Max(gGauge.Max, (float)FlightGlobals.ActiveVessel.geeForce);

            //SortedDictionary<int, Dictionary<PartResourceDefinition, resourceInfo>> stageResources = new SortedDictionary<int, Dictionary<PartResourceDefinition, resourceInfo>> { };


            //FlightGlobals.ActiveVessel.Parts.ForEach(part =>
            //{
            //    if (!stageResources.ContainsKey(part.inStageIndex))
            //    {
            //        stageResources.Add(part.inStageIndex, new Dictionary<PartResourceDefinition, resourceInfo> { } );
            //    }
            //    foreach (PartResource item in part.Resources.dict.Values)
            //    {
            //        if (stageResources[part.inStageIndex].ContainsKey(item.info))
            //        {
            //            stageResources[part.inStageIndex][item.info].amount += (float)item.amount;
            //            stageResources[part.inStageIndex][item.info].maxAmount += (float)item.maxAmount;
            //        }
            //        else
            //        {
            //            stageResources[part.inStageIndex].Add(item.info, new resourceInfo((float)item.amount, (float)item.maxAmount));
            //        }
            //    }
            //});

            //lfGauge.Percent = stageResources.Max().Value[PartResourceLibrary.Instance.GetDefinition("LiquidFuel")].amount / stageResources.Max().Value[PartResourceLibrary.Instance.GetDefinition("LiquidFuel")].maxAmount;
            //oxGauge.Percent = stageResources.Max().Value[PartResourceLibrary.Instance.GetDefinition("Oxidizer")].amount / stageResources.Max().Value[PartResourceLibrary.Instance.GetDefinition("Oxidizer")].maxAmount;

            //FlightGlobals.ActiveVessel.GetConnectedResourceTotals(PartResourceLibrary.Instance.GetDefinition("LiquidFuel").id, out double lfAmount, out double lfMaxAmount, true);
            //FlightGlobals.ActiveVessel.GetConnectedResourceTotals(PartResourceLibrary.Instance.GetDefinition("Oxidizer").id, out double oxAmount, out double oxMaxAmount, true);

            ARPResourceList rsList = KSPAlternateResourcePanel.KSPAlternateResourcePanel.APIInstance.lstResourcesLastStage;

            lfGauge.Percent = (float)(rsList[PartResourceLibrary.Instance.GetDefinition("LiquidFuel").id].AmountValue / rsList[PartResourceLibrary.Instance.GetDefinition("LiquidFuel").id].MaxAmountValue);
            oxGauge.Percent = (float)(rsList[PartResourceLibrary.Instance.GetDefinition("Oxidizer").id].AmountValue / rsList[PartResourceLibrary.Instance.GetDefinition("Oxidizer").id].MaxAmountValue);


            if (!UIMasterController.Instance.IsUIShowing)
            {

                List<Vector4> barPositions = new List<Vector4> { };
                List<Vector4> barStartColor = new List<Vector4> { };
                List<Vector4> barEndColor = new List<Vector4> { };
                List<float> barRounding = new List<float> { };
                List<float> barThickness = new List<float> { };

                barGauges.ForEach(barGauge =>
                {
                    Vector2 primaryPosition = barGauge.primaryPosition;
                    Vector2 primarySize = barGauge.primarySize;

                    barPositions.Add(new Vector4(primaryPosition.x, primaryPosition.y, primarySize.x, primarySize.y));
                    barRounding.Add(barGauge.Rounding);
                    barStartColor.Add(barGauge.StartColor);
                    barEndColor.Add(barGauge.EndColor);
                    barThickness.Add(barGauge.thickness);

                    barPositions.Add(new Vector4(barGauge.Position.x, barGauge.Position.y, barGauge.Size.x, barGauge.Size.y));
                    barRounding.Add(0.02f);
                    barStartColor.Add(barGauge.BackgroundColor);
                    barEndColor.Add(barGauge.BackgroundColor);
                    barThickness.Add(0.02f);
                });

                gaugeMaterial.SetInt("_barCount", barPositions.Count);
                if (barPositions.Count > 0)
                {
                    gaugeMaterial.SetVectorArray("_barStartColor", barStartColor.ToArray());
                    gaugeMaterial.SetVectorArray("_barEndColor", barEndColor.ToArray());
                    gaugeMaterial.SetVectorArray("_barPosition", barPositions.ToArray());
                    gaugeMaterial.SetFloatArray("_barRounding", barRounding.ToArray());
                    gaugeMaterial.SetFloatArray("_barThickness", barThickness.ToArray());
                }


                if (!spaceReached && FlightGlobals.ActiveVessel.atmDensity == 0)
                {
                    spaceReached = true;
                    //machGauge.disable();
                }
                else if (spaceReached && FlightGlobals.ActiveVessel.atmDensity > 0)
                {
                    spaceReached = false;
                    //machGauge.enable();
                }

                List<Vector4> vectorCircles = new List<Vector4> { };  // Each (x, y, radius, innerRadius)
                List<Vector4> circleFill = new List<Vector4> { }; // Each fill amount
                List<Vector4> colors = new List<Vector4> { };   // Each (r, g, b, alpha)

                foreach (circle circle in Circles)
                {
                    vectorCircles.Add(new Vector4(circle.position.x, circle.position.y, circle.size.x, circle.size.y));
                    circleFill.Add(new Vector4(circle.start, circle.end, circle.deg, 0));
                    colors.Add(circle.color);
                }

                gaugeMaterial.SetInt("_CircleCount", vectorCircles.Count);
                if (vectorCircles.Count > 0)
                {
                    gaugeMaterial.SetVectorArray("_Circles", vectorCircles.ToArray());
                    gaugeMaterial.SetVectorArray("_CircleFill", circleFill.ToArray());
                    gaugeMaterial.SetVectorArray("_CircleColors", colors.ToArray());
                }

                List<Vector4> rectangles = new List<Vector4> { };
                List<float> rectangleRotations = new List<float> { };
                List<Vector4> rectangleColors = new List<Vector4> { };

                foreach (rectangle rect in Rectangles)
                {
                    rectangles.Add(new Vector4(rect.position.x, rect.position.y, rect.size.x, rect.size.y));
                    rectangleRotations.Add(rect.getRadians());
                    rectangleColors.Add(rect.color);
                }

                gaugeMaterial.SetInt("_RectangleCount", rectangles.Count);
                if (rectangles.Count > 0)
                {
                    gaugeMaterial.SetVectorArray("_Rectangles", rectangles.ToArray());
                    gaugeMaterial.SetFloatArray("_RectangleRotation", rectangleRotations.ToArray());
                    gaugeMaterial.SetVectorArray("_RectangleColors", rectangleColors.ToArray());
                }

                uiCamera.enabled = true;

                RenderTexture.active = renderTexture;
                GL.Clear(true, true, new Color(0, 0, 0, 0.5f)); // Ensure transparency

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
            GUI.DrawTexture(new Rect(0, 1440 - overlaySize.y, overlaySize.x, overlaySize.y), renderTexture, ScaleMode.ScaleToFit, true);
        }

        //void RenderText(string text)

        public static UrlDir.UrlConfig[] GetConfigsByName(string name)
        {
            return GameDatabase.Instance.GetConfigs(name);
        }

        private void OnDestroy()
        {
            assetBundle.Unload(true);

        }
    }
}
