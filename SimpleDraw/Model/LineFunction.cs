﻿using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDraw.Model
{
    internal class LineFunction
    {
        public double A { get; set; }
        public double B { get; set; }
        public double C { get; set; }

        public double a => -A;
        public double c => -C;
        public bool isVertical { get; set; }

        public float angle => (float)Math.Atan(B);

        public LineFunction()
        {
        }

        public LineFunction(Point p1, Point p2)
        {
            double x1 = p1.X;
            double y1 = p1.Y;
            double x2 = p2.X;
            double y2 = p2.Y;

            if (x2 - x1 == 0)
            {
                isVertical = true;
            }
            else
            {
                isVertical = false;
                A = -((y2 - y1) / (x2 - x1));
                B = 1;
                C = -(y1 - A* (-1) * x1);
            }

        }


        public LineFunction GetPLine(Point p)
        {
            LineFunction pLine = new LineFunction
            {
                A = -1 / A,
                B = 1
            };
            pLine.C = -(p.Y - pLine.a * p.X);

            return pLine;
        }


        
    }
}
