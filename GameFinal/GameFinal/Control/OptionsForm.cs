using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;
using Microsoft.Xna.Framework.Graphics;
using System.IO;

namespace GameFinal
{
    public partial class OptionsForm : Form
    {
        public bool isServer { get; set; }
        public string IP_Adress { get; set; }
        public int mapNumber { get; set; }
        public int preffered_Width { get; set; }
        public int preffered_Height { get; set; }
        public bool is_Full_Screen { get; set; }
        public int skinNumber { get; set; }
        public string playerName { get; set; }
        private List<Texture2D> tankSkins;
        private List<string> skinNames;
        string[] iPStrings;


        public OptionsForm(string defaultIP, List<Texture2D> tankSkins, List<string> skinNames)
        {
            this.tankSkins = tankSkins;
            this.skinNames = skinNames;
            string divider = ".";
            iPStrings = defaultIP.Split(divider.ToCharArray(), StringSplitOptions.None);
            isServer = true;
            //resolutionsComboBox.SelectedIndex = 3;
            InitializeComponent();
        }

        private void clientRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (clientRadioButton.Checked)
            {
                iPGroupBox.Enabled = true;
                isServer = false;
                serverSettingsGroup.Enabled = false;
            }
        }

        private void serverRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            if (serverRadioButton.Checked)
            {
                iPGroupBox.Enabled = false;
                isServer = true;
                serverSettingsGroup.Enabled = true;
            }
        }

        private void OptionsForm_KeyDown(object sender, KeyEventArgs e)
        {
            switch (e.KeyCode)
            {
                case Keys.Enter:
                    goButton_Click(this, e);
                    break;
            }
        }

        private void OptionsForm_Load(object sender, EventArgs e)
        {
            iPBox1.Text = iPStrings[0];
            iPBox2.Text = iPStrings[1];
            iPBox3.Text = iPStrings[2];
            iPBox4.Text = iPStrings[3];
            foreach (string s in skinNames)
            {
                skinComboBox.Items.Add(s);
            }
            Random r = new Random();
            skinComboBox.SelectedIndex = r.Next(0, skinNames.Count);
            skinPictureBox.Image = Texture2Image(tankSkins[skinComboBox.SelectedIndex]);
            resolutionsComboBox.SelectedIndex = 2;
        }

        private void goButton_Click(object sender, EventArgs e)
        {
            IP_Adress = iPBox1.Text + "." + iPBox2.Text + "." + iPBox3.Text + "." + iPBox4.Text;
            mapNumber = Convert.ToInt32(mapNumberBox.Text);
            skinNumber = skinComboBox.SelectedIndex;
            playerName = nameTextBox.Text;
            if (FullScreenCheckBox.Checked)
                is_Full_Screen = true;
            else
                is_Full_Screen = false;
            DialogResult = System.Windows.Forms.DialogResult.OK;
            try
            {
                switch (resolutionsComboBox.SelectedIndex)
                {
                    case 0:
                        preffered_Width = 640;
                        preffered_Height = 480;
                        break;
                    case 1:
                        preffered_Width = 720;
                        preffered_Height = 480;
                        break;
                    case 2:
                        preffered_Width = 720;
                        preffered_Height = 576;
                        break;
                    case 3:
                        preffered_Width = 800;
                        preffered_Height = 600;
                        break;
                    case 4:
                        preffered_Width = 1024;
                        preffered_Height = 768;
                        break;
                    case 5:
                        preffered_Width = 1152;
                        preffered_Height = 864;
                        break;
                    case 6:
                        preffered_Width = 1280;
                        preffered_Height = 720;
                        break;
                    case 7:
                        preffered_Width = 1280;
                        preffered_Height = 768;
                        break;
                    case 8:
                        preffered_Width = 1280;
                        preffered_Height = 800;
                        break;
                    case 9:
                        preffered_Width = 1366;
                        preffered_Height = 768;
                        break;
                    case 10:
                        preffered_Width = 1600;
                        preffered_Height = 900;
                        break;
                    case 11:
                        preffered_Width = 1920;
                        preffered_Height = 1080;
                        break;

                }
            }
            catch 
            {
                DialogResult = System.Windows.Forms.DialogResult.Abort;
            }
            this.Close();
        }
        /// 640, 480
        /// 720, 480
        /// 720, 576
        /// 800, 600
        /// 1024, 768
        /// 1152, 864
        /// 1280, 720
        /// 1280, 768
        /// 1280, 800
        /// 1366, 768
        /// 1600, 900
        /// 1920, 1080


        private void skinComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            skinPictureBox.Image = Texture2Image(tankSkins[skinComboBox.SelectedIndex]);
        }

        private Image Texture2Image(Texture2D texture)
        {
            if (texture == null)
            {
                return null;
            }

            if (texture.IsDisposed)
            {
                return null;
            }

            //Memory stream to store the bitmap data.
            MemoryStream ms = new MemoryStream();

            //Save the texture to the stream.
            texture.SaveAsPng(ms, texture.Width, texture.Height);

            //Seek the beginning of the stream.
            ms.Seek(0, SeekOrigin.Begin);

            //Create an image from a stream.
            System.Drawing.Image bmp2 = System.Drawing.Bitmap.FromStream(ms);

            //Close the stream, we nolonger need it.
            ms.Close();
            ms = null;
            return bmp2;
        }
    }
}
