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
        
     

        public override bool isResticted(Vertex moved, Vertex toCheck, Vector2D vector)
        {
            return moved.MoveTo.Value.Y == toCheck.vPoint.Y;

        }

        public override bool preserveRestriction(Vertex moved, Vertex toCheck, Vector2D vector)
        {
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
