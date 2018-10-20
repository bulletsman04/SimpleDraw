using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDraw.Model
{
    enum Mode
    {
        Draw,
        Edit
    }
    internal class DrawingState
    {
        public Mode Mode { get; set; }
      
        public Polygon CurrentPolygon { get; set; }

        public Vertex FirstVertex => CurrentPolygon.Vertices[0];

        public Vertex PrevVertex => CurrentPolygon.Vertices[CurrentPolygon.Vertices.Count - 1];
        public Vertex MovedVertex { get; set; }
        public Edge ClickedEdge { get; set; }

        public Point? MousePosition { get; set; }

        public DrawingState()
        {
            Mode = Mode.Draw;
            MousePosition = null;
        }
    }
}
