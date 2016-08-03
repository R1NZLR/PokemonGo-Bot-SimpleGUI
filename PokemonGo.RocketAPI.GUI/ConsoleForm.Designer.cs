using System.Windows.Forms;

namespace PokemonGo.RocketAPI.GUI
{
    partial class ConsoleForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ConsoleForm));
            this.boxConsole = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // boxConsole
            // 
            this.boxConsole.Dock = DockStyle.Fill;
            this.boxConsole.Location = new System.Drawing.Point(0, 0);
            this.boxConsole.Multiline = true;
            this.boxConsole.Name = "boxConsole";
            this.boxConsole.Size = new System.Drawing.Size(1044, 61);
            this.boxConsole.TabIndex = 0;
            this.boxConsole.ScrollBars = ScrollBars.Vertical;
            // 
            // ConsoleForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1044, 61);
            this.Controls.Add(this.boxConsole);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "ConsoleForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text = "Console";
            this.FormClosing += new FormClosingEventHandler(this.ConsoleForm_FormClosing);
            this.Load += new System.EventHandler(this.ConsoleForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        public TextBox boxConsole;
    }
}