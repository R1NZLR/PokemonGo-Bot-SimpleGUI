using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Net;
using System.IO;
using System.Collections;
using PokemonGo.RocketAPI.Logic;
using PokemonGo.RocketAPI.GUI.Helpers;
using POGOProtos.Networking.Responses;

namespace PokemonGo.RocketAPI.GUI
{
    public partial class PokemonForm : Form
    {
        private Client _client;
        private Inventory _inventory;

        public PokemonForm(Client client)
        {
            _client = client;
            _inventory = new Inventory(client);
            InitializeComponent();
        }

        private async void PokemonForm_Load(object sender, EventArgs e)
        {
            try
            {
                resetLoadProgress();
                await Execute();
            }
            catch (Exception ex)
            {
                ErrorReportCreator.Create("MyPokemonList", "Unable to Open My Pokemon List", ex);
            }           
        
        }

        private async Task Execute()
        {
            pokemonListView.Clear();

            // Get Pokemons and Pokemon Families
            var pokemons = await _inventory.GetPokeListPokemon();
            var families = await _inventory.GetPokeListPokemonFamilies();

            var imageList = new ImageList { ImageSize = new Size(50, 50) };
            var myPokemonsCount = pokemons.Count();
            pokemonListView.ShowItemToolTips = true;

            // Add Pokemon ListViewItems to a list
            var pokeList = new List<ListViewItem>();
            var pokeIndex = 0;
            foreach (var pokemon in pokemons)
            {
                ListViewItem listViewItem = new ListViewItem();

                // Get Pokemon Image Index
                var imageIndex = imageList.Images.IndexOfKey(pokemon.PokemonId.ToString());

                // Checker for Pokemon Image
                if (imageIndex == -1)
                    imageList.Images.Add(pokemon.PokemonId.ToString(), await GetPokemonImageAsync((int)pokemon.PokemonId));
                else
                    imageList.Images.Add(pokemon.PokemonId.ToString(), imageList.Images[imageIndex]);

                pokemonListView.LargeImageList = imageList;
                var pokemonIv = Math.Floor(Logic.Logic.CalculatePokemonPerfection(pokemon));
                listViewItem.SubItems.Add(pokemon.PokemonId.ToString());
                listViewItem.SubItems.Add(pokemon.Cp.ToString());
                listViewItem.SubItems.Add(pokemonIv.ToString());

                var currentCandy = families
                    .Where(i => (int)i.FamilyId <= (int)pokemon.PokemonId)
                    .Select(f => f.Candy_)
                    .First();

                listViewItem.ImageKey = pokemon.PokemonId.ToString();
                listViewItem.Text = string.Format("{0}\nCP {1} IV {2}%", pokemon.PokemonId, pokemon.Cp, pokemonIv);
                listViewItem.Tag = pokemon.Id;
                listViewItem.ToolTipText = "Candy: " + currentCandy;

                // Pokemon List Loading Progress
                pokeIndex += 1;
                lbPokeListLoading.Text = $"{pokeIndex}/{myPokemonsCount}";

                pokeList.Add(listViewItem);
            }

            lbPokeListLoading.Visible = false;

            // Add all Pokemon ListViewItems to pokemonListView all at once
            ListViewItem[] pokeArray = pokeList.ToArray();
            pokemonListView.BeginUpdate();
            pokemonListView.Items.AddRange(pokeArray);
            pokemonListView.EndUpdate();
        }

        private void resetLoadProgress()
        {
            lbPokeListLoading.Text = "Loading...";
            lbPokeListLoading.Visible = true;
        }

        private async Task<Bitmap> GetPokemonImageAsync(int pokemonId)
        {
            WebRequest req = WebRequest.Create("http://pokeapi.co/media/sprites/pokemon/" + pokemonId + ".png");
            WebResponse res = await req.GetResponseAsync();
            Stream resStream = res.GetResponseStream();
            return new Bitmap(resStream);
        }

        private void pokemonListView_SelectedIndexChanged(object sender, EventArgs e)
        {
            MessageBox.Show(pokemonListView.SelectedItems[0].Tag.ToString());
        }

        private void pokemonListViewItemSorter(int subItemsColumn)
        {
            ItemComparer sorter = pokemonListView.ListViewItemSorter as ItemComparer;

            if (sorter == null)
            {
                sorter = new ItemComparer(subItemsColumn);

                // Bug fix for IV (Occurs when Sort IV is selected first, as it should Descend)
                if (subItemsColumn == 3)
                    sorter.Order = SortOrder.Ascending;

                pokemonListView.ListViewItemSorter = sorter;
            }

            // If clicked column is already the column that is being sorted
            if (subItemsColumn == sorter.Column)
            {
                // Reverse the current sort direction
                if (sorter.Order == SortOrder.Ascending)
                    sorter.Order = SortOrder.Descending;
                else
                    sorter.Order = SortOrder.Ascending;
            }
            else
            {
                // Set the column number that is to be sorted.
                sorter.Column = subItemsColumn;

                // Default Sort Order for Cp
                if (subItemsColumn == 2)
                    sorter.Order = SortOrder.Descending;

                // Default Sort Order for IV
                else if (subItemsColumn == 3)
                    sorter.Order = SortOrder.Descending;

                // Default Sort Order for Name
                else if (subItemsColumn == 1)
                    sorter.Order = SortOrder.Ascending;
            }

            pokemonListView.Sort();
        }

        private void sortByCPToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pokemonListViewItemSorter(2);
        }

        private void sortByIVToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pokemonListViewItemSorter(3);
        }

        private void sortByNameToolStripMenuItem_Click(object sender, EventArgs e)
        {
            pokemonListViewItemSorter(1);
        }

        private async void transferSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Do you really want to transfer selected pokemon(s)?\nYou will never see them again :(", "Confirm Transfer", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                foreach (ListViewItem item in pokemonListView.SelectedItems)
                {
                    await _client.TransferPokemon((ulong)item.Tag);
                    Logger.Write($"Transferred {item.SubItems[1].Text} with {item.SubItems[2].Text} CP and an IV of {item.SubItems[3].Text}.");
                    pokemonListView.Items.Remove(item);
                }

                // Logging
                Logger.Write("Finished Transfering Pokemons.");
            }
        }

        private async void evolveSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Do you really want to evolve selected pokemon(s)?", "Confirm evolve", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                foreach (ListViewItem item in pokemonListView.SelectedItems)
                {
                    var id = (ulong)item.Tag;
                    var newPokemon = await _client.EvolvePokemon(id);

                    if (newPokemon.Result == EvolvePokemonResponse.Types.Result.Success)
                        MessageBox.Show($"Congratulations with your new pokemon {newPokemon.EvolvedPokemonData.PokemonId} with {newPokemon.EvolvedPokemonData.Cp} CP!");
                    else if (newPokemon.Result == EvolvePokemonResponse.Types.Result.FailedInsufficientResources)
                        MessageBox.Show("Insufficient Resources!");
                    else
                        MessageBox.Show($"Error: {newPokemon.Result.ToString()}");

                }
            }

            await Execute();
        }

        private async void powerupSelectedToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DialogResult dialogResult = MessageBox.Show("Do you really want to power up selected pokemon(s)?", "Confirm power up", MessageBoxButtons.YesNo);
            if (dialogResult == DialogResult.Yes)
            {
                foreach (ListViewItem item in pokemonListView.SelectedItems)
                {
                    var id = (ulong)item.Tag;
                    var poweredUpPokemon = await _client.PowerUpPokemon(id);

                    if (poweredUpPokemon.Result == EvolvePokemonResponse.Types.Result.FailedInsufficientResources)
                        MessageBox.Show("Insufficient Resources!");
                    else if (poweredUpPokemon.Result == EvolvePokemonResponse.Types.Result.FailedPokemonCannotEvolve)
                        MessageBox.Show("Unable to powerup more for your current level!");
                    else if (poweredUpPokemon.Result == EvolvePokemonResponse.Types.Result.Success)
                        MessageBox.Show($"Powerup success! New cp: {poweredUpPokemon.EvolvedPokemonData.Cp}");
                    else
                        MessageBox.Show($"Error: {poweredUpPokemon.Result}");

                }
            }

            await Execute();
        }

        private void pokemonListView_MouseClick(object sender, MouseEventArgs e)
        {
            if (e.Button == MouseButtons.Right)
            {
                if (pokemonListView.FocusedItem.Bounds.Contains(e.Location) == true)
                {
                    ContextMenuStrip myContextMenuStrip = new ContextMenuStrip();
                    myContextMenuStrip.Items.Add("Transfer selected pokémons", null, transferSelectedToolStripMenuItem_Click);
                    myContextMenuStrip.Items.Add("Evolve selected pokémons", null, evolveSelectedToolStripMenuItem_Click);
                    myContextMenuStrip.Items.Add("Powerup selected pokémons", null, powerupSelectedToolStripMenuItem_Click);
                    myContextMenuStrip.Show(pokemonListView, e.Location);
                }
            }
        }
    }

    public class ItemComparer : IComparer
    {
        // Column used for comparison
        public int Column { get; set; }

        // Order of sorting
        public SortOrder Order { get; set; }

        public ItemComparer(int colIndex)
        {
            Column = colIndex;
            Order = SortOrder.None;
        }

        public int Compare(object a, object b)
        {
            int result;

            ListViewItem itemA = a as ListViewItem;
            ListViewItem itemB = b as ListViewItem;

            if (itemA == null && itemB == null)
                result = 0;
            else if (itemA == null)
                result = -1;
            else if (itemB == null)
                result = 1;
            if (itemA == itemB)
                result = 0;

            /*
            // datetime comparison
            DateTime x1, y1;
            // Parse the two objects passed as a parameter as a DateTime.
            if (!DateTime.TryParse(itemA.SubItems[Column].Text, out x1))
                x1 = DateTime.MinValue;
            if (!DateTime.TryParse(itemB.SubItems[Column].Text, out y1))
                y1 = DateTime.MinValue;
            result = DateTime.Compare(x1, y1);
            if (x1 != DateTime.MinValue && y1 != DateTime.MinValue)
                goto done;
            */

            // Numeric comparison
            decimal x2, y2;

            if (!Decimal.TryParse(itemA.SubItems[Column].Text, out x2))
                x2 = Decimal.MinValue;
            if (!Decimal.TryParse(itemB.SubItems[Column].Text, out y2))
                y2 = Decimal.MinValue;
            result = Decimal.Compare(x2, y2);
            if (x2 != Decimal.MinValue && y2 != Decimal.MinValue)
                goto done;

            //alphabetic comparison
            result = String.Compare(itemA.SubItems[Column].Text, itemB.SubItems[Column].Text);

        done:

            // If sort order is descending.
            if (Order == SortOrder.Descending)
                // Invert the value returned by Compare.
                result *= -1;
            return result;
        }
    }
}
