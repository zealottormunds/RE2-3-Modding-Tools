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
    public partial class Tool_RSZNameViewer : Form
    {
        public Tool_RSZNameViewer()
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
        public List<string> fileList2 = new List<string>();
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
            int entrycount = (headerlength - 0x40) / 0x10;

            int actualIndex = headerlength;
            for(int x = 0; x < entrycount - 1; x++)
            {
                // Get param1 (unknown)
                byte[] param1 = Main.b_ReadByteArray(fileBytes, actualIndex, 4);
                param1Bytes.Add(param1);
                actualIndex = actualIndex + 4;

                // Get size of main string
                int strsize = Main.b_ReadInt(fileBytes, actualIndex) * 2;
                actualIndex = actualIndex + 4;

                // Get main string
                string entry = "";
                for(int y = 0; y < strsize - 2; y++)
                {
                    entry = entry + (char)fileBytes[actualIndex + y];
                    y = y + 1;
                }
                actualIndex = actualIndex + strsize;

                // Fix index if it's not a multiple of 4
                while (actualIndex % 4 != 0) actualIndex++;
                
                // Get size of secondary string
                int str2size = Main.b_ReadInt(fileBytes, actualIndex) * 2;
                actualIndex = actualIndex + 4;

                // Get secondary string
                string entry2 = "";
                for(int y = 0; y < str2size - 2; y++)
                {
                    entry2 = entry2 + (char)fileBytes[actualIndex + y];
                    y = y + 1;
                }
                actualIndex = actualIndex + str2size;

                // Skip a few unknown bytes
                byte[] unk = Main.b_ReadByteArray(fileBytes, actualIndex, 0xA);
                unknownBytes.Add(unk);
                actualIndex = actualIndex + 0xA;

                // Add entry to the list
                listBox1.Items.Add(entry);
                fileList.Add(entry);
                fileList2.Add(entry2);
            }

            endOfRead = actualIndex;

            fileOpen = true;
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fileOpen == false || listBox1.SelectedIndex == -1) return;

            int selected = listBox1.SelectedIndex;
            textBox1.Text = fileList[selected];
            textBox2.Text = fileList2[selected];
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (fileOpen == false || listBox1.SelectedIndex == -1) return;

            int selected = listBox1.SelectedIndex;
            fileList[selected] = textBox1.Text;
            fileList2[selected] = textBox2.Text;

            listBox1.Items[selected] = textBox1.Text;
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

        public byte[] ConvertToFile()
        {
            byte[] newBytes = new byte[0];

            int headerlength = Main.b_ReadInt(fileBytes, 0x20);
            int entrycount = (headerlength - 0x40) / 0x10;

            // Add header of original rsz file
            newBytes = Main.b_AddBytes(newBytes, fileBytes, 0, 0, headerlength);

            // Write entries
            for(int x = 0; x < entrycount - 1; x++)
            {
                // Add unknown param1 of entry
                newBytes = Main.b_AddBytes(newBytes, param1Bytes[x], 0, 0, 4);

                // Add size of entry
                newBytes = Main.b_AddInt(newBytes, fileList[x].Length + 1);

                // Add entry (character, 0x0, repeat until end of string, then add 0x0 0x0)
                for(int y = 0; y < fileList[x].Length; y++)
                {
                    newBytes = Main.b_AddBytes(newBytes, new byte[] {(byte)fileList[x][y], 0x0});
                }
                newBytes = Main.b_AddBytes(newBytes, new byte[] { 0x0, 0x0 });

                // Fix paging if actualIndex % 4 != 0
                while (newBytes.Length % 4 != 0) newBytes = Main.b_AddBytes(newBytes, new byte[] { 0x0 });

                // Add secondary string size
                newBytes = Main.b_AddInt(newBytes, fileList2[x].Length + 1);

                // Add secondary string
                for (int y = 0; y < fileList2[x].Length; y++)
                {
                    newBytes = Main.b_AddBytes(newBytes, new byte[] { (byte)fileList2[x][y], 0x0 });
                }
                newBytes = Main.b_AddBytes(newBytes, new byte[] { 0x0, 0x0 });

                // Add unknown bytes
                newBytes = Main.b_AddBytes(newBytes, unknownBytes[x]);
            }

            newBytes = Main.b_AddBytes(newBytes, fileBytes, 0, endOfRead, fileBytes.Length - endOfRead);

            return newBytes;
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            fileBytes = new byte[0];
            fileOpen = false;
            filePath = "";

            fileList.Clear();
            fileList2.Clear();
            param1Bytes.Clear();
            unknownBytes.Clear();
            endOfRead = 0;

            textBox1.Text = "";
            textBox2.Text = "";

            listBox1.Items.Clear();
        }
    }
}
