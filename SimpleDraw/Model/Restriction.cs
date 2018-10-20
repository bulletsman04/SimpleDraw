using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDraw.Model
{
    internal abstract class Restriction
    {

        public abstract bool isResticted(Vertex moved, Vector2D vector);

        public abstract bool preserveRestriction(Vertex moved, Vector2D vector);

    }
}
