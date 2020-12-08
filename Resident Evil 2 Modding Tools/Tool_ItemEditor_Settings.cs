using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Resident_Evil_2_Modding_Tools
{
    public partial class Tool_ItemEditor_Settings : Form
    {
        public Tool_ItemEditor tool;

        public Tool_ItemEditor_Settings(Tool_ItemEditor t)
        {
            tool = t;
            InitializeComponent();
        }

        // Save
        private void button1_Click(object sender, EventArgs e)
        {
            tool.DisplayOnly((float)numericUpDown1.Value, (float)numericUpDown2.Value);
        }

        // All items
        private void button5_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = -100;
            numericUpDown2.Value = 100;
        }

        private void button2_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = -1;
            numericUpDown2.Value = 4;
        }

        private void button3_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = 4;
            numericUpDown2.Value = 9;
        }

        private void button4_Click(object sender, EventArgs e)
        {
            numericUpDown1.Value = 9;
            numericUpDown2.Value = 14;
        }
    }
}
