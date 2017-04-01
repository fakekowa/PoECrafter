using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsFormsApplication3;

namespace WindowsFormsApplication3
{
    public partial class Form3 : Form
    {
        public Form3()
        {
            InitializeComponent();
            CraftMatX.Text = Properties.Settings.Default.CraftItemX.ToString();
            CraftMatY.Text = Properties.Settings.Default.CraftItemY.ToString();
            FusingX.Text = Properties.Settings.Default.FusingX.ToString();
            FusingY.Text = Properties.Settings.Default.FusingY.ToString();
            ChromaticX.Text = Properties.Settings.Default.ChromaticX.ToString();
            ChromaticY.Text = Properties.Settings.Default.ChromaticY.ToString();
            JewelersX.Text = Properties.Settings.Default.JewellerX.ToString();
            JewelersY.Text = Properties.Settings.Default.JewellerY.ToString();
        }

        private void Form3_Load(object sender, EventArgs e)
        {
        }

        private void CraftMatX_TextChanged(object sender, EventArgs e)
        {
            int Location = int.Parse(CraftMatX.Text);
            Properties.Settings.Default.CraftItemX = Location;
            Properties.Settings.Default.Save();
            ProgressBar.UpdateLocations();
        }

        private void CraftMatY_TextChanged(object sender, EventArgs e)
        {
            int Location = int.Parse(CraftMatY.Text);
            Properties.Settings.Default.CraftItemY = Location;
            Properties.Settings.Default.Save();
            ProgressBar.UpdateLocations();
        }

        private void ChromaticX_TextChanged(object sender, EventArgs e)
        {
            int Location = int.Parse(ChromaticX.Text);
            Properties.Settings.Default.ChromaticX = Location;
            Properties.Settings.Default.Save();
            ProgressBar.UpdateLocations();
        }

        private void ChromaticY_TextChanged(object sender, EventArgs e)
        {
            int Location = int.Parse(ChromaticY.Text);
            Properties.Settings.Default.ChromaticY = Location;
            Properties.Settings.Default.Save();
            ProgressBar.UpdateLocations();
        }

        private void JewelersX_TextChanged(object sender, EventArgs e)
        {
            int Location = int.Parse(JewelersX.Text);
            Properties.Settings.Default.JewellerX = Location;
            Properties.Settings.Default.Save();
            ProgressBar.UpdateLocations();
        }

        private void JewelersY_TextChanged(object sender, EventArgs e)
        {
            int Location = int.Parse(JewelersY.Text);
            Properties.Settings.Default.JewellerY = Location;
            Properties.Settings.Default.Save();
            ProgressBar.UpdateLocations();
        }

        private void FusingX_TextChanged(object sender, EventArgs e)
        {
            int Location = int.Parse(FusingX.Text);
            Properties.Settings.Default.FusingX = Location;
            Properties.Settings.Default.Save();
            ProgressBar.UpdateLocations();
        }

        private void FusingY_TextChanged(object sender, EventArgs e)
        {
            int Location = int.Parse(FusingY.Text);
            Properties.Settings.Default.FusingY = Location;
            Properties.Settings.Default.Save();
            ProgressBar.UpdateLocations();
        }
    }
}
