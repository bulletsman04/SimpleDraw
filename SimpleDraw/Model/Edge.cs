using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDraw.Model
{
    internal class Edge
    {
        public (Vertex left, Vertex right) ends;
        public List<Restriction> Restrictions { get; set; }
        public Vector2D Vector2D { get; set; }
        public Rectangle Rectangle { get; set; }
        public LineFunction function { get; set; }

        public Edge((Vertex left, Vertex right) ends)
        {
            this.ends = ends;
            Vector2D = new Vector2D(ends.left.vPoint,ends.right.vPoint);
            function = new LineFunction(ends.left.vPoint,ends.right.vPoint);
            int x1 = ends.left.vPoint.X;
            int y1 = ends.left.vPoint.Y;
            int x2 = ends.right.vPoint.X;
            int y2 = ends.right.vPoint.Y;
            int width = Math.Abs(x1 - x2) > 5 ? Math.Abs(x1 - x2) : 6;
            int height = Math.Abs(y1 - y2) > 5 ? Math.Abs(y1 - y2) : 6;
            Rectangle = new Rectangle(Math.Min(x1,x2), Math.Min(y1,y2 ),width, height);
        }

        public bool IsPointClose(Point p)
        {
            if (!Rectangle.Contains(p))
                return false;
            if (function.isVertical == true)
                return true;

            double dist = Math.Abs(function.A * p.X + function.B * p.Y + function.C) / Math.Sqrt(function.A * function.A + function.B*function.B);
            if (dist > 6)
                return false;

            return true;
        }
    }
}
