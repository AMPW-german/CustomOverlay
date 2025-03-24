using System;
using System.Collections.Generic;
using System.Linq;
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
    public class CircleSymetrie
    {
        private List<Circle> circles;
        private KSPActionGroup actionGroup;

        public float radius;
        public float OuterSize;
        public float FilledSize;
        public float EmptySize;

        public void Update()
        {
            if (FlightGlobals.ActiveVessel.ActionGroups[actionGroup])
            {
                Enable();
            }
            else
            {
                Disable();
            }
        }

        public void Enable()
        {
            circles.ForEach(circle => circle.size = new Vector2(OuterSize, FilledSize));
        }

        public void Disable()
        {
            circles.ForEach(circle => circle.size = new Vector2(OuterSize, EmptySize));
        }

        public CircleSymetrie(CustomUI instance, int count, Vector2 midPoint, float Size, float EmptySize, float startDegreeOffset, Vector4 color)
        {
            circles = new List<Circle>();

            int degreeOffset = 360 / count;

            for (int i = 0; i < count; i++)
            {
                Vector2 circlePoint = new Vector2((float)(Size * Math.Sin((degreeOffset * i + startDegreeOffset) * 0.01745329252f) * Settings.OverlaySize.y / Settings.OverlaySize.x) + midPoint.x, (float)(Size * Math.Cos((degreeOffset * i + startDegreeOffset) * 0.01745329252f)) + midPoint.y);
                circles.Add(new Circle(circlePoint, new Vector2(Size, 0), color, 0, 360, 360));
            }

            circles.ForEach(circle => instance.circles.Add(circle));

            this.OuterSize = Size;
            this.EmptySize = EmptySize;
        }

        public CircleSymetrie(CustomUI instance, ConfigNode node)
        {
            circles = new List<Circle>();

            int count = int.Parse(node.GetValue("count"));
            float startDegreeOffset = float.Parse(node.GetValue("startDegreeOffset"));
            Vector2 midPoint = new Vector2(float.Parse(node.GetValue("midPointX")), float.Parse(node.GetValue("midPointY")));
            radius = float.Parse(node.GetValue("radius")) / 2;
            OuterSize = float.Parse(node.GetValue("OuterSize"));
            FilledSize = float.Parse(node.GetValue("FilledSize"));
            EmptySize = float.Parse(node.GetValue("EmptySize"));

            Vector4 color = new Vector4(float.Parse(node.GetValue("colorR")), float.Parse(node.GetValue("colorG")), float.Parse(node.GetValue("colorB")), float.Parse(node.GetValue("colorA")));

            int degreeOffset = 360 / count;

            for (int i = 0; i < count; i++)
            {
                Vector2 circlePoint = new Vector2((float)(radius * Math.Sin((degreeOffset * i + startDegreeOffset) * 0.01745329252f) * Settings.OverlaySize.y / Settings.OverlaySize.x) + midPoint.x, (float)(radius * Math.Cos((degreeOffset * i + startDegreeOffset) * 0.01745329252f)) + midPoint.y);
                circles.Add(new Circle(circlePoint, new Vector2(OuterSize, FilledSize), color, 0, 360, 360));
            }
            circles.ForEach(circle => instance.circles.Add(circle));

            string actionGroupNum = node.GetValue("actionGroup");
            actionGroup = (KSPActionGroup)Enum.Parse(typeof(KSPActionGroup), Enum.GetNames(typeof(KSPActionGroup)).ToList().Where(x => x.Contains(actionGroupNum)).First());
            instance.symertrieCircles.Add(this);
        }
    }
}
