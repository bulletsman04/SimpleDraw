using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDraw.Model
{
    internal class LengthRestriciton: Restriction
    {
       
        private double _length;

        public LengthRestriciton( double length)
        {
            _length = length;

        }

        public override bool isResticted(Vertex moved, Vertex toCheck, Vector2D vector)
        {
            Point p1 = moved.MoveTo.Value;
            Point p2 = toCheck.vPoint;
            return Math.Abs(Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2)) - _length) <= Math.Sqrt(2)/2;

        }

        public override bool preserveRestriction(Vertex moved, Vertex toCheck, Vector2D vector)
        {
            if (isResticted(moved,toCheck, vector))
                return false;

            Edge next;
            Vertex nextV;

            if (moved.edges.left == toCheck.edges.right)
            {
                next = toCheck.edges.left;
                nextV = next.ends.left;
            }
            else
            {
                next = toCheck.edges.right;
                nextV = next.ends.right;
            }

            if (next.Restrictions.Count != 0 && next.Restrictions[0] is VerticalRestriction)
            {
                if (Math.Abs(moved.MoveTo.Value.X - toCheck.vPoint.X) <= _length)
                {
                    Point possible =  FindPointOnVertical(toCheck.vPoint, moved.MoveTo.Value).Value;
                    if (!(possible.Y < 0 || possible.Y > 700 || Math.Abs(nextV.vPoint.Y - possible.Y)<20))
                    {
                        toCheck.MoveTo = possible;
                        return true;
                    }
                }
            }

            if (next.Restrictions.Count != 0 && next.Restrictions[0] is HorizontalRestriction)
            {
                if (Math.Abs(moved.MoveTo.Value.Y - toCheck.vPoint.Y) <= _length)
                {
                    Point possible = FindPointOnHorizontal(toCheck.vPoint, moved.MoveTo.Value).Value;
                    if (!(possible.X < 0 || possible.X > 836))
                    {
                        toCheck.MoveTo = possible;
                        return true;
                    }
                }
            }

            toCheck.MoveTo = Vector2D.MovePoint(toCheck.vPoint,vector);
            
            return true;

        }

        private Point? FindPointOnVertical(Point p1, Point p2)
        {
            double x1 = p1.X;
            double x2 = p2.X;
            double y1 = p1.Y;
            double y2 = p2.Y;

            double A = 1;
            double B = -2*y2;
            double C = y2 * y2 + (x2 - x1) * (x2 - x1) - _length * _length;

            double delta = B * B - 4 * A * C;

            int Y1 = (int)Math.Round((-B - Math.Sqrt(delta)) / (2 * A));
            int Y2 = (int)Math.Round((-B + Math.Sqrt(delta)) / (2 * A));

           

            return (Math.Abs(Y1 - y1) > Math.Abs(Y2 - y1))? new Point((int)x1,Y2) : new Point((int)x1, Y1);

        }

        private Point? FindPointOnHorizontal(Point p1, Point p2)
        {
            double x1 = p1.X;
            double x2 = p2.X;
            double y1 = p1.Y;
            double y2 = p2.Y;

            double A = 1;
            double B = -2 * x2;
            double C = x2 * x2 + (y2 - y1) * (y2 - y1) - _length * _length;

            double delta = B * B - 4 * A * C;

            int X1 = (int)Math.Round((-B - Math.Sqrt(delta)) / (2 * A));
            int X2 = (int)Math.Round((-B + Math.Sqrt(delta)) / (2 * A));



            return (Math.Abs(X1 - x1) > Math.Abs(X2 - x1)) ? new Point(X2, (int)y1) : new Point(X1, (int)y1);

        }

        public override string ToString()
        {
            return "***";
        }
    }
}
