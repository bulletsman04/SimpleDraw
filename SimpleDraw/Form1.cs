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
            NewMethod(sender, e, x, y, mousePoint);
        }

        private void NewMethod(object sender, MouseEventArgs e, int x, int y, Point mousePoint)
        {
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

                if (edge.IsPointClose(mousePoint))
                {
                    // ToDo: To trzeba potem wyczyścić
                    workingArea.State.ClickedEdge = edge;
                    if (e.Button == MouseButtons.Right)
                    {
                        HandleRightEdgeClick(sender, e);
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
                    newVertex.edges = (newEdge, null);
                    workingArea.State.PrevVertex.edges = (workingArea.State.PrevVertex.edges.left, newEdge);
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
            Edge eright = workingArea.State.MovedVertex.edges.right;
            Vertex vright = workingArea.State.MovedVertex;
            Edge eleft = workingArea.State.MovedVertex.edges.left;
            Vertex vleft = workingArea.State.MovedVertex;
            List<Vertex> movedVertices = new List<Vertex>();
            movedVertices.Add(vleft);
            bool changedLeft = true;
            bool changedRight = true;
            bool anyAction = false;

            //left && right
            while (changedLeft || changedRight)
            {
                changedLeft = eleft.PreserveRestrictions(vleft, new Vector2D(vleft.vPoint, vleft.MoveTo.Value),true);
                changedRight = eright.PreserveRestrictions(vright, new Vector2D(vright.vPoint, vright.MoveTo.Value), false);


                if (changedLeft)
                {
                    vleft = eleft.ends.left;
                    eleft = vleft.edges.left;
                    movedVertices.Add(vleft);
                    anyAction = true;
                }

                if (changedRight)
                {
                    vright = eright.ends.right;
                    eright = vright.edges.right;
                    movedVertices.Add(vright);
                    anyAction = true;
                }

                //ToDo: I wyczyść MoveTo ?
                if (anyAction && vleft == vright)
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
            contextMenu.MenuItems.Add("Vertical Restriction", new EventHandler(HandleVerticalRestrictionClick));
            contextMenu.MenuItems.Add("Horizontal Restriction", new EventHandler(HandleHorizontalRestrictionClick));
            contextMenu.MenuItems.Add("Clear Restriction", new EventHandler((sender1,e1) => workingArea.State.ClickedEdge.Restrictions.Clear()));
            pictureBox.ContextMenu = contextMenu;
            pictureBox.ContextMenu.Show(pictureBox,new Point(e.X,e.Y));
            pictureBox.ContextMenu = null;
        }

        private void HandleLenghtRestrictionClick( object sender, EventArgs e)
        {
            int length = (int)workingArea.State.ClickedEdge.Length;
            double desiredLength=length;

            LengthWindow lw = new LengthWindow(ref desiredLength);
            var result = lw.ShowDialog();
            desiredLength = lw._length;

            if (result == DialogResult.Cancel || desiredLength==0)
                return;

            AddRestriction(new LengthRestriciton(workingArea.State.ClickedEdge, desiredLength));

            if (desiredLength != length)
            {
                workingArea.State.MovedVertex = workingArea.State.ClickedEdge.ends.left;
                if (workingArea.State.ClickedEdge.Length == desiredLength)
                    return;
                Point? moveTo = workingArea.State.ClickedEdge.FindLengthPoint(desiredLength);
                HandleVertexMove(moveTo.Value);
                workingArea.State.MovedVertex = null;
            }
        }

        private void HandleVerticalRestrictionClick(object sender, EventArgs e)
        {
            Edge left = workingArea.State.ClickedEdge.ends.left.edges.left;
            Edge right = workingArea.State.ClickedEdge.ends.right.edges.right;

            if (left.Restrictions.Count >= 1 && left.Restrictions[0] is VerticalRestriction)
                return;
            if (right.Restrictions.Count >= 1 && right.Restrictions[0] is VerticalRestriction)
                return;

            AddRestriction(new VerticalRestriction(workingArea.State.ClickedEdge));
            workingArea.State.MovedVertex = workingArea.State.ClickedEdge.ends.left;
            Point moveTo = new Point(workingArea.State.ClickedEdge.ends.right.vPoint.X, workingArea.State.MovedVertex.vPoint.Y);
            HandleVertexMove(moveTo);
            workingArea.State.MovedVertex = null;
        }

        private void HandleHorizontalRestrictionClick(object sender, EventArgs e)
        {
            Edge left = workingArea.State.ClickedEdge.ends.left.edges.left;
            Edge right = workingArea.State.ClickedEdge.ends.right.edges.right;

            if (left.Restrictions.Count >= 1 && left.Restrictions[0] is HorizontalRestriction)
                return;
            if (right.Restrictions.Count >= 1 && right.Restrictions[0] is HorizontalRestriction)
                return;

            AddRestriction(new HorizontalRestriction(workingArea.State.ClickedEdge));

            workingArea.State.MovedVertex = workingArea.State.ClickedEdge.ends.left;
            Point moveTo = new Point(workingArea.State.MovedVertex.vPoint.X, workingArea.State.ClickedEdge.ends.right.vPoint.Y);
            HandleVertexMove(moveTo);
            workingArea.State.MovedVertex = null;
        }

        private void AddRestriction(Restriction restriction)
        {
           

            if (workingArea.State.ClickedEdge.Restrictions.Count == 0)
            {
                workingArea.State.ClickedEdge.Restrictions.Add(restriction);
            }
            else
            {
                workingArea.State.ClickedEdge.Restrictions[0] = restriction;
            }
        }
    }
}
