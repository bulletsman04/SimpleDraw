using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyDrawing;


namespace SimpleDraw.Model
{
    internal class WorkingArea
    {
        public List<Polygon> Polygons { get; set; }
        public Bitmap Bitmap { get; }
        public DrawingState State { get; }

        public WorkingArea(Bitmap bitmap)
        {
            Bitmap = bitmap;
            State = new DrawingState();;
            Polygons = new List<Polygon>();
        }

        public void RepaintBitmap()
        {
            using (Graphics gr = Graphics.FromImage(Bitmap))
            {
                gr.Clear(Color.White);
                Pen thick_pen = new Pen(Color.Black, 2);
                Brush brush = new SolidBrush(Color.Blue);
                Font drawFont = new Font("Arial", 12);
                SolidBrush drawBrush = new SolidBrush(Color.Black);

                foreach (Vertex v in State.CurrentPolygon.Vertices)
                {

                    gr.FillRectangle(brush, v.Rectangle);

                }
                //Test MyDrawing
                MyGraphics myGraphics = new MyGraphics(Bitmap);

                foreach (var edge in State.CurrentPolygon.Edges)
                {
                    
                    gr.DrawLine(thick_pen, edge.ends.left.vPoint, edge.ends.right.vPoint);
                    // ToDo: Te zwolnić, zrobić środek krawędzi i tangens krawędzi do wypozycjonowania oznaczenia
                    Point edgeMiddle = edge.EdgeMiddle;
                    
                    foreach (var edgeRestriction in edge.Restrictions)
                    {
                        gr.DrawString(edgeRestriction.ToString(),drawFont,drawBrush,edgeMiddle.X,edgeMiddle.Y);
                    }
                }
                
                if (State.Mode == Mode.Edit)
                {
                    gr.DrawLine(thick_pen, State.PrevVertex.vPoint, State.FirstVertex.vPoint);
                }

                else if (State.MousePosition != null)
                {
                    //Test MyDrawing
                    // gr.DrawLine(thick_pen, State.PrevVertex.vPoint, State.MousePosition.Value);
                    myGraphics.MyDrawLine(thick_pen, State.PrevVertex.vPoint, State.MousePosition.Value);
                    State.MousePosition = null;
                }

                drawFont.Dispose();
                brush.Dispose();
                thick_pen.Dispose();
                drawBrush.Dispose();
            }
        }
    }

   
}
