using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDraw.Model
{
    internal class HorizontalRestriction: Restriction
    {
        private Edge _edge;
        public HorizontalRestriction(Edge edge)
        {
            _edge = edge;
        }

        public override bool isResticted(Vertex moved, Vertex toCheck, Vector2D vector)
        {
            // TODO: robimy MoveTo który śledzi vPoint i jest inne tylko w czasie przesuwania i dodawania ograniczenia. I Tutaj ograniczenia opieramy o MoveTo
            return moved.MoveTo.Value.Y == toCheck.vPoint.Y;

        }

        public override bool preserveRestriction(Vertex moved, Vector2D vector, bool left)
        {


            Vertex toCheck = left ? _edge.ends.left : _edge.ends.right;
            if (isResticted(moved,toCheck, vector))
                return false;
            
            toCheck.MoveTo = new Point(toCheck.vPoint.X, moved.MoveTo.Value.Y);

            return true;

        }

        public override string ToString()
        {
            return "-";
        }
    }
}
