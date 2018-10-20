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

        public static double CrossProduct(Vector2D v1, Vector2D v2)
        {
            Point movedPointV1 = new Point(v1.End.X - v1.Start.X, v1.End.Y - v1.Start.Y);
            Point movedPointV2 = new Point(v2.End.X - v1.Start.X, v2.End.Y - v1.Start.Y);

            return movedPointV1.X*movedPointV2.Y - movedPointV2.X*movedPointV1.Y;
        }

        public static Point MovePoint(Point p, Vector2D v)
        {
            return new Point(p.X+v.Coordinates.x,p.Y + v.Coordinates.y);
        }
    }
}
