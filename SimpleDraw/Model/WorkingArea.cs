using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDraw.Model
{
    internal class WorkingArea
    {
        public List<Polygon> Polygons { get; set; }
        public Bitmap Bitmap { get; }

        public WorkingArea(Bitmap bitmap)
        {
            Bitmap = bitmap;
        }
    }
}
