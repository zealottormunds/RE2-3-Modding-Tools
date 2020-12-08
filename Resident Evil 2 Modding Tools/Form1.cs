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
    public partial class Form1 : Form
    {
        public Form1()
        {
            InitializeComponent();
        }

        private void button1_Click(object sender, EventArgs e)
        {
            Tool_RSZNameViewer t = new Tool_RSZNameViewer();
            t.ShowDialog();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            Tool_SCNNameEditor t = new Tool_SCNNameEditor();
            t.ShowDialog();
        }

        private void button3_Click(object sender, EventArgs e)
        {
            Tool_GimmickEditor t = new Tool_GimmickEditor();
            t.ShowDialog();
        }

        private void button4_Click(object sender, EventArgs e)
        {
            Tool_ItemEditor t = new Tool_ItemEditor();
            t.ShowDialog();
        }

        private void button5_Click(object sender, EventArgs e)
        {
            Tool_EnemyPlacement t = new Tool_EnemyPlacement();
            t.ShowDialog();
        }

        private void aboutToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Resident Evil 2/3 Modding Tools by Zealot Tormunds.\n\nContact: Zealot#5316 on Discord.");
        }
    }
}
