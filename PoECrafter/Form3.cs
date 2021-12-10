using System;
using Gma.System.MouseKeyHook;
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
        private IKeyboardMouseEvents m_GlobalHook;
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

        private void label2_Click(object sender, EventArgs e)
        {
            m_GlobalHook = Hook.GlobalEvents();

            var dic = new Dictionary<Combination, Action>();
            dic.Add(Combination.TriggeredBy(Keys.Escape), () =>
            {
                unhook();
            }
            );
            m_GlobalHook.MouseMove += Form3_MouseMove;
            m_GlobalHook.OnCombination(dic);
        }

        private void unhook()
        {
            m_GlobalHook.MouseMove -= Form3_MouseMove;
            m_GlobalHook.MouseMove -= Form1_MouseMove;
            m_GlobalHook.MouseMove -= Form2_MouseMove;
            m_GlobalHook.Dispose();
        }

        private void Form3_MouseMove(object sender, MouseEventArgs e)
        {
            CraftMatX.Text = e.X.ToString();
            CraftMatY.Text = e.Y.ToString();
        }
        private void Form2_MouseMove(object sender, MouseEventArgs e)
        {
            ChromaticX.Text = e.X.ToString();
            ChromaticY.Text = e.Y.ToString();
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            FusingX.Text = e.X.ToString();
            FusingY.Text = e.Y.ToString();
        }

        private void label5_Click(object sender, EventArgs e)
        {
            m_GlobalHook = Hook.GlobalEvents();

            var dic = new Dictionary<Combination, Action>();
            dic.Add(Combination.TriggeredBy(Keys.Escape), () =>
            {
                unhook();
            }
            );
            m_GlobalHook.MouseMove += Form2_MouseMove;
            m_GlobalHook.OnCombination(dic);
        }

        private void label11_Click(object sender, EventArgs e)
        {
            m_GlobalHook = Hook.GlobalEvents();

            var dic = new Dictionary<Combination, Action>();
            dic.Add(Combination.TriggeredBy(Keys.Escape), () =>
            {
                unhook();
            }
            );
            m_GlobalHook.MouseMove += Form1_MouseMove;
            m_GlobalHook.OnCombination(dic);
        }
    }
}
