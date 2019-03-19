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
                int width = Math.Abs(x1 - x2) > 7 ? Math.Abs(x1 - x2) : 8;
                int height = Math.Abs(y1 - y2) > 7 ? Math.Abs(y1 - y2) : 8;
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
            if (dist > 8)
                return false;

            return true;
        }

        public bool PreserveRestrictions(Vertex moved, Vertex toCheck, Vector2D vector)
        {
            bool changed = false;
            foreach (Restriction restriction in Restrictions)
            {
                if (restriction.preserveRestriction(moved,toCheck, vector) == true)
                    changed = true;
            }

            return changed;
        }

        public Point? FindLengthPoint(double length)
        {
            int x2 = ends.right.vPoint.X;
            int y2 = ends.right.vPoint.Y;
            int x1 = ends.left.vPoint.X;
            int y1 = ends.left.vPoint.Y;

            if (function.B == 0)
            {
                return y2 < y1 ? new Point(x2, (int) (y2 + length)) : new Point(x2, (int) (y2 - length));
            }
            double a = -function.A;
            double b = -function.C;
            double a2 = a * a;
            double b2 = b * b;
          
            double A = a2 + 1;
            double B = 2 * a * b - 2 * y2 * a - 2 * x2;
            double C = x2 * x2 + y2 * y2 - 2 * y2 * b + b2 - length * length;

            double delta = B * B - 4 * A * C;

            int xR1 = (int)Math.Round((-B - Math.Sqrt(delta)) / (2 * A));
            int yR1 = (int)Math.Round(a * (-B - Math.Sqrt(delta)) / (2 * A) + b);
            int xR2 = (int)Math.Round((-B + Math.Sqrt(delta)) / (2 * A));
            int yR2 = (int)Math.Round(a * (-B + Math.Sqrt(delta)) / (2 * A) + b);

            
            double dist1 = Math.Sqrt(Math.Pow(x1 - xR1, 2) + Math.Pow(y1 - yR1, 2));
            double dist2 = Math.Sqrt(Math.Pow(x1 - xR2, 2) + Math.Pow(y1 - yR2, 2));

            return  dist1 <= dist2 ? new Point(xR1, yR1) : new Point(xR2, yR2);

        }

        public static bool EdgesIntersection(Edge e1, Edge e2)
        {
            Point p1 = e1.ends.left.vPoint;
            Point p2 = e1.ends.right.vPoint;
            Point p3 = e2.ends.left.vPoint;
            Point p4 = e2.ends.right.vPoint;

            double d1 = CrossProduct(new Point(p4.X - p3.X, p4.Y - p3.Y), new Point(p1.X - p3.X, p1.Y - p3.Y));
            double d2 = CrossProduct(new Point(p4.X - p3.X, p4.Y - p3.Y), new Point(p2.X - p3.X, p2.Y - p3.Y));
            double d3 = CrossProduct(new Point(p2.X - p1.X, p2.Y - p1.Y), new Point(p3.X - p1.X, p3.Y - p1.Y));
            double d4 = CrossProduct(new Point(p2.X - p1.X, p2.Y - p1.Y), new Point(p4.X - p1.X, p4.Y - p1.Y));

            double d12 = d1 * d2;
            double d34 = d3 * d4;

            if (d12 > 0 || d34 > 0)
                return false;
            if (d12 < 0 || d34 < 0)
                return true;

            return OnRectangle(p1, p3, p4) || OnRectangle(p2, p3, p4) || OnRectangle(p3, p1, p2) ||
                   OnRectangle(p4, p1, p2);

        }

        private static double CrossProduct(Point p1, Point p2)
        {
            return p1.X * p2.Y - p2.X * p1.Y;
        }

        private static bool OnRectangle(Point q, Point p1, Point p2)
        {
            return Math.Min(p1.X, p2.X) <= q.X && q.X <= Math.Max(p1.X, p2.X) && Math.Min(p1.Y, p2.Y) <= q.Y &&
                   q.Y <= Math.Max(p1.Y, p2.Y);
        }
    }
}
