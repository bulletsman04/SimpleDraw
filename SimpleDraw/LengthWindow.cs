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
    public partial class LengthWindow : Form
    {
        public double _length;
        public LengthWindow(ref double length)
        {
            InitializeComponent();
            lengthInput.Text = length.ToString();
            _length = length;
        }

        private void lengthInput_TextChanged(object sender, EventArgs e)
        {
            if (!double.TryParse((sender as TextBox)?.Text, out _length))
                _length = 0;
        }

        private void OkButton_Click(object sender, EventArgs e)
        {
            Close();
        }
        
        private void close_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}
