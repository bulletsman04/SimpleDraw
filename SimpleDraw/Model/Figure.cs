using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleDraw.Model
{
    internal abstract class Figure
    {
        public abstract bool isInside(Point p);

    }
}
