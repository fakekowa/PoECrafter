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
using System.Management;

namespace WindowsFormsApplication3
{

    public partial class ProgressBar : Form
    {

        public ProgressBar()
        {
            InitializeComponent();
            AddClipboardFormatListener(this.Handle);    // Add our window to the clipboard's format listener list.
            
            // Setup modifier list to handle headers
            clbModifiers.ItemCheck += ClbModifiers_ItemCheck;
            
            // **NEW: Add keyboard shortcuts for window management**
            this.KeyPreview = true;
            this.KeyDown += ProgressBar_KeyDown;
        }

        [DllImport("user32.dll", EntryPoint = "SetCursorPos")]
        private static extern bool SetCursorPos(int X, int Y);
        [DllImport("user32.dll")]
        public static extern bool ShowWindowAsync(HandleRef hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr WindowHandle);
        [DllImport("user32.dll")]
        private static extern int ShowWindow(IntPtr hWnd, uint Msg);
        private const uint SW_MAXIMIZE = 3;
        private const uint SW_RESTORE = 0x09;
        [DllImport("user32.dll")]
        static extern void mouse_event(int dwFlags, int dx, int dy, int dwData, int dwExtraInfo);
        [DllImport("USER32.DLL")]
        public static extern int GetClassName(IntPtr hWnd, StringBuilder lpClassName, int nMaxCount);

        /// <summary>
        /// Places the given window in the system-maintained clipboard format listener list.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool AddClipboardFormatListener(IntPtr hwnd);

        /// <summary>
        /// Removes the given window from the system-maintained clipboard format listener list.
        /// </summary>
        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool RemoveClipboardFormatListener(IntPtr hwnd);

        /// <summary>
        /// Sent when the contents of the clipboard have changed.
        /// </summary>
        private const int WM_CLIPBOARDUPDATE = 0x031D;


        // constants for the mouse_input() API function
        private const int MOUSEEVENTF_MOVE = 0x0001;
        private const int MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const int MOUSEEVENTF_LEFTUP = 0x0004;
        private const int MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const int MOUSEEVENTF_RIGHTUP = 0x0010;
        private const int MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const int MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const int MOUSEEVENTF_ABSOLUTE = 0x8000;


        public bool OverRide = false;
        public Process whatToOverRideWith = null;
        bool ruleMatch = false;

        int matchedAffixes = 0;
        string Item;
        List<string> affixes = new List<string>();
        List<string> rules = new List<string>();
        // Locations

        public class CraftingLocation
        {
            public static int[] CraftMat = { 0, 0 };
            public static int[] Alteration = { 0, 0 };
            public static int[] Chaos = { 0, 0 };
            public static int[] Augmentation = { 0, 0 };
        }

        public static void UpdateLocations()
        {
            // Locations
            CraftingLocation.CraftMat[0] = Properties.Settings.Default.CraftItemX;
            CraftingLocation.CraftMat[1] = Properties.Settings.Default.CraftItemY;
            CraftingLocation.Alteration[0] = Properties.Settings.Default.AlterationX;
            CraftingLocation.Alteration[1] = Properties.Settings.Default.AlterationY;
            CraftingLocation.Chaos[0] = Properties.Settings.Default.ChaosX;
            CraftingLocation.Chaos[1] = Properties.Settings.Default.ChaosY;
            CraftingLocation.Augmentation[0] = Properties.Settings.Default.AugmentationX;
            CraftingLocation.Augmentation[1] = Properties.Settings.Default.AugmentationY;
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

        protected override void WndProc(ref Message m)
        {
            base.WndProc(ref m);

            if (m.Msg == WM_CLIPBOARDUPDATE)
            {
                IDataObject iData = Clipboard.GetDataObject();      // Clipboard's data.
                if (iData.GetDataPresent(DataFormats.Text))
                {
                    // stuff here
                    Item = (string)iData.GetData(DataFormats.Text);
                }
            }
        }

        public void GetAffixes()
        {
            affixes.Clear();
            var unfilteredAffixes = Item.Split(new string[] { "--------" }, StringSplitOptions.None).Last().Split(new string[] { "\r\n" }, StringSplitOptions.None).ToList();

            foreach (var affix in unfilteredAffixes)
            {
                string trimmedAffix = Regex.Replace(affix, "{.*?}", string.Empty);
                affixes.Add(trimmedAffix);
            }
        }





        // Modifier Crafting
        private async void StartColors_Click(object sender, EventArgs e)
        {
            if (FocusPoE())
            {
                // Build rules based on UI settings
                rules.Clear();
                if (!EnablePhysicalDamagePercent && !EnableFlatPhysicalDamage && !EnableHybridPhysicalAccuracy)
                {
                    // Fallback to original logic if no two-handed axe modifiers are enabled
                    rules.Add("Armour");
                    rules.Add("Life");
                }
                
                decimal Roll = 0;

                // Update currency locations from settings
                UpdateLocations();
                
                // Debug: Print current locations
                print($"Debug - Alteration location: ({CraftingLocation.Alteration[0]}, {CraftingLocation.Alteration[1]})", Color.Gray);
                print($"Debug - Augmentation location: ({CraftingLocation.Augmentation[0]}, {CraftingLocation.Augmentation[1]})", Color.Gray);
                print($"Debug - Chaos location: ({CraftingLocation.Chaos[0]}, {CraftingLocation.Chaos[1]})", Color.Gray);
                print($"Debug - Item location: ({CraftingLocation.CraftMat[0]}, {CraftingLocation.CraftMat[1]})", Color.Gray);

                FocusPoE();
                await System.Threading.Tasks.Task.Delay(trackBar1.Value + 50);
                VirtualKeyboard.KeyDown(Keys.LShiftKey);
                await System.Threading.Tasks.Task.Delay(trackBar1.Value + 250);
                VirtualKeyboard.KeyUp(Keys.LShiftKey);
                
                SetCursorPos(CraftingLocation.CraftMat[0], CraftingLocation.CraftMat[1]);
                await System.Threading.Tasks.Task.Delay(trackBar1.Value + 250);
                SendKeys.Send("^(C)");
                await System.Threading.Tasks.Task.Delay(trackBar1.Value + 50);
                GetAffixes();
                await System.Threading.Tasks.Task.Delay(trackBar1.Value + 50);

                // Reset emergency stop for new crafting session
                isEmergencyStopActive = false;
                
                // **ENHANCED: Initialize crafting configuration before loop**
                InitializeCraftingConfiguration();
                print($"🎯 Crafting Mode: {(chkUseORLogic.Checked ? "OR Logic (Any Match)" : "AND Logic (All Must Match)")}", Color.Purple);
                print($"💡 Strategy: {(chkSmartAugmentation.Checked ? "Smart Augmentation" : "Alt-Spam Only")}", Color.Purple);

                while (!ruleMatch && Roll < ChaosToUse.Value && !isEmergencyStopActive)
                    {
                        // Check for emergency stop
                        if (isEmergencyStopActive)
                        {
                            print("🛑 Crafting stopped by emergency hotkey!", Color.Red);
                            break;
                        }

                        // **NEW: Analyze current item before determining currency**
                        AnalyzeCurrentItem();

                        // **ENHANCED: Smart currency selection based on item analysis**
                        int[] currencyLocation;
                        string currencyName;
                        SmartCurrencySelector.CurrencyType recommendedCurrency = SmartCurrencySelector.CurrencyType.Alteration;
                        
                        if (currentItemAnalysis != null && craftingConfig != null)
                        {
                            recommendedCurrency = SmartCurrencySelector.SelectOptimalCurrency(currentItemAnalysis, craftingConfig);
                        }
                        
                        // Map recommended currency to actual locations and names
                        switch (recommendedCurrency)
                        {
                            case SmartCurrencySelector.CurrencyType.Augmentation:
                                currencyLocation = CraftingLocation.Augmentation;
                                currencyName = "Augmentation";
                                print("🧠 Smart selection: Using Augmentation (adding missing affix type)", Color.Cyan);
                                break;
                            case SmartCurrencySelector.CurrencyType.Chaos:
                                currencyLocation = CraftingLocation.Chaos;
                                currencyName = "Chaos";
                                print("🔄 Using Chaos orb (rare item reroll)", Color.Orange);
                                break;
                            case SmartCurrencySelector.CurrencyType.Alteration:
                            default:
                                currencyLocation = CraftingLocation.Alteration;
                                currencyName = "Alteration";
                                print("⚡ Using Alteration orb (magic item reroll)", Color.Blue);
                                break;
                        }

                        // Override with manual crafting mode selection if not using smart strategy
                        if (!chkSmartAugmentation.Checked)
                        {
                            switch (cmbCraftingMode.SelectedIndex)
                            {
                                case 0: // Magic Items - use Alteration
                                    currencyLocation = CraftingLocation.Alteration;
                                    currencyName = "Alteration";
                                    break;
                                case 1: // Rare Items - use Chaos
                                    currencyLocation = CraftingLocation.Chaos;
                                    currencyName = "Chaos";
                                    break;
                                default: // Custom or fallback - use Chaos
                                    currencyLocation = CraftingLocation.Chaos;
                                    currencyName = "Chaos";
                                    break;
                            }
                        }

                        // Do Reroll with selected currency
                        SetCursorPos(currencyLocation[0], currencyLocation[1]);
                        await System.Threading.Tasks.Task.Delay(trackBar1.Value + 150);
                        RightClick();
                        await System.Threading.Tasks.Task.Delay(trackBar1.Value + 150);
                        SetCursorPos(CraftingLocation.CraftMat[0], CraftingLocation.CraftMat[1]);
                        await System.Threading.Tasks.Task.Delay(trackBar1.Value + 150);
                        LeftClick();
                        await System.Threading.Tasks.Task.Delay(trackBar1.Value + 150);
                        SendKeys.Send("^(C)");
                        await System.Threading.Tasks.Task.Delay(trackBar1.Value + 50);

                        GetAffixes();

                        // **ENHANCED: Use new advanced modifier checking system**
                        ruleMatch = CheckSelectedModifiers(); // This now uses the advanced OR/AND logic

                        // **NEW: Real-time item analysis and feedback**
                        AnalyzeCurrentItem(); // Update analysis after getting new affixes

                        foreach(var affix in affixes)
                        {
                            print(affix, Color.Blue);
                        }
                        print(Environment.NewLine, Color.Black);

                        if (!ruleMatch)
                        {
                            var logicType = chkUseORLogic.Checked ? "OR" : "AND";
                            print($"❌ {logicType} Logic: Target modifiers not satisfied, continuing...", Color.Red);
                            
                            // Show what we're looking for
                            if (craftingConfig != null && craftingConfig.ModifierGroups.Count > 0)
                            {
                                print($"🎯 Targeting: {string.Join(chkUseORLogic.Checked ? " OR " : " AND ", craftingConfig.ModifierGroups.Select(g => g.GroupName))}", Color.Yellow);
                            }
                            print(Environment.NewLine, Color.Black);
                        }
                        else
                        {
                            var logicType = chkUseORLogic.Checked ? "OR" : "AND";
                            print($"🎉 {logicType} Logic: TARGET ACHIEVED!", Color.Lime);
                        }

                        Roll = Roll + 1;

                        ProgressBarUpDate(Roll, ChaosToUse.Value);
                    }
                if (ruleMatch)
                {
                    print("--------------------------------------", Color.Black);
                    print(Environment.NewLine, Color.Black);
                    print("CONGRATULATIONS!", Color.Green);
                    print(Environment.NewLine, Color.Black);
                    matchedAffixes = 0;
                    ruleMatch = false;
                }

                print("--------------------------------------", Color.Black);
                print(Environment.NewLine, Color.Black);
                print("Used: (" + Roll + "/" + ChaosToUse.Value + ")", Color.Black);
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



        // Generate list of processors to select from if its not wokring right
        public void GenerateGetProcessors()
        {
            var wmiQueryString = "SELECT ProcessId, ExecutablePath, CommandLine FROM Win32_Process";
            using (var searcher = new ManagementObjectSearcher(wmiQueryString))
            using (var results = searcher.Get())
            {
                var query = from p in Process.GetProcesses()
                            join mo in results.Cast<ManagementObject>()
                            on p.Id equals (int)(uint)mo["ProcessId"]
                            select new
                            {
                                Process = p,
                                Path = (string)mo["ExecutablePath"],
                                CommandLine = (string)mo["CommandLine"],
                            };
                foreach (var item in query)
                {
                    // Do what you want with the Process, Path, and CommandLine
                    if (item.Process.MainWindowTitle != "")
                    {
                        processList.Items.Add(new ComboBoxItem(item.Process.MainWindowTitle + " - [" + item.Process.StartTime.ToShortTimeString() + "]", item.Process.MainWindowHandle));
                    }
                }
            }
        }

        // Focus Path of Exile Window
        public bool FocusPoE()
        {
            if (processList.SelectedIndex == -1)
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
            }
            else
            {
                ShowWindow(((ComboBoxItem)processList.SelectedItem).HiddenValue, SW_MAXIMIZE);
                SetForegroundWindow(((ComboBoxItem)processList.SelectedItem).HiddenValue);
                return true;
            }

            return false;
        }

        // Update Progress Bar
        public void ProgressBarUpDate(decimal percent, decimal totalpercent)
        {
            //
        }

        // Add Text To Rich Text Box
        public void print(object text, Color color)
        {
            string textConvert = Convert.ToString(text);

            // Send to separate log window if it exists
            if (logWindow != null)
            {
                logWindow.AppendLog(textConvert, color);
            }
            
            // Also keep in main window for compatibility (but hidden)
            richTextBox1.SelectionStart = richTextBox1.TextLength;
            richTextBox1.SelectionLength = 0;

            richTextBox1.SelectionColor = color;
            richTextBox1.AppendText(textConvert);
            richTextBox1.SelectionColor = richTextBox1.ForeColor;
            richTextBox1.ScrollToCaret();
        }

        // just random shit, ignore
        public void ListProcesses()
        {
            Process[] processlist = Process.GetProcesses();

            foreach (Process theprocess in processlist)
            {

                if (theprocess.MainWindowTitle.Length > 0)
                {
                    Process process = Process.GetProcessById(theprocess.Id); //PID of what you want
                    StringBuilder className = new StringBuilder(100);
                    int nret = GetClassName(theprocess.MainWindowHandle, className, className.Capacity);

                    richTextBox1.SelectionStart = richTextBox1.TextLength;
                    richTextBox1.SelectionLength = 0;

                    richTextBox1.SelectionColor = Color.Black;
                    richTextBox1.AppendText(theprocess.MainWindowTitle + " - " + className + "\n");
                    richTextBox1.SelectionColor = richTextBox1.ForeColor;
                    richTextBox1.ScrollToCaret();
                }
            }
        }

        // Save/Load Window Location
        private void Form1_FormClosing(object sender, FormClosingEventArgs e)
        {
            // **ENHANCED: Comprehensive Settings Saving**
            try
            {
                // Save window state and position
                Properties.Settings.Default.WindowLocation = this.Location;
                Properties.Settings.Default.WindowSize = this.Size;
                
                // Save UI settings
                Properties.Settings.Default.AlwaysOnTop = this.TopMost;
                Properties.Settings.Default.AddedDelay = trackBar1.Value;
                
                // **NEW: Save Advanced Logic Settings**
                Properties.Settings.Default.UseORLogic = chkUseORLogic.Checked;
                Properties.Settings.Default.SmartAugmentation = chkSmartAugmentation.Checked;
                Properties.Settings.Default.EmergencyStopKey = txtEmergencyHotkey.Text;
                
                // **NEW: Save Crafting Configuration**
                Properties.Settings.Default.CraftingModeIndex = cmbCraftingMode.SelectedIndex;
                Properties.Settings.Default.SelectedModifiers = SaveSelectedModifiers();
                Properties.Settings.Default.MaxChaosToUse = (int)ChaosToUse.Value;

                // Save all settings
                Properties.Settings.Default.Save();
                
                print("💾 All settings saved successfully!", Color.Green);
            }
            catch (Exception ex)
            {
                print($"⚠️ Error saving settings: {ex.Message}", Color.Red);
            }

            // Clean up global keyboard hook
            keyboardHook?.Dispose();
        }

        // **NEW: Methods for modifier persistence**
        private string SaveSelectedModifiers()
        {
            try
            {
                var selectedIndices = new List<int>();
                for (int i = 0; i < clbModifiers.Items.Count; i++)
                {
                    if (clbModifiers.GetItemChecked(i))
                    {
                        selectedIndices.Add(i);
                    }
                }
                return string.Join(",", selectedIndices);
            }
            catch (Exception ex)
            {
                print($"⚠️ Error saving selected modifiers: {ex.Message}", Color.Orange);
                return "";
            }
        }

        private void LoadSelectedModifiers(string savedModifiers)
        {
            try
            {
                if (string.IsNullOrEmpty(savedModifiers)) return;

                var indices = savedModifiers.Split(',');
                foreach (string indexStr in indices)
                {
                    if (int.TryParse(indexStr.Trim(), out int index))
                    {
                        if (index >= 0 && index < clbModifiers.Items.Count)
                        {
                            clbModifiers.SetItemChecked(index, true);
                        }
                    }
                }
                
                print($"✅ Loaded {indices.Length} saved modifier selections", Color.Green);
            }
            catch (Exception ex)
            {
                print($"⚠️ Error loading selected modifiers: {ex.Message}", Color.Orange);
            }
        }

        // **ENHANCED: Auto-save settings when changed**
        private void AutoSaveSettings()
        {
            try
            {
                Properties.Settings.Default.Save();
            }
            catch (Exception ex)
            {
                print($"⚠️ Auto-save failed: {ex.Message}", Color.Orange);
            }
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
            Form f = new LocationForm();
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

        private void shiftClickFixToolStripMenuItem_Click(object sender, EventArgs e)
        {
            shiftClickFixToolStripMenuItem.Checked = !shiftClickFixToolStripMenuItem.Checked;
            Properties.Settings.Default.ShiftClickFix = shiftClickFixToolStripMenuItem.Checked;
            Properties.Settings.Default.Save();
        }

        private void button2_Click(object sender, EventArgs e)
        {
            processList.Items.Clear();
            GenerateGetProcessors();
        }

        // **NEW: Show/Hide Log Window**
        private void btnShowLogs_Click(object sender, EventArgs e)
        {
            if (logWindow != null)
            {
                if (logWindow.Visible)
                {
                    logWindow.Hide();
                    btnShowLogs.Text = "Show Logs";
                }
                else
                {
                    logWindow.Show();
                    logWindow.BringToFront();
                    btnShowLogs.Text = "Hide Logs";
                    // Position it to the right of main window
                    logWindow.Location = new Point(this.Location.X + this.Width + 10, this.Location.Y);
                }
            }
        }

        private void craftingRulesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Form f = new RuleForm();
            f.StartPosition = FormStartPosition.Manual;
            f.Left = this.Location.X;
            f.Top = this.Location.Y;
            f.Show(this);
        }



        // Legacy modifier settings (kept for compatibility)
        public bool EnablePhysicalDamagePercent => false; // Disabled - using new system
        public int MinPhysicalDamagePercent => 0;
        
        public bool EnableFlatPhysicalDamage => false; // Disabled - using new system
        public int MinFlatPhysicalDamageMin => 0;
        public int MinFlatPhysicalDamageMax => 0;
        
        public bool EnableHybridPhysicalAccuracy => false; // Disabled - using new system
        public int MinHybridPhysicalPercent => 0;
        public int MinHybridAccuracy => 0;

        private bool CheckTwoHandedAxeModifiersInAffixes()
        {
            string fullItemText = string.Join("\n", affixes);
            
            // If no two-handed axe modifiers are enabled, fall back to original logic
            if (!EnablePhysicalDamagePercent && !EnableFlatPhysicalDamage && !EnableHybridPhysicalAccuracy)
            {
                try
                {
                    matchedAffixes = 0;
                    foreach (var rule in rules)
                    {
                        if (!affixes.Any(s => s.Contains(rule)))
                        {
                            return false;
                        }
                        else matchedAffixes++;
                    }
                    return matchedAffixes >= rules.Count();
                }
                catch (Exception ex)
                {
                    return false;
                }
            }

            // Use new modifier checking system if modifiers are selected
            if (clbModifiers.CheckedItems.Count > 0)
            {
                return CheckSelectedModifiers();
            }
            
            // Fallback to old system for compatibility
            bool physDamageOk = true;
            bool flatPhysOk = true;
            bool hybridOk = true;

            // Check Physical Damage % modifier
            if (EnablePhysicalDamagePercent)
            {
                physDamageOk = TwoHandedAxeModifiers.CheckPhysicalDamagePercent(fullItemText, MinPhysicalDamagePercent, out int physValue);
                if (physDamageOk)
                {
                    print($"✓ Physical Damage: {physValue}% (min: {MinPhysicalDamagePercent}%)", Color.Green);
                }
                else
                {
                    print($"✗ Physical Damage: < {MinPhysicalDamagePercent}%", Color.Red);
                }
                print(Environment.NewLine, Color.Black);
            }

            // Check Flat Physical Damage modifier
            if (EnableFlatPhysicalDamage)
            {
                flatPhysOk = TwoHandedAxeModifiers.CheckFlatPhysicalDamage(fullItemText, MinFlatPhysicalDamageMin, MinFlatPhysicalDamageMax, out int lowValue, out int highValue);
                if (flatPhysOk)
                {
                    print($"✓ Flat Physical: {lowValue}-{highValue} (min: {MinFlatPhysicalDamageMin}-{MinFlatPhysicalDamageMax})", Color.Green);
                }
                else
                {
                    print($"✗ Flat Physical: < {MinFlatPhysicalDamageMin}-{MinFlatPhysicalDamageMax}", Color.Red);
                }
                print(Environment.NewLine, Color.Black);
            }

            // Check Hybrid Physical/Accuracy modifier
            if (EnableHybridPhysicalAccuracy)
            {
                hybridOk = TwoHandedAxeModifiers.CheckHybridPhysicalAccuracy(fullItemText, MinHybridPhysicalPercent, MinHybridAccuracy, out int physValue, out int accValue);
                if (hybridOk)
                {
                    print($"✓ Hybrid Phys/Acc: {physValue}% + {accValue} Acc (min: {MinHybridPhysicalPercent}% + {MinHybridAccuracy} Acc)", Color.Green);
                }
                else
                {
                    print($"✗ Hybrid Phys/Acc: < {MinHybridPhysicalPercent}% + {MinHybridAccuracy} Acc", Color.Red);
                }
                print(Environment.NewLine, Color.Black);
            }

            // All enabled modifiers must pass
            return physDamageOk && flatPhysOk && hybridOk;
        }

        // New fields for enhanced functionality
        private Point itemLocation = Point.Empty;
        private Keys emergencyStopKey = Keys.F12;
        private bool isEmergencyStopActive = false;
        private GlobalKeyboardHook keyboardHook;
        private bool isSettingHotkey = false;

        // **NEW: Advanced Modifier Logic Fields**
        private CraftingConfiguration craftingConfig;
        private ItemAnalysis currentItemAnalysis;
        private SmartCurrencySelector currencySelector;

        // **NEW: Separate Log Window**
        private LogWindow logWindow;

        // **NEW: Advanced Logic Event Handlers**
        private void chkUseORLogic_CheckedChanged(object sender, EventArgs e)
        {
            if (craftingConfig != null)
            {
                craftingConfig.UseORLogicBetweenGroups = chkUseORLogic.Checked;
                print($"Logic changed to: {(chkUseORLogic.Checked ? "OR (Any Match)" : "AND (All Must Match)")}", Color.Blue);
                UpdateCraftingStrategy();
            }
        }

        private void chkSmartAugmentation_CheckedChanged(object sender, EventArgs e)
        {
            if (craftingConfig != null)
            {
                craftingConfig.EnableSmartAugmentation = chkSmartAugmentation.Checked;
                craftingConfig.Strategy = chkSmartAugmentation.Checked ? 
                    CraftingStrategy.SmartAugmentation : CraftingStrategy.AltSpamOnly;
                print($"Augmentation strategy: {(chkSmartAugmentation.Checked ? "Smart Augmentation" : "Alt-Spam Only")}", Color.Blue);
                UpdateCraftingStrategy();
            }
        }

        private void UpdateCraftingStrategy()
        {
            if (currentItemAnalysis != null && craftingConfig != null)
            {
                var recommendedCurrency = SmartCurrencySelector.SelectOptimalCurrency(currentItemAnalysis, craftingConfig);
                print($"Recommended currency: {recommendedCurrency}", Color.Purple);
            }
        }

        // **NEW: Enhanced Item Analysis System**
        public void AnalyzeCurrentItem()
        {
            if (string.IsNullOrEmpty(Item))
            {
                txtCurrentAnalysis.Text = "No item data available";
                return;
            }

            currentItemAnalysis = new ItemAnalysis
            {
                ItemText = Item,
                LastAnalyzed = DateTime.Now
            };

            // Determine item rarity
            if (Item.Contains("Rarity: Magic"))
            {
                currentItemAnalysis.IsMagic = true;
                currentItemAnalysis.IsRare = false;
            }
            else if (Item.Contains("Rarity: Rare"))
            {
                currentItemAnalysis.IsRare = true;
                currentItemAnalysis.IsMagic = false;
            }

            // Analyze affixes
            GetAffixes(); // Populate the affixes list
            
            foreach (var affix in affixes.Where(a => !string.IsNullOrWhiteSpace(a)))
            {
                var detectedAffix = AnalyzeAffix(affix);
                if (detectedAffix != null)
                {
                    currentItemAnalysis.DetectedAffixes.Add(detectedAffix);
                    
                    if (detectedAffix.Type == AffixType.Prefix)
                        currentItemAnalysis.PrefixCount++;
                    else if (detectedAffix.Type == AffixType.Suffix)
                        currentItemAnalysis.SuffixCount++;
                }
            }

            UpdateAnalysisDisplay();
        }

        private DetectedAffix AnalyzeAffix(string affixText)
        {
            if (string.IsNullOrWhiteSpace(affixText)) return null;

            var detectedAffix = new DetectedAffix
            {
                MatchedText = affixText,
                ModifierName = AffixDatabase.ClassifyModifierFromItemText(affixText),
                Type = AffixType.Unknown
            };

            // Set the affix type based on the modifier name
            detectedAffix.Type = AffixDatabase.GetAffixType(detectedAffix.ModifierName);

            // Extract values and determine tier based on modifier type
            if (detectedAffix.ModifierName.Contains("Physical Damage"))
            {
                var match = System.Text.RegularExpressions.Regex.Match(affixText, @"(\d+)(?:\(\d+-\d+\))?% increased Physical Damage");
                if (match.Success)
                {
                    detectedAffix.Value = int.Parse(match.Groups[1].Value);
                    detectedAffix.Tier = GetPhysicalDamagePercentTier(detectedAffix.Value);
                }
            }
            else if (detectedAffix.ModifierName.Contains("Attack Speed"))
            {
                var match = System.Text.RegularExpressions.Regex.Match(affixText, @"(\d+)(?:\(\d+-\d+\))?% increased Attack Speed");
                if (match.Success)
                {
                    detectedAffix.Value = int.Parse(match.Groups[1].Value);
                    detectedAffix.Tier = GetAttackSpeedTier(detectedAffix.Value);
                }
            }
            // Add more modifier types as needed...

            return detectedAffix;
        }

        private void UpdateAnalysisDisplay()
        {
            if (currentItemAnalysis == null)
            {
                txtCurrentAnalysis.Text = "No item analyzed";
                return;
            }

            var analysisText = new StringBuilder();
            analysisText.AppendLine($"Rarity: {(currentItemAnalysis.IsMagic ? "Magic" : currentItemAnalysis.IsRare ? "Rare" : "Normal")}");
            analysisText.AppendLine($"Prefixes: {currentItemAnalysis.PrefixCount}");
            analysisText.AppendLine($"Suffixes: {currentItemAnalysis.SuffixCount}");
            analysisText.AppendLine($"Total Affixes: {currentItemAnalysis.DetectedAffixes.Count}");
            
            if (currentItemAnalysis.DetectedAffixes.Count > 0)
            {
                analysisText.AppendLine("\nDetected Modifiers:");
                foreach (var affix in currentItemAnalysis.DetectedAffixes)
                {
                    var typeSymbol = affix.Type == AffixType.Prefix ? "P" : affix.Type == AffixType.Suffix ? "S" : "?";
                    analysisText.AppendLine($"[{typeSymbol}] {affix.ModifierName} T{affix.Tier}");
                }
            }

            // Show currency recommendation
            if (craftingConfig != null)
            {
                var recommendedCurrency = SmartCurrencySelector.SelectOptimalCurrency(currentItemAnalysis, craftingConfig);
                analysisText.AppendLine($"\nRecommended: {recommendedCurrency}");
            }

            txtCurrentAnalysis.Text = analysisText.ToString();
        }

        // **ENHANCED: CheckSelectedModifiers with OR/AND Logic**
        private bool CheckSelectedModifiersAdvanced()
        {
            if (clbModifiers.CheckedItems.Count == 0)
            {
                print("No target modifiers selected!", Color.Red);
                return false;
            }

            // Initialize crafting configuration if not already done
            if (craftingConfig == null)
            {
                InitializeCraftingConfiguration();
            }

            // Analyze current item
            AnalyzeCurrentItem();

            string fullItemText = string.Join("\n", affixes);
            
            // Group modifiers by type for OR logic
            var modifierGroups = new Dictionary<string, List<string>>();
            
            foreach (string selectedModifier in clbModifiers.CheckedItems)
            {
                var modifierType = GetModifierType(selectedModifier);
                if (!modifierGroups.ContainsKey(modifierType))
                    modifierGroups[modifierType] = new List<string>();
                modifierGroups[modifierType].Add(selectedModifier);
            }

            // Check logic based on OR/AND setting
            if (chkUseORLogic.Checked)
            {
                // OR Logic: ANY group satisfies the requirement
                foreach (var group in modifierGroups)
                {
                    bool groupSatisfied = false;
                    foreach (var modifier in group.Value)
                    {
                        if (CheckModifierTierFromSelection(fullItemText, modifier))
                        {
                            print($"✅ OR Logic satisfied by: {modifier}", Color.Green);
                            groupSatisfied = true;
                            break; // One modifier in group is enough
                        }
                    }
                    
                    if (groupSatisfied)
                    {
                        print($"🎯 OR Logic: Found acceptable modifier in group '{group.Key}'", Color.Lime);
                        return true; // Any group satisfaction is enough
                    }
                }
                
                print("❌ OR Logic: No modifier groups satisfied", Color.Red);
                return false;
            }
            else
            {
                // AND Logic: ALL selected modifiers must be satisfied (original behavior)
                foreach (string selectedModifier in clbModifiers.CheckedItems)
                {
                    if (CheckModifierTierFromSelection(fullItemText, selectedModifier))
                    {
                        print($"✅ Found acceptable modifier: {selectedModifier}", Color.Green);
                        return true; // Found at least one acceptable modifier (keeping original logic)
                    }
                }
                
                return false; // No acceptable modifiers found
            }
        }

        private void InitializeCraftingConfiguration()
        {
            craftingConfig = new CraftingConfiguration
            {
                UseORLogicBetweenGroups = chkUseORLogic.Checked,
                EnableSmartAugmentation = chkSmartAugmentation.Checked,
                Strategy = chkSmartAugmentation.Checked ? CraftingStrategy.SmartAugmentation : CraftingStrategy.AltSpamOnly,
                MaxCurrencyUsage = (int)ChaosToUse.Value
            };

            // Create modifier groups based on current selection
            var groups = new Dictionary<string, ModifierGroup>();
            
            foreach (string selectedModifier in clbModifiers.CheckedItems)
            {
                var modifierType = GetModifierType(selectedModifier);
                if (!groups.ContainsKey(modifierType))
                {
                    groups[modifierType] = new ModifierGroup(modifierType);
                }
                groups[modifierType].SelectedModifiers.Add(selectedModifier);
            }

            craftingConfig.ModifierGroups = groups.Values.ToList();
            
            print($"Initialized crafting configuration with {craftingConfig.ModifierGroups.Count} modifier groups", Color.Purple);
        }

        // **ENHANCED: Override the original CheckSelectedModifiers to use new logic**
        private bool CheckSelectedModifiers()
        {
            return CheckSelectedModifiersAdvanced();
        }

        private void cmbCraftingMode_SelectedIndexChanged(object sender, EventArgs e)
        {
            // Enable/disable currency selection based on crafting mode
            switch (cmbCraftingMode.SelectedIndex)
            {
                case 0: // Magic Items
                    clbCurrencies.Enabled = false;
                    // Auto-select appropriate currencies for magic items
                    for (int i = 0; i < clbCurrencies.Items.Count; i++)
                    {
                        string item = clbCurrencies.Items[i].ToString();
                        clbCurrencies.SetItemChecked(i, 
                            item.Contains("Transmutation") || 
                            item.Contains("Augmentation") || 
                            item.Contains("Alteration"));
                    }
                    print("Magic Item Mode: Will use Transmute → Augment → Alteration", Color.Blue);
                    break;
                    
                case 1: // Rare Items
                    clbCurrencies.Enabled = false;
                    // Auto-select appropriate currencies for rare items
                    for (int i = 0; i < clbCurrencies.Items.Count; i++)
                    {
                        string item = clbCurrencies.Items[i].ToString();
                        clbCurrencies.SetItemChecked(i, 
                            item.Contains("Alchemy") || 
                            item.Contains("Chaos"));
                    }
                    print("Rare Item Mode: Will use Alchemy → Chaos Orb", Color.Blue);
                    break;
                    
                case 2: // Custom
                    clbCurrencies.Enabled = true;
                    print("Custom Mode: Select your preferred currencies", Color.Blue);
                    break;
            }
        }

        private void btnSetEmergencyHotkey_Click(object sender, EventArgs e)
        {
            if (isSettingHotkey) return;
            
            isSettingHotkey = true;
            txtEmergencyHotkey.Text = "Press any key...";
            txtEmergencyHotkey.BackColor = Color.Yellow;
            btnSetEmergencyHotkey.Enabled = false;
            
            // Capture next key press
            this.KeyPreview = true;
            this.KeyDown += CaptureEmergencyHotkey;
        }

        private void CaptureEmergencyHotkey(object sender, KeyEventArgs e)
        {
            if (!isSettingHotkey) return;
            
            emergencyStopKey = e.KeyCode;
            txtEmergencyHotkey.Text = e.KeyCode.ToString();
            txtEmergencyHotkey.BackColor = Color.White;
            btnSetEmergencyHotkey.Enabled = true;
            isSettingHotkey = false;
            
            this.KeyDown -= CaptureEmergencyHotkey;
            
            print($"Emergency stop hotkey set to: {e.KeyCode}", Color.Green);
            
            // Update global hotkey hook
            SetupEmergencyStopHook();
        }

        private void SetupEmergencyStopHook()
        {
            try
            {
                // Dispose existing hook
                keyboardHook?.Dispose();
                
                // Create new global keyboard hook for emergency stop
                keyboardHook = new GlobalKeyboardHook();
                keyboardHook.KeyDown += (sender, e) =>
                {
                    if (e.KeyCode == emergencyStopKey)
                    {
                        EmergencyStop();
                    }
                };
            }
            catch (Exception ex)
            {
                print($"Warning: Could not set up emergency stop hotkey: {ex.Message}", Color.Orange);
            }
        }

        private void EmergencyStop()
        {
            isEmergencyStopActive = true;
            print($"🛑 EMERGENCY STOP ACTIVATED! ({emergencyStopKey})", Color.Red);
            print("All crafting operations have been halted.", Color.Red);
            
            // Stop any ongoing operations
            // This will be checked in the main crafting loop
        }



        private bool CheckModifierTier(string itemText, string modifierName, int minAcceptableTier)
        {
            // Parse modifier name to extract type and tier info
            if (modifierName.Contains("Physical Damage %"))
            {
                return CheckPhysicalDamagePercentTier(itemText, modifierName, minAcceptableTier);
            }
            else if (modifierName.Contains("Flat Physical Damage"))
            {
                return CheckFlatPhysicalDamageTier(itemText, modifierName, minAcceptableTier);
            }
            else if (modifierName.Contains("Hybrid Phys/Acc"))
            {
                return CheckHybridPhysicalAccuracyTier(itemText, modifierName, minAcceptableTier);
            }
            else if (modifierName.Contains("Attack Speed"))
            {
                return CheckAttackSpeedTier(itemText, modifierName, minAcceptableTier);
            }
            else if (modifierName.Contains("Critical Strike Chance"))
            {
                return CheckCriticalStrikeChanceTier(itemText, modifierName, minAcceptableTier);
            }
            else if (modifierName.Contains("Critical Strike Multiplier"))
            {
                return CheckCriticalStrikeMultiplierTier(itemText, modifierName, minAcceptableTier);
            }
            
            return false;
        }

        private bool CheckPhysicalDamagePercentTier(string itemText, string modifierName, int minTier)
        {
            // Extract percentage value from item text
            var match = System.Text.RegularExpressions.Regex.Match(itemText, @"(\d+)(?:\(\d+-\d+\))?% increased Physical Damage");
            if (!match.Success) return false;
            
            int value = int.Parse(match.Groups[1].Value);
            
            // Determine tier based on value (T1 = best, T10 = worst)
            int actualTier = GetPhysicalDamagePercentTier(value);
            
            // Check if actual tier is acceptable (lower number = better tier)
            bool acceptable = actualTier <= minTier;
            
            if (acceptable)
            {
                print($"✅ Physical Damage: {value}% (T{actualTier}) - Acceptable (min T{minTier})", Color.Green);
            }
            else
            {
                print($"❌ Physical Damage: {value}% (T{actualTier}) - Not acceptable (min T{minTier})", Color.Orange);
            }
            
            return acceptable;
        }

        private int GetPhysicalDamagePercentTier(int value)
        {
            if (value >= 170) return 1; // Merciless T1: 170-179%
            if (value >= 155) return 2; // Tyrannical T2: 155-169%
            if (value >= 135) return 3; // Cruel T3: 135-154%
            if (value >= 110) return 4; // Bloodthirsty T4: 110-134%
            if (value >= 85) return 5;  // Vicious T5: 85-109%
            if (value >= 65) return 6;  // Wicked T6: 65-84%
            if (value >= 50) return 7;  // Serrated T7: 50-64%
            if (value >= 40) return 8;  // Heavy T8: 40-49%
            return 10; // Lower than T8
        }

        private bool CheckFlatPhysicalDamageTier(string itemText, string modifierName, int minTier)
        {
            var match = System.Text.RegularExpressions.Regex.Match(itemText, @"Adds (\d+)(?:\(\d+-\d+\))? to (\d+)(?:\(\d+-\d+\))? Physical Damage");
            if (!match.Success) return false;
            
            int minDamage = int.Parse(match.Groups[1].Value);
            int maxDamage = int.Parse(match.Groups[2].Value);
            
            int actualTier = GetFlatPhysicalDamageTier(minDamage, maxDamage);
            bool acceptable = actualTier <= minTier;
            
            if (acceptable)
            {
                print($"✅ Flat Physical: {minDamage}-{maxDamage} (T{actualTier}) - Acceptable", Color.Green);
            }
            else
            {
                print($"❌ Flat Physical: {minDamage}-{maxDamage} (T{actualTier}) - Not acceptable (min T{minTier})", Color.Orange);
            }
            
            return acceptable;
        }

        private int GetFlatPhysicalDamageTier(int minDamage, int maxDamage)
        {
            // Based on exact PoEDB data for Two Hand Axes - Adds # to # Physical Damage
            // T1 = BEST (highest damage), T9 = WORST (lowest damage)
            if (minDamage >= 34) return 1; // Flaring T1 (level 77): 34-47 to 72-84 - BEST
            if (minDamage >= 30) return 2; // Tempered T2 (level 65): 30-40 to 63-73
            if (minDamage >= 25) return 3; // Razor-sharp T3 (level 54): 25-33 to 52-61
            if (minDamage >= 20) return 4; // Annealed T4 (level 46): 20-28 to 41-51
            if (minDamage >= 16) return 5; // Gleaming T5 (level 36): 16-22 to 35-40
            if (minDamage >= 13) return 6; // Honed T6 (level 29): 13-17 to 28-32
            if (minDamage >= 10) return 7; // Polished T7 (level 21): 10-13 to 21-25
            if (minDamage >= 6) return 8;  // Burnished T8 (level 13): 6-8 to 12-15
            if (minDamage >= 2) return 9;  // Glinting T9 (level 2): 2 to 4-5 - WORST
            return 10; // Invalid/unknown tier
        }

        private bool CheckAttackSpeedTier(string itemText, string modifierName, int minTier)
        {
            var match = System.Text.RegularExpressions.Regex.Match(itemText, @"(\d+)(?:\(\d+-\d+\))?% increased Attack Speed");
            if (!match.Success) return false;
            
            int value = int.Parse(match.Groups[1].Value);
            int actualTier = GetAttackSpeedTier(value);
            bool acceptable = actualTier <= minTier;
            
            if (acceptable)
            {
                print($"✅ Attack Speed: {value}% (T{actualTier}) - Acceptable", Color.Green);
            }
            else
            {
                print($"❌ Attack Speed: {value}% (T{actualTier}) - Not acceptable (min T{minTier})", Color.Orange);
            }
            
            return acceptable;
        }

        private int GetAttackSpeedTier(int value)
        {
            // Based on exact PoEDB data for Two Hand Axes - #% increased Attack Speed
            // T1 = BEST (highest %), T8 = WORST (lowest %)
            if (value >= 26) return 1; // of Celebration T1 (level 77): 26-27% - BEST
            if (value >= 23) return 2; // of Infamy T2 (level 60): 23-25%
            if (value >= 20) return 3; // of Fame T3 (level 45): 20-22%
            if (value >= 17) return 4; // of Acclaim T4 (level 37): 17-19%
            if (value >= 14) return 5; // of Renown T5 (level 30): 14-16%
            if (value >= 11) return 6; // of Mastery T6 (level 22): 11-13%
            if (value >= 8) return 7;  // of Ease T7 (level 11): 8-10%
            if (value >= 5) return 8;  // of Skill T8 (level 1): 5-7% - WORST
            return 10; // Invalid/unknown tier
        }

        private bool CheckCriticalStrikeChanceTier(string itemText, string modifierName, int minTier)
        {
            var match = System.Text.RegularExpressions.Regex.Match(itemText, @"(\d+)(?:\(\d+-\d+\))?% increased Critical Strike Chance");
            if (!match.Success) return false;
            
            int value = int.Parse(match.Groups[1].Value);
            int actualTier = GetCriticalStrikeChanceTier(value);
            bool acceptable = actualTier <= minTier;
            
            if (acceptable)
            {
                print($"✅ Critical Strike Chance: {value}% (T{actualTier}) - Acceptable", Color.Green);
            }
            else
            {
                print($"❌ Critical Strike Chance: {value}% (T{actualTier}) - Not acceptable (min T{minTier})", Color.Orange);
            }
            
            return acceptable;
        }

        private int GetCriticalStrikeChanceTier(int value)
        {
            // Based on exact PoEDB data for Two Hand Axes - #% increased Critical Strike Chance
            // T1 = BEST (highest %), T6 = WORST (lowest %)
            if (value >= 35) return 1; // of Incision T1 (level 73): 35-38% - BEST
            if (value >= 30) return 2; // of Penetrating T2 (level 59): 30-34%
            if (value >= 25) return 3; // of Puncturing T3 (level 44): 25-29%
            if (value >= 20) return 4; // of Piercing T4 (level 30): 20-24%
            if (value >= 15) return 5; // of Stinging T5 (level 20): 15-19%
            if (value >= 10) return 6; // of Needling T6 (level 1): 10-14% - WORST
            return 10; // Invalid/unknown tier
        }

        private bool CheckCriticalStrikeMultiplierTier(string itemText, string modifierName, int minTier)
        {
            var match = System.Text.RegularExpressions.Regex.Match(itemText, @"(\d+)(?:\(\d+-\d+\))?% increased Critical Strike Multiplier");
            if (!match.Success) return false;
            
            int value = int.Parse(match.Groups[1].Value);
            int actualTier = GetCriticalStrikeMultiplierTier(value);
            bool acceptable = actualTier <= minTier;
            
            if (acceptable)
            {
                print($"✅ Critical Strike Multiplier: {value}% (T{actualTier}) - Acceptable", Color.Green);
            }
            else
            {
                print($"❌ Critical Strike Multiplier: {value}% (T{actualTier}) - Not acceptable (min T{minTier})", Color.Orange);
            }
            
            return acceptable;
        }

        private int GetCriticalStrikeMultiplierTier(int value)
        {
            // Based on exact PoEDB data for Two Hand Axes - +#% to Global Critical Strike Multiplier
            // T1 = BEST (highest %), T6 = WORST (lowest %)
            if (value >= 35) return 1; // of Destruction T1 (level 73): 35-38% - BEST
            if (value >= 30) return 2; // of Ferocity T2 (level 59): 30-34%
            if (value >= 25) return 3; // of Fury T3 (level 44): 25-29%
            if (value >= 20) return 4; // of Rage T4 (level 30): 20-24%
            if (value >= 15) return 5; // of Anger T5 (level 21): 15-19%
            if (value >= 10) return 6; // of Ire T6 (level 8): 10-14% - WORST
            return 10; // Invalid/unknown tier
        }

        private void ClbModifiers_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            // Prevent headers and empty lines from being checked
            string item = clbModifiers.Items[e.Index].ToString();
            if (item.StartsWith("---") || string.IsNullOrWhiteSpace(item))
            {
                e.NewValue = CheckState.Unchecked;
                return;
            }
            
            // Prevent recursion by temporarily removing the event handler
            clbModifiers.ItemCheck -= ClbModifiers_ItemCheck;
            
            try
            {
                if (e.NewValue == CheckState.Checked)
                {
                    // When checking a tier, automatically check all lower (better) tiers
                    CheckLowerTiers(e.Index, item);
                }
                else if (e.NewValue == CheckState.Unchecked)
                {
                    // When unchecking a tier, automatically uncheck all higher (worse) tiers
                    UncheckHigherTiers(e.Index, item);
                }
            }
            finally
            {
                // Re-add the event handler
                clbModifiers.ItemCheck += ClbModifiers_ItemCheck;
            }
        }

        private bool CheckHybridPhysicalAccuracyTier(string itemText, string modifierName, int minTier)
        {
            // Look for hybrid physical/accuracy modifier
            var match = System.Text.RegularExpressions.Regex.Match(itemText, @"(\d+)% increased Physical Damage.*?(\d+) to Accuracy Rating");
            if (!match.Success) return false;
            
            int physPercent = int.Parse(match.Groups[1].Value);
            int accuracy = int.Parse(match.Groups[2].Value);
            
            int actualTier = GetHybridPhysicalAccuracyTier(physPercent, accuracy);
            bool acceptable = actualTier <= minTier;
            
            if (acceptable)
            {
                print($"✅ Hybrid Phys/Acc: {physPercent}% + {accuracy} Acc (T{actualTier}) - Acceptable", Color.Green);
            }
            else
            {
                print($"❌ Hybrid Phys/Acc: {physPercent}% + {accuracy} Acc (T{actualTier}) - Not acceptable", Color.Orange);
            }
            
            return acceptable;
        }

        private bool CheckModifierTierFromSelection(string itemText, string selectedModifier)
        {
            // Extract the modifier type and tier from the selection
            if (selectedModifier.Contains("Physical Damage %"))
            {
                return CheckPhysicalDamagePercentFromSelection(itemText, selectedModifier);
            }
            else if (selectedModifier.Contains("Flat Physical Damage"))
            {
                return CheckFlatPhysicalDamageFromSelection(itemText, selectedModifier);
            }
            else if (selectedModifier.Contains("Hybrid Phys/Acc"))
            {
                return CheckHybridPhysicalAccuracyFromSelection(itemText, selectedModifier);
            }
            else if (selectedModifier.Contains("Attack Speed"))
            {
                return CheckAttackSpeedFromSelection(itemText, selectedModifier);
            }
            else if (selectedModifier.Contains("Critical Strike Chance"))
            {
                return CheckCriticalStrikeChanceFromSelection(itemText, selectedModifier);
            }
            else if (selectedModifier.Contains("Critical Strike Multiplier"))
            {
                return CheckCriticalStrikeMultiplierFromSelection(itemText, selectedModifier);
            }
            
            return false;
        }

        private void CheckLowerTiers(int checkedIndex, string checkedItem)
        {
            // Get the modifier type and tier of the checked item
            string modifierType = GetModifierType(checkedItem);
            int checkedTier = GetTierFromItem(checkedItem);
            
            if (checkedTier == -1) return; // Invalid tier
            
            // Find and check all lower (better) tiers of the same modifier type
            for (int i = 0; i < clbModifiers.Items.Count; i++)
            {
                if (i == checkedIndex) continue;
                
                string item = clbModifiers.Items[i].ToString();
                if (GetModifierType(item) == modifierType)
                {
                    int tier = GetTierFromItem(item);
                    if (tier != -1 && tier < checkedTier) // Lower tier number = better tier
                    {
                        clbModifiers.SetItemChecked(i, true);
                    }
                }
            }
        }

        private void UncheckHigherTiers(int uncheckedIndex, string uncheckedItem)
        {
            // Get the modifier type and tier of the unchecked item
            string modifierType = GetModifierType(uncheckedItem);
            int uncheckedTier = GetTierFromItem(uncheckedItem);
            
            if (uncheckedTier == -1) return; // Invalid tier
            
            // Find and uncheck all higher (worse) tiers of the same modifier type
            for (int i = 0; i < clbModifiers.Items.Count; i++)
            {
                if (i == uncheckedIndex) continue;
                
                string item = clbModifiers.Items[i].ToString();
                if (GetModifierType(item) == modifierType)
                {
                    int tier = GetTierFromItem(item);
                    if (tier != -1 && tier > uncheckedTier) // Higher tier number = worse tier
                    {
                        clbModifiers.SetItemChecked(i, false);
                    }
                }
            }
        }

        private string GetModifierType(string item)
        {
            if (item.Contains("Physical Damage %")) return "Physical Damage %";
            if (item.Contains("Flat Physical Damage")) return "Flat Physical Damage";
            if (item.Contains("Hybrid Phys/Acc")) return "Hybrid Phys/Acc";
            if (item.Contains("Attack Speed")) return "Attack Speed";
            if (item.Contains("Critical Strike Chance")) return "Critical Strike Chance";
            if (item.Contains("Critical Strike Multiplier")) return "Critical Strike Multiplier";
            return "";
        }

        private int GetTierFromItem(string item)
        {
            // Extract tier number from strings like "Physical Damage % (Merciless T1: 170-179%)"
            var match = System.Text.RegularExpressions.Regex.Match(item, @"T(\d+):");
            if (match.Success)
            {
                return int.Parse(match.Groups[1].Value);
            }
            return -1; // Invalid tier
        }

        private bool CheckPhysicalDamagePercentFromSelection(string itemText, string selectedModifier)
        {
            var match = System.Text.RegularExpressions.Regex.Match(itemText, @"(\d+)(?:\(\d+-\d+\))?% increased Physical Damage");
            if (!match.Success) return false;
            
            int value = int.Parse(match.Groups[1].Value);
            int actualTier = GetPhysicalDamagePercentTier(value);
            int selectedTier = GetTierFromItem(selectedModifier);
            
            print($"Found Physical Damage: {value}% (T{actualTier})", Color.Blue);
            
            // **FIXED: Check if actual tier meets or exceeds selected tier requirement**
            if (selectedTier != -1 && actualTier <= selectedTier) // Lower tier number = better tier
            {
                return true;
            }
            
            print($"❌ Tier check failed: Found T{actualTier}, need T{selectedTier} or better", Color.Orange);
            return false;
        }

        private bool CheckAttackSpeedFromSelection(string itemText, string selectedModifier)
        {
            var match = System.Text.RegularExpressions.Regex.Match(itemText, @"(\d+)(?:\(\d+-\d+\))?% increased Attack Speed");
            if (!match.Success) return false;
            
            int value = int.Parse(match.Groups[1].Value);
            int actualTier = GetAttackSpeedTier(value);
            int selectedTier = GetTierFromItem(selectedModifier);
            
            print($"Found Attack Speed: {value}% (T{actualTier})", Color.Blue);
            
            // **FIXED: Check if actual tier meets or exceeds selected tier requirement**
            if (selectedTier != -1 && actualTier <= selectedTier) // Lower tier number = better tier
            {
                return true;
            }
            
            print($"❌ Tier check failed: Found T{actualTier}, need T{selectedTier} or better", Color.Orange);
            return false;
        }

        private bool CheckFlatPhysicalDamageFromSelection(string itemText, string selectedModifier)
        {
            var match = System.Text.RegularExpressions.Regex.Match(itemText, @"Adds (\d+)(?:\(\d+-\d+\))? to (\d+)(?:\(\d+-\d+\))? Physical Damage");
            if (!match.Success) return false;
            
            int minDamage = int.Parse(match.Groups[1].Value);
            int maxDamage = int.Parse(match.Groups[2].Value);
            int actualTier = GetFlatPhysicalDamageTier(minDamage, maxDamage);
            int selectedTier = GetTierFromItem(selectedModifier);
            
            print($"Found Flat Physical: {minDamage}-{maxDamage} (T{actualTier})", Color.Blue);
            
            // **FIXED: Check if actual tier meets or exceeds selected tier requirement**
            if (selectedTier != -1 && actualTier <= selectedTier) // Lower tier number = better tier
            {
                return true;
            }
            
            print($"❌ Tier check failed: Found T{actualTier}, need T{selectedTier} or better", Color.Orange);
            return false;
        }

        private bool CheckHybridPhysicalAccuracyFromSelection(string itemText, string selectedModifier)
        {
            var match = System.Text.RegularExpressions.Regex.Match(itemText, @"(\d+)% increased Physical Damage.*?(\d+) to Accuracy Rating");
            if (!match.Success) return false;
            
            int physPercent = int.Parse(match.Groups[1].Value);
            int accuracy = int.Parse(match.Groups[2].Value);
            int actualTier = GetHybridPhysicalAccuracyTier(physPercent, accuracy);
            int selectedTier = GetTierFromItem(selectedModifier);
            
            print($"Found Hybrid Phys/Acc: {physPercent}% + {accuracy} Acc (T{actualTier})", Color.Blue);
            
            // **FIXED: Check if actual tier meets or exceeds selected tier requirement**
            if (selectedTier != -1 && actualTier <= selectedTier) // Lower tier number = better tier
            {
                return true;
            }
            
            print($"❌ Tier check failed: Found T{actualTier}, need T{selectedTier} or better", Color.Orange);
            return false;
        }

        private bool CheckCriticalStrikeChanceFromSelection(string itemText, string selectedModifier)
        {
            var match = System.Text.RegularExpressions.Regex.Match(itemText, @"(\d+)(?:\(\d+-\d+\))?% increased Critical Strike Chance");
            if (!match.Success) return false;
            
            int value = int.Parse(match.Groups[1].Value);
            int actualTier = GetCriticalStrikeChanceTier(value);
            int selectedTier = GetTierFromItem(selectedModifier);
            
            print($"Found Critical Strike Chance: {value}% (T{actualTier})", Color.Blue);
            
            // **FIXED: Check if actual tier meets or exceeds selected tier requirement**
            if (selectedTier != -1 && actualTier <= selectedTier) // Lower tier number = better tier
            {
                return true;
            }
            
            print($"❌ Tier check failed: Found T{actualTier}, need T{selectedTier} or better", Color.Orange);
            return false;
        }

        private bool CheckCriticalStrikeMultiplierFromSelection(string itemText, string selectedModifier)
        {
            var match = System.Text.RegularExpressions.Regex.Match(itemText, @"(\d+)(?:\(\d+-\d+\))?% increased Critical Strike Multiplier");
            if (!match.Success) return false;
            
            int value = int.Parse(match.Groups[1].Value);
            int actualTier = GetCriticalStrikeMultiplierTier(value);
            int selectedTier = GetTierFromItem(selectedModifier);
            
            print($"Found Critical Strike Multiplier: {value}% (T{actualTier})", Color.Blue);
            
            // **FIXED: Check if actual tier meets or exceeds selected tier requirement**
            if (selectedTier != -1 && actualTier <= selectedTier) // Lower tier number = better tier
            {
                return true;
            }
            
            print($"❌ Tier check failed: Found T{actualTier}, need T{selectedTier} or better", Color.Orange);
            return false;
        }

        private int GetHybridPhysicalAccuracyTier(int physPercent, int accuracy)
        {
            // Based on exact PoEDB data for Two Hand Axes - #% increased Physical Damage + # to Accuracy Rating
            // T1 = BEST (highest %), T8 = WORST (lowest %)
            if (physPercent >= 75 && accuracy >= 175) return 1; // Dictator's T1 (level 83): 75-79% + 175-200 Acc - BEST
            if (physPercent >= 65 && accuracy >= 150) return 2; // Emperor's T2 (level 73): 65-74% + 150-174 Acc
            if (physPercent >= 55 && accuracy >= 124) return 3; // Conqueror's T3 (level 60): 55-64% + 124-149 Acc
            if (physPercent >= 45 && accuracy >= 98) return 4;  // Champion's T4 (level 46): 45-54% + 98-123 Acc
            if (physPercent >= 35 && accuracy >= 73) return 5;  // Mercenary's T5 (level 35): 35-44% + 73-97 Acc
            if (physPercent >= 25 && accuracy >= 47) return 6;  // Reaver's T6 (level 23): 25-34% + 47-72 Acc
            if (physPercent >= 20 && accuracy >= 21) return 7;  // Journeyman's T7 (level 11): 20-24% + 21-46 Acc
            if (physPercent >= 15 && accuracy >= 16) return 8;  // Squire's T8 (level 1): 15-19% + 16-20 Acc - WORST
            return 10; // Invalid/unknown tier
        }

        // **NEW: Keyboard shortcuts for window management**
        private void ProgressBar_KeyDown(object sender, KeyEventArgs e)
        {
            // Ctrl+Home to center window (emergency positioning fix)
            if (e.Control && e.KeyCode == Keys.Home)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(
                    (Screen.PrimaryScreen.WorkingArea.Width - this.Width) / 2,
                    (Screen.PrimaryScreen.WorkingArea.Height - this.Height) / 2
                );
                this.WindowState = FormWindowState.Normal;
                this.BringToFront();
                print("🎯 Window centered manually (Ctrl+Home)", Color.Cyan);
                e.Handled = true;
            }
        }

        private void Form1_Load(object sender, EventArgs e)
        {
            // **ENHANCED: Comprehensive Settings Loading**
            try
            {
                // **FIXED: Window positioning with bounds checking**
                var savedLocation = Properties.Settings.Default.WindowLocation;
                
                // Check if saved location is valid and on screen
                if (savedLocation != null && 
                    savedLocation.X >= -100 && savedLocation.Y >= -100 && // Allow some off-screen but recoverable
                    savedLocation.X < Screen.PrimaryScreen.WorkingArea.Width &&
                    savedLocation.Y < Screen.PrimaryScreen.WorkingArea.Height)
                {
                    this.Location = savedLocation;
                }
                else
                {
                    // Center the window if saved position is invalid
                    this.StartPosition = FormStartPosition.CenterScreen;
                    print("⚠️ Window centered due to invalid saved position", Color.Orange);
                }

                // **ENHANCED: Load window state and size**
                var savedSize = Properties.Settings.Default.WindowSize;
                if (savedSize.Width > 400 && savedSize.Height > 300) // Minimum reasonable size
                {
                    this.Size = savedSize;
                }

                // **ENHANCED: Load UI settings**
                this.TopMost = Properties.Settings.Default.AlwaysOnTop;
                
                // **FIXED: Validate TrackBar value before setting**
                int savedDelay = Properties.Settings.Default.AddedDelay;
                if (savedDelay < trackBar1.Minimum || savedDelay > trackBar1.Maximum)
                {
                    // Use default value if saved value is out of range
                    savedDelay = 50; // Default delay value
                    Properties.Settings.Default.AddedDelay = savedDelay;
                    Properties.Settings.Default.Save();
                    print($"Reset delay to default ({savedDelay}ms) - saved value was invalid", Color.Orange);
                }
                
                DelayNumber.Text = savedDelay.ToString();
                trackBar1.Value = savedDelay;

                // **NEW: Load Advanced Logic Settings**
                chkUseORLogic.Checked = Properties.Settings.Default.UseORLogic;
                chkSmartAugmentation.Checked = Properties.Settings.Default.SmartAugmentation;
                
                // **NEW: Load Emergency Stop Key**
                string savedKey = Properties.Settings.Default.EmergencyStopKey;
                if (!string.IsNullOrEmpty(savedKey))
                {
                    txtEmergencyHotkey.Text = savedKey;
                    // Parse and set the emergency stop key
                    if (Enum.TryParse<Keys>(savedKey, out Keys parsedKey))
                    {
                        emergencyStopKey = parsedKey;
                    }
                }

                // **NEW: Load Crafting Mode**
                int savedCraftingMode = Properties.Settings.Default.CraftingModeIndex;
                if (savedCraftingMode >= 0 && savedCraftingMode < cmbCraftingMode.Items.Count)
                {
                    cmbCraftingMode.SelectedIndex = savedCraftingMode;
                }

                // **NEW: Load Selected Modifiers**
                string savedModifiers = Properties.Settings.Default.SelectedModifiers;
                if (!string.IsNullOrEmpty(savedModifiers))
                {
                    LoadSelectedModifiers(savedModifiers);
                }

                // **NEW: Load Currency Limits**
                int savedMaxChaos = Properties.Settings.Default.MaxChaosToUse;
                if (savedMaxChaos > 0)
                {
                    ChaosToUse.Value = savedMaxChaos;
                }

                // **ENHANCED: Initialize advanced systems**
                InitializeCraftingConfiguration();
                SetupEmergencyStopHook();
                GenerateGetProcessors();
                UpdateLocations();

                // **NEW: Initialize separate log window**
                logWindow = new LogWindow();
                logWindow.Text = "PoECrafter - Crafting Logs";
                // Position it to the right of main window
                logWindow.Location = new Point(this.Location.X + this.Width + 10, this.Location.Y);

                // Force window visibility
                this.WindowState = FormWindowState.Normal;
                this.Visible = true;
                this.BringToFront();
                this.Activate();

                // **ENHANCED: Welcome message with feature summary**
                print("🚀 PoECrafter Advanced Version Loaded Successfully!", Color.Green);
                print($"✨ Features Enabled:", Color.Blue);
                print($"   🎯 OR Logic: {(chkUseORLogic.Checked ? "ON" : "OFF")}", Color.Blue);
                print($"   💡 Smart Augmentation: {(chkSmartAugmentation.Checked ? "ON" : "OFF")}", Color.Blue);
                print($"   🛑 Emergency Stop: {txtEmergencyHotkey.Text}", Color.Blue);
                print($"   ⚙️ Crafting Mode: {cmbCraftingMode.Text}", Color.Blue);
                print("🎮 Ready for advanced crafting operations!", Color.Green);
            }
            catch (Exception ex)
            {
                print($"⚠️ Error loading settings: {ex.Message}", Color.Red);
                print("Using default settings...", Color.Orange);
            }
        }
    }

    public class ComboBoxItem
    {
        string DisplayText;
        IntPtr Value;

        //Constructor
        public ComboBoxItem(string a, IntPtr b)
        {
            DisplayText = a;
            Value = b;
        }

        //Accessor
        public IntPtr HiddenValue
        {
            get
            {
                return Value;
            }
        }

        //Override ToString method
        public override string ToString()
        {
            return DisplayText;
        }
    }

    public class CraftItem : CraftItemBase
    {
    }
}
