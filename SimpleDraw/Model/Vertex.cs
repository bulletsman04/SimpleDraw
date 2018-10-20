using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDraw.Model
{
    internal class Vertex
    {
        public const int vertexSize = 8;
        private Point _vPoint;
        public Point vPoint {
            get => _vPoint;
            set
            {
                _vPoint = value;
                int x = value.X;
                int y = value.Y;
                Rectangle = Rectangle = new Rectangle(x - vertexSize / 2, y - vertexSize / 2, vertexSize, vertexSize);
            }
        }
        public Rectangle Rectangle { get; set; }
        public (Edge left,Edge right) edges { get; set; }
        public Point? MoveTo { get; set; }

        public Vertex(int x, int y)
        {
            vPoint = new Point(x,y);
            Rectangle = new Rectangle(x - vertexSize/2, y - vertexSize/2, vertexSize, vertexSize);
        }

       
    }
}
