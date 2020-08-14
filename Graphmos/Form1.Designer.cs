namespace Graphmos
{
    partial class Form1
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
            this.EquationIO = new System.Windows.Forms.TextBox();
            this.GraphPanel = new System.Windows.Forms.Panel();
            this.trackBar1 = new System.Windows.Forms.TrackBar();
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).BeginInit();
            this.SuspendLayout();
            // 
            // EquationIO
            // 
            this.EquationIO.Location = new System.Drawing.Point(12, 202);
            this.EquationIO.Multiline = true;
            this.EquationIO.Name = "EquationIO";
            this.EquationIO.Size = new System.Drawing.Size(437, 410);
            this.EquationIO.TabIndex = 1;
            this.EquationIO.TextChanged += new System.EventHandler(this.EquationIO_TextChanged);
            // 
            // GraphPanel
            // 
            this.GraphPanel.AutoSizeMode = System.Windows.Forms.AutoSizeMode.GrowAndShrink;
            this.GraphPanel.Location = new System.Drawing.Point(471, 12);
            this.GraphPanel.Name = "GraphPanel";
            this.GraphPanel.Size = new System.Drawing.Size(800, 600);
            this.GraphPanel.TabIndex = 0;
            this.GraphPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.GraphPanel_Paint);
            this.GraphPanel.MouseHover += new System.EventHandler(this.Hover);
            this.GraphPanel.MouseMove += new System.Windows.Forms.MouseEventHandler(this.Move);
            this.GraphPanel.MouseWheel += new System.Windows.Forms.MouseEventHandler(this.MW);
            // 
            // trackBar1
            // 
            this.trackBar1.Location = new System.Drawing.Point(12, 145);
            this.trackBar1.Minimum = -10;
            this.trackBar1.Name = "trackBar1";
            this.trackBar1.Size = new System.Drawing.Size(437, 69);
            this.trackBar1.TabIndex = 2;
            this.trackBar1.Scroll += new System.EventHandler(this.trackBar1_Scroll);
            // 
            // Form1
            // 
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
            this.ClientSize = new System.Drawing.Size(1283, 639);
            this.Controls.Add(this.trackBar1);
            this.Controls.Add(this.EquationIO);
            this.Controls.Add(this.GraphPanel);
            this.Name = "Form1";
            this.Text = "Form1";
            ((System.ComponentModel.ISupportInitialize)(this.trackBar1)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.TextBox EquationIO;

        private System.Windows.Forms.Panel GraphPanel;
        private System.Windows.Forms.TrackBar trackBar1;
    }
}

