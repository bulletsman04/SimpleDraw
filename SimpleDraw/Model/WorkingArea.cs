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
        public DrawingState State { get; set; }
        private PictureBox pictureBox;

        public WorkingArea(Bitmap bitmap, PictureBox pictureBox)
        {
            Bitmap = bitmap;
            State = new DrawingState();
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
                Font drawFont = new Font("Arial", 18);
                SolidBrush drawBrush = new SolidBrush(Color.Purple);


                if (CheckIntersections() == true)
                {
                    thick_pen = new Pen(Color.Red, 2);
                }

                //Test MyDrawing
                MyGraphics myGraphics = new MyGraphics(Bitmap);

                foreach (var edge in State.CurrentPolygon.Edges)
                {

                    myGraphics.MyDrawLine(thick_pen, edge.ends.left.vPoint, edge.ends.right.vPoint);
                    Point edgeMiddle = edge.EdgeMiddle;

                    foreach (var edgeRestriction in edge.Restrictions)
                    {
                        gr.DrawString(edgeRestriction.ToString(), drawFont, drawBrush, edgeMiddle.X-20, edgeMiddle.Y);
                    }
                }

               if (State.Mode == Mode.Draw && State.MousePosition != null)
                {
                     myGraphics.MyDrawLine(thick_pen, State.PrevVertex.vPoint, State.MousePosition.Value);
                    State.MousePosition = null;
                }

                foreach (Vertex v in State.CurrentPolygon.Vertices)
                {

                    gr.FillRectangle(brush, v.Rectangle);

                }

                drawFont.Dispose();
                brush.Dispose();
                thick_pen.Dispose();
                drawBrush.Dispose();
            }
        }

        private bool CheckIntersections()
        {
            Edge drawingEdge = null;
            if (State.Mode == Mode.Draw)
            {
                if (State.MousePosition != null)
                    drawingEdge = new Edge((State.PrevVertex,
                        new Vertex(State.MousePosition.Value.X, State.MousePosition.Value.Y)));
            }
            foreach (var edge1 in State.CurrentPolygon.Edges)
            {
                foreach (var edge2 in State.CurrentPolygon.Edges)
                {
                    if(edge1.ends.left.edges.left == edge2 || edge1.ends.right.edges.right == edge2 || edge2==edge1)
                        continue;

                    if (Edge.EdgesIntersection(edge1, edge2))
                        return true;
                }

                if (drawingEdge != null && edge1.ends.left != State.PrevVertex && edge1.ends.right!= State.PrevVertex)
                {
                    if (Edge.EdgesIntersection(edge1, drawingEdge))
                        return true;
                }
            }

            return false;
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
                    else if (e.Button == MouseButtons.Left)
                    {
                        HandleLeftEdgeClick(mousePoint);
                    }

                    return true;
                }
            }

            return false;
        }

        public void HandleMouseDoubleClick(MouseEventArgs e, Point mousePoint)
        {
            if (e.Button == MouseButtons.Right)
            {
                Vertex deletedVertex = null;
                foreach (var v in State.CurrentPolygon.Vertices)
                {
                    if (v.Rectangle.Contains(mousePoint))
                    {
                        deletedVertex = v;
                       
                    }
                }

                if (deletedVertex != null)
                {
                    Edge newEdge = new Edge((deletedVertex.edges.left.ends.left, deletedVertex.edges.right.ends.right));
                    deletedVertex.edges.left.ends.left.edges = (deletedVertex.edges.left.ends.left.edges.left, newEdge);
                    deletedVertex.edges.right.ends.right.edges = (newEdge, deletedVertex.edges.right.ends.right.edges.right);
                    State.CurrentPolygon.Edges.Remove(deletedVertex.edges.left);
                    State.CurrentPolygon.Edges.Remove(deletedVertex.edges.right);
                    State.CurrentPolygon.Edges.Add(newEdge);

                    State.CurrentPolygon.Vertices.Remove(deletedVertex);
                }
            }
        }

        private void HandleRightEdgeClick(MouseEventArgs e)
        {
            ContextMenu contextMenu = new ContextMenu();
            contextMenu.MenuItems.Add("Length Restriction", HandleLenghtRestrictionClick);
            contextMenu.MenuItems.Add("Vertical Restriction", HandleVerticalRestrictionClick);
            contextMenu.MenuItems.Add("Horizontal Restriction", HandleHorizontalRestrictionClick);
            contextMenu.MenuItems.Add("Clear Restriction", (sender1, e1) => State.ClickedEdge.Restrictions.Clear());
            pictureBox.ContextMenu = contextMenu;
            pictureBox.ContextMenu.Show(pictureBox, new Point(e.X, e.Y));
            pictureBox.ContextMenu = null;
        }

        private void HandleLeftEdgeClick(Point mousePoint)
        {
            Point middle = State.ClickedEdge.EdgeMiddle;
            LineFunction pLine = State.ClickedEdge.function.GetPLine(middle);
            Point newPoint;

            if (State.ClickedEdge.ends.left.vPoint.X == State.ClickedEdge.ends.right.vPoint.X)
            {
                newPoint = new Point(middle.X-20,middle.Y);
            }
            else if (State.ClickedEdge.ends.left.vPoint.Y == State.ClickedEdge.ends.right.vPoint.Y)
            {
                newPoint = new Point(middle.X, middle.Y-20);
            }
            else
            {
               newPoint = FindLengthPoint(mousePoint, middle, pLine, 40).Value;
            }
            Vertex newVertex = new Vertex(newPoint.X, newPoint.Y);

            Edge left = new Edge((State.ClickedEdge.ends.left, newVertex));
            Edge right = new Edge((newVertex, State.ClickedEdge.ends.right));
            State.ClickedEdge.ends.left.edges = (State.ClickedEdge.ends.left.edges.left, left);
            State.ClickedEdge.ends.right.edges = (right, State.ClickedEdge.ends.right.edges.right);
            newVertex.edges = (left, right);
            State.CurrentPolygon.Edges.Remove(State.ClickedEdge);
            State.ClickedEdge = null;
            State.CurrentPolygon.Edges.Add(left);
            State.CurrentPolygon.Edges.Add(right);
            State.CurrentPolygon.Vertices.Add(newVertex);

        }

        public Point? FindLengthPoint(Point p1, Point p2, LineFunction function, double length)
        {
            int x2 = p2.X;
            int y2 = p2.Y;
            int x1 = p1.X;
            int y1 = p1.Y;

            if (function.B == 0)
            {
                return y2 < y1 ? new Point(x2, (int)(y2 + length)) : new Point(x2, (int)(y2 - length));
            }
            double a = -function.A;
            double b = -function.C;
            double a2 = a * a;
            double b2 = b * b;

            double A = a2 + 1;
            double B = 2 * a * b - 2 * y2 * a - 2 * x2;
            double C = x2 * x2 + y2 * y2 - 2 * y2 * b + b2 - length * length;

            double delta = B * B - 4 * A * C;

            int xR1 = (int)Math.Round((-B - Math.Sqrt(delta)) / (2 * A));
            int yR1 = (int)Math.Round(a * (-B - Math.Sqrt(delta)) / (2 * A) + b);
            int xR2 = (int)Math.Round((-B + Math.Sqrt(delta)) / (2 * A));
            int yR2 = (int)Math.Round(a * (-B + Math.Sqrt(delta)) / (2 * A) + b);


            double dist1 = Math.Sqrt(Math.Pow(x1 - xR1, 2) + Math.Pow(y1 - yR1, 2));
            double dist2 = Math.Sqrt(Math.Pow(x1 - xR2, 2) + Math.Pow(y1 - yR2, 2));

            return dist1 <= dist2 ? new Point(xR1, yR1) : new Point(xR2, yR2);

        }


        private void HandleLenghtRestrictionClick(object sender, EventArgs e)
        {
            int length = (int) State.ClickedEdge.Length;
            double desiredLength = length;
            if (sender != null)
            {
                LengthWindow lw = new LengthWindow(ref desiredLength);
                var result = lw.ShowDialog();
                desiredLength = lw._length;

                if (result == DialogResult.Cancel || desiredLength == 0)
                    return;
            }

            AddRestriction(new LengthRestriciton(desiredLength));

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

            AddRestriction(new VerticalRestriction());
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

            AddRestriction(new HorizontalRestriction());

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
            int countEdges = 0;
            int maxEdges = State.CurrentPolygon.Edges.Count;

            //left && right
            while (changedLeft || changedRight)
            {
                changedLeft = eleft.PreserveRestrictions(vleft, eleft.ends.left,
                    new Vector2D(vleft.vPoint, vleft.MoveTo.Value));
                changedRight = eright.PreserveRestrictions(vright, eright.ends.right,
                    new Vector2D(vright.vPoint, vright.MoveTo.Value));


                if (changedLeft)
                {
                    vleft = eleft.ends.left;
                    eleft = vleft.edges.left;
                    movedVertices.Add(vleft);
                    anyAction = true;
                    countEdges++;
                }

                if (changedRight)
                {
                    vright = eright.ends.right;
                    eright = vright.edges.right;
                    movedVertices.Add(vright);
                    anyAction = true;
                    countEdges++;
                }

                //Czy my przypadkiem nie chcemy nie dopuścić do ogr na wszystkich krawędziach?
                if (anyAction && countEdges >= maxEdges)
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

        public void Reset()
        {
            State = new DrawingState();
            Polygons = new List<Polygon> { new Polygon() };
            State.CurrentPolygon = Polygons[Polygons.Count - 1];
            RepaintBitmap();
            pictureBox.Refresh();
        }

        public void CreateTestPolygon()
        {
            HandleMouseClick(new MouseEventArgs(MouseButtons.Left, 1, 300, 100, 0), new Point(300, 100));
            HandleMouseClick(new MouseEventArgs(MouseButtons.Left, 1, 400, 100, 0), new Point(400, 100));
            HandleMouseClick(new MouseEventArgs(MouseButtons.Left, 1, 450, 200, 0), new Point(450, 200));
            HandleMouseClick(new MouseEventArgs(MouseButtons.Left, 1, 450, 300, 0), new Point(450, 300));
            HandleMouseClick(new MouseEventArgs(MouseButtons.Left, 1, 250, 300, 0), new Point(250, 300));
            HandleMouseClick(new MouseEventArgs(MouseButtons.Left, 1, 450, 300, 0), new Point(450, 300));
            HandleMouseClick(new MouseEventArgs(MouseButtons.Left, 1, 250, 300, 0), new Point(250, 300));
            HandleMouseClick(new MouseEventArgs(MouseButtons.Left, 1, 250, 200, 0), new Point(250, 200));
            HandleMouseClick(new MouseEventArgs(MouseButtons.Left, 1, 300, 100, 0), new Point(300, 100));

            State.ClickedEdge = State.CurrentPolygon.Edges[0];
            HandleHorizontalRestrictionClick(null,null);
            State.ClickedEdge = State.CurrentPolygon.Edges[5];
            HandleLenghtRestrictionClick(null, null);
            State.ClickedEdge = State.CurrentPolygon.Edges[4];
            HandleVerticalRestrictionClick(null, null);

            RepaintBitmap();
            pictureBox.Refresh();
        }

    }


}
