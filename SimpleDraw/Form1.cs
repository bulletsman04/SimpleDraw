using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using SimpleDraw.Model;

namespace SimpleDraw
{
    public partial class SimpleDraw : Form
    {
        internal WorkingArea workingArea;
        public SimpleDraw()
        {
            
            InitializeComponent();
            this.Size = new Size((int)(0.65 * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width),
                (int)(0.85 * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height));

            workingArea = new WorkingArea(new Bitmap(pictureBox.Width,pictureBox.Height));
            pictureBox.Image = workingArea.Bitmap;
        }

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            int x = e.Location.X;
            int y = e.Location.Y;

            if (workingArea.State.Mode == Mode.Edit)
                return;

            if (workingArea.Polygons.Count == 0)
            {
                workingArea.Polygons.Add(new Polygon());
                workingArea.State.CurrentPolygon = workingArea.Polygons[workingArea.Polygons.Count - 1];
            }
            Point mousePoint = new Point(x, y);
            bool modelChange = true;

            // Sprawdzmy czy nie kliknęliśmy w wierzchołek
            foreach (var v in workingArea.State.CurrentPolygon.Vertices)
            {
                if (v.Rectangle.Contains(mousePoint))
                {
                    if (workingArea.State.CurrentPolygon.Vertices.Count >=3 && v == workingArea.State.FirstVertex)
                    {
                        modelChange = false;
                        workingArea.State.Mode = Mode.Edit;
                        // ToDo: Code repetition - down
                        Edge newEdge = new Edge((workingArea.State.PrevVertex, workingArea.State.FirstVertex));
                        workingArea.State.CurrentPolygon.Edges.Add(newEdge);
                        workingArea.State.FirstVertex.edges = (newEdge, workingArea.State.FirstVertex.edges.right);
                        workingArea.State.PrevVertex.edges = (workingArea.State.PrevVertex.edges.left, newEdge);
                        workingArea.RepaintBitmap();
                        pictureBox.Refresh();
                    }
                    else
                    {
                        modelChange = false;
                    }
                }
            }

            // Sprawdzamy czy nie kliknęliśmy w krawędź ToDO: wykonać tylko jeśli nie kilknięto w wierzchołek - chociaż tu chyba nieistotne
            foreach (var edge in workingArea.State.CurrentPolygon.Edges)
            {
                // Niekoniecznie działa - źle czyta dla pionowych, czasem też źle dla innych

                //if (edge.Rectangle.Contains(mousePoint))
                //{
                //    double crossProduct = Vector2D.CrossProduct(edge.Vector2D,new Vector2D(new Point(0,0),mousePoint));
                //    if (Math.Abs(crossProduct) < 2000)
                //        modelChange = false;
                //}

                if (edge.IsPointClose(mousePoint))
                {
                    modelChange = false;
                }
            }



            if (modelChange)
            {
                Vertex newVertex = new Vertex(x, y);
                if (workingArea.State.CurrentPolygon.Vertices.Count != 0)
                {
                    // ToDo: Code repetition - up
                    Edge newEdge = new Edge((workingArea.State.PrevVertex, newVertex));
                    workingArea.State.CurrentPolygon.Edges.Add(newEdge);
                    newVertex.edges = (newEdge,null);
                    workingArea.State.PrevVertex.edges = (workingArea.State.PrevVertex.edges.left,newEdge);
                }
                workingArea.State.CurrentPolygon.Vertices.Add(newVertex);

                workingArea.RepaintBitmap();
                pictureBox.Refresh();
            }
        }

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            if (workingArea.Polygons.Count == 0 || workingArea.State.CurrentPolygon.Vertices.Count == 0)
                return;

            int x = e.Location.X;
            int y = e.Location.Y;
            Point mousePoint = new Point(x, y);
           
            workingArea.State.MousePosition = mousePoint;
            workingArea.RepaintBitmap();
            pictureBox.Refresh();
        }
    }
}
