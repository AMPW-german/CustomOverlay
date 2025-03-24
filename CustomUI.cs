using System.Collections.Generic;
using TMPro;
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
    public class CustomUI
    {
        public string name;

        public Vector4 backgroundColor = new Vector4(0, 0, 0, 0.5f);

        public List<Circle> circles;
        public List<CircleSymetrie> symertrieCircles;
        public List<Rectangle> rectangles;
        public List<CircleGauge> circleGauges;
        public List<BarGauge> barGauges;
        public List<Text> texts;
        public List<TextMeshProUGUI> textMeshGUIs;

        public void Update()
        {
            symertrieCircles.ForEach(circles => circles.Update());
            circleGauges.ForEach(gauge => gauge.updateValue());
            barGauges.ForEach(gauge => gauge.updateValue());
            circles.ForEach(circle => circle.updateValue());
            texts.ForEach(text => text.updateValue());

        }

        public void GetCircleVectors(out Vector4[] vectorCircles, out Vector4[] circleFill, out Vector4[] colors)
        {
            vectorCircles = new Vector4[100];
            circleFill = new Vector4[100];
            colors = new Vector4[100];


            Circle[] circleArray = new Circle[100];

            for (int i = 0; i < circles.Count; i++)
            {
                circleArray[i] = circles[i];
            }

            for (int i = circles.Count; i < 100; i++)
            {
                circleArray[i] = Circle.getEmpty();
            }

            for (int i = 0; i < 100; i++)
            {
                vectorCircles[i] = new Vector4(circleArray[i].position.x, circleArray[i].position.y, circleArray[i].size.x, circleArray[i].size.y);
                circleFill[i] = new Vector4(circleArray[i].start, circleArray[i].end, circleArray[i].deg, 0);
                colors[i] = circleArray[i].color;
            }
        }

        public void GetVectorRectangles(out Vector4[] vectorRectangles, out float[] rotations, out Vector4[] colors)
        {
            vectorRectangles = new Vector4[50];
            rotations = new float[50];
            colors = new Vector4[50];


            Rectangle[] rectangleArray = new Rectangle[50];

            for (int i = 0; i < rectangles.Count; i++)
            {
                rectangleArray[i] = rectangles[i];
            }

            for (int i = rectangles.Count; i < 50; i++)
            {
                rectangleArray[i] = Rectangle.getEmpty();
            }

            for (int i = 0; i < 50; i++)
            {
                vectorRectangles[i] = new Vector4(rectangleArray[i].position.x, rectangleArray[i].position.y, rectangleArray[i].size.x, rectangleArray[i].size.y);
                rotations[i] = rectangleArray[i].getRadians();
                colors[i] = rectangleArray[i].color;
            }
        }

        public void GetVectorBars(out Vector4[] vectorBars, out Vector4[] startColor, out Vector4[] endColor, out float[] barRounding, out float[] barThickness)
        {
            vectorBars = new Vector4[40];
            startColor = new Vector4[40];
            endColor = new Vector4[40];
            barRounding = new float[40];
            barThickness = new float[40];

            BarGauge[] barArray = new BarGauge[20];

            for (int i = 0; i < barGauges.Count; i++)
            {
                barArray[i] = barGauges[i];
            }

            for (int i = barGauges.Count; i < 20; i++)
            {
                barArray[i] = BarGauge.GetEmpty();
            }

            int offset = 20;
            for (int i = 0; i < 20; i++)
            {
                vectorBars[i] = new Vector4(barArray[i].primaryPosition.x, barArray[i].primaryPosition.y, barArray[i].primarySize.x, barArray[i].primarySize.y);
                startColor[i] = barArray[i].StartColor;
                endColor[i] = barArray[i].EndColor;
                barRounding[i] = barArray[i].Rounding;
                barThickness[i] = barArray[i].thickness;

                vectorBars[i + offset] = new Vector4(barArray[i].Position.x, barArray[i].Position.y, barArray[i].Size.x, barArray[i].Size.y);
                startColor[i + offset] = barArray[i].BackgroundColor;
                endColor[i + offset] = barArray[i].BackgroundColor;
                barRounding[i + offset] = barArray[i].Rounding;
                barThickness[i + offset] = barArray[i].thickness;
            }
        }



        public CustomUI()
        {
            circles = new List<Circle> { };
            symertrieCircles = new List<CircleSymetrie> { };
            rectangles = new List<Rectangle> { };
            circleGauges = new List<CircleGauge> { };
            barGauges = new List<BarGauge> { };
            texts = new List<Text> { };
            textMeshGUIs = new List<TextMeshProUGUI> { };
        }
    }
}
