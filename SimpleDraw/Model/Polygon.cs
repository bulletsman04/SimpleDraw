using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDraw.Model
{
    class Polygon: Figure
    {
        public List<Edge> Edges { get; }
        public List<Vertex> Vertices { get; }
        public Rectangle Rectangle { get; }
        public override bool isInside(Point p)
        {
            throw new NotImplementedException();
        }

        public Polygon()
        {
            Vertices = new List<Vertex>();
            Edges = new List<Edge>();
        }

        // Może jakiś visitor do metody przechodzącej po wierzchołkach - choć to chyba przerost formy nad treścią
    }
}
