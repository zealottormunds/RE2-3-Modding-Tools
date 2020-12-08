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
    public partial class Tool_GimmickEditor : Form
    {
        public Tool_GimmickEditor()
        {
            InitializeComponent();
        }

        public struct Vector3
        {
            public float x;
            public float y;
            public float z;

            public Vector3(float x, float y, float z)
            {
                this.x = x;
                this.y = y;
                this.z = z;
            }

            public static Vector3 Zero()
            {
                return new Vector3(0, 0, 0);
            }
        }

        List<string> re2door = new List<string>(new string[]
        {
            "Door_2_1_001_gimmick",
            "Door_2_1_003_gimmick",
            "Door_2_1_004_gimmick",
            "Door_2_1_005_gimmick",
            "Door_3_1_027_gimmick",
            "Door_2_1_028_gimmick",
            "Door_2_1_029_gimmick",
            "Door_2_1_208_gimmick",
            "Door_2_1_209_gimmick",

            "Door_2_1_114_gimmick",
            "Door_2_1_116_gimmick",
            "Door_2_1_117_gimmick",
            "Door_2_1_118_gimmick",
            "Door_2_1_119_gimmick",
            "Door_2_1_120_gimmick",
            "Door_2_1_121_gimmick",
            "Door_2_1_122_gimmick",

            "Door_2_1_206_gimmick",
        });

        string[] re2door_disp =
        {
            "Reception to West Hallway",
            "Main Hall to West Office",
            "West Hallway to Records Room",
            "Operation Room Chain Door",
            "Guard Room in Courtyard",
            "Eagle Courtyard Door",
            "Tapped Door in Entrance to Courtyard",
            "Heart Door, 3F East Storage",
            "One-Side Lock Door, 3F",

            "2F Hall to Waiting Room",
            "Spade Door, Waiting Room",
            "2F Emergency Exit",
            "2F Rooftops",
            "Art Room Door",
            "Heart Door, Iron's Office",
            "Iron's Private Section Hallway Door",
            "Iron's Private Section Door",

            "3F East Storage to Stairs"
        };

        public bool fileOpen = false;
        public string filePath = "";
        public byte[] fileBytes;

        public List<string> gimmickList = new List<string>();
        public List<int> gimmickIndex = new List<int>();
        public List<int> nameIndex = new List<int>();
        public List<bool> isDoor = new List<bool>();

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseFile();

            if(fileOpen == false) OpenFile();
        }

        public void OpenFile()
        {
            OpenFileDialog o = new OpenFileDialog();
            o.ShowDialog();

            if(o.FileName != "" && File.Exists(o.FileName))
            {
                filePath = o.FileName;
                fileBytes = File.ReadAllBytes(filePath);
                int headerlen = Main.b_ReadInt(fileBytes, 0x38);

                // G i m m i c k
                List<int> indices = Main.b_FindBytesList(fileBytes, new byte[] { 0x47, 0x00, 0x69, 0x00, 0x6D, 0x00, 0x6D, 0x00, 0x69, 0x00, 0x63, 0x00, 0x6B, 0x00, 0x00, 0x00});
                gimmickIndex = indices;

                for (int x = 0; x < indices.Count; x++)
                {
                    int ind = indices[x];
                    int ind2 = ind - 0xC;

                    byte a = fileBytes[ind2];
                    while(a != 0x0)
                    {
                        ind2 = ind2 - 2;
                        a = fileBytes[ind2];
                    }

                    ind2 = ind2 + 2;

                    string name = Main.b_ReadRE2String(fileBytes, ind2);
                    gimmickList.Add(name);
                    nameIndex.Add(ind2);
                    isDoor.Add(false);

                    listBox1.Items.Add(name);
                }

                // P r o p s 
                List<int> propIndices = Main.b_FindBytesList(fileBytes, new byte[] { 0x50, 0x00, 0x72, 0x00, 0x6F, 0x00, 0x70, 0x00, 0x73, 0x00, 0x00, 0x00 });
                for (int x = 0; x < propIndices.Count; x++)
                {
                    int ind = propIndices[x];
                    int ind2 = ind - 0xC;

                    byte a = fileBytes[ind2];
                    while (a != 0x0)
                    {
                        ind2 = ind2 - 2;
                        a = fileBytes[ind2];
                    }

                    ind2 = ind2 + 2;

                    string name = Main.b_ReadRE2String(fileBytes, ind2);
                    gimmickIndex.Add(ind - 0x4);
                    gimmickList.Add(name);
                    nameIndex.Add(ind2);
                    isDoor.Add(false);

                    listBox1.Items.Add(name);
                }

                // 0 1 A
                List<int> prop01a_indices = Main.b_FindBytesList(fileBytes, new byte[] { 0x30, 0x00, 0x31, 0x00, 0x41, 0x00, 0x00, 0x00, 0x00, 0x00, 0x01 });
                for (int x = 0; x < prop01a_indices.Count; x++)
                {
                    int ind = prop01a_indices[x];
                    int ind2 = ind;

                    byte a = fileBytes[ind2];
                    while (a != 0x0)
                    {
                        ind2 = ind2 - 2;
                        a = fileBytes[ind2];
                    }

                    ind2 = ind2 + 2;

                    string name = Main.b_ReadRE2String(fileBytes, ind2);
                    
                    if(gimmickIndex.Contains(ind - 0x2) == false)
                    {
                        gimmickIndex.Add(ind - 0x2);
                        gimmickList.Add(name);
                        nameIndex.Add(ind2);
                        isDoor.Add(false);

                        listBox1.Items.Add("[Mesh] " + name);
                    }
                }

                // s m
                // 00 00 73 00 6D
                List<int> indices3 = Main.b_FindBytesList(fileBytes, new byte[] { 0x00, 0x00, 0x00, 0x73, 0x00, 0x6D });
                for (int x = 0; x < indices3.Count; x++)
                {
                    int ind = indices3[x];
                    int ind2 = ind + 3;

                    string name = Main.b_ReadRE2String(fileBytes, ind2);

                    int realindex = Main.b_FindBytes(fileBytes, new byte[] { 0x00, 0x00, 0x80, 0xBF }, ind2) - 0x14;

                    if (gimmickIndex.Contains(realindex) == false)
                    {
                        gimmickIndex.Add(realindex);
                        gimmickList.Add(name);
                        nameIndex.Add(ind2);
                        isDoor.Add(false);

                        listBox1.Items.Add("[SMXX] " + name);
                    }
                }

                // G a t e
                List<int> doorIndices = Main.b_FindBytesList(fileBytes, new byte[] { 0x67, 0x00, 0x69, 0x00, 0x6D, 0x00, 0x6D, 0x00, 0x69, 0x00, 0x63, 0x00, 0x6B, 0x00, 0x00, 0x00, 0x00, 0x00, 0x05, 0x00, 0x00, 0x00, 0x47, 0x00, 0x61, 0x00, 0x74, 0x00, 0x65, 0x00 });

                for (int x = 0; x < doorIndices.Count; x++)
                {
                    int ind = doorIndices[x];
                    int ind2 = ind;

                    byte a = fileBytes[ind2];
                    while (a != 0x0)
                    {
                        ind2 = ind2 - 2;
                        a = fileBytes[ind2];
                    }

                    ind2 = ind2 + 2;

                    string name = Main.b_ReadRE2String(fileBytes, ind2);
                    gimmickIndex.Add(ind + 0xC + 2);
                    gimmickList.Add(name);
                    nameIndex.Add(ind2);
                    isDoor.Add(true);

                    int knownDoor = re2door.IndexOf(name);
                    if (knownDoor != -1)
                    {
                        name = name + " (" + re2door_disp[knownDoor] + ")";
                    }

                    listBox1.Items.Add(name);
                }

                fileOpen = true;
            }
            else
            {
                return;
            }
        }

        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fileOpen == false || listBox1.SelectedIndex == -1) return;

            int selected = listBox1.SelectedIndex;

            textBox6.Text = gimmickList[selected];

            for (int x = 0; x < 14; x++) changedValue[x] = false;

            // Get properties
            int propindex = gimmickIndex[selected] + 0x14;

            float prop = Main.b_ReadFloat(fileBytes, propindex);
            numericUpDown1.Value = (decimal)prop;
            propindex = propindex + 0x4;

            prop = Main.b_ReadFloat(fileBytes, propindex);
            numericUpDown2.Value = (decimal)prop;
            propindex = propindex + 0x4;

            prop = Main.b_ReadFloat(fileBytes, propindex);
            numericUpDown3.Value = (decimal)prop;
            propindex = propindex + 0x4;

            prop = Main.b_ReadFloat(fileBytes, propindex);
            numericUpDown4.Value = (decimal)prop;
            propindex = propindex + 0x4;

            prop = Main.b_ReadFloat(fileBytes, propindex);
            numericUpDown5.Value = (decimal)prop;
            propindex = propindex + 0x4;

            prop = Main.b_ReadFloat(fileBytes, propindex);
            numericUpDown6.Value = (decimal)prop;
            propindex = propindex + 0x4;

            prop = Main.b_ReadFloat(fileBytes, propindex);
            numericUpDown7.Value = (decimal)prop;
            propindex = propindex + 0x4;

            prop = Main.b_ReadFloat(fileBytes, propindex);
            numericUpDown8.Value = (decimal)prop;
            propindex = propindex + 0x4;

            prop = Main.b_ReadFloat(fileBytes, propindex);
            numericUpDown9.Value = (decimal)prop;
            propindex = propindex + 0x4;

            prop = Main.b_ReadFloat(fileBytes, propindex);
            numericUpDown10.Value = (decimal)prop;
            propindex = propindex + 0x4;

            prop = Main.b_ReadFloat(fileBytes, propindex);
            numericUpDown11.Value = (decimal)prop;
            propindex = propindex + 0x4;

            prop = Main.b_ReadFloat(fileBytes, propindex);
            numericUpDown12.Value = (decimal)prop;
            propindex = propindex + 0x4;

            prop = Main.b_ReadFloat(fileBytes, propindex);
            numericUpDown13.Value = (decimal)prop;
            propindex = propindex + 0x4;

            prop = Main.b_ReadFloat(fileBytes, propindex);
            numericUpDown14.Value = (decimal)prop;
            propindex = propindex + 0x4;
        }

        private void button1_Click(object sender, EventArgs e)
        {
            if (fileOpen == false || listBox1.SelectedIndex == -1) return;

            int selected = listBox1.SelectedIndex;

            // Get properties
            int propindex = gimmickIndex[selected] + 0x14;

            if(changedValue[0]) fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes((float)numericUpDown1.Value), propindex);
            propindex = propindex + 0x4;

            if (changedValue[1]) fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes((float)numericUpDown2.Value), propindex);
            propindex = propindex + 0x4;

            if (changedValue[2]) fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes((float)numericUpDown3.Value), propindex);
            propindex = propindex + 0x4;

            if (changedValue[3]) fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes((float)numericUpDown4.Value), propindex);
            propindex = propindex + 0x4;

            if (changedValue[4]) fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes((float)numericUpDown5.Value), propindex);
            propindex = propindex + 0x4;

            if (changedValue[5]) fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes((float)numericUpDown6.Value), propindex);
            propindex = propindex + 0x4;

            if (changedValue[6]) fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes((float)numericUpDown7.Value), propindex);
            propindex = propindex + 0x4;

            if (changedValue[7]) fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes((float)numericUpDown8.Value), propindex);
            propindex = propindex + 0x4;

            if (changedValue[8]) fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes((float)numericUpDown9.Value), propindex);
            propindex = propindex + 0x4;

            if (changedValue[9]) fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes((float)numericUpDown10.Value), propindex);
            propindex = propindex + 0x4;

            if (changedValue[10]) fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes((float)numericUpDown11.Value), propindex);
            propindex = propindex + 0x4;

            if (changedValue[11]) fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes((float)numericUpDown12.Value), propindex);
            propindex = propindex + 0x4;

            if (changedValue[12]) fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes((float)numericUpDown13.Value), propindex);
            propindex = propindex + 0x4;

            if (changedValue[13]) fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes((float)numericUpDown14.Value), propindex);
            propindex = propindex + 0x4;

            if (textBox6.Text != gimmickList[selected])
            {
                gimmickList[selected] = textBox6.Text;
                List<byte> newname = new List<byte>();
                for (int x = 0; x < textBox6.Text.Length; x++)
                {
                    newname.Add((byte)textBox6.Text[x]);
                    newname.Add(0x00);
                }
                fileBytes = Main.b_ReplaceBytes(fileBytes, newname.ToArray(), nameIndex[selected]);

                listBox1.Items[selected] = textBox6.Text;
            }
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            File.WriteAllBytes(filePath, fileBytes);
            MessageBox.Show("File saved to " + filePath);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog s = new SaveFileDialog();
            s.ShowDialog();

            if(s.FileName != "")
            {
                filePath = s.FileName;
                File.WriteAllBytes(filePath, fileBytes);
                MessageBox.Show("File saved to " + filePath);
            }
        }

        bool[] changedValue =
        {
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
            false,
        };

        private void numericUpDown1_ValueChanged(object sender, EventArgs e) { changedValue[0] = true; }
        private void numericUpDown2_ValueChanged(object sender, EventArgs e) { changedValue[1] = true; }
        private void numericUpDown3_ValueChanged(object sender, EventArgs e) { changedValue[2] = true; }
        private void numericUpDown4_ValueChanged(object sender, EventArgs e) { changedValue[3] = true; }
        private void numericUpDown5_ValueChanged(object sender, EventArgs e) { changedValue[4] = true; }
        private void numericUpDown6_ValueChanged(object sender, EventArgs e) { changedValue[5] = true; }
        private void numericUpDown7_ValueChanged(object sender, EventArgs e) { changedValue[6] = true; }
        private void numericUpDown8_ValueChanged(object sender, EventArgs e) { changedValue[7] = true; }
        private void numericUpDown9_ValueChanged(object sender, EventArgs e) { changedValue[8] = true; }
        private void numericUpDown10_ValueChanged(object sender, EventArgs e) { changedValue[9] = true; }
        private void numericUpDown11_ValueChanged(object sender, EventArgs e) { changedValue[10] = true; }
        private void numericUpDown12_ValueChanged(object sender, EventArgs e) { changedValue[11] = true; }
        private void numericUpDown13_ValueChanged(object sender, EventArgs e) { changedValue[12] = true; }
        private void numericUpDown14_ValueChanged(object sender, EventArgs e) { changedValue[13] = true; }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (fileOpen == false) return;

            DialogResult a = MessageBox.Show("Are you sure you want to close this file?", "", MessageBoxButtons.YesNo);
            if(a == DialogResult.Yes) CloseFile();
        }

        void CloseFile()
        {
            fileOpen = false;
            filePath = "";
            fileBytes = new byte[0];

            listBox1.SelectedIndex = -1;

            textBox6.Text = "";
            numericUpDown1.Value = 0;
            numericUpDown2.Value = 0;
            numericUpDown3.Value = 0;
            numericUpDown4.Value = 0;
            numericUpDown5.Value = 0;
            numericUpDown6.Value = 0;
            numericUpDown7.Value = 0;
            numericUpDown8.Value = 0;
            numericUpDown9.Value = 0;
            numericUpDown10.Value = 0;
            numericUpDown11.Value = 0;
            numericUpDown12.Value = 0;
            numericUpDown13.Value = 0;
            numericUpDown14.Value = 0;

            gimmickList.Clear();
            gimmickIndex.Clear();
            nameIndex.Clear();
            isDoor.Clear();
            listBox1.Items.Clear();

            for (int x = 0; x < 14; x++) changedValue[x] = false;
        }
    }
}
