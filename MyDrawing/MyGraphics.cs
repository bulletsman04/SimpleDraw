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
        
        public void MyDrawLine(Pen pen, Point p1, Point p2)
        {
            int dx, dy, d, incrE, incrNe, x, y, incrX, incrY;
            int x1 = p1.X;
            int y1 = p1.Y;
            int x2 = p2.X;
            int y2 = p2.Y;
            
            if (x2 > x1)
            {
                incrX = 1;
                dx = x2 - x1;
            }
            else
            {
                incrX = -1;
                dx = x1 - x2;
            }

            if (y2 > y1)
            {
                incrY = 1;
                dy = y2 - y1;
            }
            else
            {
                incrY = -1;
                dy = y1 - y2;
            }

            x = x1;
            y = y1;
            if(CanDraw(x,y))
                 bitmap.SetPixel(x, y, pen.Color);



            if (dx > dy)
            {

                d = 2 * dy - dx;
                incrE = 2 * dy;
                incrNe = 2 * (dy - dx);

                while (Math.Abs(x - x2) != 0)
                {
                    if (d < 0)
                    {
                        d += incrE;
                        x += incrX;
                    }
                    else
                    {
                        d += incrNe;
                        x += incrX;
                        y += incrY;
                    }

                    if (CanDraw(x, y) && CanDraw(x,y+1) && CanDraw(x,y-1))
                    {
                        bitmap.SetPixel(x, y, pen.Color);
                        bitmap.SetPixel(x, y + 1, pen.Color);
                        bitmap.SetPixel(x, y - 1, pen.Color);
                    }

                }
            }
            else
            {
                int temp = dy;
                dy = dx;
                dx = temp;

                d = 2 * dy - dx;
                incrE = 2 * dy;
                incrNe = 2 * (dy - dx);
                while (Math.Abs(y - y2) != 0)
                {
                    if (d < 0)
                    {
                        d += incrE;
                        y += incrY;
                    }
                    else
                    {
                        d += incrNe;
                        x += incrX;
                        y += incrY;
                    }

                    if (CanDraw(x, y) && CanDraw(x-1, y) && CanDraw(x+1, y))
                    {
                        bitmap.SetPixel(x - 1, y, pen.Color);
                        bitmap.SetPixel(x, y, pen.Color);
                        bitmap.SetPixel(x + 1, y, pen.Color);
                    }
                }
            }
        }

        bool CanDraw(int x, int y)
        {
            return x >= 0 && x < bitmap.Width && y < bitmap.Height && y >= 0;
        }
    }
}
