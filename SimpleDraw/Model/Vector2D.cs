using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDraw.Model
{
    internal class Vector2D
    {
        public Point Start { get; }
        public Point End { get; }
        public (int x, int y) Coordinates { get; }

        public Vector2D(Point start, Point end)
        {
            Start = start;
            End = end;
            Coordinates = (end.X - start.X, end.Y - start.Y);
        }
    }
}
