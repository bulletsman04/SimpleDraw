using System;
using System.Collections.Generic;
using System.Drawing;
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

                foreach (Vertex v in State.CurrentPolygon.Vertices)
                {

                    gr.FillRectangle(brush, v.Rectangle);

                }
                //Test MyDrawing
                MyGraphics myGraphics = new MyGraphics(Bitmap);

                foreach (var edge in State.CurrentPolygon.Edges)
                {
                    
                    gr.DrawLine(thick_pen, edge.ends.left.vPoint, edge.ends.right.vPoint);
                  
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

               
                brush.Dispose();
                thick_pen.Dispose();
            }
        }
    }

   
}
