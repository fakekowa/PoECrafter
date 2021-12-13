namespace WindowsFormsApplication3
{
    partial class ProgressBar
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.label1 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.StartColors = new System.Windows.Forms.Button();
            this.button1 = new System.Windows.Forms.Button();
            this.ChromaticsToUse = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.FusingLabel = new System.Windows.Forms.Label();
            this.FusingsToUse = new System.Windows.Forms.NumericUpDown();
            this.progressBar1 = new System.Windows.Forms.ProgressBar();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.JewelersToUse = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.SocketStart = new System.Windows.Forms.Button();
            this.label8 = new System.Windows.Forms.Label();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.aboutToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.informationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.settingsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.alwaysOntopToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.materialLocationToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.shiftClickFixToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            this.label9 = new System.Windows.Forms.Label();
            this.DelayNumber = new System.Windows.Forms.Label();
            this.processList = new System.Windows.Forms.ComboBox();
            this.label10 = new System.Windows.Forms.Label();
            this.button2 = new System.Windows.Forms.Button();
            this.craftingRulesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.ChromaticsToUse)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.FusingsToUse)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.JewelersToUse)).BeginInit();
            this.menuStrip1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label1.Location = new System.Drawing.Point(337, 62);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(121, 23);
            this.label1.TabIndex = 4;
            this.label1.Text = "Chaos Craft";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label5.Location = new System.Drawing.Point(342, 359);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(267, 23);
            this.label5.TabIndex = 8;
            this.label5.Text = "Aug Craft (NOT FOR NOW)";
            // 
            // StartColors
            // 
            this.StartColors.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.StartColors.Location = new System.Drawing.Point(342, 152);
            this.StartColors.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.StartColors.Name = "StartColors";
            this.StartColors.Size = new System.Drawing.Size(316, 27);
            this.StartColors.TabIndex = 10;
            this.StartColors.Text = "GO!";
            this.StartColors.UseVisualStyleBackColor = true;
            this.StartColors.Click += new System.EventHandler(this.StartColors_Click);
            // 
            // button1
            // 
            this.button1.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.button1.Location = new System.Drawing.Point(342, 462);
            this.button1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.button1.Name = "button1";
            this.button1.Size = new System.Drawing.Size(316, 27);
            this.button1.TabIndex = 11;
            this.button1.Text = "GO!";
            this.button1.UseVisualStyleBackColor = true;
            this.button1.Click += new System.EventHandler(this.button1_Click);
            // 
            // ChromaticsToUse
            // 
            this.ChromaticsToUse.Location = new System.Drawing.Point(465, 123);
            this.ChromaticsToUse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ChromaticsToUse.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.ChromaticsToUse.Name = "ChromaticsToUse";
            this.ChromaticsToUse.Size = new System.Drawing.Size(78, 23);
            this.ChromaticsToUse.TabIndex = 12;
            this.ChromaticsToUse.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label6.Location = new System.Drawing.Point(449, 104);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(94, 16);
            this.label6.TabIndex = 13;
            this.label6.Text = "Chaos to use";
            // 
            // FusingLabel
            // 
            this.FusingLabel.AutoSize = true;
            this.FusingLabel.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.FusingLabel.Location = new System.Drawing.Point(464, 414);
            this.FusingLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.FusingLabel.Name = "FusingLabel";
            this.FusingLabel.Size = new System.Drawing.Size(79, 16);
            this.FusingLabel.TabIndex = 14;
            this.FusingLabel.Text = "Aug to use";
            this.FusingLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FusingsToUse
            // 
            this.FusingsToUse.Location = new System.Drawing.Point(465, 433);
            this.FusingsToUse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.FusingsToUse.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.FusingsToUse.Name = "FusingsToUse";
            this.FusingsToUse.Size = new System.Drawing.Size(78, 23);
            this.FusingsToUse.TabIndex = 15;
            this.FusingsToUse.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // progressBar1
            // 
            this.progressBar1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.progressBar1.Location = new System.Drawing.Point(0, 559);
            this.progressBar1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.progressBar1.Name = "progressBar1";
            this.progressBar1.Size = new System.Drawing.Size(685, 27);
            this.progressBar1.Step = 1;
            this.progressBar1.TabIndex = 16;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Location = new System.Drawing.Point(14, 62);
            this.richTextBox1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.ScrollBars = System.Windows.Forms.RichTextBoxScrollBars.ForcedVertical;
            this.richTextBox1.Size = new System.Drawing.Size(316, 425);
            this.richTextBox1.TabIndex = 17;
            this.richTextBox1.Text = "";
            // 
            // JewelersToUse
            // 
            this.JewelersToUse.Location = new System.Drawing.Point(465, 271);
            this.JewelersToUse.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.JewelersToUse.Maximum = new decimal(new int[] {
            5000,
            0,
            0,
            0});
            this.JewelersToUse.Name = "JewelersToUse";
            this.JewelersToUse.Size = new System.Drawing.Size(78, 23);
            this.JewelersToUse.TabIndex = 22;
            this.JewelersToUse.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label7.Location = new System.Drawing.Point(441, 252);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(118, 16);
            this.label7.TabIndex = 21;
            this.label7.Text = "Alteration to use";
            this.label7.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // SocketStart
            // 
            this.SocketStart.Font = new System.Drawing.Font("Verdana", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.SocketStart.Location = new System.Drawing.Point(342, 300);
            this.SocketStart.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.SocketStart.Name = "SocketStart";
            this.SocketStart.Size = new System.Drawing.Size(316, 27);
            this.SocketStart.TabIndex = 20;
            this.SocketStart.Text = "GO!";
            this.SocketStart.UseVisualStyleBackColor = true;
            this.SocketStart.Click += new System.EventHandler(this.SocketStart_Click);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Font = new System.Drawing.Font("Verdana", 14.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.label8.Location = new System.Drawing.Point(337, 220);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(156, 23);
            this.label8.TabIndex = 18;
            this.label8.Text = "Alteration Craft";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.aboutToolStripMenuItem,
            this.settingsToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Padding = new System.Windows.Forms.Padding(7, 2, 0, 2);
            this.menuStrip1.Size = new System.Drawing.Size(685, 24);
            this.menuStrip1.TabIndex = 23;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // aboutToolStripMenuItem
            // 
            this.aboutToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.informationToolStripMenuItem});
            this.aboutToolStripMenuItem.Name = "aboutToolStripMenuItem";
            this.aboutToolStripMenuItem.Size = new System.Drawing.Size(44, 20);
            this.aboutToolStripMenuItem.Text = "Help";
            // 
            // informationToolStripMenuItem
            // 
            this.informationToolStripMenuItem.Name = "informationToolStripMenuItem";
            this.informationToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.informationToolStripMenuItem.Text = "Information";
            this.informationToolStripMenuItem.Click += new System.EventHandler(this.informationToolStripMenuItem_Click);
            // 
            // settingsToolStripMenuItem
            // 
            this.settingsToolStripMenuItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.alwaysOntopToolStripMenuItem,
            this.materialLocationToolStripMenuItem,
            this.shiftClickFixToolStripMenuItem,
            this.craftingRulesToolStripMenuItem});
            this.settingsToolStripMenuItem.Name = "settingsToolStripMenuItem";
            this.settingsToolStripMenuItem.Size = new System.Drawing.Size(61, 20);
            this.settingsToolStripMenuItem.Text = "Settings";
            // 
            // alwaysOntopToolStripMenuItem
            // 
            this.alwaysOntopToolStripMenuItem.Name = "alwaysOntopToolStripMenuItem";
            this.alwaysOntopToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.alwaysOntopToolStripMenuItem.Text = "Always ontop";
            this.alwaysOntopToolStripMenuItem.Click += new System.EventHandler(this.alwaysOntopToolStripMenuItem_Click);
            // 
            // materialLocationToolStripMenuItem
            // 
            this.materialLocationToolStripMenuItem.Name = "materialLocationToolStripMenuItem";
            this.materialLocationToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.materialLocationToolStripMenuItem.Text = "Material Location";
            this.materialLocationToolStripMenuItem.Click += new System.EventHandler(this.settingsToolStripMenuItem_Click);
            // 
            // shiftClickFixToolStripMenuItem
            // 
            this.shiftClickFixToolStripMenuItem.Name = "shiftClickFixToolStripMenuItem";
            this.shiftClickFixToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.shiftClickFixToolStripMenuItem.Text = "Shift Click Fix";
            this.shiftClickFixToolStripMenuItem.Click += new System.EventHandler(this.shiftClickFixToolStripMenuItem_Click);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(0, 495);
            this.trackBar1.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.trackBar1.Maximum = 1000;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(685, 45);
            this.trackBar1.TabIndex = 24;
            this.trackBar1.TickStyle = System.Windows.Forms.TickStyle.None;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(8, 520);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(68, 15);
            this.label9.TabIndex = 25;
            this.label9.Text = "Extra Delay:";
            // 
            // DelayNumber
            // 
            this.DelayNumber.AutoSize = true;
            this.DelayNumber.Location = new System.Drawing.Point(90, 522);
            this.DelayNumber.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.DelayNumber.Name = "DelayNumber";
            this.DelayNumber.Size = new System.Drawing.Size(0, 15);
            this.DelayNumber.TabIndex = 26;
            // 
            // processList
            // 
            this.processList.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.processList.FormattingEnabled = true;
            this.processList.Location = new System.Drawing.Point(124, 31);
            this.processList.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.processList.Name = "processList";
            this.processList.Size = new System.Drawing.Size(508, 23);
            this.processList.TabIndex = 27;
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(10, 35);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(95, 15);
            this.label10.TabIndex = 28;
            this.label10.Text = "Process Override";
            // 
            // button2
            // 
            this.button2.BackgroundImage = global::WindowsFormsApplication3.Properties.Resources._21x21_REFRESH;
            this.button2.BackgroundImageLayout = System.Windows.Forms.ImageLayout.Stretch;
            this.button2.Location = new System.Drawing.Point(639, 31);
            this.button2.Margin = new System.Windows.Forms.Padding(0);
            this.button2.Name = "button2";
            this.button2.Size = new System.Drawing.Size(24, 24);
            this.button2.TabIndex = 29;
            this.button2.UseVisualStyleBackColor = true;
            this.button2.Click += new System.EventHandler(this.button2_Click);
            // 
            // craftingRulesToolStripMenuItem
            // 
            this.craftingRulesToolStripMenuItem.Name = "craftingRulesToolStripMenuItem";
            this.craftingRulesToolStripMenuItem.Size = new System.Drawing.Size(180, 22);
            this.craftingRulesToolStripMenuItem.Text = "Crafting Rules";
            this.craftingRulesToolStripMenuItem.Click += new System.EventHandler(this.craftingRulesToolStripMenuItem_Click);
            // 
            // ProgressBar
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(685, 586);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.button2);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.processList);
            this.Controls.Add(this.DelayNumber);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.JewelersToUse);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.SocketStart);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.progressBar1);
            this.Controls.Add(this.FusingsToUse);
            this.Controls.Add(this.FusingLabel);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.ChromaticsToUse);
            this.Controls.Add(this.button1);
            this.Controls.Add(this.StartColors);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.menuStrip1);
            this.MainMenuStrip = this.menuStrip1;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ProgressBar";
            this.Text = "Crafty by Pontus";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.Load += new System.EventHandler(this.Form1_Load);
            ((System.ComponentModel.ISupportInitialize)(this.ChromaticsToUse)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.FusingsToUse)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.JewelersToUse)).EndInit();
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button StartColors;
        private System.Windows.Forms.Button button1;
        private System.Windows.Forms.NumericUpDown ChromaticsToUse;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label FusingLabel;
        private System.Windows.Forms.NumericUpDown FusingsToUse;
        private System.Windows.Forms.ProgressBar progressBar1;
        private System.Windows.Forms.RichTextBox richTextBox1;
        private System.Windows.Forms.NumericUpDown JewelersToUse;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Button SocketStart;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem aboutToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem informationToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem settingsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem alwaysOntopToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem materialLocationToolStripMenuItem;
        private System.Windows.Forms.TrackBar trackBar1;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.Label DelayNumber;
        private System.Windows.Forms.ToolStripMenuItem shiftClickFixToolStripMenuItem;
        private System.Windows.Forms.Label label10;
        public System.Windows.Forms.ComboBox processList;
        private System.Windows.Forms.Button button2;
        private System.Windows.Forms.ToolStripMenuItem craftingRulesToolStripMenuItem;
    }
}

