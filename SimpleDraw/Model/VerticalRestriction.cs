using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDraw.Model
{
    internal class VerticalRestriction: Restriction
    {
        private Edge _edge;
        public VerticalRestriction(Edge edge)
        {
            _edge = edge;
        }

        public override bool isResticted(Vertex moved, Vertex toCheck, Vector2D vector)
        {
            return moved.MoveTo.Value.X == toCheck.vPoint.X;

        }

        public override bool preserveRestriction(Vertex moved, Vector2D vector, bool left)
        {

            Vertex toCheck = left ? _edge.ends.left : _edge.ends.right;

            if (isResticted(moved,toCheck, vector))
                return false;
            
            toCheck.MoveTo = new Point(moved.MoveTo.Value.X, toCheck.vPoint.Y);

            return true;

        }

        public override string ToString()
        {
            return "|";
        }
    }
}
