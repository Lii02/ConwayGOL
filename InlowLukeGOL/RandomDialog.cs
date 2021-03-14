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
    public partial class RandomDialog : Form
    {
        public int seed { get; set; }

        public RandomDialog()
        {
            InitializeComponent();
            this.seed = 0;
        }

        private void seedBox_ValueChanged(object sender, EventArgs e)
        {
            this.seed = (int)seedBox.Value;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            this.DialogResult = DialogResult.Cancel;
        }

        private void randomSeedButton_Click(object sender, EventArgs e)
        {
            this.seed = (int)(new Random().Next());
            this.seedBox.Value = seed;
        }
    }
}
