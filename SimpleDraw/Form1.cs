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
            this.Size = new Size((int)(0.65 * Screen.PrimaryScreen.WorkingArea.Width),
                (int)(0.85 * Screen.PrimaryScreen.WorkingArea.Height));

            workingArea = new WorkingArea(new Bitmap(pictureBox.Width,pictureBox.Height),pictureBox);
            workingArea.RepaintBitmap();
        }

        private void pictureBox_MouseClick(object sender, MouseEventArgs e)
        {
            Point mousePoint = new Point(e.Location.X, e.Location.Y);
            workingArea.HandleMouseClick(e, mousePoint);

            pictureBox.Refresh();
        }

       

        private void pictureBox_MouseMove(object sender, MouseEventArgs e)
        {
            workingArea.HandleMouseMove(e);
        }

      


        private void pictureBox_MouseDown(object sender, MouseEventArgs e)
        {

            Point mousePoint = new Point(e.Location.X, e.Location.Y);

            workingArea.HandleMouseDown(mousePoint);
        }

     

        private void pictureBox_MouseUp(object sender, MouseEventArgs e)
        {
            workingArea.HandleMouseUp();
        }

     

        
    }
}
