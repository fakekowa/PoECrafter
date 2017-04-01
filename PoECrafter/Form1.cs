using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.Windows.Forms;
using System.Diagnostics;

namespace WindowsFormsApplication3
{

    public partial class ProgressBar : Form
    {

        public ProgressBar()
        {
            InitializeComponent();
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        private static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr WindowHandle);
        public const int SW_RESTORE = 9;
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);

        // constants for the mouse_input() API function
        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;

        string Item;
        string Sockets;
        decimal SocketCount;
        string Colors;
        int LinkCount;
        decimal BlueSockets = 0;
        decimal GreenSockets = 0;
        decimal RedSockets = 0;
        decimal WhiteSockets = 0;

        // Locations

        public class CraftingLocation
        {
            public static int[] CraftMat = { 0, 0 };
            public static int[] Fusing = { 0, 0 };
            public static int[] Chromatic = { 0, 0 };
            public static int[] Jeweler = { 0, 0 };
        }

        public static void UpdateLocations()
        {
            // Locations
            CraftingLocation.CraftMat[0] = Properties.Settings.Default.CraftItemX;
            CraftingLocation.CraftMat[1] = Properties.Settings.Default.CraftItemY;
            CraftingLocation.Fusing[0] = Properties.Settings.Default.FusingX;
            CraftingLocation.Fusing[1] = Properties.Settings.Default.FusingY;
            CraftingLocation.Chromatic[0] = Properties.Settings.Default.ChromaticX;
            CraftingLocation.Chromatic[1] = Properties.Settings.Default.ChromaticY;
            CraftingLocation.Jeweler[0] = Properties.Settings.Default.JewellerX;
            CraftingLocation.Jeweler[1] = Properties.Settings.Default.JewellerY;
        }

        public static class VirtualKeyboard
        {
            [DllImport("user32.dll")]
            static extern uint keybd_event(byte bVk, byte bScan, int dwFlags, int dwExtraInfo);
            public static void KeyDown(Keys key)
            {
                keybd_event((byte)key, 0, 0, 0);
            }

            public static void KeyUp(Keys key)
            {
                keybd_event((byte)key, 0, 2, 0);
            }
        }

        public static void LeftClick()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
            mouse_event(MOUSEEVENTF_LEFTUP, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
        }

        public static void RightClick()
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
            mouse_event(MOUSEEVENTF_RIGHTUP, Control.MousePosition.X, Control.MousePosition.Y, 0, 0);
        }

        public void GetItem()
        {
            // Puts all lines into arrays
            string[] result = Regex.Split(Item, "\r\n|\r|\n");

            // Loops through all lines (arrays)
            // Getting Sockets
            for (int ctr = 0; ctr < result.Length; ctr++)
            {
                if (result[ctr].Contains("Sockets: "))
                {
                    Sockets = result[ctr].Replace("Sockets: ", "");
                    LinkCount = GetHighestLink(Sockets);
                    SocketCount = Sockets.Count(char.IsLetter);
                    Colors = Sockets.Replace("-", "").Replace(" ", "");
                    BlueSockets = Sockets.Count(x => x == 'B');
                    GreenSockets = Sockets.Count(x => x == 'G');
                    RedSockets = Sockets.Count(x => x == 'R');
                    WhiteSockets = Sockets.Count(x => x == 'W');
                }
            }
        }

        // Gets Highest Link
        public int GetHighestLink(string links)
        {
            int MaxLink;
            var StopLooking = false;

            if (!StopLooking && Regex.IsMatch(links, ".-.-.-.-.-."))
            {
                MaxLink = 6;
                StopLooking = true;

            }
            else if (!StopLooking && Regex.IsMatch(links, ".-.-.-.-."))
            {
                MaxLink = 5;
                StopLooking = true;

            }
            else if (!StopLooking && Regex.IsMatch(links, ".-.-.-."))
            {
                MaxLink = 4;
                StopLooking = true;

            }
            else if (!StopLooking && Regex.IsMatch(links, ".-.-."))
            {
                MaxLink = 3;
                StopLooking = true;

            }
            else if (!StopLooking && Regex.IsMatch(links, ".-."))
            {
                MaxLink = 2;
                StopLooking = true;
            }
            else
            {
                MaxLink = 1;
            }

            return MaxLink;
        }

        // Link Crafting
        private async void button1_Click(object sender, EventArgs e)
        {
            if (FocusPoE())
            {
                decimal Roll = 0;
                var CraftStarted = false;
                VirtualKeyboard.KeyDown(Keys.LShiftKey);
                await System.Threading.Tasks.Task.Delay(trackBar1.Value + 250);
                VirtualKeyboard.KeyUp(Keys.LShiftKey);

                SetCursorPos(CraftingLocation.CraftMat[0], CraftingLocation.CraftMat[1]);
                await System.Threading.Tasks.Task.Delay(trackBar1.Value + 250);
                SendKeys.Send("^(C)");
                await System.Threading.Tasks.Task.Delay(trackBar1.Value + 50);
                Item = Clipboard.GetText();
                GetItem();
                await System.Threading.Tasks.Task.Delay(trackBar1.Value + 50);
                while (LinkCount < WantLinks.Value && Roll < FusingsToUse.Value)
                {

                    // Do Reroll
                    if (!CraftStarted)
                    {
                        VirtualKeyboard.KeyDown(Keys.LShiftKey);
                        await System.Threading.Tasks.Task.Delay(trackBar1.Value + 250);
                        SetCursorPos(CraftingLocation.Fusing[0], CraftingLocation.Fusing[1]);
                        await System.Threading.Tasks.Task.Delay(trackBar1.Value + 250);
                        RightClick();
                        await System.Threading.Tasks.Task.Delay(trackBar1.Value + 250);
                        SetCursorPos(CraftingLocation.CraftMat[0], CraftingLocation.CraftMat[1]);
                        await System.Threading.Tasks.Task.Delay(trackBar1.Value + 250);

                    }
                    CraftStarted = true;
                    LeftClick();
                    await System.Threading.Tasks.Task.Delay(trackBar1.Value + 250);
                    SendKeys.Send("^(C)");
                    await System.Threading.Tasks.Task.Delay(trackBar1.Value + 50);
                    Item = Clipboard.GetText();
                    GetItem();

                    print(LinkCount + " Links", Color.Black);
                    print(Environment.NewLine, Color.Black);


                    Roll = Roll + 1;

                    ProgressBarUpDate(Roll, FusingsToUse.Value);
                }
                if (LinkCount >= WantLinks.Value)
                {
                    print("--------------------------------------", Color.Black);
                    print(Environment.NewLine, Color.Black);
                    print("CONGRATULATIONS!", Color.Green);
                    print(Environment.NewLine, Color.Black);
                }

                print("--------------------------------------", Color.Black);
                print(Environment.NewLine, Color.Black);
                print("Used: (" + Roll + "/" + FusingsToUse.Value + ")", Color.Black);
                print(Environment.NewLine, Color.Black);
                print("--------------------------------------", Color.Black);
                print(Environment.NewLine, Color.Black);
                print(Environment.NewLine, Color.Black);
                VirtualKeyboard.KeyUp(Keys.LShiftKey);
                progressBar1.Value = 100;
                Roll = 0;
            }
            else
            {
                print("PATH OF EXILE IS NOT RUNNING", Color.Red);
                print(Environment.NewLine, Color.Black);
            }

        }

        // Chromatic Crafting
        private async void StartColors_Click(object sender, EventArgs e)
        {
            if (FocusPoE())
            {
                decimal Roll = 0;
                var CraftStarted = false;

                decimal RedDesired = RedWant.Value;
                decimal GreenDesired = GreenWant.Value;
                decimal BlueDesired = BlueWant.Value;

                FocusPoE();
                VirtualKeyboard.KeyDown(Keys.LShiftKey);
                await System.Threading.Tasks.Task.Delay(trackBar1.Value + 250);
                VirtualKeyboard.KeyUp(Keys.LShiftKey);

                SetCursorPos(CraftingLocation.CraftMat[0], CraftingLocation.CraftMat[1]);
                await System.Threading.Tasks.Task.Delay(trackBar1.Value + 250);
                SendKeys.Send("^(C)");
                await System.Threading.Tasks.Task.Delay(trackBar1.Value + 50);
                Item = Clipboard.GetText();
                GetItem();
                await System.Threading.Tasks.Task.Delay(trackBar1.Value + 50);

                if (RedWant.Value == 0)
                    RedDesired = RedSockets;
                if (GreenWant.Value == 0)
                    GreenDesired = GreenSockets;
                if (BlueWant.Value == 0)
                    BlueDesired = BlueSockets;

                while ((RedSockets < RedDesired || GreenSockets < GreenDesired || BlueSockets < BlueDesired) && Roll < ChromaticsToUse.Value)
                {

                    // Do Reroll
                    if (!CraftStarted)
                    {
                        VirtualKeyboard.KeyDown(Keys.LShiftKey);
                        await System.Threading.Tasks.Task.Delay(trackBar1.Value + 250);
                        SetCursorPos(CraftingLocation.Chromatic[0], CraftingLocation.Chromatic[1]);
                        await System.Threading.Tasks.Task.Delay(trackBar1.Value + 250);
                        RightClick();
                        await System.Threading.Tasks.Task.Delay(trackBar1.Value + 250);
                        SetCursorPos(CraftingLocation.CraftMat[0], CraftingLocation.CraftMat[1]);
                        await System.Threading.Tasks.Task.Delay(trackBar1.Value + 250);
                    }
                    CraftStarted = true;
                    LeftClick();
                    await System.Threading.Tasks.Task.Delay(trackBar1.Value + 250);
                    SendKeys.Send("^(C)");
                    await System.Threading.Tasks.Task.Delay(trackBar1.Value + 50);
                    Item = Clipboard.GetText();
                    GetItem();
                    
                    if (RedWant.Value == 0)
                        RedDesired = RedSockets;
                    if (GreenWant.Value == 0)
                        GreenDesired = GreenSockets;
                    if (BlueWant.Value == 0)
                        BlueDesired = BlueSockets;

                    print(RedSockets, Color.Red);
                    print("          ", Color.Black);
                    print(GreenSockets, Color.Green);
                    print("          ", Color.Black);
                    print(BlueSockets, Color.Blue);
                    print("          ", Color.Black);
                    print(Environment.NewLine, Color.Black);


                    Roll = Roll + 1;

                    ProgressBarUpDate(Roll, ChromaticsToUse.Value);
                }
                if (RedSockets >= RedDesired && GreenSockets >= GreenDesired && BlueSockets >= BlueDesired)
                {
                    print("--------------------------------------", Color.Black);
                    print(Environment.NewLine, Color.Black);
                    print("CONGRATULATIONS!", Color.Green);
                    print(Environment.NewLine, Color.Black);
                }

                print("--------------------------------------", Color.Black);
                print(Environment.NewLine, Color.Black);
                print("Used: (" + Roll + "/" + ChromaticsToUse.Value + ")", Color.Black);
                print(Environment.NewLine, Color.Black);
                print("--------------------------------------", Color.Black);
                print(Environment.NewLine, Color.Black);
                print(Environment.NewLine, Color.Black);
                VirtualKeyboard.KeyUp(Keys.LShiftKey);
                progressBar1.Value = 100;
                Roll = 0;
            }
            else
            {
                print("PATH OF EXILE IS NOT RUNNING", Color.Red);
                print(Environment.NewLine, Color.Black);
            }
        }

        // Socket Crafting
        private async void SocketStart_Click(object sender, EventArgs e)
        {
            if (FocusPoE())
            {
                decimal Roll = 0;
                var CraftStarted = false;
                FocusPoE();
                VirtualKeyboard.KeyDown(Keys.LShiftKey);
                await System.Threading.Tasks.Task.Delay(trackBar1.Value + 250);
                VirtualKeyboard.KeyUp(Keys.LShiftKey);

                SetCursorPos(CraftingLocation.CraftMat[0], CraftingLocation.CraftMat[1]);
                await System.Threading.Tasks.Task.Delay(trackBar1.Value + 250);
                SendKeys.Send("^(C)");
                await System.Threading.Tasks.Task.Delay(trackBar1.Value + 50);
                Item = Clipboard.GetText();
                GetItem();
                await System.Threading.Tasks.Task.Delay(trackBar1.Value + 50);
                while (SocketCount < WantSockets.Value && Roll < JewelersToUse.Value)
                {

                    // Do Reroll
                    if (!CraftStarted)
                    {
                        VirtualKeyboard.KeyDown(Keys.LShiftKey);
                        await System.Threading.Tasks.Task.Delay(trackBar1.Value + 250);
                        SetCursorPos(CraftingLocation.Jeweler[0], CraftingLocation.Jeweler[1]);
                        await System.Threading.Tasks.Task.Delay(trackBar1.Value + 250);
                        RightClick();
                        await System.Threading.Tasks.Task.Delay(trackBar1.Value + 250);
                        SetCursorPos(CraftingLocation.CraftMat[0], CraftingLocation.CraftMat[1]);
                        await System.Threading.Tasks.Task.Delay(trackBar1.Value + 250);

                    }
                    CraftStarted = true;
                    LeftClick();
                    await System.Threading.Tasks.Task.Delay(trackBar1.Value + 250);
                    SendKeys.Send("^(C)");
                    await System.Threading.Tasks.Task.Delay(trackBar1.Value + 50);
                    Item = Clipboard.GetText();
                    GetItem();

                    print(SocketCount + " Sockets", Color.Black);
                    print(Environment.NewLine, Color.Black);


                    Roll = Roll + 1;

                    ProgressBarUpDate(Roll, JewelersToUse.Value);
                }
                if (SocketCount >= WantSockets.Value)
                {
                    print("--------------------------------------", Color.Black);
                    print(Environment.NewLine, Color.Black);
                    print("CONGRATULATIONS!", Color.Green);
                    print(Environment.NewLine, Color.Black);
                }

                print("--------------------------------------", Color.Black);
                print(Environment.NewLine, Color.Black);
                print("Used: (" + Roll + "/" + JewelersToUse.Value + ")", Color.Black);
                print(Environment.NewLine, Color.Black);
                print("--------------------------------------", Color.Black);
                print(Environment.NewLine, Color.Black);
                print(Environment.NewLine, Color.Black);
                VirtualKeyboard.KeyUp(Keys.LShiftKey);
                progressBar1.Value = 100;
                Roll = 0; 
            }
            else
            {
                print("PATH OF EXILE IS NOT RUNNING", Color.Red);
                print(Environment.NewLine, Color.Black);
            }

        }

        // Focus Path of Exile Window
        public static bool FocusPoE()
        {
            foreach (Process p in Process.GetProcesses("."))
            {
                try
                {
                    if (p.MainWindowTitle.Length > 0)
                    {

                        if (p.MainWindowTitle.Contains("Path of Exile"))
                        {
                            SetForegroundWindow(p.MainWindowHandle);
                            return true;
                        }
                    }
                }
                catch (Exception ex)
                {
                }
            }

            return false;
        }

        // Update Progress Bar
        public void ProgressBarUpDate(decimal percent, decimal totalpercent)
        {
            decimal ProgressPercent = (percent / totalpercent) * 100;
            progressBar1.Value = Convert.ToInt32(ProgressPercent);
        }

        // Add Text To Rich Text Box
        public void print(object text, Color color)
        {
            string textConvert = Convert.ToString(text);

            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.SelectionLength = 0;

            richTextBox1.SelectionColor = color;
            richTextBox1.AppendText(textConvert);
            richTextBox1.SelectionColor = richTextBox1.ForeColor;
            richTextBox1.ScrollToCaret();
        }

        // Save/Load Window Location
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            Properties.Settings.Default.WindowLocation = this.Location;
            Properties.Settings.Default.AddedDelay = trackBar1.Value;
            Properties.Settings.Default.Save();
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            this.Location = Properties.Settings.Default.WindowLocation;
            this.TopMost = Properties.Settings.Default.AlwaysOnTop;
            DelayNumber.Text = Properties.Settings.Default.AddedDelay.ToString();
            trackBar1.Value = Properties.Settings.Default.AddedDelay;
            alwaysOntopToolStripMenuItem.Checked = Properties.Settings.Default.AlwaysOnTop;
            UpdateLocations();
        }

        // Open instructions
        private void informationToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form f = new Form2();
            f.StartPosition = FormStartPosition.Manual;
            f.Left = this.Location.X;
            f.Top = this.Location.Y;
            f.Show(this);
        }

        private void settingsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form f = new Form3();
            f.StartPosition = FormStartPosition.Manual;
            f.Left = this.Location.X;
            f.Top = this.Location.Y;
            f.Show(this);
        }

        private void alwaysOntopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            alwaysOntopToolStripMenuItem.Checked = !alwaysOntopToolStripMenuItem.Checked;
            this.TopMost = !this.TopMost;
            Properties.Settings.Default.AlwaysOnTop = alwaysOntopToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
        }

        private void trackBar1_Scroll(object sender, EventArgs e)
        {
            DelayNumber.Text = trackBar1.Value.ToString();
        }
    }
}
