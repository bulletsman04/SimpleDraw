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
        private Edge _edge;
        private double _length;

        public LengthRestriciton(Edge edge, double length)
        {
            _edge = edge;
            _length = length;

        }

        public override bool isResticted(Vertex moved, Vertex toCheck, Vector2D vector)
        {
            Point p1 = moved.MoveTo.Value;
            Point p2 = toCheck.vPoint;
            // ToDo: tu chyba trzeba dać jakiś błąd - bo pixele przy wyliczaniu punktu mogą być zaokrąglane
            return Math.Abs(Math.Sqrt(Math.Pow(p1.X - p2.X, 2) + Math.Pow(p1.Y - p2.Y, 2)) - _length) < 1;

        }

        // ToDo: Change using left bool
        public override bool preserveRestriction(Vertex moved, Vector2D vector, bool left)
        {
            Vertex toCheck = left ? _edge.ends.left : _edge.ends.right;
            if (isResticted(moved,toCheck, vector))
                return false;
            
            toCheck.MoveTo = Vector2D.MovePoint(toCheck.vPoint,vector);
            
            return true;

        }

        public override string ToString()
        {
            return _length.ToString();
        }
    }
}
