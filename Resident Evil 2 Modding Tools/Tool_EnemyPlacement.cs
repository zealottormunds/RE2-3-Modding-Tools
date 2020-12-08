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
    public partial class Tool_EnemyPlacement : Form
    {
        public Tool_EnemyPlacement()
        {
            InitializeComponent();

            // Male Zombie
            HashList.Add(new byte[] { 0x83, 0xD8, 0x11, 0xDE, 0x75, 0x32, 0xC6, 0x70 });
            IdList.Add(0x00);

            // Female Zombie
            HashList.Add(new byte[] { 0x6F, 0xC1, 0x42, 0x5E, 0x88, 0x4C, 0x84, 0x05 });
            IdList.Add(0x01);

            // Fat Zombie
            HashList.Add(new byte[] { 0x72, 0x1B, 0x8D, 0x28, 0xA7, 0xC5, 0x8B, 0x05 });
            IdList.Add(0x02);

            // Licker
            HashList.Add(new byte[] { 0x2F, 0xBF, 0xAC, 0x1A, 0xD1, 0xA2, 0xB5, 0x0E });
            IdList.Add(0x03);

            // Dog
            HashList.Add(new byte[] { 0xBD, 0xDB, 0xC2, 0xBD, 0xB2, 0x1A, 0x63, 0xCA });
            IdList.Add(0x04);

            // Ivy
            HashList.Add(new byte[] { 0xFE, 0xF3, 0x3C, 0xFA, 0x8C, 0x1F, 0x8A, 0xC3 });
            IdList.Add(0x07);

            // G-Adult
            HashList.Add(new byte[] { 0x64, 0xF1, 0x70, 0x2A, 0xD8, 0xED, 0xAA, 0xAF });
            IdList.Add(0x08);

            // Tyrant
            HashList.Add(new byte[] { 0x7E, 0xD2, 0x9C, 0xB9, 0x93, 0xF2, 0x53, 0x10 });
            IdList.Add(0x0A);

            // Super Tyrant
            HashList.Add(new byte[] { 0x6C, 0x64, 0x68, 0x1C, 0x74, 0x0D, 0xF3, 0xA0 });
            IdList.Add(0x0B);

            // G-1
            HashList.Add(new byte[] { 0xEC, 0x05, 0xEA, 0x1E, 0x6C, 0x81, 0x65, 0xBF });
            IdList.Add(0x0C);

            // G-2
            HashList.Add(new byte[] { 0x1F, 0xB3, 0x5C, 0xBF, 0x67, 0x07, 0x10, 0x9E });
            IdList.Add(0x0D);

            // G-3
            HashList.Add(new byte[] { 0xF8, 0x84, 0xD0, 0x71, 0x5B, 0xA1, 0x1E, 0x0C });
            IdList.Add(0x0F);

            // G-4
            HashList.Add(new byte[] { 0x88, 0x87, 0xAB, 0x37, 0x17, 0x17, 0x28, 0x85 });
            IdList.Add(0x10);

            // G-5
            HashList.Add(new byte[] { 0x7C, 0x86, 0x3C, 0x7F, 0x6E, 0x50, 0xA0, 0x57 });
            IdList.Add(0x11);

            // Bomb Zombie
            HashList.Add(new byte[] { 0xAD, 0x69, 0x7D, 0xAA, 0x54, 0x40, 0xDE, 0x98 });
            IdList.Add(0x15);

            // Pale Head
            HashList.Add(new byte[] { 0x17, 0x97, 0x8E, 0x2F, 0x77, 0x4D, 0x41, 0x26 });
            IdList.Add(0x17);

            // Poison Zombie
            HashList.Add(new byte[] { 0x3A, 0x0B, 0xC7, 0x4E, 0x4C, 0x07, 0x7C, 0xC8 });
            IdList.Add(0x18);
        }

        List<byte[]> HashList = new List<byte[]>();
        List<byte> IdList = new List<byte>();

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFile();
        }

        public bool fileOpen = false;
        public string filePath = "";
        public byte[] fileBytes;

        List<int> hashIndex = new List<int>();
        List<int> entryIndex = new List<int>();

        public void OpenFile()
        {
            OpenFileDialog o = new OpenFileDialog();
            o.ShowDialog();

            if (o.FileName == "" || File.Exists(o.FileName) == false) return;

            filePath = o.FileName;
            fileBytes = File.ReadAllBytes(filePath);

            List<int> indices = Main.b_FindBytesList(fileBytes, new byte[] { 0x17, 0xA4, 0x02, 0xC9, 0x0F, 0x78, 0xC2, 0x76 });
            for(int x = 0; x < indices.Count; x++)
            {
                byte[] seq = Main.b_ReadByteArray(fileBytes, indices[x] - 0x8, 8);

                int enemyIndex = -1;
                for (int y = 0; y < HashList.Count; y++)
                {
                    if (Enumerable.SequenceEqual(seq, HashList[y]) == true)
                    {
                        enemyIndex = y;
                        y = HashList.Count;
                    }
                }

                string seqstr = "";
                for (int y = 0; y < seq.Length; y++) seqstr = seqstr + seq[y].ToString("X2") + " ";

                bool isSpawn = (enemyIndex != -1);

                if (isSpawn)
                {
                    hashIndex.Add(indices[x] - 0x8);

                    string enemyName = listBox1.Items.Count.ToString() + " - " + comboBox1.Items[enemyIndex].ToString();
                    listBox1.Items.Add(enemyName);
                }
                else
                {
                    hashIndex.Add(indices[x] - 0x8);

                    string enemyName = listBox1.Items.Count.ToString() + " - UNK: [" + seqstr + "]";
                    listBox1.Items.Add(enemyName);
                }
            }

            /*

            int spawnCount = Main.b_ReadInt(fileBytes, 0x4);
            List<int> hashCount = new List<int>();

            int actualIndex = 0x58;
            for(int x = 0; x < spawnCount; x++)
            {
                int c = fileBytes[actualIndex];
                //MessageBox.Show(x.ToString() + ": " + c.ToString());
                hashCount.Add(c);
                actualIndex = actualIndex + 0x20;
            }
            //MessageBox.Show("Spawn count: " + spawnCount.ToString("X2"));

            int rszOffset = Main.b_ReadInt(fileBytes, 0x38);
            int hashOffset = rszOffset + Main.b_ReadInt(fileBytes, rszOffset + 0x18);
            //MessageBox.Show("RSZ Offset: " + rszOffset.ToString("X2"));
            //MessageBox.Show("Hash Offset: " + hashOffset.ToString("X2"));

            int actualSpawnIndex = rszOffset + 0x30;

            for(int x = 0; x < spawnCount; x++)
            {
                int spawnIndex = actualSpawnIndex;
                int spawnHashIndex = Main.b_ReadInt(fileBytes, spawnIndex + (hashCount[x] * 0x4));
                int spawnHashOffset = hashOffset + (0x8 * spawnHashIndex);
                actualSpawnIndex = actualSpawnIndex + (hashCount[x] * 0x4) + 0x4;

                byte[] byteSequence = Main.b_ReadByteArray(fileBytes, spawnHashOffset, 8);
                string seq = "";
                for (int y = 0; y < byteSequence.Length; y++) seq = seq + byteSequence[y].ToString("X2") + " ";

                MessageBox.Show(x.ToString() + " - " + "SID: " + spawnHashIndex.ToString("X2") + ", Spawn Hash Offset: " + spawnHashOffset.ToString("X2") + ", Hash: " + seq);

                int enemyIndex = -1;
                for(int y = 0; y < HashList.Count; y++)
                {
                    if(Enumerable.SequenceEqual(byteSequence, HashList[y]) == true)
                    {
                        enemyIndex = y;
                        y = HashList.Count;
                    }
                }

                bool isSpawn = (enemyIndex != -1);

                if (isSpawn)
                {
                    hashIndex.Add(spawnHashOffset);

                    string enemyName = listBox1.Items.Count.ToString() + " - " + comboBox1.Items[enemyIndex].ToString();
                    listBox1.Items.Add(enemyName);
                }
                else
                {
                    hashIndex.Add(spawnHashOffset);

                    string enemyName = listBox1.Items.Count.ToString() + " - UNK: [" + seq + "]";
                    listBox1.Items.Add(enemyName);
                }
            }*/
        }
    }
}
