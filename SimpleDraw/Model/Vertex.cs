using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDraw.Model
{
    internal class Vertex
    {
        public int X { get; set; }
        public int Y { get; set; }
        public (Edge left,Edge right) edges { get; set; }
        public Action<Vector2D> Move { get; set; }
    }
}
