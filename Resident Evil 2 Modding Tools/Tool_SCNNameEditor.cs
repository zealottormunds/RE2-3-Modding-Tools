using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.IO;

namespace Resident_Evil_2_Modding_Tools
{
    public partial class Tool_SCNNameEditor : Form
    {
        public Tool_SCNNameEditor()
        {
            InitializeComponent();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        public byte[] fileBytes;
        public bool fileOpen = false;

        public string filePath = "";
        public List<string> fileList = new List<string>();
        public List<byte[]> param1Bytes = new List<byte[]>();
        public List<byte[]> unknownBytes = new List<byte[]>();

        int endOfRead = 0;

        public void OpenFile()
        {
            OpenFileDialog o = new OpenFileDialog();
            o.ShowDialog();

            if (o.FileName == "" || !File.Exists(o.FileName)) return;

            filePath = o.FileName;
            fileBytes = File.ReadAllBytes(filePath);

            int headerlength = Main.b_ReadInt(fileBytes, 0x20);
            int firstindex = Main.b_ReadInt(fileBytes, headerlength);
            int entrycount = (firstindex - headerlength) / 0x8;

            int index = firstindex;

            for(int x = 0; x < entrycount; x++)
            {
                string entry = "";

                while(fileBytes[index] != 0x0)
                {
                    entry = entry + (char)fileBytes[index];
                    index = index + 2;
                }

                index = index + 2;

                fileList.Add(entry);
                listBox1.Items.Add(entry);
            }

            endOfRead = index;

            fileOpen = true;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fileOpen == false || listBox1.SelectedIndex == -1) return;

            int selected = listBox1.SelectedIndex;
            textBox1.Text = fileList[selected];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (fileOpen == false || listBox1.SelectedIndex == -1) return;

            int selected = listBox1.SelectedIndex;
            fileList[selected] = textBox1.Text;
            listBox1.Items[selected] = textBox1.Text;
        }

        public byte[] ConvertToFile()
        {
            byte[] newBytes = new byte[0];

            int headerlength = Main.b_ReadInt(fileBytes, 0x20);
            int firstindex = Main.b_ReadInt(fileBytes, headerlength);
            int entrycount = (firstindex - headerlength) / 0x8;

            // Add header of original scn file
            newBytes = Main.b_AddBytes(newBytes, fileBytes, 0, 0, headerlength + (entrycount * 0x8));

            for(int x = 0; x < entrycount; x++)
            {
                // Fix offset
                newBytes = Main.b_ReplaceBytes(newBytes, BitConverter.GetBytes(newBytes.Length), headerlength + (x * 0x8));

                // Add entry (character, 0x0, repeat until end of string, then add 0x0 0x0)
                for (int y = 0; y < fileList[x].Length; y++)
                {
                    newBytes = Main.b_AddBytes(newBytes, new byte[] { (byte)fileList[x][y], 0x0 });
                }
                newBytes = Main.b_AddBytes(newBytes, new byte[] { 0x0, 0x0 });
            }

            int eof = newBytes.Length;
            newBytes = Main.b_ReplaceBytes(newBytes, BitConverter.GetBytes(eof), 0x38);

            newBytes = Main.b_AddBytes(newBytes, fileBytes, 0, endOfRead, fileBytes.Length - endOfRead);

            return newBytes;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileOpen == false) return;

            byte[] newf = ConvertToFile();

            File.WriteAllBytes(filePath, newf);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileOpen == false) return;

            byte[] newf = ConvertToFile();

            SaveFileDialog s = new SaveFileDialog();
            s.ShowDialog();

            if (s.FileName != "")
            {
                File.WriteAllBytes(s.FileName, newf);
            }
        }
    }
}
