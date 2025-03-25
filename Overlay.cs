using KSP.UI.Screens;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using ToolbarControl_NS;
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
        public int activeUIIndex = 0;

        private Camera uiCamera;

        public static Overlay instance;

        ToolbarControl toolbarControl;

        protected void Awake()
        {
            string filePath = $"{KSPUtil.ApplicationRootPath}GameData/CustomOverlay/OverlayShader";
            if (Application.platform == RuntimePlatform.LinuxPlayer || (Application.platform == RuntimePlatform.WindowsPlayer && SystemInfo.graphicsDeviceVersion.StartsWith("OpenGL")))
            {
                filePath += "-linux.unity3d";
            }
            else if (Application.platform == RuntimePlatform.WindowsPlayer)
            {
                filePath += "-windows.unity3d";
            }
            else if (Application.platform == RuntimePlatform.OSXPlayer)
            {
                filePath += "-macosx.unity3d";
            }
            
            assetBundle = AssetBundle.LoadFromFile(filePath);

            instance = this;

            UIs = new List<CustomUI> { };
            activeUI = new CustomUI();

            Settings.Load();

            renderTexture = new RenderTexture(Settings.OverlaySize.x, Settings.OverlaySize.y, 32);
        }

        void Start()
        {
            CreateButtonIcon();

            GameEvents.onShowUI.Add(showUi);
            GameEvents.onHideUI.Add(hideUI);

            reload();
        }

        private void CreateButtonIcon()
        {
            toolbarControl = gameObject.AddComponent<ToolbarControl>();
            toolbarControl.AddToAllToolbars(NextUI, NextUI,
                ApplicationLauncher.AppScenes.FLIGHT,
                "Customoverlay_NS",
                "CustomOverlayButton",
                "CustomOverlay/UIIcon",
                "CustomOverlay/UIIcon",
                "CustomOverlay"
            );
        }

        private void NextUI()
        {
            activeUIIndex++;
            activeUIIndex %= UIs.Count;
            setActiveUI(UIs[activeUIIndex]);
            ScreenMessages.PostScreenMessage($"Now active: {activeUI.name}", 10f, ScreenMessageStyle.UPPER_RIGHT);
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

            switch (alignment)
            {
                case textAlignment.left:
                    textRect.localPosition = new Vector2(position.x * Settings.OverlaySize.x - Settings.OverlaySize.x / 2 + tmp.preferredWidth * Settings.textSizeMultiplier, position.y * Settings.OverlaySize.y - Settings.OverlaySize.y / 2);
                    break;
                case textAlignment.center:
                    textRect.localPosition = new Vector2(position.x * Settings.OverlaySize.x - Settings.OverlaySize.x / 2, position.y * Settings.OverlaySize.y - Settings.OverlaySize.y / 2);
                    break;
                case textAlignment.right:
                    textRect.localPosition = new Vector2(position.x * Settings.OverlaySize.x - Settings.OverlaySize.x / 2 - tmp.preferredWidth * Settings.textSizeMultiplier, position.y * Settings.OverlaySize.y - Settings.OverlaySize.y / 2);
                    break;
            }

            Debug.Log($"Text Position: {textRect.anchoredPosition}"); // Debugging

            return tmp;
        }

        public void setActiveUI(CustomUI ui)
        {
            UIs.Where(u => u != ui).ToList().ForEach(u => u.textMeshGUIs.ForEach(t => t.alpha = 0));
            ui.textMeshGUIs.ForEach(t => t.alpha = 1);
            activeUI = ui;
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
            gaugeMaterial.SetFloat("_AspectRatio", Settings.OverlaySize.x / (float)Settings.OverlaySize.y);


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
            canvasRect.sizeDelta = new Vector2(Settings.OverlaySize.x, Settings.OverlaySize.y); // Match render texture size

            UIs = Settings.LoadUIs();
            setActiveUI(UIs.First());

            RenderTexture.active = renderTexture;
            GL.Clear(true, true, new Color(0, 0, 0, 0.5f)); // Ensure transparency

            Graphics.Blit(null, renderTexture, gaugeMaterial);

            RenderTexture.active = null;
        }

        void Update()
        {
            if (Settings.ShowCustomUI)
            {
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
            GUI.DrawTexture(new Rect(Settings.ScreenSize.x - Settings.OverlaySize.x, Settings.ScreenSize.y - Settings.OverlaySize.y, Settings.OverlaySize.x, Settings.OverlaySize.y), renderTexture, ScaleMode.ScaleAndCrop, true);
        }

        public static UrlDir.UrlConfig[] GetConfigsByName(string name)
        {
            return GameDatabase.Instance.GetConfigs(name);
        }

        private void OnDestroy()
        {
            toolbarControl.OnDestroy();
            Destroy(toolbarControl);

            GameEvents.onShowUI.Remove(showUi);
            GameEvents.onHideUI.Remove(hideUI);

            assetBundle.Unload(true);
        }
    }
}
