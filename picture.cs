using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using UnityEngine;

namespace CustomOverlay
{
    public class picture
    {
        public static Texture2D CreateAtlas(List<picture> pictures, out Vector4[] uvRects)
        {
            int atlasSizeX = pictures.Max(p => p.Img.width);
            int atlasSizeY = pictures.Sum(p => p.Img.height);

            Texture2D atlas = new Texture2D(atlasSizeX, atlasSizeY, TextureFormat.RGBA32, false);
            uvRects = new Vector4[pictures.Count];

            int yOffset = 0;

            for (int i = 0; i < pictures.Count; i++)
            {
                Texture2D tex = pictures[i].Img;

                atlas.SetPixels(0, yOffset, tex.width, tex.height, tex.GetPixels());
                atlas.Apply();

                uvRects[i] = new Vector4(
                    0,
                    (float)yOffset / atlasSizeY, // yMin
                    (float)tex.width / atlasSizeX, // xMax
                    (float)(yOffset + tex.height) / atlasSizeY // yMax
                );

                yOffset += tex.height;
            }

            atlas.Apply();
            return atlas;
        }

        public static picture getEmpty()
        {
            return new picture();
        }


        public enum pictureMode
        {
            None,
            rotationX,
            rotationY,
            Pitch,
            heading,
        }

        public pictureMode mode;
        public Vector2 position;
        public float size;
        public float rotation;
        public float radianRotation { get { return rotation * Mathf.Deg2Rad; } }


        /// <summary>
        /// The code to get the pitch was derived from MechJeb2's implementation for getting the vessel's surface relative rotation.
        /// </summary>
        public void update()
        {
            Quaternion surfaceRotation = GetSurfaceRotation(FlightGlobals.ActiveVessel);
            float pitch = 0;
            switch (mode)
            {
                case pictureMode.rotationX:
                    pitch = (surfaceRotation.eulerAngles.x > 180.0f
                    ? 360.0f - surfaceRotation.eulerAngles.x
                    : -surfaceRotation.eulerAngles.x);
                    rotation = ((surfaceRotation.eulerAngles.y < 180 ?
                    -pitch
                         :
                    180 + pitch
                    ) + 90) % 360; // The image is -90° rotated
                    break;
                case pictureMode.rotationY:
                    surfaceRotation = GetSurfaceRotation(FlightGlobals.ActiveVessel);
                    pitch = (surfaceRotation.eulerAngles.x < 90 || surfaceRotation.eulerAngles.x > 270
                    ? 360.0f - surfaceRotation.eulerAngles.x
                    : -surfaceRotation.eulerAngles.x);
                    rotation = ((surfaceRotation.eulerAngles.y < 180 ?
                    -pitch
                         :
                    180 + pitch
                    ) + 90) % 360; // The image is -90° rotated
                    break;
                case pictureMode.Pitch:
                    rotation = (surfaceRotation.eulerAngles.x + 90) % 360;
                    break;
                case pictureMode.heading:
                    rotation = FlightDataManager.FlightData(flightData.heading);
                    break;
                    //case pictureMode.Orientation:

            }
        }

        private Quaternion GetSurfaceRotation(Vessel vessel)
        {
            // This code was derived from MechJeb2's implementation for getting the vessel's surface relative rotation.
            Vector3 centreOfMass = vessel.CoMD;
            Vector3 up = (centreOfMass - vessel.mainBody.position).normalized;
            Vector3 north = Vector3.ProjectOnPlane((vessel.mainBody.position + vessel.mainBody.transform.up * (float)vessel.mainBody.Radius) - centreOfMass, up).normalized;

            return Quaternion.Inverse(Quaternion.Euler(90.0f, 0.0f, 0.0f) * Quaternion.Inverse(vessel.transform.rotation) * Quaternion.LookRotation(north, up));
        }

        private Texture2D img;
        public Texture2D Img
        {
            get
            {
                return img;
            }
        }

        private string path;

        private picture()
        {
            this.path = "";
            this.position = Vector2.zero;
            this.size = 0.0f;
            this.rotation = 0.0f;
            this.img = new Texture2D(1, 1);
            this.img.SetPixel(0, 0, Color.clear);
            this.img.Apply();
        }

        public picture(ConfigNode node)
        {
            path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) + "/" + node.GetValue("path");
            float.TryParse(node.GetValue("positionX"), out position.x);
            float.TryParse(node.GetValue("positionY"), out position.y);
            float.TryParse(node.GetValue("size"), out size);
            if (node.HasValue("mode"))
            {
                string modeText = node.GetValue("mode");

                switch (modeText)
                {
                    case "rotationX":
                        mode = pictureMode.rotationX;
                        break;
                    case "rotationY":
                        mode = pictureMode.rotationY;
                        break;
                    case "Pitch":
                        mode = pictureMode.Pitch;
                        break;
                    case "heading":
                        mode = pictureMode.heading;
                        break;
                    default:
                        mode = pictureMode.None;
                        break;
                }
            }
            else
            {
                mode = pictureMode.None;
                float.TryParse(node.GetValue("rotation"), out rotation);
            }

            if (File.Exists(path))
            {
                img = new Texture2D(2, 2);
                img.LoadImage(File.ReadAllBytes(path));
                img.Apply();
            }
        }
    }
}
