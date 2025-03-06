using System;
using System.Linq;
using TMPro;
using UnityEngine;

namespace CustomOverlay
{
    [KSPAddon(KSPAddon.Startup.Flight, false)]
    public class Overlay : MonoBehaviour
    {
        private Shader gaugeShader;
        private Material gaugeMaterial;
        private RenderTexture renderTexture;
        public Vector4[] circles;  // Each (x, y, radius, innerRadius)
        public Vector4[] circleFill; // Each fill amount
        public Vector4[] colors;   // Each (r, g, b, alpha)
        private Vector2 overlaySize;

        private TextMeshProUGUI tmpText1;
        private TextMeshProUGUI tmpText2;
        private Camera uiCamera;

        private void OnEnable()
        {
            overlaySize = new Vector2(3440, 256);

            var assetBundle = AssetBundle.LoadFromFile($"{KSPUtil.ApplicationRootPath}GameData/CustomOverlay/OverlayShader-windows.unity3d");
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

            circles = new Vector4[]
            {
                new Vector4(0.1f, 0.4f, 0.3f, 0.25f), // Circle 1
                new Vector4(0.1f, 0.4f, 0.34f, 0.315f), // Circle 1
            };

            circleFill = new Vector4[]
            {
                new Vector4(0.15f, 0.85f, 0.5f, 1), // Circle 1 fill amount
                new Vector4(0.15f, 0.85f, 1, 1), // Circle 1 fill amount
            };

            // Define their colors (r, g, b, alpha)
            colors = new Vector4[]
            {
                new Vector4(1, 1, 1, 1),
                new Vector4(1, 1, 1, 1),
            };

            // Create a material using the shader
            gaugeMaterial = new Material(gaugeShader);
            // Pass data to shader
            gaugeMaterial.SetInt("_CircleCount", circles.Length);
            gaugeMaterial.SetVectorArray("_Circles", circles);
            gaugeMaterial.SetVectorArray("_CircleFill", circleFill);
            gaugeMaterial.SetVectorArray("_CircleColors", colors);
            gaugeMaterial.SetFloat("_AspectRatio", overlaySize.x / overlaySize.y);

            renderTexture = new RenderTexture((int)overlaySize.x, (int)overlaySize.y, 32);

            RenderTexture.active = renderTexture;
            GL.Clear(true, true, new Color(0, 0, 0, 0.85f)); // Ensure transparency

            Graphics.Blit(null, renderTexture, gaugeMaterial);

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

            // Create the first TextMeshProUGUI
            tmpText1 = CreateText("Hello, RenderTexture!", new Vector3(-1376, -40, 0));

            // Create the second TextMeshProUGUI
            tmpText2 = CreateText("Second Text Here", new Vector3(-1376, -60, 0)); // Positioned on the right side


            RenderTexture.active = null;
        }

        private TextMeshProUGUI CreateText(string text, Vector3 position)
        {
            GameObject textObj = new GameObject("TextMeshPro");
            textObj.transform.SetParent(GameObject.Find("Text Canvas").transform);
            textObj.layer = LayerMask.NameToLayer("UI"); // Set the text to the "UI" layer

            TextMeshProUGUI tmp = textObj.AddComponent<TextMeshProUGUI>();
            tmp.text = text;
            tmp.fontSize = 0.8f;
            tmp.alignment = TextAlignmentOptions.Center;
            tmp.color = Color.white;

            RectTransform textRect = tmp.GetComponent<RectTransform>();
            textRect.sizeDelta = new Vector2(3440, 256); // Match render texture size
            textRect.localPosition = position;

            return tmp;
        }

        void Start()
        {

        }

        void Update()
        {
            circleFill[0][2] = FlightGlobals.ActiveVessel.ctrlState.mainThrottle;
            gaugeMaterial.SetVectorArray("_CircleFill", circleFill);

            //gaugeMaterial.SetFloat("_FillAmount", 0.75f); // 75% full
            //gaugeMaterial.SetColor("_Color", Color.green); // Green gauge

            RenderTexture.active = renderTexture;
            GL.Clear(true, true, new Color(0, 0, 0, 0.85f)); // Ensure transparency

            Graphics.Blit(null, renderTexture, gaugeMaterial);

            tmpText1.text = $"{Math.Round(FlightGlobals.ActiveVessel.ctrlState.mainThrottle * 100, 1)}%";
            tmpText2.text = $"Throttle";

            RenderTexture.active = null;
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
    }
}
