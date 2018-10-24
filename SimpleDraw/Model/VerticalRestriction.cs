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
       

        public override bool isResticted(Vertex moved, Vertex toCheck, Vector2D vector)
        {
            return moved.MoveTo.Value.X == toCheck.vPoint.X;

        }

        public override bool preserveRestriction(Vertex moved, Vertex toCheck, Vector2D vector)
        {
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
