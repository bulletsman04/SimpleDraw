using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDraw.Model
{
    internal class Edge
    {
        public (Vertex left, Vertex right) ends;
        public List<Restriction> Restrictions { get; set; }

    }
}
