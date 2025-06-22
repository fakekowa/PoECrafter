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
    public partial class LocationForm : Form
    {
        private IKeyboardMouseEvents m_GlobalHook;
        public LocationForm()
        {
            InitializeComponent();
            CraftMatX.Text = Properties.Settings.Default.CraftItemX.ToString();
            CraftMatY.Text = Properties.Settings.Default.CraftItemY.ToString();
            AlterationX.Text = Properties.Settings.Default.AlterationX.ToString();
            AlterationY.Text = Properties.Settings.Default.AlterationY.ToString();
            ChaosX.Text = Properties.Settings.Default.ChaosX.ToString();
            ChaosY.Text = Properties.Settings.Default.ChaosY.ToString();
            AugmentationX.Text = Properties.Settings.Default.AugmentationX.ToString();
            AugmentationY.Text = Properties.Settings.Default.AugmentationY.ToString();
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

        private void ChaosX_TextChanged(object sender, EventArgs e)
        {
            int Location = int.Parse(ChaosX.Text);
            Properties.Settings.Default.ChaosX = Location;
            Properties.Settings.Default.Save();
            ProgressBar.UpdateLocations();
        }

        private void ChaosY_TextChanged(object sender, EventArgs e)
        {
            int Location = int.Parse(ChaosY.Text);
            Properties.Settings.Default.ChaosY = Location;
            Properties.Settings.Default.Save();
            ProgressBar.UpdateLocations();
        }

        private void AugmentationX_TextChanged(object sender, EventArgs e)
        {
            int Location = int.Parse(AugmentationX.Text);
            Properties.Settings.Default.AugmentationX = Location;
            Properties.Settings.Default.Save();
            ProgressBar.UpdateLocations();
        }

        private void AugmentationY_TextChanged(object sender, EventArgs e)
        {
            int Location = int.Parse(AugmentationY.Text);
            Properties.Settings.Default.AugmentationY = Location;
            Properties.Settings.Default.Save();
            ProgressBar.UpdateLocations();
        }

        private void AlterationX_TextChanged(object sender, EventArgs e)
        {
            int Location = int.Parse(AlterationX.Text);
            Properties.Settings.Default.AlterationX = Location;
            Properties.Settings.Default.Save();
            ProgressBar.UpdateLocations();
        }

        private void AlterationY_TextChanged(object sender, EventArgs e)
        {
            int Location = int.Parse(AlterationY.Text);
            Properties.Settings.Default.AlterationY = Location;
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
            ChaosX.Text = e.X.ToString();
            ChaosY.Text = e.Y.ToString();
        }
        private void Form1_MouseMove(object sender, MouseEventArgs e)
        {
            AlterationX.Text = e.X.ToString();
            AlterationY.Text = e.Y.ToString();
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

        private void btnSelectCraftMat_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            button.Text = "Move mouse to item location, then press ESC";
            button.BackColor = Color.Yellow;
            
            m_GlobalHook = Hook.GlobalEvents();
            var dic = new Dictionary<Combination, Action>();
            dic.Add(Combination.TriggeredBy(Keys.Escape), () =>
            {
                button.Text = "Select";
                button.BackColor = Color.White;
                unhook();
            });
            m_GlobalHook.MouseMove += Form3_MouseMove;
            m_GlobalHook.OnCombination(dic);
        }

        private void btnSelectChaos_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            button.Text = "Move mouse to Chaos orb, then press ESC";
            button.BackColor = Color.Yellow;
            
            m_GlobalHook = Hook.GlobalEvents();
            var dic = new Dictionary<Combination, Action>();
            dic.Add(Combination.TriggeredBy(Keys.Escape), () =>
            {
                button.Text = "Select";
                button.BackColor = Color.White;
                unhook();
            });
            m_GlobalHook.MouseMove += Form2_MouseMove;
            m_GlobalHook.OnCombination(dic);
        }

        private void btnSelectAugmentation_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            button.Text = "Move mouse to Augmentation orb, then press ESC";
            button.BackColor = Color.Yellow;
            
            m_GlobalHook = Hook.GlobalEvents();
            var dic = new Dictionary<Combination, Action>();
            dic.Add(Combination.TriggeredBy(Keys.Escape), () =>
            {
                button.Text = "Select";
                button.BackColor = Color.White;
                unhook();
            });
            m_GlobalHook.MouseMove += FormAugmentation_MouseMove;
            m_GlobalHook.OnCombination(dic);
        }

        private void btnSelectAlteration_Click(object sender, EventArgs e)
        {
            var button = (Button)sender;
            button.Text = "Move mouse to Alteration orb, then press ESC";
            button.BackColor = Color.Yellow;
            
            m_GlobalHook = Hook.GlobalEvents();
            var dic = new Dictionary<Combination, Action>();
            dic.Add(Combination.TriggeredBy(Keys.Escape), () =>
            {
                button.Text = "Select";
                button.BackColor = Color.White;
                unhook();
            });
            m_GlobalHook.MouseMove += Form1_MouseMove;
            m_GlobalHook.OnCombination(dic);
        }

        private void FormAugmentation_MouseMove(object sender, MouseEventArgs e)
        {
            AugmentationX.Text = e.X.ToString();
            AugmentationY.Text = e.Y.ToString();
        }
    }
}
