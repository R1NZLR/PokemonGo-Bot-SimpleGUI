namespace PokemonGo.RocketAPI.GUI
{
    partial class PokemonSnipingForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PokemonSnipingForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lbSnipeUrl = new System.Windows.Forms.Label();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.boxCoordinates = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.btnSnipe = new System.Windows.Forms.Button();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.textResults = new System.Windows.Forms.TextBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.lbSnipeUrl);
            this.groupBox1.Location = new System.Drawing.Point(12, 12);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(404, 72);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "1. Open this website and pick the pokemon you want to attempt to snipe";
            // 
            // lbSnipeUrl
            // 
            this.lbSnipeUrl.AutoSize = true;
            this.lbSnipeUrl.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.lbSnipeUrl.ForeColor = System.Drawing.Color.Blue;
            this.lbSnipeUrl.Location = new System.Drawing.Point(13, 18);
            this.lbSnipeUrl.Name = "lbSnipeUrl";
            this.lbSnipeUrl.Size = new System.Drawing.Size(376, 30);
            this.lbSnipeUrl.TabIndex = 0;
            this.lbSnipeUrl.Text = "http://spawns.sebastienvercammen.be/";
            this.lbSnipeUrl.Click += new System.EventHandler(this.lbSnipeUrl_Click);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(67, 48);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(277, 13);
            this.label1.TabIndex = 1;
            this.label1.Text = "You can click on the link to open the website directly";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label3);
            this.groupBox2.Controls.Add(this.boxCoordinates);
            this.groupBox2.Location = new System.Drawing.Point(12, 90);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(404, 82);
            this.groupBox2.TabIndex = 2;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "2. Copy/Paste here the coordinates of the Pokemon you want to snipe";
            // 
            // boxCoordinates
            // 
            this.boxCoordinates.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.boxCoordinates.Location = new System.Drawing.Point(18, 21);
            this.boxCoordinates.Name = "boxCoordinates";
            this.boxCoordinates.Size = new System.Drawing.Size(371, 35);
            this.boxCoordinates.TabIndex = 2;
            this.boxCoordinates.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(87, 59);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(225, 13);
            this.label3.TabIndex = 3;
            this.label3.Text = "It must be in a format like 20.0000, 20.0000";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.btnSnipe);
            this.groupBox3.Location = new System.Drawing.Point(12, 178);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(404, 73);
            this.groupBox3.TabIndex = 4;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "3. Click the button to validate the coordinates and start the sniping";
            // 
            // btnSnipe
            // 
            this.btnSnipe.Location = new System.Drawing.Point(18, 21);
            this.btnSnipe.Name = "btnSnipe";
            this.btnSnipe.Size = new System.Drawing.Size(371, 42);
            this.btnSnipe.TabIndex = 5;
            this.btnSnipe.Text = "Start Pokemon Sniping";
            this.btnSnipe.UseVisualStyleBackColor = true;
            this.btnSnipe.Click += new System.EventHandler(this.btnSnipe_Click);
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.textResults);
            this.groupBox4.Location = new System.Drawing.Point(12, 257);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(404, 206);
            this.groupBox4.TabIndex = 6;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "4. Hope for the best, sniping is still under development";
            // 
            // textResults
            // 
            this.textResults.Dock = System.Windows.Forms.DockStyle.Fill;
            this.textResults.Location = new System.Drawing.Point(3, 18);
            this.textResults.Multiline = true;
            this.textResults.Name = "textResults";
            this.textResults.Size = new System.Drawing.Size(398, 185);
            this.textResults.TabIndex = 0;
            // 
            // PokemonSnipingForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(432, 475);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Name = "PokemonSnipingForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Pokemon Sniping";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label lbSnipeUrl;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox boxCoordinates;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.Button btnSnipe;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox textResults;
    }
}