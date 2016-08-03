using PokemonGo.RocketAPI.Enums;
using PokemonGo.RocketAPI.GUI.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.GUI
{
    public partial class LoginForm : Form
    {
        public AuthType auth;
        public bool loginSelected = false;

        public LoginForm()
        {
            InitializeComponent();
        }

        private void LoginForm_Load(object sender, EventArgs e)
        {            
            boxUsername.Text = GUISettings.Default.username;
            boxPassword.Text = GUISettings.Default.password;

            if( GUISettings.Default.newLoginMethodFirstTimeSee )
            {
                MessageBox.Show("Enter your Google account or PTC, the program will know which one to use.", "PoGo Bot");
                GUISettings.Default.newLoginMethodFirstTimeSee = false;
                GUISettings.Default.Save();
            }
        }

        private void btnLogin_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrWhiteSpace(boxUsername.Text))
            {
                if (!string.IsNullOrWhiteSpace(boxPassword.Text))
                {
                    if (cbRemember.Checked)
                    {
                        GUISettings.Default.username = boxUsername.Text;
                        GUISettings.Default.password = boxPassword.Text;
                        GUISettings.Default.Save();
                    }
                    else
                    {
                        GUISettings.Default.username = string.Empty;
                        GUISettings.Default.password = string.Empty;
                        GUISettings.Default.Save();
                    }

                    loginSelected = true;

                    // Google Accounts use Email / PTC don't.
                    if (boxUsername.Text.Contains('@'))
                        auth = AuthType.Google;
                    else
                        auth = AuthType.Ptc;

                    this.Hide();
                }
                else
                {
                    MessageBox.Show("Password textbox cannot be empty", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            else
            {
                MessageBox.Show("Username textbox cannot be empty", "Validation", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void boxPassword_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
                btnLogin_Click(null, null);
        }

        private void LoginForm_FormClosing(object sender, FormClosingEventArgs e)
        {

        }
    }
}
