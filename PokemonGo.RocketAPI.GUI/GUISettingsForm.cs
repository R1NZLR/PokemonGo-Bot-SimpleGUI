using System;
using System.Windows.Forms;

namespace PokemonGo.RocketAPI.GUI
{
    public partial class GUISettingsForm : Form
    {
        public GUISettingsForm()
        {
            InitializeComponent();
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

                GUISettings.Default.autoEvolve = checkAutoEvolve.Checked;
                GUISettings.Default.autoTransfer = checkAutoTransfer.Checked;

                GUISettings.Default.humanWalkingEnabled = checkHumanWalking.Checked;
                GUISettings.Default.humanWalkingSpeed = double.Parse(boxHumanWalkingSpeed.Text);

                GUISettings.Default.enableSilentRecycle = checkSilentRecycle.Checked;

                GUISettings.Default.Save();
                this.Close();
            }
            catch
            {
                MessageBox.Show("You have invalid data in your settings.", "PoGo Bot");
            }
        }

        private void GUISettingsForm_Load(object sender, EventArgs e)
        {
            boxPokestopDelay.Text = GUISettings.Default.pokestopDelay.ToString();
            boxPokemonDelay.Text = GUISettings.Default.pokemonDelay.ToString();

            boxCPMin.Text = GUISettings.Default.minCP.ToString();
            boxIVMin.Text = GUISettings.Default.minIV.ToString();
            boxMinBerry.Text = GUISettings.Default.minBerry.ToString();

            checkAutoEvolve.Checked = GUISettings.Default.autoEvolve;
            checkAutoTransfer.Checked = GUISettings.Default.autoTransfer;

            checkHumanWalking.Checked = GUISettings.Default.humanWalkingEnabled;
            boxHumanWalkingSpeed.Text = GUISettings.Default.humanWalkingSpeed.ToString();

            checkSilentRecycle.Checked = GUISettings.Default.enableSilentRecycle;
        }
    }
}
