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
    public partial class Tool_ItemEditor : Form
    {
        public Tool_ItemEditor()
        {
            InitializeComponent();

            UseItems(0);
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CloseFile();

            if(fileOpen == false) OpenFile();
        }

        public bool fileOpen = false;
        public string filePath = "";
        public byte[] fileBytes;
        public List<int> itemIndices = new List<int>();
        public List<string> itemDesc = new List<string>();

        public List<PictureBox> pictures = new List<PictureBox>();
        //ItemDisplay3D render = new ItemDisplay3D();

        public List<string> actualItems = new List<string>();
        public List<string> actualWeapons = new List<string>();

        string GetName(int itemid, int wepid, int quant)
        {
            bool isItem = (wepid == -1);

            string it = "";
            if (isItem)
            {
                it = "ITEM - ";
                if (itemid < 0x100) it = it + "00";
                else it = it + "0";
                if (actualItems.Count > itemid) it = it + actualItems[itemid];
                it = it + " (x" + quant.ToString() + ")";
            }
            else
            {
                it = "WEP - ";

                if (wepid < 0x100) it = it + "00";
                else it = it + "0";

                if (wepid > 0 && actualWeapons.Count > wepid) it = it + actualWeapons[wepid];
                else it = it + wepid.ToString("X2") + " ???";

                it = it + " (x" + quant.ToString() + ")";
            }

            return it;
        }

        public void OpenFile()
        {
            OpenFileDialog o = new OpenFileDialog();
            o.ShowDialog();

            if (o.FileName == "" || File.Exists(o.FileName) == false) return;

            filePath = o.FileName;
            fileBytes = File.ReadAllBytes(filePath);

            string[] spl = filePath.Split('\\');
            int lenspl = spl.Length;
            string name = "";
            for(int x = 3; x > 0; x--)
            {
                int actual = lenspl - x;
                if (lenspl > actual) name = name + "\\" + spl[actual];
            }

            this.Text = "Item List Editor (" + name + ")";

            List<int> indices = Main.b_FindBytesList(fileBytes, new byte[] { 0x50, 0x00, 0x69, 0x00, 0x63, 0x00, 0x6B, 0x00, 0x00, 0x00, 0x01 });
            MessageBox.Show("Found " + (indices.Count / 2).ToString() + " entries");

            for(int x = 0; x < indices.Count; x++)
            {
                itemIndices.Add(indices[x]);
                //itemDesc.Add("[Null]");
                x = x + 1;
            }

            for(int x = 0; x < itemIndices.Count; x++)
            {
                int itemid = Main.b_ReadInt(fileBytes, itemIndices[x] - 0x80);
                int wepid = Main.b_ReadInt(fileBytes, itemIndices[x] - 0x7C);
                int quant = Main.b_ReadInt(fileBytes, itemIndices[x] - 0x74);

                string it = GetName(itemid, wepid, quant);

                listBox1.Items.Add(it);
            }

            // Create item map
            for(int x = 0; x < itemIndices.Count; x++)
            {
                float px = Main.b_ReadFloat(fileBytes, itemIndices[x] - 0xBC);
                float py = Main.b_ReadFloat(fileBytes, itemIndices[x] - 0xBC + 4);
                float pz = Main.b_ReadFloat(fileBytes, itemIndices[x] - 0xBC + 8);

                if (px < minx) minx = px;
                if (pz < minz) minz = pz;
                if (px > maxx) maxx = px;
                if (pz > maxz) maxz = pz;

                //render.items.Add(new Vector3(px, py, pz));
            }
            
            maxxper = maxx + Math.Abs(minx);
            maxzper = maxz + Math.Abs(minz);

            // Load metadata if it exists
            int metaindex = Main.b_FindString(fileBytes, "ITEMEDITORDATA");
            int itemeditordata = metaindex;

            if(metaindex != -1)
            {
                metaindex = metaindex + 0xE;

                bool checkbox = Main.b_ReadInt(fileBytes, metaindex) == 1;
                enableMetadataSavingToolStripMenuItem.Checked = checkbox;

                minx = Main.b_ReadFloat(fileBytes, metaindex + 4);
                minz = Main.b_ReadFloat(fileBytes, metaindex + 8);
                maxx = Main.b_ReadFloat(fileBytes, metaindex + 12);
                maxz = Main.b_ReadFloat(fileBytes, metaindex + 16);
                maxxper = Main.b_ReadFloat(fileBytes, metaindex + 20);
                maxzper = Main.b_ReadFloat(fileBytes, metaindex + 24);

                metaindex = metaindex + 28;
                int descCount = Main.b_ReadInt(fileBytes, metaindex);

                metaindex = metaindex + 4;
                for(int x = 0; x < descCount; x++)
                {
                    int len = Main.b_ReadInt(fileBytes, metaindex);
                    metaindex = metaindex + 4;

                    string str = Main.b_ReadString(fileBytes, metaindex, len);
                    //MessageBox.Show("i: " + metaindex.ToString("X2") + ", str: " + str);
                    itemDesc.Add(str);

                    metaindex = metaindex + len;
                }

                byte[] newfilebytes = new byte[0];
                newfilebytes = Main.b_AddBytes(newfilebytes, fileBytes, 0, 0, itemeditordata);
                fileBytes = newfilebytes;
            }

            int descc = itemDesc.Count;
            for (int x = 0; x < itemIndices.Count - descc; x++)
            {
                int itemStringIndex = Main.b_FindRE2String(fileBytes, "Item", itemIndices[x] - 0x11C) - 0xC;
                //MessageBox.Show(itemStringIndex.ToString("X2"));

                string desc = "";

                if(itemStringIndex > 0)
                {
                    byte a = fileBytes[itemStringIndex];
                    while (a != 0x0)
                    {
                        itemStringIndex = itemStringIndex - 2;
                        a = fileBytes[itemStringIndex];
                    }

                    itemStringIndex = itemStringIndex + 2;

                    desc = Main.b_ReadRE2String(fileBytes, itemStringIndex);
                }

                if(desc.Length > 3) itemDesc.Add(desc);
                else itemDesc.Add("[Null]");
            }

            /*descc = itemDesc.Count;
            for (int x = 0; x < indices.Count - descc; x++)
            {
                itemDesc.Add("[Null]");
                x = x + 1;
            }*/

            for (int x = 0; x < itemIndices.Count; x++)
            {
                float px = Main.b_ReadFloat(fileBytes, itemIndices[x] - 0xBC) + Math.Abs(minx);
                float pz = Main.b_ReadFloat(fileBytes, itemIndices[x] - 0xBC + 8) + Math.Abs(minz);

                float perx = (px) / maxxper;
                float perz = (pz) / maxzper;

                PictureBox p = new PictureBox();
                p.BackColor = Color.Red;
                p.Visible = true;
                p.Size = new Size(8, 8);
                p.Location = new Point(294 + (int)(perx * 364), 123 + (int)(perz * 212));
                this.Controls.Add(p);

                pictures.Add(p);
            }

            //MessageBox.Show("minx: " + minx.ToString() + ", minz: " + minz.ToString() + ", maxx: " + maxx.ToString() + ", maxz: " + maxz.ToString() + ", maxxper: " + maxxper.ToString() + ", maxzper: " + maxzper.ToString());

            fileOpen = true;

            //render.Run(60, 60);
        }

        float minx = 999999;
        float minz = 999999;
        float maxx = -999999;
        float maxz = -999999;
        float maxxper = 0;
        float maxzper = 0;

        public Point PosToPoint(int px, int pz)
        {
            float perx = (px + Math.Abs(minx)) / maxxper;
            float perz = (pz + Math.Abs(minz)) / maxzper;

            return new Point(294 + (int)(perx * 364), 123 + (int)(perz * 212));
        }

        public void SetGraphicsToRed()
        {
            for(int x = 0; x < pictures.Count; x++)
            {
                pictures[x].BackColor = Color.Red;
            }
        }

        // Change selected
        private void listBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fileOpen == false || listBox1.SelectedIndex == -1) return;

            int selected = listBox1.SelectedIndex;

            int itemid = Main.b_ReadInt(fileBytes, itemIndices[selected] - 0x80);
            int wepid = Main.b_ReadInt(fileBytes, itemIndices[selected] - 0x7C);
            int ammoid = Main.b_ReadInt(fileBytes, itemIndices[selected] - 0x78);
            int quant = Main.b_ReadInt(fileBytes, itemIndices[selected] - 0x74);

            bool isItem = (wepid == -1);

            textBox1.Text = itemDesc[selected];

            if(isItem == false)
            {
                comboBox1.SelectedIndex = -1;
                comboBox2.SelectedIndex = wepid;
                comboBox3.SelectedIndex = ammoid;
                numericUpDown1.Value = quant;
            }
            else
            {
                comboBox1.SelectedIndex = itemid;
                comboBox2.SelectedIndex = -1;
                comboBox3.SelectedIndex = 0;
                numericUpDown1.Value = quant;
            }

            float px = Main.b_ReadFloat(fileBytes, itemIndices[selected] - 0xBC);
            float py = Main.b_ReadFloat(fileBytes, itemIndices[selected] - 0xBC + 4);
            float pz = Main.b_ReadFloat(fileBytes, itemIndices[selected] - 0xBC + 8);

            if(px < -99999 || px > 99999 || py < -99999 || py > 99999 || pz < -99999 || pz > 99999)
            {
                MessageBox.Show("This item cannot be displayed");
            }
            else
            {
                numericUpDown2.Value = (decimal)px;
                numericUpDown3.Value = (decimal)py;
                numericUpDown4.Value = (decimal)pz;

                SetGraphicsToRed();
                pictures[selected].BackColor = Color.Blue;

                // Select any items with the same position
                for (int x = 0; x < itemIndices.Count; x++)
                {
                    float px2 = Main.b_ReadFloat(fileBytes, itemIndices[x] - 0xBC);
                    float py2 = Main.b_ReadFloat(fileBytes, itemIndices[x] - 0xBC + 4);
                    float pz2 = Main.b_ReadFloat(fileBytes, itemIndices[x] - 0xBC + 8);

                    if(px2 == px && py2 == py && pz2 == pz)
                    {
                        pictures[x].BackColor = Color.Blue;
                    }
                }
            }
        }

        // Save entry
        private void button1_Click(object sender, EventArgs e)
        {
            if (fileOpen == false || listBox1.SelectedIndex == -1) return;

            int selected = listBox1.SelectedIndex;

            int itemid = comboBox1.SelectedIndex;
            int wepid = comboBox2.SelectedIndex;
            int ammoid = comboBox3.SelectedIndex;
            int quant = (int)numericUpDown1.Value;

            float px = (float)numericUpDown2.Value;
            float py = (float)numericUpDown3.Value;
            float pz = (float)numericUpDown4.Value;

            itemDesc[selected] = textBox1.Text;

            bool isItem = (wepid == -1);

            if (isItem)
            {
                fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes(itemid), itemIndices[selected] - 0x80);
                fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes(-1), itemIndices[selected] - 0x7C);
                fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes(0), itemIndices[selected] - 0x78);
            }
            else
            {
                fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes(0), itemIndices[selected] - 0x80);
                fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes(wepid), itemIndices[selected] - 0x7C);
                fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes(ammoid), itemIndices[selected] - 0x78);
            }

            fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes(quant), itemIndices[selected] - 0x74);

            fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes(px), itemIndices[selected] - 0xBC);
            fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes(py), itemIndices[selected] - 0xBC + 4);
            fileBytes = Main.b_ReplaceBytes(fileBytes, BitConverter.GetBytes(pz), itemIndices[selected] - 0xBC + 8);

            pictures[selected].Location = PosToPoint((int)px, (int)pz);

            SetGraphicsToRed();
            pictures[selected].BackColor = Color.Blue;

            // Select any items with the same position
            for (int x = 0; x < itemIndices.Count; x++)
            {
                float px2 = Main.b_ReadFloat(fileBytes, itemIndices[x] - 0xBC);
                float py2 = Main.b_ReadFloat(fileBytes, itemIndices[x] - 0xBC + 4);
                float pz2 = Main.b_ReadFloat(fileBytes, itemIndices[x] - 0xBC + 8);

                if (px2 == px && py2 == py && pz2 == pz)
                {
                    pictures[x].BackColor = Color.Blue;
                }
            }

            string it = GetName(itemid, wepid, quant);
            listBox1.Items[selected] = it;
        }

        public byte[] SaveMetaData()
        {
            byte[] bytes = new byte[0];
            bytes = Main.b_AddBytes(bytes, fileBytes);

            bytes = Main.b_AddString(bytes, "ITEMEDITORDATA");

            // Save checkbox
            if (enableMetadataSavingToolStripMenuItem.Checked) bytes = Main.b_AddInt(bytes, 1);
            else Main.b_AddInt(bytes, 0);

            // Save map position data
            bytes = Main.b_AddFloat(bytes, minx);
            bytes = Main.b_AddFloat(bytes, minz);
            bytes = Main.b_AddFloat(bytes, maxx);
            bytes = Main.b_AddFloat(bytes, maxz);
            bytes = Main.b_AddFloat(bytes, maxxper);
            bytes = Main.b_AddFloat(bytes, maxzper);

            // Save descriptions
            bytes = Main.b_AddInt(bytes, itemDesc.Count);
            for(int x = 0; x < itemDesc.Count; x++)
            {
                bytes = Main.b_AddInt(bytes, itemDesc[x].Length);
                bytes = Main.b_AddString(bytes, itemDesc[x]);
            }

            return bytes;
        }

        private void saveToolStripMenuItem_Click(object sender, EventArgs e)
        {
            bool savedata = enableMetadataSavingToolStripMenuItem.Checked;
            if (savedata) File.WriteAllBytes(filePath, SaveMetaData());
            else File.WriteAllBytes(filePath, fileBytes);
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SaveFileDialog s = new SaveFileDialog();
            s.ShowDialog();

            if(s.FileName != "")
            {
                filePath = s.FileName;

                bool savedata = enableMetadataSavingToolStripMenuItem.Checked;
                if (savedata) File.WriteAllBytes(filePath, SaveMetaData());
                else File.WriteAllBytes(filePath, fileBytes);
            }
        }

        public string[] itemList =
        {
            "00 Nothing",
            "01 First Aid Spray",
            "02 Green Herb",
            "03 Red Herb",
            "04 Blue Herb",
            "05 G+G Herb",
            "06 G+R Herb",
            "07 G+B Herb",
            "08 G+G+B? Herb",
            "09 3x G Herb",
            "0A G+R+B",
            "0B R+B",
            "0C G Herb",
            "0D R Herb",
            "0E B Herb",
            "0F Handgun Ammo",
            "10 Shotgun Shells",
            "11 Machinegun Ammo",
            "12 Magnum Ammo",
            "13 ???",
            "14 ???",
            "15 ???",
            "16 Acid Rounds",
            "17 Incendiary Rounds",
            "18 Needle Rounds",
            "19 Flamethrower Oil",
            "1A .45 Ammo",
            "1B SLS 60 Ammo",
            "1C ???",
            "1D ???",
            "1E ???",
            "1F C4 Detonator",
            "20 Ink Ribbon",
            "21 Wooden Board",
            "22 Electronic Gadget",
            "23 Battery",
            "24 Gunpowder",
            "25 Gunpowder Large",
            "26 Y Gunpowder",
            "27 W Gunpowder",
            "28 ???",
            "29 ???",
            "2A ???",
            "2B ???",
            "2C ???",
            "2D ???",
            "2E ???",
            "2F ???",
            "30 Extended Mag (Matilda)",
            "31 Muzzle Attachment (Matilda)",
            "32 Stock (Matilda)",
            "33 Speed Loader (SLS)",
            "34 Laser Sight (HP3)",
            "35 Reinforced Frame (SLS)",
            "36 Extended Mag (HP3)",
            "37 Wooden Stock (Shotgun)",
            "38 Pump (Shotgun)",
            "39 ???",
            "3A Extended Mag (MQ11)",
            "3B ???",
            "3C Silencer?",
            "3D 2x Scope",
            "3E Custom Part ???",
            "3F ???",
            "40 Stock (Grenade Launcher)",
            "41 Cable w/ Gas Valve?",
            "42 ???",
            "43 ???",
            "44 ???",
            "45 ???",
            "46 ???",
            "47 ???",
            "48 Film Roll",
            "49 Film Roll",
            "4A Flim Roll",
            "4B Film Roll",
            "4C Film Roll",
            "4D Gas Station Key",
            "4E ???",
            "4F Jack Handle (Bookcases)",
            "50 Red Crank",
            "51 Unicorn Medal",
            "52 Spade Key",
            "53 Police Garage Card",
            "54 Locker Card",
            "55 ???",
            "56 Valve (Gas Pipe)",
            "57 Stars Badge Key Item",
            "58 Gold Scepter",
            "59 ???",
            "5A Ruby",
            "5B Jewelry Box",
            "5C ??? Key item",
            "5D Bishop Plug",
            "5E Rook Plug",
            "5F King Plug",
            "60 ???",
            "61 ???",
            "62 Picture Block",
            "63 ???",
            "64 ???",
            "65 ???",
            "66 Stars Badge USB",
            "67 ???",
            "68 ???",
            "69 ???",
            "6A ???",
            "6B ???",
            "6C ???",
            "6D ???",
            "6E ???",
            "6F ???",
            "70 Keypad Button",
            "71 ???",
            "72 Red Leather Book",
            "73 Statue hand",
            "74 Statue hand w/ book",
            "75 ???",
            "76 Lion Medallion",
            "77 Diamond Key",
            "78 Police Car Key",
            "79 ???",
            "7A ??? Key item",
            "7B ???",
            "7C Maiden Medallion",
            "7D ?",
            "7E Electronic Part",
            "7F Electronic Part",
            "80 Lovers Relief Key Item",
            "81 Small Gear",
            "82 Big Gear",
            "83 Eagle Gate Key (Courtyard)",
            "84 Knight Plug",
            "85 Pawn Plug",
            "86 Queen Plug",
            "87 Boxed Electronic Part",
            "88 Boxed Electronic Part",
            "89 ???",
            "8A ???",
            "8B ???",
            "8C ???",
            "8D ???",
            "8E ???",
            "8F ???",
            "90 ???",
            "91 ???",
            "92 ???",
            "93 ???",
            "94 ???",
            "95 ???",
            "96 ???",
            "97 ???",
            "98 ???",
            "99 ???",
            "9A ???",
            "9B ???",
            "9C ???",
            "9D ???",
            "9E ???",
            "9F Orphanage Key",
            "A0 Club Key",
            "A1 ???",
            "A2 ???",
            "A3 ???",
            "A4 ???",
            "A5 ???",
            "A6 ???",
            "A7 ???",
            "A8 ???",
            "A9 Heart Key",
            "AA VHS tape (USS)",
            "AB ???",
            "AC ???",
            "AD ???",
            "AE ???",
            "AF ???",
            "B0 T-Handle Tool",
            "B1 ???",
            "B2 ???",
            "B3 Cartridge (Greenhouse, Empty)",
            "B4 Herbicide (Uncooled)",
            "B5 Herbicide (Cooled)",
            "B6 ???",
            "B7 Joint Plug",
            "B8 ???",
            "B9 ???",
            "BA Admin Wristband Chip",
            "BB Admin Bracelet (Lab)",
            "BC General Staff Bracelet",
            "BD Frequency Device",
            "BE Lab Trophy",
            "BF Lab Trophy",
            "C0 -",
            "C1 -",
            "C2 Sewer Key",
            "C3 Visitor Bracelet (Lab)",
            "C4 General Staff Bracelet (Lab)",
            "C5 Bracelet Senior Staff",
            "C6 General Staff Chip",
            "C7 Senior Staff Chip (Lab)",
            "C8 Bracelet w/ Green Label",
            "C9 Blue Bracelet General Staff",
            "CA Bracelet Senior Staff",
            "CB VHS Tape (Lab)",
            "CC ???",
            "CD ???",
            "CE ???",
            "CF ???",
            "D0 ???",
            "D1 ???",
            "D2 ???",
            "D3 ???",
            "D4 ???",
            "D5 ???",
            "D6 ???",
            "D7 ???",
            "D8 ???",
            "D9 ???",
            "DA ???",
            "DB ???",
            "DC ???",
            "DD ???",
            "DE ???",
            "DF ???",
            "E0 ???",
            "E1 ???",
            "E2 ???",
            "E3 ???",
            "E4 ???",
            "E5 ???",
            "E6 ???",
            "E7 ???",
            "E8 ???",
            "E9 ???",
            "EA ???",
            "EB ???",
            "EC ???",
            "ED ???",
            "EE ???",
            "EF ???",
            "F0 Fuse",
            "F1 Fuse",
            "F2 ???",
            "F3 Pink Scissors",
            "F4 Bolt Cutter",
            "F5 Doll",
            "F6 ???",
            "F7 ???",
            "F8 ???",
            "F9 ???",
            "FA ???",
            "FB Police Station Map F1",
            "FC Police Station Map B1",
            "FD ???",
            "FE ???",
            "FF ???",
            "100 ???",
            "101 ???",
            "102 ???",
            "103 ???",
            "104 ???",
            "105 Police Station Upper Floor Map",
            "106 Hip Pouch",
            "107 ???",
            "108 ???",
            "109 ???",
            "10A ???",
            "10B ???",
            "10C ???",
            "10D ???",
            "10E ???",
            "10F ???",
            "110 ???",
            "111 ???",
            "112 ???",
            "113 ???",
            "114 ???",
            "115 ???",
            "116 ???",
            "117 ???",
            "118 ???",
            "119 ???",
            "11A ???",
            "11B ???",
            "11C ???",
            "11D ???",
            "11E ???",
            "11F ???",
            "120 ???",
            "121 ???",
            "122 ???",
            "123 Portable Safe",
            "124 ???",
            "125 Tin Storage Box",
            "126 Wood Crate?",
            "127 Wood Crate?",
            "128 ???"
        };

        public string[] wepList =
        {
            "00 ???",
            "01 VP70",
            "02 M1911",
            "03 JMP HP3",
            "04 SAA",
            "05 ???",
            "06 VP70 + Mag",
            "07 USP",
            "08 Mauser HSC",
            "09 Ladysmith",
            "0A JMP HP (Mag)",
            "0B Shotgun",
            "0C JMP HP (Mag, laser)",
            "0D SAA",
            "0E ???",
            "0F USP",
            "10 Ladysmith (Inf.)",
            "11 Ladysmith (Inf.)",
            "12 329",
            "13 329 (Inf.)",
            "14 Ladysmith (Inf.)",
            "15 MAC11",
            "16 Shotgun (Pump)",
            "17 MP5",
            "18 Shotgun (Pump, stock)",
            "19 MAC11 (Inf.)",
            "1A MAC11 (Silencer)",
            "1B MAC11 (Mag)",
            "1C MAC11 (Stock, mag, silencer)",
            "1D MP5 (Inf.)",
            "1E Deagle inf",
            "1F Deagle",
            "20 Deagle (Scope)",
            "21 Deagle (Scope, Barrel)",
            "22 Hack Tool",
            "23 M79",
            "24 M79 (Stock)",
            "25 Flamethrower",
            "26 Flamethrower (Scope)",
            "27 Sparkshot",
            "28 Sparkshot (Scope)",
            "29 Hack tool",
            "2A M79",
            "2B Flamethrower",
            "2C Sparkshot",
            "2D AT4",
            "2E Knife",
            "2F Knife (Inf.)",
            "30 Minigun",
            "31 M202",
            "32 Minigun",
            "33 ???",
            "34 Minigun",
            "35 ???",
            "36 ???",
            "37 ???",
            "38 ???",
            "39 Samurai Edge",
            "3A ???",
            "3B ???",
            "3C ???",
            "3D ???",
            "3E ???",
            "3F ???",
            "40 ???",
            "41 Frag Grenade",
            "42 Flash Grenade",
            "43 ???",
            "44 ???",
            "45 ???",
            "46 ???",
            "47 ???",
            "48 AT4 (Inf.)",
            "49 ???",
            "4A M202 (Inf.)",
            "4B Minigun",
            "4C ???"
        };

        public string[] re3itemList =
        {
            "0x01 - First Aid Spray",
            "0x02 - Green Herb",
            "0x03 - Red Herb",
            "0x05 - G + G",
            "0x06 - G + R",
            "0x09 - G + G + G",
            "0x0C - RE2 Green Herb",
            "0x0D - RE2 Red Herb",
            "0x13 - RE2 9mm Ammo",
            "0x14 - RE2 Shotgun Ammo",
            "0x15 - RE2 AR Ammo",
            "0x16 - Green Herb",
            "0x17 - Red Herb",
            "0x19 - Incendiary Grenades (No name)",
            "0x1A - Explosive Grenades (No name)	",
            "0x1B - Mines (No name)",
            "0x1F - 9mm Ammo",
            "0x20 - Shotgun Ammo",
            "0x21 - AR Ammo",
            "0x22 - Magnum Ammo",
            "0x23 - 9mm Ammo (No name)",
            "0x24 - Mines",
            "0x25 - Explosive Grenades",
            "0x26 - Acid Grenades",
            "0x27 - Incendiary Grenades",
            "0x28 - Explosive Grenades (No name)",
            "0x2A - Gunpowder (No name)",
            "0x2B - Special Gunpowder (No name)",
            "0x2C - Explosive A (No name)",
            "0x2D - Explosive B (No name)",
            "0x3D - Gunpowder",
            "0x3E - Quality Gunpowder (No name)",
            "0x3F - Explosive A",
            "0x40 - Explosive B",
            "0x4C - G19 Silencer",
            "0x4D - G19 Mira",
            "0x4E - G19 Extended Mag",
            "0x52 - RE2 Battery",
            "0x53 - Strange Key (No name)",
            "0x55 - Brad STARS ID (No name)",
            "0x56 - Explosive (No name)",
            "0x57 - Explosive (Turned on)",
            "0x5B - M3 Semiautomatic Accessory",
            "0x5C - M3 Culata",
            "0x5D - M3 Extended Mag",
            "0x5F - Small Box ??? (No name)",
            "0x60 - AR Scope",
            "0x61 - AR Extended Mag",
            "0x62 - AR Empuñadura",
            "0x63 - Red Crystal",
            "0x64 - Dr. Bard's ID? (No name)",
            "0x65 - Magnum Extended Mag",
            "0x66 - Cassette (No name)",
            "0x67 - MP3 Player (No name)",
            "0x68 - Vaccine (No name)",
            "0x6B - Key with red chain (No name)",
            "0x6E - Umbrella USB (No name)",
            "0x6F - Tube (Full)",
            "0x70 - Tube (Empty)",
            "0x71 - Tube (Also empty)",
            "0x72 - Tube (Also also empty)",
            "0x77 - Hip Pouch (No name)",
            "0x7F - Defense Coin (No name)",
            "0x80 - Assault Coin (No name)",
            "0x81 - Restoration Coin (No name)",
            "0x82 - Machine ???",
            "0x83 - Cassette",
            "0x89 - Nemesis Ammo Box A (No name)",
            "0x8A - Nemesis Ammo Box B (No name)",
            "0x8B - Nemesis Ammo Box C (No name)",
            "0x8C - Nemesis Ammo Box B (No name)",
            "0x8D - Nemesis Ammo Box C (No name)",
            "0x91 - Yellow Star (No name)",
            "0x93 - Ganzua (No name)",
            "0x94 - Bolt Cutter (No name)",
            "0x95 - Cassette (No name)",
            "0x97 - Ganzua",
            "0x98 - Bolt Cutter",
            "0x9D - Fuse (No name)",
            "0x9E - Fuse (No name)",
            "0x9F - Fuse (No name)",
            "0xA1 - Battery",
            "0xA2 - Strange Key",
            "0xA4 - Brad ID Card",
            "0xA5 - Electronic Device (Bomb)",
            "0xA6 - Bomb",
            "0xA8 - AR Estabilizador (No name)",
            "0xAB - AR Extended Mag (No name)",
            "0xAC - AR Culata",
            "0xAE - Jewel Box (No name)",
            "0xAF - Jewel Box (No name)",
            "0xB0 - Jewel Box (No name)",
            "0xB5 - Fire Hose",
            "0xB6 - Kendo's Key",
            "0xB9 - Estuche",
            "0xBA - Battery (Two spaces)",
            "0xBB - Green Jewel",
            "0xBC - Blue Jewel",
            "0xBD - Red Jewel",
            "0xC0 - Jewel Box",
            "0xC1 - Jewel Box",
            "0xC2 - Jewel Box",
            "0xC3 - Yellow Star (No name, same as 0x91)",
            "0xD3 - Bard's ID",
            "0xD4 - Recorder",
            "0xD5 - Cassette",
            "0xD6 - Recorder",
            "0xD7 - Vaccine Sample",
            "0xD9 - Bomb",
            "0xDA - Key with red chain",
            "0xDE - Fuse",
            "0xDF - Fuse",
            "0xD0 - Fuse",
            "0xE8 - Umbrella USB",
            "0xE9 - Vaccine",
            "0xEA - Cultive Sample",
            "0xEB - Tube (Full)",
            "0xEC - Base Vaccine",
            "0xF0 - Yellow Star (No name, same as 0x91 and 0xC39)",
            "0xF1 - Hip Pouch (No name)",
            "0x0105 - Hip Pouch",
            "0x0108 - Fire Hose",
            "0x0110 - Umbrella USB (No name)",
            "0x0111 - Vaccine Sample (No name)",
            "0x012D - Defense Coin (No name)",
            "0x012E - Assault Coin (No name)",
            "0x012F - Restoration Coin (No name)",
            "0x0130 - Ammo Tool",
            "0x0131 - STARS Combat Manual",
            "0x0137 - Nemesis Ammo Box",
            "0x0138 - Nemesis Ammo Box",
            "0x0139 - Nemesis Ammo Box",
            "0x013A - Nemesis Ammo Box",
            "0x013B - Nemesis Ammo Box",
            "0x013C - Nemesis Ammo Box"
        };

        private void comboBox1_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fileOpen == false || listBox1.SelectedIndex == -1 || comboBox1.SelectedIndex == -1) return;

            comboBox2.SelectedIndex = -1;
        }

        private void comboBox2_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (fileOpen == false || listBox1.SelectedIndex == -1 || comboBox2.SelectedIndex == -1) return;

            comboBox1.SelectedIndex = -1;
        }

        void SetBG(int n)
        {
            switch(n)
            {
                case 0:
                    this.BackgroundImage = Properties.Resources.rpd1;
                    minx = -36.46678f;
                    minz = -33.8545f;
                    maxx = 42.69022f;
                    maxz = 20.39007f;
                    maxxper = 79.157f;
                    maxzper = 54.24457f;
                    break;
                case 1:
                    this.BackgroundImage = Properties.Resources.rpd2;
                    minx = -36.46678f;
                    minz = -33.8545f;
                    maxx = 42.69022f;
                    maxz = 20.39007f;
                    maxxper = 79.157f;
                    maxzper = 54.24457f;
                    break;
                case 2:
                    this.BackgroundImage = Properties.Resources.rpd3;
                    minx = -36.46678f;
                    minz = -33.8545f;
                    maxx = 42.69022f;
                    maxz = 20.39007f;
                    maxxper = 79.157f;
                    maxzper = 54.24457f;
                    break;
            }

            for(int x = 0; x < itemIndices.Count; x++)
            {
                float px = Main.b_ReadFloat(fileBytes, itemIndices[x] - 0xBC);
                float py = Main.b_ReadFloat(fileBytes, itemIndices[x] - 0xBC + 4);
                float pz = Main.b_ReadFloat(fileBytes, itemIndices[x] - 0xBC + 8);

                pictures[x].Location = PosToPoint((int)px, (int)pz);
            }
        }

        private void rE2RPD1FToolStripMenuItem_Click(object sender, EventArgs e) { SetBG(0); }
        private void rE2RPD2FToolStripMenuItem_Click(object sender, EventArgs e) { SetBG(1); }
        private void rE2RPD3FToolStripMenuItem_Click(object sender, EventArgs e) { SetBG(2); }
        private void rE2RPDB1ToolStripMenuItem_Click(object sender, EventArgs e) { SetBG(3); }

        public void DisplayOnly(float min, float max)
        {
            for(int x = 0; x < itemIndices.Count; x++)
            {
                float ypos = Main.b_ReadFloat(fileBytes, itemIndices[x] - 0xBC + 4);
                if (ypos >= min && ypos <= max) pictures[x].Visible = true;
                else pictures[x].Visible = false;
            }
        }

        private void displaySettingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Tool_ItemEditor_Settings t = new Tool_ItemEditor_Settings(this);
            t.ShowDialog();
        }

        void CloseFile()
        {
            this.Text = "Item List Editor";

            fileOpen = false;
            filePath = "";
            fileBytes = new byte[0];
            itemIndices.Clear();
            itemDesc.Clear();

            for(int x = 0; x < pictures.Count; x++)
            {
                this.Controls.Remove(pictures[x]);
            }
            pictures = new List<PictureBox>();

            listBox1.Items.Clear();
            comboBox1.SelectedIndex = -1;
            comboBox2.SelectedIndex = -1;
            numericUpDown1.Value = 0;
            numericUpDown2.Value = 0;
            numericUpDown3.Value = 0;
            numericUpDown4.Value = 0;
            textBox1.Text = "";

            minx = 999999;
            minz = 999999;
            maxx = -999999;
            maxz = -999999;
            maxxper = 0;
            maxzper = 0;

            SetBG(0);
        }

        private void closeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if(fileOpen)
            {
                DialogResult d = MessageBox.Show("Do you want to close this file?", "Are you sure?", MessageBoxButtons.OKCancel);
                if(d == DialogResult.OK) CloseFile();
            }
        }

        private void customToolStripMenuItem_Click(object sender, EventArgs e)
        {
            OpenFileDialog o = new OpenFileDialog();
            o.ShowDialog();

            if(o.FileName != "" && File.Exists(o.FileName))
            {
                BackgroundImage = Image.FromFile(o.FileName);
            }
        }

        public void UseItems(int n)
        {
            listBox1.SelectedIndex = -1;
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();

            actualItems.Clear();
            actualWeapons.Clear();

            List<string> items = new List<string>();
            List<string> weapons = new List<string>();

            if (n == 0)
            {
                // Fill items
                for (int x = 0; x < itemList.Length; x++)
                {
                    comboBox1.Items.Add(itemList[x]);
                    items.Add(itemList[x]);
                }

                for (int x = itemList.Length; x < 512; x++)
                {
                    comboBox1.Items.Add(x.ToString("X2") + " ???");
                    items.Add(x.ToString("X2") + " ???");
                }

                // Fill weapons
                for (int x = 0; x < wepList.Length; x++)
                {
                    comboBox2.Items.Add(wepList[x]);
                    weapons.Add(wepList[x]);
                }

                for (int x = wepList.Length; x < 512; x++)
                {
                    comboBox2.Items.Add(x.ToString("X2") + " ???");
                    weapons.Add(x.ToString("X2") + " ???");
                }

                actualItems = items;
                actualWeapons = weapons;
            }
            else
            {
                int max = 0;

                items.Add("00 Nothing");
                comboBox1.Items.Add("00 - Nothing");

                // Fill items
                for (int x = 0; x < re3itemList.Length; x++)
                {
                    string actual = re3itemList[x];
                    string[] div = actual.Split(' ');
                    int parsed = int.Parse(div[0].Substring(2, div[0].Length - 2), System.Globalization.NumberStyles.HexNumber);
                    if (parsed > max) max = parsed;

                    while(items.Count - 1 < max)
                    {
                        string str = items.Count.ToString("X2") + " - ???";
                        items.Add(str);
                        comboBox1.Items.Add(str);
                    }

                    string str2 = parsed.ToString("X2") + " " + actual.Substring(div[0].Length + 1, actual.Length - (div[0].Length + 1));
                    items[parsed] = str2;
                    comboBox1.Items[parsed] = str2;
                }

                for (int x = itemList.Length; x < 512; x++)
                {
                    comboBox1.Items.Add(x.ToString("X2") + " ???");
                    items.Add(x.ToString("X2") + " ???");
                }

                // Fill weapons
                for (int x = wepList.Length; x < 512; x++)
                {
                    comboBox2.Items.Add(x.ToString("X2") + " ???");
                    weapons.Add(x.ToString("X2") + " ???");
                }

                actualItems = items;
                actualWeapons = weapons;
            }

            comboBox3.Items.Clear();
            for (int x = 0; x < comboBox1.Items.Count; x++) comboBox3.Items.Add(comboBox1.Items[x]);

            if (fileOpen)
            {
                for (int x = 0; x < itemIndices.Count; x++)
                {
                    int itemid = Main.b_ReadInt(fileBytes, itemIndices[x] - 0x80);
                    int wepid = Main.b_ReadInt(fileBytes, itemIndices[x] - 0x7C);
                    int quant = Main.b_ReadInt(fileBytes, itemIndices[x] - 0x74);

                    string it = GetName(itemid, wepid, quant);

                    listBox1.Items[x] = it;
                }
            }
        }

        private void rE2ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UseItems(0);
        }

        private void rE3ToolStripMenuItem_Click(object sender, EventArgs e)
        {
            UseItems(1);
        }

        private void importItemListFromtxtToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Select the item list file.");
            OpenFileDialog o = new OpenFileDialog();
            o.ShowDialog();
            if (o.FileName == "" || File.Exists(o.FileName) == false) return;

            MessageBox.Show("Select the weapon list file.");
            OpenFileDialog o1 = new OpenFileDialog();
            o1.ShowDialog();
            if (o1.FileName == "" || File.Exists(o1.FileName) == false) return;

            string[] newItems = File.ReadAllLines(o.FileName);
            string[] newWeapons = File.ReadAllLines(o1.FileName);

            actualItems = newItems.ToList();
            actualWeapons = newWeapons.ToList();

            // Refill lists

            listBox1.SelectedIndex = -1;
            comboBox1.Items.Clear();
            comboBox2.Items.Clear();

            // Fill items
            for (int x = 0; x < actualItems.Count; x++) comboBox1.Items.Add(actualItems[x]);
            for (int x = actualItems.Count; x < 512; x++) comboBox1.Items.Add(x.ToString("X2") + " ???");

            // Fill weapons
            for (int x = 0; x < actualWeapons.Count; x++) comboBox2.Items.Add(actualWeapons[x]);
            for (int x = actualWeapons.Count; x < 512; x++) comboBox2.Items.Add(x.ToString("X2") + " ???");

            // Fill ammo types
            comboBox3.Items.Clear();
            for (int x = 0; x < comboBox1.Items.Count; x++) comboBox3.Items.Add(comboBox1.Items[x]);

            if (fileOpen)
            {
                for (int x = 0; x < itemIndices.Count; x++)
                {
                    int itemid = Main.b_ReadInt(fileBytes, itemIndices[x] - 0x80);
                    int wepid = Main.b_ReadInt(fileBytes, itemIndices[x] - 0x7C);
                    int quant = Main.b_ReadInt(fileBytes, itemIndices[x] - 0x74);

                    string it = GetName(itemid, wepid, quant);

                    listBox1.Items[x] = it;
                }
            }
        }

        private void exportItemListToolStripMenuItem_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Select the location where you want to save the current item list.");
            SaveFileDialog s = new SaveFileDialog();
            s.ShowDialog();
            if (s.FileName == null) return;

            MessageBox.Show("Select the location where you want to save the current weapon list.");
            SaveFileDialog s1 = new SaveFileDialog();
            s1.ShowDialog();
            if (s.FileName == null) return;

            File.WriteAllLines(s.FileName, actualItems.ToArray());
            File.WriteAllLines(s1.FileName, actualWeapons.ToArray());
        }
    }
}
