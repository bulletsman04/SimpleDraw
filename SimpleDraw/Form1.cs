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
            Point mousePoint = new Point(x, y);
            bool modelChange = workingArea.State.Mode == Mode.Draw;

            if (workingArea.Polygons.Count == 0)
            {
                workingArea.Polygons.Add(new Polygon());
                workingArea.State.CurrentPolygon = workingArea.Polygons[workingArea.Polygons.Count - 1];
            }
            
           
            // Sprawdzmy czy nie kliknęliśmy w wierzchołek
            foreach (var v in workingArea.State.CurrentPolygon.Vertices)
            {
                if (v.Rectangle.Contains(mousePoint))
                {
                    // ToDo: TO niedobry if jeśli chodzi o obsługę wielu wielokątów
                    if (workingArea.State.Mode == Mode.Draw)
                    {
                        if (workingArea.State.CurrentPolygon.Vertices.Count >= 3 && v == workingArea.State.FirstVertex)
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
                    // ToDo: To trzeba potem wyczyścić
                    workingArea.State.ClickedEdge = edge;
                    if (e.Button == MouseButtons.Right)
                    {
                        HandleRightEdgeClick(sender,e);
                    }

                    modelChange = false;
                    break;
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

            if (workingArea.State.MovedVertex != null)
            {
                HandleVertexMove(mousePoint);
            }

            workingArea.State.MousePosition = mousePoint;
            workingArea.RepaintBitmap();
            pictureBox.Refresh();
        }

        private void HandleVertexMove(Point mousePoint)
        {
            //workingArea.State.MovedVertex.vPoint = mousePoint;

            workingArea.State.MovedVertex.MoveTo = mousePoint;
            Edge e = workingArea.State.MovedVertex.edges.left;
            Vertex v = workingArea.State.MovedVertex;
            List<Vertex> movedVertices = new List<Vertex>();
            movedVertices.Add(v);
            bool changed = true;
            bool anyAction = false;
            while (changed )
            {
                changed = e.PreserveRestrictions(v, new Vector2D(v.vPoint, v.MoveTo.Value));

                if (changed)
                {
                    v = e.ends.left;
                    e = v.edges.left;
                    movedVertices.Add(v);
                    anyAction = true;
                }

                //ToDo: I wyczyść MoveTo ?
                if (anyAction && e.ends.right == workingArea.State.MovedVertex)
                    return;
            }

            foreach (var movedVertex in movedVertices)
            {
                movedVertex.vPoint = movedVertex.MoveTo.Value;
                movedVertex.MoveTo = null;
            }
        }

        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {
            // ToDo: tymczasowe (chyba)
            if (workingArea.State.Mode == Mode.Draw)
                return;

            int x = e.Location.X;
            int y = e.Location.Y;
            Point mousePoint = new Point(x, y);

            foreach (var v in workingArea.State.CurrentPolygon.Vertices)
            {
                if (v.Rectangle.Contains(mousePoint))
                {
                    workingArea.State.MovedVertex = v;
                }
            }
        }

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            if (workingArea.State.Mode == Mode.Draw)
                return;
            workingArea.State.MovedVertex = null;
        }

        private void HandleRightEdgeClick(object sender, MouseEventArgs e)
        {
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add("Length Restriction", new EventHandler(HandleLenghtRestrictionClick));
            pictureBox.ContextMenu = contextMenu;
            pictureBox.ContextMenu.Show(pictureBox,new Point(e.X,e.Y));
        }

        private void HandleLenghtRestrictionClick( object sender, EventArgs e)
        {
            workingArea.State.ClickedEdge.Restrictions.Add(new LengthRestriciton(workingArea.State.ClickedEdge, 10));
        }
    }
}
