using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace InlowLukeGOL
{
    public partial class PropertiesDialog : Form
    {
        public int universeX { get; set; }
        public int universeY { get; set; }
        public int timeInterval { get; set; }

        public PropertiesDialog(int universeX, int universeY, int timeInterval)
        {
            InitializeComponent();
            this.universeX = universeX;
            this.universeY = universeY;
            this.timeInterval = timeInterval;

            // Set initial numeric values
            numericUpDown4.Value = universeX;
            numericUpDown3.Value = universeY;
            numericUpDown1.Value = timeInterval;
        }

        private void numericUpDown4_ValueChanged(object sender, EventArgs e)
        {
            this.universeX = (int)numericUpDown4.Value;
        }

        private void numericUpDown3_ValueChanged(object sender, EventArgs e)
        {
            this.universeY = (int)numericUpDown3.Value;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void numericUpDown1_ValueChanged(object sender, EventArgs e)
        {
            this.timeInterval = (int)numericUpDown1.Value;
        }
    }
}
