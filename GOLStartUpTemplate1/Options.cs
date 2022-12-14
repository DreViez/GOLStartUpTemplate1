using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GOLStartUpTemplate1
{
    public partial class Options : Form
    {
        public int interval;
        public int X;
        public int Y;
        public Options()
        {
            InitializeComponent();
        }

        private void buttonOK_Click(object sender, EventArgs e)
        {
            interval = (int)numericUpDown1.Value;
            X = (int)numericUpDown2.Value;
            Y = (int)numericUpDown3.Value;
        }

        private void Options_Load(object sender, EventArgs e)
        {

        }
    }
}
