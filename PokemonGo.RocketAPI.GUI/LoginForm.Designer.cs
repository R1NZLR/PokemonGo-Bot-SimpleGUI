namespace PokemonGo.RocketAPI.GUI
{
    partial class LoginForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(LoginForm));
            this.btnLogin = new System.Windows.Forms.Button();
            this.boxPassword = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.boxUsername = new System.Windows.Forms.TextBox();
            this.usernameToolTip = new System.Windows.Forms.ToolTip(this.components);
            this.cbRemember = new System.Windows.Forms.CheckBox();
            this.SuspendLayout();
            // 
            // btnLogin
            // 
            this.btnLogin.Location = new System.Drawing.Point(310, 114);
            this.btnLogin.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.btnLogin.Name = "btnLogin";
            this.btnLogin.Size = new System.Drawing.Size(136, 41);
            this.btnLogin.TabIndex = 3;
            this.btnLogin.Text = "Login";
            this.btnLogin.UseVisualStyleBackColor = true;
            this.btnLogin.Click += new System.EventHandler(this.btnLogin_Click);
            // 
            // boxPassword
            // 
            this.boxPassword.Location = new System.Drawing.Point(134, 65);
            this.boxPassword.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.boxPassword.Name = "boxPassword";
            this.boxPassword.PasswordChar = '*';
            this.boxPassword.Size = new System.Drawing.Size(312, 35);
            this.boxPassword.TabIndex = 1;
            this.boxPassword.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.boxPassword.KeyDown += new System.Windows.Forms.KeyEventHandler(this.boxPassword_KeyDown);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(9, 65);
            this.label1.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(99, 30);
            this.label1.TabIndex = 3;
            this.label1.Text = "Password";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 16);
            this.label2.Margin = new System.Windows.Forms.Padding(6, 0, 6, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(106, 30);
            this.label2.TabIndex = 5;
            this.label2.Text = "Username";
            // 
            // boxUsername
            // 
            this.boxUsername.Location = new System.Drawing.Point(134, 16);
            this.boxUsername.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.boxUsername.Name = "boxUsername";
            this.boxUsername.Size = new System.Drawing.Size(312, 35);
            this.boxUsername.TabIndex = 0;
            this.boxUsername.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // usernameToolTip
            // 
            this.usernameToolTip.Tag = "Enter your PTC or Google Account the application will know which Authentication m" +
    "ethod to use.";
            this.usernameToolTip.ToolTipTitle = "Google Auth / PTC";
            // 
            // cbRemember
            // 
            this.cbRemember.AutoSize = true;
            this.cbRemember.Checked = true;
            this.cbRemember.CheckState = System.Windows.Forms.CheckState.Checked;
            this.cbRemember.Location = new System.Drawing.Point(14, 118);
            this.cbRemember.Name = "cbRemember";
            this.cbRemember.Size = new System.Drawing.Size(277, 34);
            this.cbRemember.TabIndex = 6;
            this.cbRemember.Text = "Remember my credentials.";
            this.cbRemember.UseVisualStyleBackColor = true;
            // 
            // LoginForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.ClientSize = new System.Drawing.Size(461, 171);
            this.Controls.Add(this.cbRemember);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.btnLogin);
            this.Controls.Add(this.boxUsername);
            this.Controls.Add(this.boxPassword);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 15.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedToolWindow;
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(6, 7, 6, 7);
            this.Name = "LoginForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "Select Login Method";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.LoginForm_FormClosing);
            this.Load += new System.EventHandler(this.LoginForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private System.Windows.Forms.Button btnLogin;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        public System.Windows.Forms.TextBox boxPassword;
        public System.Windows.Forms.TextBox boxUsername;
        private System.Windows.Forms.ToolTip usernameToolTip;
        private System.Windows.Forms.CheckBox cbRemember;
    }
}