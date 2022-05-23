namespace Desktop
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.openFileDialog1 = new System.Windows.Forms.OpenFileDialog();
            this.openFileDialog2 = new System.Windows.Forms.OpenFileDialog();
            this.labelEngine1 = new System.Windows.Forms.Label();
            this.buttonLoad1 = new System.Windows.Forms.Button();
            this.buttonLoad2 = new System.Windows.Forms.Button();
            this.labelEngine2 = new System.Windows.Forms.Label();
            this.richTextBox1 = new System.Windows.Forms.RichTextBox();
            this.buttonSearch = new System.Windows.Forms.Button();
            this.buttonSearch2 = new System.Windows.Forms.Button();
            this.buttonPlay = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // openFileDialog1
            // 
            this.openFileDialog1.DefaultExt = "exe";
            this.openFileDialog1.FileName = "openFileDialog1";
            this.openFileDialog1.Filter = "exe files| *.exe";
            this.openFileDialog1.Title = "Load Engine 1";
            // 
            // openFileDialog2
            // 
            this.openFileDialog2.DefaultExt = "exe";
            this.openFileDialog2.FileName = "openFileDialog2";
            this.openFileDialog2.Filter = "exe files| *.exe";
            this.openFileDialog2.Title = "Load Engine 2";
            // 
            // labelEngine1
            // 
            this.labelEngine1.AutoSize = true;
            this.labelEngine1.Location = new System.Drawing.Point(12, 44);
            this.labelEngine1.Name = "labelEngine1";
            this.labelEngine1.Size = new System.Drawing.Size(0, 20);
            this.labelEngine1.TabIndex = 0;
            // 
            // buttonLoad1
            // 
            this.buttonLoad1.Location = new System.Drawing.Point(12, 12);
            this.buttonLoad1.Name = "buttonLoad1";
            this.buttonLoad1.Size = new System.Drawing.Size(130, 29);
            this.buttonLoad1.TabIndex = 1;
            this.buttonLoad1.Text = "Load Engine 1";
            this.buttonLoad1.UseVisualStyleBackColor = true;
            this.buttonLoad1.Click += new System.EventHandler(this.buttonLoad1_Click);
            // 
            // buttonLoad2
            // 
            this.buttonLoad2.Location = new System.Drawing.Point(1138, 12);
            this.buttonLoad2.Name = "buttonLoad2";
            this.buttonLoad2.Size = new System.Drawing.Size(130, 29);
            this.buttonLoad2.TabIndex = 3;
            this.buttonLoad2.Text = "Load Engine 2";
            this.buttonLoad2.UseVisualStyleBackColor = true;
            this.buttonLoad2.Click += new System.EventHandler(this.buttonLoad2_Click);
            // 
            // labelEngine2
            // 
            this.labelEngine2.AutoSize = true;
            this.labelEngine2.Location = new System.Drawing.Point(745, 44);
            this.labelEngine2.Name = "labelEngine2";
            this.labelEngine2.Size = new System.Drawing.Size(0, 20);
            this.labelEngine2.TabIndex = 2;
            // 
            // richTextBox1
            // 
            this.richTextBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left)));
            this.richTextBox1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(64)))), ((int)(((byte)(64)))), ((int)(((byte)(64)))));
            this.richTextBox1.ForeColor = System.Drawing.Color.White;
            this.richTextBox1.Location = new System.Drawing.Point(18, 67);
            this.richTextBox1.Name = "richTextBox1";
            this.richTextBox1.Size = new System.Drawing.Size(676, 420);
            this.richTextBox1.TabIndex = 5;
            this.richTextBox1.Text = "";
            // 
            // buttonSearch
            // 
            this.buttonSearch.Location = new System.Drawing.Point(148, 12);
            this.buttonSearch.Name = "buttonSearch";
            this.buttonSearch.Size = new System.Drawing.Size(94, 29);
            this.buttonSearch.TabIndex = 6;
            this.buttonSearch.Text = "Search";
            this.buttonSearch.UseVisualStyleBackColor = true;
            this.buttonSearch.Click += new System.EventHandler(this.buttonSearch_Click);
            // 
            // buttonSearch2
            // 
            this.buttonSearch2.Location = new System.Drawing.Point(1274, 12);
            this.buttonSearch2.Name = "buttonSearch2";
            this.buttonSearch2.Size = new System.Drawing.Size(94, 29);
            this.buttonSearch2.TabIndex = 6;
            this.buttonSearch2.Text = "Search";
            this.buttonSearch2.UseVisualStyleBackColor = true;
            this.buttonSearch2.Click += new System.EventHandler(this.buttonSearch2_Click);
            // 
            // buttonPlay
            // 
            this.buttonPlay.Location = new System.Drawing.Point(651, 12);
            this.buttonPlay.Name = "buttonPlay";
            this.buttonPlay.Size = new System.Drawing.Size(94, 29);
            this.buttonPlay.TabIndex = 6;
            this.buttonPlay.Text = "Play";
            this.buttonPlay.UseVisualStyleBackColor = true;
            this.buttonPlay.Click += new System.EventHandler(this.buttonPlay_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1380, 503);
            this.Controls.Add(this.buttonSearch2);
            this.Controls.Add(this.buttonPlay);
            this.Controls.Add(this.buttonSearch);
            this.Controls.Add(this.richTextBox1);
            this.Controls.Add(this.buttonLoad2);
            this.Controls.Add(this.labelEngine2);
            this.Controls.Add(this.buttonLoad1);
            this.Controls.Add(this.labelEngine1);
            this.Name = "Form1";
            this.Text = "Form1";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.Form1_FormClosing);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private OpenFileDialog openFileDialog1;
        private OpenFileDialog openFileDialog2;
        private Label labelEngine1;
        private Button buttonLoad1;
        private Button buttonLoad2;
        private Label labelEngine2;
        private RichTextBox richTextBox1;
        private Button buttonSearch;
        private Button buttonSearch2;
        private Button buttonPlay;
    }
}