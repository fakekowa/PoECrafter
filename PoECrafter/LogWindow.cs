using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsFormsApplication3
{
    public partial class LogWindow : Form
    {
        public RichTextBox LogTextBox { get; private set; }

        public LogWindow()
        {
            InitializeComponent();
            this.StartPosition = FormStartPosition.Manual;
            this.ShowInTaskbar = false; // Don't show in taskbar
            this.TopMost = true; // Keep on top
        }

        private void InitializeComponent()
        {
            this.LogTextBox = new RichTextBox();
            this.SuspendLayout();
            
            // 
            // LogTextBox
            // 
            this.LogTextBox.Dock = DockStyle.Fill;
            this.LogTextBox.Font = new Font("Consolas", 9F, FontStyle.Regular, GraphicsUnit.Point);
            this.LogTextBox.Name = "LogTextBox";
            this.LogTextBox.ReadOnly = true;
            this.LogTextBox.ScrollBars = RichTextBoxScrollBars.ForcedVertical;
            this.LogTextBox.TabIndex = 0;
            this.LogTextBox.Text = "";
            this.LogTextBox.BackColor = Color.White;
            this.LogTextBox.ForeColor = Color.Black;
            
            // 
            // LogWindow
            // 
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new Size(600, 400);
            this.Controls.Add(this.LogTextBox);
            this.Icon = null;
            this.MinimumSize = new Size(400, 200);
            this.Name = "LogWindow";
            this.Text = "PoECrafter - Crafting Logs";
            this.ResumeLayout(false);
        }

        public void AppendLog(string text, Color color)
        {
            if (LogTextBox.InvokeRequired)
            {
                LogTextBox.Invoke(new Action(() => AppendLog(text, color)));
                return;
            }

            LogTextBox.SelectionStart = LogTextBox.TextLength;
            LogTextBox.SelectionLength = 0;
            LogTextBox.SelectionColor = color;
            LogTextBox.AppendText(text + Environment.NewLine);
            LogTextBox.SelectionColor = LogTextBox.ForeColor;
            LogTextBox.ScrollToCaret();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // Don't actually close, just hide
            e.Cancel = true;
            this.Hide();
        }
    }
} 