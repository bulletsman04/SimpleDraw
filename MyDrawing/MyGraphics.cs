using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace MyDrawing
{
    public class MyGraphics
    {
        private  Bitmap bitmap;

        public MyGraphics(Bitmap bitmap)
        {
            this.bitmap = bitmap;
        }

        //public static MyGraphics FromImage(Bitmap _bitmap)
        //{
        //    bitmap = _bitmap;
        //    return this;

        //}

        public void MyDrawLine(Pen pen, Point p1, Point p2)
        {
            int dx, dy, d, incrE, incrNe, x, y;
            int x1 = p1.X;
            int y1 = p1.Y;
            int x2 = p2.X;
            int y2 = p2.Y;
            dx = x2 - x1;
            dy = y2 - y1;
            d = 2 * dy - dx;
            incrE = 2 * dy;
            incrNe = 2 * (dy - dx);
            x = x1;
            y = y1;
            bitmap.SetPixel(x,y,pen.Color);
            while (x < x2)
            {
                if (d < 0)
                {
                    d += incrE;
                    x++;
                }
                else
                {
                    d += incrNe;
                    x++;
                    y++;
                }
                bitmap.SetPixel(x, y, pen.Color);
            }
        }
    }
}
