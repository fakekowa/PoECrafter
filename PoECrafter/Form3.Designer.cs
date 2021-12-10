using Gma.System.MouseKeyHook;

namespace WindowsFormsApplication3
{
    partial class Form3
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
            this.CraftMatX = new System.Windows.Forms.TextBox();
            this.CraftMatY = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.label3 = new System.Windows.Forms.Label();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.ChromaticY = new System.Windows.Forms.TextBox();
            this.ChromaticX = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.label9 = new System.Windows.Forms.Label();
            this.JewelersY = new System.Windows.Forms.TextBox();
            this.JewelersX = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.label11 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.FusingY = new System.Windows.Forms.TextBox();
            this.FusingX = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // CraftMatX
            // 
            this.CraftMatX.Location = new System.Drawing.Point(107, 39);
            this.CraftMatX.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CraftMatX.Name = "CraftMatX";
            this.CraftMatX.Size = new System.Drawing.Size(116, 23);
            this.CraftMatX.TabIndex = 0;
            this.CraftMatX.TextChanged += new System.EventHandler(this.CraftMatX_TextChanged);
            // 
            // CraftMatY
            // 
            this.CraftMatY.Location = new System.Drawing.Point(323, 39);
            this.CraftMatY.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.CraftMatY.Name = "CraftMatY";
            this.CraftMatY.Size = new System.Drawing.Size(116, 23);
            this.CraftMatY.TabIndex = 1;
            this.CraftMatY.TextChanged += new System.EventHandler(this.CraftMatY_TextChanged);
            // 
            // label1
            // 
            this.label1.Location = new System.Drawing.Point(1, 2);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(520, 33);
            this.label1.TabIndex = 0;
            this.label1.Text = "Crafting Mat Location";
            this.label1.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(80, 43);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(17, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "X:";
            this.label2.Click += new System.EventHandler(this.label2_Click);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(296, 43);
            this.label3.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(17, 15);
            this.label3.TabIndex = 3;
            this.label3.Text = "Y:";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(296, 118);
            this.label4.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(17, 15);
            this.label4.TabIndex = 8;
            this.label4.Text = "Y:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(80, 118);
            this.label5.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(17, 15);
            this.label5.TabIndex = 7;
            this.label5.Text = "X:";
            this.label5.Click += new System.EventHandler(this.label5_Click);
            // 
            // label6
            // 
            this.label6.Location = new System.Drawing.Point(1, 77);
            this.label6.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(520, 33);
            this.label6.TabIndex = 4;
            this.label6.Text = "Chromatic Location";
            this.label6.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // ChromaticY
            // 
            this.ChromaticY.Location = new System.Drawing.Point(323, 114);
            this.ChromaticY.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ChromaticY.Name = "ChromaticY";
            this.ChromaticY.Size = new System.Drawing.Size(116, 23);
            this.ChromaticY.TabIndex = 6;
            this.ChromaticY.TextChanged += new System.EventHandler(this.ChromaticY_TextChanged);
            // 
            // ChromaticX
            // 
            this.ChromaticX.Location = new System.Drawing.Point(107, 114);
            this.ChromaticX.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.ChromaticX.Name = "ChromaticX";
            this.ChromaticX.Size = new System.Drawing.Size(116, 23);
            this.ChromaticX.TabIndex = 5;
            this.ChromaticX.TextChanged += new System.EventHandler(this.ChromaticX_TextChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(295, 189);
            this.label7.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(17, 15);
            this.label7.TabIndex = 13;
            this.label7.Text = "Y:";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(79, 189);
            this.label8.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(17, 15);
            this.label8.TabIndex = 12;
            this.label8.Text = "X:";
            // 
            // label9
            // 
            this.label9.Location = new System.Drawing.Point(0, 149);
            this.label9.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(520, 33);
            this.label9.TabIndex = 9;
            this.label9.Text = "Jewelers Location";
            this.label9.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // JewelersY
            // 
            this.JewelersY.Location = new System.Drawing.Point(322, 186);
            this.JewelersY.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.JewelersY.Name = "JewelersY";
            this.JewelersY.Size = new System.Drawing.Size(116, 23);
            this.JewelersY.TabIndex = 11;
            this.JewelersY.TextChanged += new System.EventHandler(this.JewelersY_TextChanged);
            // 
            // JewelersX
            // 
            this.JewelersX.Location = new System.Drawing.Point(106, 186);
            this.JewelersX.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.JewelersX.Name = "JewelersX";
            this.JewelersX.Size = new System.Drawing.Size(116, 23);
            this.JewelersX.TabIndex = 10;
            this.JewelersX.TextChanged += new System.EventHandler(this.JewelersX_TextChanged);
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(295, 258);
            this.label10.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(17, 15);
            this.label10.TabIndex = 18;
            this.label10.Text = "Y:";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(79, 258);
            this.label11.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(17, 15);
            this.label11.TabIndex = 17;
            this.label11.Text = "X:";
            this.label11.Click += new System.EventHandler(this.label11_Click);
            // 
            // label12
            // 
            this.label12.Location = new System.Drawing.Point(0, 218);
            this.label12.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(520, 33);
            this.label12.TabIndex = 14;
            this.label12.Text = "Fusing Location";
            this.label12.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // FusingY
            // 
            this.FusingY.Location = new System.Drawing.Point(322, 255);
            this.FusingY.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.FusingY.Name = "FusingY";
            this.FusingY.Size = new System.Drawing.Size(116, 23);
            this.FusingY.TabIndex = 16;
            this.FusingY.TextChanged += new System.EventHandler(this.FusingY_TextChanged);
            // 
            // FusingX
            // 
            this.FusingX.Location = new System.Drawing.Point(106, 255);
            this.FusingX.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.FusingX.Name = "FusingX";
            this.FusingX.Size = new System.Drawing.Size(116, 23);
            this.FusingX.TabIndex = 15;
            this.FusingX.TextChanged += new System.EventHandler(this.FusingX_TextChanged);
            // 
            // Form3
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(522, 302);
            this.Controls.Add(this.label10);
            this.Controls.Add(this.label11);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.FusingY);
            this.Controls.Add(this.FusingX);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label9);
            this.Controls.Add(this.JewelersY);
            this.Controls.Add(this.JewelersX);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.ChromaticY);
            this.Controls.Add(this.ChromaticX);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.CraftMatY);
            this.Controls.Add(this.CraftMatX);
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "Form3";
            this.Text = "Crafting Mat Locations X Y";
            this.Load += new System.EventHandler(this.Form3_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox CraftMatX;
        private System.Windows.Forms.TextBox CraftMatY;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox ChromaticY;
        private System.Windows.Forms.TextBox ChromaticX;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox JewelersY;
        private System.Windows.Forms.TextBox JewelersX;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox FusingY;
        private System.Windows.Forms.TextBox FusingX;
    }
}