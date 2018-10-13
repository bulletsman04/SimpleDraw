using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDraw.Model
{
    enum RestrictionType
    {
        Horizontal,
        Vertical,
        Length
    }
    internal class Restriction
    {
        private RestrictionType restrictionType;

        public bool isResticted(Vertex moved, Vector2D vector)
        {

        }

        bool preserveRestriction(Vertex moved, Vector2D vector)
        {

        }

    }
}
