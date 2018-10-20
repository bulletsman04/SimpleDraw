using System;
using System.Collections.Generic;
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

        public override bool isResticted(Vertex moved, Vector2D vector)
        {
            return _length == _edge.Length;
            
        }

        public override bool preserveRestriction(Vertex moved, Vector2D vector)
        {
            if (isResticted(moved, vector))
                return false;

            bool rightMoved = moved == _edge.ends.right;
            Vertex toCheck = rightMoved ? _edge.ends.left : _edge.ends.right;

            toCheck.MoveTo = Vector2D.MovePoint(toCheck.vPoint,vector);
            
            return true;

        }
    }
}
