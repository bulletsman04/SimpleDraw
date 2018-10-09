using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace SimpleDraw
{
    public partial class SimpleDraw : Form
    {
        public SimpleDraw()
        {
            
            InitializeComponent();
            this.Size = new Size((int)(0.65 * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Width),
                (int)(0.85 * System.Windows.Forms.Screen.PrimaryScreen.WorkingArea.Height));
        }
    }
}
