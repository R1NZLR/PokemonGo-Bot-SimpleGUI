namespace PokemonGo.RocketAPI.GUI
{
    partial class GUISettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GUISettingsForm));
            this.label1 = new System.Windows.Forms.Label();
            this.boxPokestopDelay = new System.Windows.Forms.TextBox();
            this.btnSaveSettings = new System.Windows.Forms.Button();
            this.checkAutoEvolve = new System.Windows.Forms.CheckBox();
            this.boxPokemonDelay = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.boxCPMin = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.boxIVMin = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.label6 = new System.Windows.Forms.Label();
            this.label7 = new System.Windows.Forms.Label();
            this.boxMinBerry = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.checkAutoTransfer = new System.Windows.Forms.CheckBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.label10 = new System.Windows.Forms.Label();
            this.boxHumanWalkingSpeed = new System.Windows.Forms.TextBox();
            this.checkHumanWalking = new System.Windows.Forms.CheckBox();
            this.label9 = new System.Windows.Forms.Label();
            this.checkSilentRecycle = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 15);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(145, 13);
            this.label1.TabIndex = 0;
            this.label1.Text = "Delay between PokeStops?";
            // 
            // boxPokestopDelay
            // 
            this.boxPokestopDelay.Location = new System.Drawing.Point(203, 12);
            this.boxPokestopDelay.Name = "boxPokestopDelay";
            this.boxPokestopDelay.Size = new System.Drawing.Size(50, 22);
            this.boxPokestopDelay.TabIndex = 0;
            this.boxPokestopDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // btnSaveSettings
            // 
            this.btnSaveSettings.Location = new System.Drawing.Point(431, 262);
            this.btnSaveSettings.Name = "btnSaveSettings";
            this.btnSaveSettings.Size = new System.Drawing.Size(97, 30);
            this.btnSaveSettings.TabIndex = 5;
            this.btnSaveSettings.Text = "Save Settings";
            this.btnSaveSettings.UseVisualStyleBackColor = true;
            this.btnSaveSettings.Click += new System.EventHandler(this.btnSaveSettings_Click);
            // 
            // checkAutoEvolve
            // 
            this.checkAutoEvolve.AutoSize = true;
            this.checkAutoEvolve.Location = new System.Drawing.Point(15, 226);
            this.checkAutoEvolve.Name = "checkAutoEvolve";
            this.checkAutoEvolve.Size = new System.Drawing.Size(181, 30);
            this.checkAutoEvolve.TabIndex = 4;
            this.checkAutoEvolve.Text = "Automatically Evolve Pokemon\r\nafter visiting all PokeStops";
            this.checkAutoEvolve.UseVisualStyleBackColor = true;
            // 
            // boxPokemonDelay
            // 
            this.boxPokemonDelay.Location = new System.Drawing.Point(203, 40);
            this.boxPokemonDelay.Name = "boxPokemonDelay";
            this.boxPokemonDelay.Size = new System.Drawing.Size(50, 22);
            this.boxPokemonDelay.TabIndex = 1;
            this.boxPokemonDelay.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 43);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(144, 13);
            this.label2.TabIndex = 4;
            this.label2.Text = "Delay between Pokemons?";
            // 
            // boxCPMin
            // 
            this.boxCPMin.Location = new System.Drawing.Point(203, 128);
            this.boxCPMin.Name = "boxCPMin";
            this.boxCPMin.Size = new System.Drawing.Size(50, 22);
            this.boxCPMin.TabIndex = 2;
            this.boxCPMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(12, 131);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(169, 13);
            this.label3.TabIndex = 6;
            this.label3.Text = "Minimum CP to Keep Pokemon?";
            // 
            // boxIVMin
            // 
            this.boxIVMin.Location = new System.Drawing.Point(203, 156);
            this.boxIVMin.Name = "boxIVMin";
            this.boxIVMin.Size = new System.Drawing.Size(50, 22);
            this.boxIVMin.TabIndex = 3;
            this.boxIVMin.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(12, 159);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(166, 13);
            this.label4.TabIndex = 8;
            this.label4.Text = "Minimum IV to keep Pokemon?";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.ForeColor = System.Drawing.Color.Red;
            this.label5.Location = new System.Drawing.Point(34, 65);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(220, 39);
            this.label5.TabIndex = 10;
            this.label5.Text = "The delay must be specified in SECONDS. \r\nKeep in mind that lower delays will giv" +
    "e\r\nyou more ran away pokemons.\r\n";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.ForeColor = System.Drawing.Color.Red;
            this.label6.Location = new System.Drawing.Point(34, 181);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(210, 26);
            this.label6.TabIndex = 11;
            this.label6.Text = "Minimum CP and IV affects which \r\npokemons are automatically transfered.";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.ForeColor = System.Drawing.Color.Red;
            this.label7.Location = new System.Drawing.Point(303, 50);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(213, 26);
            this.label7.TabIndex = 14;
            this.label7.Text = "Example, use berry if pokemon \r\ncatch is below 40%, you would enter 40.";
            // 
            // boxMinBerry
            // 
            this.boxMinBerry.Location = new System.Drawing.Point(472, 12);
            this.boxMinBerry.Name = "boxMinBerry";
            this.boxMinBerry.Size = new System.Drawing.Size(50, 22);
            this.boxMinBerry.TabIndex = 12;
            this.boxMinBerry.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(281, 15);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(149, 26);
            this.label8.TabIndex = 13;
            this.label8.Text = "Use Berry if Pokemon catch \r\npercentage is below?";
            // 
            // checkAutoTransfer
            // 
            this.checkAutoTransfer.AutoSize = true;
            this.checkAutoTransfer.Location = new System.Drawing.Point(15, 262);
            this.checkAutoTransfer.Name = "checkAutoTransfer";
            this.checkAutoTransfer.Size = new System.Drawing.Size(189, 30);
            this.checkAutoTransfer.TabIndex = 15;
            this.checkAutoTransfer.Text = "Automatically Transfer Pokemon\r\nafter visiting all PokeStops";
            this.checkAutoTransfer.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label10);
            this.groupBox1.Controls.Add(this.boxHumanWalkingSpeed);
            this.groupBox1.Controls.Add(this.checkHumanWalking);
            this.groupBox1.Controls.Add(this.label9);
            this.groupBox1.Location = new System.Drawing.Point(284, 107);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(244, 71);
            this.groupBox1.TabIndex = 16;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Human Walking Simulation";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.ForeColor = System.Drawing.Color.Red;
            this.label10.Location = new System.Drawing.Point(3, 44);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(234, 13);
            this.label10.TabIndex = 17;
            this.label10.Text = "Regular human walking speed is around 5.0";
            // 
            // boxHumanWalkingSpeed
            // 
            this.boxHumanWalkingSpeed.Location = new System.Drawing.Point(188, 19);
            this.boxHumanWalkingSpeed.Name = "boxHumanWalkingSpeed";
            this.boxHumanWalkingSpeed.Size = new System.Drawing.Size(50, 22);
            this.boxHumanWalkingSpeed.TabIndex = 17;
            this.boxHumanWalkingSpeed.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // checkHumanWalking
            // 
            this.checkHumanWalking.AutoSize = true;
            this.checkHumanWalking.Location = new System.Drawing.Point(6, 21);
            this.checkHumanWalking.Name = "checkHumanWalking";
            this.checkHumanWalking.Size = new System.Drawing.Size(66, 17);
            this.checkHumanWalking.TabIndex = 17;
            this.checkHumanWalking.Text = "Enable?";
            this.checkHumanWalking.UseVisualStyleBackColor = true;
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(138, 22);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(44, 13);
            this.label9.TabIndex = 18;
            this.label9.Text = "Speed?";
            // 
            // checkSilentRecycle
            // 
            this.checkSilentRecycle.AutoSize = true;
            this.checkSilentRecycle.Location = new System.Drawing.Point(284, 184);
            this.checkSilentRecycle.Name = "checkSilentRecycle";
            this.checkSilentRecycle.Size = new System.Drawing.Size(245, 30);
            this.checkSilentRecycle.TabIndex = 17;
            this.checkSilentRecycle.Text = "Enable Silent Recyle (This will recycle items \r\nsilently every 5 minutes).";
            this.checkSilentRecycle.UseVisualStyleBackColor = true;
            // 
            // GUISettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(539, 303);
            this.Controls.Add(this.checkSilentRecycle);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.checkAutoTransfer);
            this.Controls.Add(this.label7);
            this.Controls.Add(this.boxMinBerry);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.boxIVMin);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.boxCPMin);
            this.Controls.Add(this.label3);
            this.Controls.Add(this.boxPokemonDelay);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.checkAutoEvolve);
            this.Controls.Add(this.btnSaveSettings);
            this.Controls.Add(this.boxPokestopDelay);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "GUISettingsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "GUI Settings";
            this.Load += new System.EventHandler(this.GUISettingsForm_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox boxPokestopDelay;
        private System.Windows.Forms.Button btnSaveSettings;
        private System.Windows.Forms.CheckBox checkAutoEvolve;
        private System.Windows.Forms.TextBox boxPokemonDelay;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.TextBox boxCPMin;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox boxIVMin;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox boxMinBerry;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.CheckBox checkAutoTransfer;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox boxHumanWalkingSpeed;
        private System.Windows.Forms.CheckBox checkHumanWalking;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.CheckBox checkSilentRecycle;
    }
}