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

        public Point EdgeMiddle => new Point((ends.left.vPoint.X + ends.right.vPoint.X)/2, (ends.left.vPoint.Y + ends.right.vPoint.Y) / 2);
        private Rectangle _rectangle;
        public Rectangle Rectangle {
            get
            {
                int x1 = ends.left.vPoint.X;
                int y1 = ends.left.vPoint.Y;
                int x2 = ends.right.vPoint.X;
                int y2 = ends.right.vPoint.Y;
                int width = Math.Abs(x1 - x2) > 5 ? Math.Abs(x1 - x2) : 6;
                int height = Math.Abs(y1 - y2) > 5 ? Math.Abs(y1 - y2) : 6;
                _rectangle = new Rectangle(Math.Min(x1, x2), Math.Min(y1, y2), width, height);
                return _rectangle;
            } }

        private LineFunction _lineFunction;

        public LineFunction function
        {
            get
            {
                _lineFunction = new LineFunction(ends.left.vPoint, ends.right.vPoint);
                return _lineFunction;
            }
        }

        public double Length
        {
            get
            {
                Point p1 = ends.left.vPoint;
                Point p2 = ends.right.vPoint;
                return Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2));
            }
        }

        public Edge((Vertex left, Vertex right) ends)
        {
            this.ends = ends;
            Vector2D = new Vector2D(ends.left.vPoint,ends.right.vPoint);
         
            Restrictions = new List<Restriction>(1);
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

        public bool PreserveRestrictions(Vertex moved, Vector2D vector, bool left)
        {
            bool changed = false;
            foreach (Restriction restriction in Restrictions)
            {
                if (restriction.preserveRestriction(moved, vector,left) == true)
                    changed = true;
            }

            return changed;
        }

        public Point? FindLengthPoint(double length)
        {
            double a = -function.A;
            double b = -function.C;
            double a2 = a * a;
            double b2 = b * b;
            int x2 = ends.right.vPoint.X;
            int y2 = ends.right.vPoint.Y;
            int x1 = ends.left.vPoint.X;
            int y1 = ends.left.vPoint.Y;

            double A = a2 + 1;
            double B = 2 * a * b - 2 * y2 * a - 2 * x2;
            double C = x2 * x2 + y2 * y2 - 2 * y2 * b + b2 - length * length;

            double delta = B * B - 4 * A * C;

            int xR1 = (int)(Math.Round(-B - Math.Sqrt(delta)) / (2 * A));
            int yR1 = (int)(Math.Round(a * xR1 + b));
            int xR2 = (int)(Math.Round(-B + Math.Sqrt(delta)) / (2 * A));
            int yR2 = (int)(Math.Round(a * xR2 + b));

            
            double dist1 = Math.Sqrt(Math.Pow(x1 - xR1, 2) + Math.Pow(y1 - yR1, 2));
            double dist2 = Math.Sqrt(Math.Pow(x1 - xR2, 2) + Math.Pow(y1 - yR2, 2));

            return dist1 <= dist2 ? new Point((int)xR1, (int)yR1) : new Point((int)xR2, (int)yR2);

        }
    }
}
