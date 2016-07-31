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
    public partial class GUISettingsForm : Form
    {
        public GUISettingsForm()
        {
            InitializeComponent();
        }

        private void GUISettings_Load(object sender, EventArgs e)
        {
            boxPokestopDelay.Text = GUISettings.Default.pokestopDelay.ToString();
            boxPokemonDelay.Text = GUISettings.Default.pokemonDelay.ToString();
            
            boxCPMin.Text = GUISettings.Default.minCP.ToString();
            boxIVMin.Text = GUISettings.Default.minIV.ToString();
            boxMinBerry.Text = GUISettings.Default.minBerry.ToString();

            checkAutoTransfer.Checked = GUISettings.Default.autoEvolveTransfer;
            autoTransferRecycle.Checked = GUISettings.Default.autoTransferRecycle;
        }

        private void btnSaveSettings_Click(object sender, EventArgs e)
        {
            try
            {
                GUISettings.Default.pokestopDelay = int.Parse(boxPokestopDelay.Text);
                GUISettings.Default.pokemonDelay = int.Parse(boxPokemonDelay.Text);

                GUISettings.Default.minCP = int.Parse(boxCPMin.Text);
                GUISettings.Default.minIV = int.Parse(boxIVMin.Text);
                GUISettings.Default.minBerry = int.Parse(boxMinBerry.Text);

                GUISettings.Default.autoEvolveTransfer = checkAutoTransfer.Checked;
                GUISettings.Default.autoTransferRecycle = autoTransferRecycle.Checked;

                GUISettings.Default.Save();
                this.Close();
            }
            catch
            {
                MessageBox.Show("You have invalid data in your settings.", "PoGo Bot");
            }
        }
    }
}
