using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MyDrawing;
using System.Windows.Forms;


namespace SimpleDraw.Model
{
    internal class WorkingArea
    {
        public List<Polygon> Polygons { get; set; }
        public Bitmap Bitmap { get; }
        public DrawingState State { get; }
        private PictureBox pictureBox;

        public WorkingArea(Bitmap bitmap, PictureBox pictureBox)
        {
            Bitmap = bitmap;
            State = new DrawingState();
            ;
            Polygons = new List<Polygon> {new Polygon()};
            State.CurrentPolygon = Polygons[Polygons.Count - 1];
            this.pictureBox = pictureBox;
            pictureBox.Image = bitmap;
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
                        gr.DrawString(edgeRestriction.ToString(), drawFont, drawBrush, edgeMiddle.X, edgeMiddle.Y);
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

        public void HandleMouseClick(MouseEventArgs e, Point mousePoint)
        {
            // Obsługujemy kliknięcie w istniejący wierzchołek
            if (VertexClicked(mousePoint))
                return;

            // Sprawdzamy czy nie kliknęliśmy w krawędź 
            if (EdgeClicked(mousePoint, e))
                return;

            // Jeśli kliknęliśmy w wolne miejsce

            // Aktualnie max jeden wielokąt
            if (State.Mode == Mode.Edit)
                return;
            Vertex newVertex = new Vertex(mousePoint.X, mousePoint.Y);
            if (State.CurrentPolygon.Vertices.Count != 0)
            {
                // Nowa krawędź
                Edge newEdge = new Edge((State.PrevVertex, newVertex));
                State.CurrentPolygon.Edges.Add(newEdge);
                newVertex.edges = (newEdge, null);
                State.PrevVertex.edges = (State.PrevVertex.edges.left, newEdge);
            }

            State.CurrentPolygon.Vertices.Add(newVertex);

            RepaintBitmap();
        }

        private bool VertexClicked(Point mousePoint)
        {
            foreach (var v in State.CurrentPolygon.Vertices)
            {
                if (v.Rectangle.Contains(mousePoint))
                {
                    // ToDo: TO niedobry if jeśli chodzi o obsługę wielu wielokątów
                    if (State.Mode == Mode.Draw)
                    {
                        if (State.CurrentPolygon.Vertices.Count >= 3 && v == State.FirstVertex)
                        {
                            State.Mode = Mode.Edit;
                            Edge newEdge = new Edge((State.PrevVertex, State.FirstVertex));
                            State.CurrentPolygon.Edges.Add(newEdge);
                            State.FirstVertex.edges = (newEdge, State.FirstVertex.edges.right);
                            State.PrevVertex.edges = (State.PrevVertex.edges.left, newEdge);
                            RepaintBitmap();
                        }

                    }
                    else
                    {
                        // ToDo: obsługa usuniecia wierzchołka jeśli PPM
                    }

                    return true;
                }
            }

            return false;
        }

        private bool EdgeClicked(Point mousePoint, MouseEventArgs e)
        {
            foreach (var edge in State.CurrentPolygon.Edges)
            {
                if (edge.IsPointClose(mousePoint))
                {
                    State.ClickedEdge = edge;
                    if (e.Button == MouseButtons.Right)
                    {
                        HandleRightEdgeClick(e);
                    }

                    return true;
                }
            }

            return false;
        }

        private void HandleRightEdgeClick(MouseEventArgs e)
        {
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add("Length Restriction", new EventHandler(HandleLenghtRestrictionClick));
            contextMenu.MenuItems.Add("Vertical Restriction", new EventHandler(HandleVerticalRestrictionClick));
            contextMenu.MenuItems.Add("Horizontal Restriction", new EventHandler(HandleHorizontalRestrictionClick));
            contextMenu.MenuItems.Add("Clear Restriction",
                new EventHandler((sender1, e1) => State.ClickedEdge.Restrictions.Clear()));
            pictureBox.ContextMenu = contextMenu;
            pictureBox.ContextMenu.Show(pictureBox, new Point(e.X, e.Y));
            pictureBox.ContextMenu = null;
        }

        private void HandleLenghtRestrictionClick(object sender, EventArgs e)
        {
            int length = (int) State.ClickedEdge.Length;
            double desiredLength = length;

            LengthWindow lw = new LengthWindow(ref desiredLength);
            var result = lw.ShowDialog();
            desiredLength = lw._length;

            if (result == DialogResult.Cancel || desiredLength == 0)
                return;

            AddRestriction(new LengthRestriciton(State.ClickedEdge, desiredLength));

            if (desiredLength != length)
            {
                State.MovedVertex = State.ClickedEdge.ends.left;
                if (State.ClickedEdge.Length == desiredLength)
                    return;
                Point? moveTo = State.ClickedEdge.FindLengthPoint(desiredLength);
                HandleVertexMove(moveTo.Value);
                State.MovedVertex = null;
            }
        }

        private void HandleVerticalRestrictionClick(object sender, EventArgs e)
        {
            Edge left = State.ClickedEdge.ends.left.edges.left;
            Edge right = State.ClickedEdge.ends.right.edges.right;

            if (left.Restrictions.Count >= 1 && left.Restrictions[0] is VerticalRestriction)
                return;
            if (right.Restrictions.Count >= 1 && right.Restrictions[0] is VerticalRestriction)
                return;

            AddRestriction(new VerticalRestriction(State.ClickedEdge));
            State.MovedVertex = State.ClickedEdge.ends.left;
            Point moveTo = new Point(State.ClickedEdge.ends.right.vPoint.X, State.MovedVertex.vPoint.Y);
            HandleVertexMove(moveTo);
            State.MovedVertex = null;
        }

        private void HandleHorizontalRestrictionClick(object sender, EventArgs e)
        {
            Edge left = State.ClickedEdge.ends.left.edges.left;
            Edge right = State.ClickedEdge.ends.right.edges.right;

            if (left.Restrictions.Count >= 1 && left.Restrictions[0] is HorizontalRestriction)
                return;
            if (right.Restrictions.Count >= 1 && right.Restrictions[0] is HorizontalRestriction)
                return;

            AddRestriction(new HorizontalRestriction(State.ClickedEdge));

            State.MovedVertex = State.ClickedEdge.ends.left;
            Point moveTo = new Point(State.MovedVertex.vPoint.X, State.ClickedEdge.ends.right.vPoint.Y);
            HandleVertexMove(moveTo);
            State.MovedVertex = null;
        }

        private void AddRestriction(Restriction restriction)
        {
            if (State.ClickedEdge.Restrictions.Count == 0)
            {
                State.ClickedEdge.Restrictions.Add(restriction);
            }
            else
            {
                State.ClickedEdge.Restrictions[0] = restriction;
            }
        }

        private void HandleVertexMove(Point mousePoint)
        {
            State.MovedVertex.MoveTo = mousePoint;
            Edge eright = State.MovedVertex.edges.right;
            Vertex vright = State.MovedVertex;
            Edge eleft = State.MovedVertex.edges.left;
            Vertex vleft = State.MovedVertex;
            List<Vertex> movedVertices = new List<Vertex>();
            movedVertices.Add(vleft);
            bool changedLeft = true;
            bool changedRight = true;
            bool anyAction = false;

            //left && right
            while (changedLeft || changedRight)
            {
                changedLeft = eleft.PreserveRestrictions(vleft, new Vector2D(vleft.vPoint, vleft.MoveTo.Value), true);
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

        public void HandleMouseMove(MouseEventArgs e)
        {
            if (Polygons.Count == 0 || State.CurrentPolygon.Vertices.Count == 0)
                return;

            int x = e.Location.X;
            int y = e.Location.Y;
            Point mousePoint = new Point(x, y);

            if (State.MovedVertex != null)
            {
                HandleVertexMove(mousePoint);
            }

            State.MousePosition = mousePoint;
            RepaintBitmap();
            pictureBox.Refresh();
        }

        public void HandleMouseDown(Point mousePoint)
        {
            // ToDo: tymczasowe (chyba)
            if (State.Mode == Mode.Draw)
                return;



            foreach (var v in State.CurrentPolygon.Vertices)
            {
                if (v.Rectangle.Contains(mousePoint))
                {
                    State.MovedVertex = v;
                }
            }
        }

        public void HandleMouseUp()
        {
            if (State.Mode == Mode.Draw)
                return;
            State.MovedVertex = null;
        }

    }


}
